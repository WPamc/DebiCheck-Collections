using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using PAMC.DatabaseConnection;
using EFT_Collections;
using RMCollectionProcessor;
using RMCollectionProcessor.Models;
using DcGenResult = RMCollectionProcessor.Models.FileGenerationResult;
using DCService = global::DatabaseService;
using EftGenResult = EFT_Collections.FileGenerationResult;

namespace DCCollections.Gui
{
    public partial class MainForm : Form
    {
        private readonly RMCollectionProcessor.CollectionService _dcCollectionservice;
        private readonly EFTImportService _eftImportService;
        private object[]? _parsedRecords;
        private DCFileType _currentFileType;
        private readonly UserSettings _settings;
        private int _importSortColumn;
        private bool _importSortDescending;
        private ProgressBar _pbImport;
        private ProgressBar _pbOperations;
        private TabPage tabBankFiles;
        private DataGridView dgvBankFiles;
        private TabPage tabLibrary;
        private ListBox lbLibraryFolders;
        private Button btnLibraryAdd;
        private Button btnLibraryRemove;
        private FlowLayoutPanel pnlLibraryButtons;
        private ProgressBar _pbLibrary;
        private List<string> _libraryPaths = new();
        private Dictionary<string, string> _libraryFiles = new();
        private const string ImportColumnName = "Import";

        private class ListViewItemComparer : System.Collections.IComparer
        {
            private readonly int _column;
            private readonly bool _desc;

            public ListViewItemComparer(int column, bool desc)
            {
                _column = column;
                _desc = desc;
            }

            public int Compare(object? x, object? y)
            {
                var lx = x as ListViewItem;
                var ly = y as ListViewItem;
                int result = string.Compare(lx?.SubItems[_column].Text, ly?.SubItems[_column].Text);
                return _desc ? -result : result;
            }
        }

        private record ImportFileTag(string Path, string RecordStatus);

        public MainForm()
        {
            InitializeComponent();
            lvImportFiles.HideSelection = false;
            lvImportFiles.FullRowSelect= true;
            string connStr = AppConfig.ConnectionString();
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                var safeConn = RemovePassword(connStr);
                Text = $"Collections - {safeConn}";

            }

            _dcCollectionservice = new RMCollectionProcessor.CollectionService();
            _eftImportService = new EFTImportService();
            _settings = UserSettings.Load();
            WindowState = FormWindowState.Maximized;
            MaximizeBox = true;
            chkTest.Checked = true;
            chkHideTestFiles.Checked = true;
            _pbImport = new ProgressBar { Dock = DockStyle.Bottom, Style = ProgressBarStyle.Marquee, Visible = false };
            tabImportFiles.Controls.Add(_pbImport);
            _pbOperations = new ProgressBar { Dock = DockStyle.Bottom, Style = ProgressBarStyle.Marquee, Visible = false };
            tabOperations.Controls.Add(_pbOperations);
            lvImportFiles.MultiSelect = true;
            tabBankFiles = new TabPage("EDI Bank Files");
            dgvBankFiles = new DataGridView { Dock = DockStyle.Fill };
            dgvBankFiles.CellContentClick += dgvBankFiles_CellContentClick;
            tabBankFiles.Controls.Add(dgvBankFiles);
            tabMain.Controls.Add(tabBankFiles);
            tabLibrary = new TabPage("Library");
            lbLibraryFolders = new ListBox { Dock = DockStyle.Fill, Enabled = false };
            pnlLibraryButtons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Enabled = false };
            btnLibraryAdd = new Button { Text = "Add" };
            btnLibraryAdd.Click += btnLibraryAdd_Click;
            btnLibraryRemove = new Button { Text = "Remove" };
            btnLibraryRemove.Click += btnLibraryRemove_Click;
            pnlLibraryButtons.Controls.Add(btnLibraryAdd);
            pnlLibraryButtons.Controls.Add(btnLibraryRemove);
            _pbLibrary = new ProgressBar { Dock = DockStyle.Bottom, Style = ProgressBarStyle.Marquee, Visible = true };
            tabLibrary.Controls.Add(lbLibraryFolders);
            tabLibrary.Controls.Add(pnlLibraryButtons);
            tabLibrary.Controls.Add(_pbLibrary);
            tabMain.Controls.Add(tabLibrary);
            PopulateBillingDates();
            LoadInitialPaths();
            LoadCounters();
            LoadBankFiles();
            var _ = LoadLibraryPathsAsync();
        }

        private static string RemovePassword(string connection)
        {
            var builder = new System.Data.Common.DbConnectionStringBuilder
            {
                ConnectionString = connection
            };
            builder.Remove("Password");
            builder.Remove("Pwd");
            return builder.ConnectionString;
        }

        private static (string date, string time) ExtractGenerationInfo(string filename)
        {
            try
            {
                var parts = filename.Split('.');
                if (parts.Length >= 2)
                {
                    var time = parts[^1];
                    var date = parts[^2];
                    return (date, time);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return (string.Empty, string.Empty);
        }

        private static string GetEftRecordStatus(string path)
        {
            try
            {
                var line = File.ReadLines(path).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(line) && line.Length > 3)
                {
                    return line.Substring(3, 1).Trim();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return string.Empty;
        }

        private void LoadBankFiles()
        {
            try
            {
                var db = new DCService();
                var table = db.GetEdiBankFiles();
                dgvBankFiles.DataSource = table;
                var counts = table.AsEnumerable().GroupBy(r => r.Field<int>("GenerationNumber")).ToDictionary(g => g.Key, g => g.Count());
                foreach (DataGridViewRow row in dgvBankFiles.Rows)
                {
                    int fileId = row.Cells["FileID"].Value == DBNull.Value ? 0 : Convert.ToInt32(row.Cells["FileID"].Value);
                    int gen = Convert.ToInt32(row.Cells["GenerationNumber"].Value);
                    if (fileId == 0)
                    {
                        row.DefaultCellStyle.BackColor = Color.Orange;
                    }
                    else if (counts[gen] > 1)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                    }
                }
                if (!dgvBankFiles.Columns.Contains(ImportColumnName))
                {
                    var btn = new DataGridViewButtonColumn { Name = ImportColumnName, HeaderText = ImportColumnName };
                    dgvBankFiles.Columns.Add(btn);
                }
                MatchBankFilesToLibrary();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// Updates the bank files grid with information about library files.
        /// </summary>
        private void MatchBankFilesToLibrary()
        {
            if (!dgvBankFiles.Columns.Contains(ImportColumnName))
                return;
            foreach (DataGridViewRow row in dgvBankFiles.Rows)
            {
                string fileName = row.Cells["FileName"].Value?.ToString() ?? string.Empty;
                bool hasFile = _libraryFiles.TryGetValue(fileName, out var path);
                var cell = row.Cells[ImportColumnName];
                if (hasFile && row.Cells["FileID"].Value == DBNull.Value)
                {
                    cell.Value = ImportColumnName;
                    cell.Tag = path;
                }
                else
                {
                    cell.Value = string.Empty;
                    cell.Tag = null;
                }
            }
        }

        /// <summary>
        /// Scans all library paths and records found files.
        /// </summary>
        private async Task ScanLibraryFilesAsync()
        {
            _pbLibrary.Visible = true;
            var files = await Task.Run(() => _libraryPaths.Where(Directory.Exists)
                .SelectMany(p => Directory.EnumerateFiles(p, "*", SearchOption.AllDirectories))
                .GroupBy(Path.GetFileName)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase));
            _libraryFiles = files;
            _pbLibrary.Visible = false;
            MatchBankFilesToLibrary();
        }

        /// <summary>
        /// Handles import button clicks in the bank files grid.
        /// </summary>
        private void dgvBankFiles_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != dgvBankFiles.Columns[ImportColumnName].Index)
                return;
            var cell = dgvBankFiles.Rows[e.RowIndex].Cells[e.ColumnIndex];
            var path = cell.Tag as string;
            if (string.IsNullOrEmpty(path))
                return;
            var fileName = dgvBankFiles.Rows[e.RowIndex].Cells["FileName"].Value?.ToString() ?? string.Empty;
            try
            {
                var db = new DCService();
                int rowId = db.GetBankFileRowId(fileName);
                if (rowId > 0)
                {
                    db.LinkFileToBankFile(rowId, path);
                    LoadBankFiles();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var result = _dcCollectionservice.ParseFile(ofd.FileName);
                    _parsedRecords = result.Records;
                    _currentFileType = result.FileType;
                    var msg = $"Parsed {_parsedRecords.Length} records (Type: {_currentFileType}).";
                    if (_currentFileType == DCFileType.StatusReport)
                    {
                        msg += $" Inserted {result.StatusRecordsInserted} of {result.StatusRecordsFound} status records.";
                    }
                    MessageBox.Show(msg, "Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            SetOperationsUiState(false);
            _pbOperations.Visible = true;
            try
            {
                int day = (int)nudDay.Value;
                bool test = chkTest.Checked;
                string? outFolder = test ? _settings.TestOutputFolderPath : _settings.LiveOutputFolderPath;
                var dateText = cmbBillingDate.SelectedItem?.ToString();
                DateTime effDate = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(dateText))
                {
                    effDate = DateTime.ParseExact(dateText, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                DcGenResult result;
                if (rdoDebiCheck.Checked)
                {
                    var start = dtStartCollectionDate.Value.Date;
                    var end = dtEndCollectionDate.Value.Date;
                    var collections = _dcCollectionservice.GetCollections(day, effDate);
                    var requests = _dcCollectionservice.GetCollectionRequests(start, end);
                    var existing = new Dictionary<string, List<BillingCollectionRequest>>();
                    foreach (var c in collections)
                    {
                        string sub = "MGS" + c.ContractReference;
                        var list = requests.Where(r => r.SubSSN == sub).ToList();
                        if (list.Any())
                            existing[sub] = list;
                    }
                    if (existing.Any())
                    {
                        using var confirm = new ConfirmCollectionsForm(collections, existing);
                        if (confirm.ShowDialog(this) != DialogResult.OK)
                            return;
                        collections = confirm.SelectedCollections;
                    }
                    result = await Task.Run(() => _dcCollectionservice.GenerateDCFile(day, effDate, test, outFolder, collections));
                }
                else
                {
                    DateTime today = DateTime.Today;
                    DateTime date = day <= today.Day
                        ? new DateTime(today.AddMonths(1).Year, today.AddMonths(1).Month, day)
                        : new DateTime(today.Year, today.Month, day);
                    string folder = outFolder ?? AppContext.BaseDirectory;
                    EftGenResult eftResult = await Task.Run(() => EFTService.GenerateEFTFile(date, test, folder));
                    MessageBox.Show($"File generated: {eftResult.FilePath}\nDatabase updated with:\nBankFiles updated: {eftResult.BankFilesUpdated}\nCollection Requests updated: {eftResult.CollectionRequestsUpdated}", "Success");
                    using var previewForm = new FilePreviewForm(eftResult.FilePath);
                    previewForm.ShowDialog(this);
                    return;
                }
                MessageBox.Show($"File generated: {result.FilePath}\nDatabase updated with:\nBankFiles updated: {result.BankFilesUpdated}\nCollection Requests updated: {result.CollectionRequestsUpdated}", "Success");
                using var resultForm = new FilePreviewForm(result.FilePath);
                resultForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                _pbOperations.Visible = false;
                SetOperationsUiState(true);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _settings.OperationFolderPath = fbd.SelectedPath;
                LoadOperationsFiles(fbd.SelectedPath);
            }
        }

        private void btnShowCurrent_Click(object sender, EventArgs e)
        {
            LoadOperationsFiles(AppContext.BaseDirectory);
        }

        private void btnLiveOutputBrowse_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtLiveOutputFolder.Text = fbd.SelectedPath;
                _settings.LiveOutputFolderPath = fbd.SelectedPath;
                _settings.Save();
            }
        }

        private void btnTestOutputBrowse_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtTestOutputFolder.Text = fbd.SelectedPath;
                _settings.TestOutputFolderPath = fbd.SelectedPath;
                _settings.Save();
            }
        }

        private void btnLiveOutputOpen_Click(object sender, EventArgs e)
        {
            OpenFolder(txtLiveOutputFolder.Text);
        }

        private void btnTestOutputOpen_Click(object sender, EventArgs e)
        {
            OpenFolder(txtTestOutputFolder.Text);
        }

        private static void OpenFolder(string? path)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", path) { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show($"Folder not found: {path}", "Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }


        private void btnCheckDuplicates_Click(object sender, EventArgs e)
        {
            try
            {
                int day = (int)nudDay.Value;
                var text = cmbBillingDate.SelectedItem?.ToString();
                DateTime effDate = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    effDate = DateTime.ParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                var table = _dcCollectionservice.GetDuplicateCollections(day, effDate);
                dgvPossibleDuplicates.DataSource = table;
                MessageBox.Show($"Found {table.Rows.Count} possible duplicates.", "Check Duplicates");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void LoadInitialPaths()
        {
            if (!string.IsNullOrWhiteSpace(_settings.OperationFolderPath) && Directory.Exists(_settings.OperationFolderPath))
            {
                LoadOperationsFiles(_settings.OperationFolderPath);
            }

            if (!string.IsNullOrWhiteSpace(_settings.LiveOutputFolderPath) && Directory.Exists(_settings.LiveOutputFolderPath))
            {
                txtLiveOutputFolder.Text = _settings.LiveOutputFolderPath;
            }

            if (!string.IsNullOrWhiteSpace(_settings.TestOutputFolderPath) && Directory.Exists(_settings.TestOutputFolderPath))
            {
                txtTestOutputFolder.Text = _settings.TestOutputFolderPath;
            }


            _importSortColumn = _settings.ImportSortColumn;
            _importSortDescending = _settings.ImportSortDescending;
            if (_importSortColumn >= lvImportFiles.Columns.Count)
                _importSortColumn = 0;

            if (!string.IsNullOrWhiteSpace(_settings.ImportFolderPath) && Directory.Exists(_settings.ImportFolderPath))
            {
                txtImportFolder.Text = _settings.ImportFolderPath;
                LoadImportFiles(_settings.ImportFolderPath);
            }
        }

        private void LoadOperationsFiles(string path)
        {
            try
            {
                foreach (var file in Directory.GetFiles(path))
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnImportBrowse_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtImportFolder.Text = fbd.SelectedPath;
                _settings.ImportFolderPath = fbd.SelectedPath;
                _settings.Save();
                LoadImportFiles(fbd.SelectedPath);
            }
        }

        private void chkHideTestFiles_CheckedChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(txtImportFolder.Text))
            {
                LoadImportFiles(txtImportFolder.Text);
            }
        }

        /// <summary>
        /// Loads files from the specified path into the Import Files list view.
        /// </summary>
        /// <param name="path">The folder to scan for files.</param>
        private void LoadImportFiles(string path)
        {
            try
            {
                lvImportFiles.Items.Clear();

                bool hideTests = chkHideTestFiles.Checked;
                string nameFilter = txtFileFilter.Text.Trim();

                var processor = new FileProcessor();
                var eftIdentifier = new EftFileIdentifier();
               
                var db = new DCService();

                foreach (var file in Directory.GetFiles(path))
                {
                    var info = new FileInfo(file);
                    if (!string.IsNullOrEmpty(nameFilter) &&
                        !info.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
                        continue;
                    var size = info.Length > 1024 ? $"{info.Length / 1024} KB" : $"{info.Length} bytes";
                    DCFileType dcType = DCFileType.Unknown;
                    EftFileType eftType = EftFileType.Unknown;
                    string recordStatus = string.Empty;

                    try
                    {
                       
                        eftType = eftIdentifier.IdentifyFileType(file);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
                    if (eftType  == EftFileType.Unknown || eftType == EftFileType.EmptyTransmission)
                        try
                        {
                            var fileProcessor = new FileProcessor();
                            var records = fileProcessor.ProcessFile(file);
                            dcType = DCFileTypeIdentifier.Identify(records);
                            if (records.Length > 0 && records[0] is RMCollectionProcessor.Models.TransmissionHeader000 th)
                                recordStatus = th.RecordStatus?.Trim() ?? string.Empty;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error");
                            eftType = eftIdentifier.IdentifyFileType(file);
                            if (eftType != EftFileType.Unknown)
                                recordStatus = GetEftRecordStatus(file);
                        }

                    bool isLive = recordStatus.Length == 0 || recordStatus.Equals("L", StringComparison.OrdinalIgnoreCase);
                    if (hideTests && !isLive)
                        continue;

                    var (genDate, genTime) = ExtractGenerationInfo(info.Name);
                    var item = new ListViewItem(info.Name)
                    {
                        Tag = new ImportFileTag(info.FullName, recordStatus)
                    };
                    item.SubItems.Add(genDate);
                    item.SubItems.Add(genTime);
                    item.SubItems.Add(size);
                    item.SubItems.Add(info.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));

                    var desc = dcType != DCFileType.Unknown ? dcType.ToString() : eftType.ToString();
                    if (!isLive && recordStatus.Length > 0)
                        desc += " (Test)";
                    try
                    {
                        var parsed = processor.ProcessFile(info.FullName);
                        var ft = DCFileTypeIdentifier.Identify(parsed);
                        if (ft != DCFileType.Unknown)
                            desc = ft.ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
                    item.SubItems.Add(desc);

                    item.SubItems.Add(isLive ? "No" : "Yes");
                    bool imported = db.GetBankFileRowId(info.Name) > 0;
                    item.SubItems.Add(imported ? "Yes" : "No");

                    lvImportFiles.Items.Add(item);
                }

                lvImportFiles.ListViewItemSorter = new ListViewItemComparer(_importSortColumn, _importSortDescending);
                lvImportFiles.Sort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void lvImportFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasSelection = lvImportFiles.SelectedItems.Count > 0;
            btnImportRead.Enabled = hasSelection;
            btnImportParse.Enabled = hasSelection;
        }

        private void lvImportFiles_ColumnClick(object? sender, ColumnClickEventArgs e)
        {
            if (e.Column == _importSortColumn)
            {
                _importSortDescending = !_importSortDescending;
            }
            else
            {
                _importSortColumn = e.Column;
                _importSortDescending = false;
            }

            _settings.ImportSortColumn = _importSortColumn;
            _settings.ImportSortDescending = _importSortDescending;
            _settings.Save();

            lvImportFiles.ListViewItemSorter = new ListViewItemComparer(_importSortColumn, _importSortDescending);
            lvImportFiles.Sort();
        }

        private void lvImportFiles_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            var hit = lvImportFiles.HitTest(e.Location);
            if (hit.Item != null)
            {
                lvImportFiles.SelectedItems.Clear();
                hit.Item.Selected = true;
                cmsImportFiles.Show(lvImportFiles, e.Location);
            }
        }

        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvImportFiles.SelectedItems.Count == 0)
                return;

            var tagObj = lvImportFiles.SelectedItems[0].Tag;
            string? path = null;
            if (tagObj is ImportFileTag tag)
                path = tag.Path;
            else
                path = tagObj as string;

            if (string.IsNullOrWhiteSpace(path))
                return;

            try
            {
                using var form = new FilePreviewForm(path);
                form.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// Extracts a DateTime from a filename based on common date/time patterns.
        /// It prioritizes 'yyyyMMdd.HHmmss' and 'yyMMdd.HHmmss' formats.
        /// </summary>
        /// <param name="fileName">The filename to parse.</param>
        /// <returns>A DateTime object if a valid date is found; otherwise, null.</returns>
        private DateTime? ExtractDateTimeFromFileName(string fileName)
        {
            var mainMatch = Regex.Match(fileName, @"\.(\d{8}|\d{6})\.(\d{6})");

            if (mainMatch.Success)
            {
                string datePart = mainMatch.Groups[1].Value;
                string timePart = mainMatch.Groups[2].Value;
                string combined = datePart + timePart;

                string format = (datePart.Length == 8 ? "yyyyMMdd" : "yyMMdd") + "HHmmss";

                if (DateTime.TryParseExact(combined, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
            }

            return null;
        }

        private async void btnImportParse_Click(object sender, EventArgs e)
        {
            if (lvImportFiles.SelectedItems.Count == 0)
                return;

            SetImportUiState(false);
            _pbImport.Visible = true;

            try
            {
                int totalFound = 0;
                int totalInserted = 0;
                int eftInserted = 0;

                var sortedItems = lvImportFiles.SelectedItems
               .Cast<ListViewItem>()
               .Select(item => new { Item = item, Date = ExtractDateTimeFromFileName(item.Text) })
               .OrderBy(x => x.Date ?? DateTime.MinValue)
               .Select(x => x.Item)
               .ToList();

                foreach (ListViewItem item in sortedItems)
                {
                    var tagObj = item.Tag;
                    string? path = null;
                    string status = string.Empty;
                    if (tagObj is ImportFileTag tag)
                    {
                        path = tag.Path;
                        status = tag.RecordStatus;
                    }
                    else
                    {
                        path = tagObj as string;
                    }

                    if (string.IsNullOrWhiteSpace(path))
                        continue;

                    if (status.Equals("T", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var eftType = EftFileType.Unknown;
                    try
                    {
                        var identifier = new EftFileIdentifier();
                        eftType = identifier.IdentifyFileType(path);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }

                    if (eftType != EftFileType.Unknown && eftType != EftFileType.EmptyTransmission)
                    {
                        var eftResult = await Task.Run(() => _eftImportService.ParseFile(path));
                        eftInserted += eftResult.RecordsInserted;
                    }
                    else
                    {
                        var result = await Task.Run(() => _dcCollectionservice.ParseFile(path));
                        if (result.FileType == DCFileType.StatusReport)
                        {
                            totalFound += result.StatusRecordsFound;
                            totalInserted += result.StatusRecordsInserted;
                        }
                    }

                    item.SubItems[item.SubItems.Count - 1].Text = "Yes";
                }

                var msg = "Import complete.";
                if (eftInserted > 0)
                {
                    msg += $" Inserted {eftInserted} EFT records.";
                }
                if (totalFound > 0)
                {
                    msg += $" Inserted {totalInserted} of {totalFound} status records.";
                }

                MessageBox.Show(msg, "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                _pbImport.Visible = false;
                SetImportUiState(true);
            }
        }

        private void btnImportRead_Click(object sender, EventArgs e)
        {
            if (lvImportFiles.SelectedItems.Count == 0)
                return;

            var tagObj = lvImportFiles.SelectedItems[0].Tag;
            string? path = null;
            if (tagObj is ImportFileTag tag)
                path = tag.Path;
            else
                path = tagObj as string;

            if (string.IsNullOrWhiteSpace(path))
                return;

            try
            {
                var text = File.ReadAllText(path);
                MessageBox.Show(text, Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void SetImportUiState(bool enabled)
        {
            pnlImportTop.Enabled = enabled;
            lvImportFiles.Enabled = enabled;
            UseWaitCursor = !enabled;
        }

        private void SetOperationsUiState(bool enabled)
        {
            grpConfig.Enabled = enabled;
            btnGenerate.Enabled = enabled;
            btnCheckDuplicates.Enabled = enabled && rdoDebiCheck.Checked;
            chkTest.Enabled = enabled;
            nudDay.Enabled = enabled;
            dgvPossibleDuplicates.Enabled = enabled;
            UseWaitCursor = !enabled;
        }

        private void rdoFileType_CheckedChanged(object sender, EventArgs e)
        {
            btnCheckDuplicates.Enabled = rdoDebiCheck.Checked;
        }

        private void btnSearchFiles_Click(object sender, EventArgs e)
        {
            string term = txtSearchFiles.Text.Trim();

            foreach (ListViewItem item in lvImportFiles.Items)
            {
                bool match = !string.IsNullOrEmpty(term) &&
                             item.SubItems.Cast<ListViewItem.ListViewSubItem>()
                                 .Any(sub => sub.Text.Contains(term, StringComparison.OrdinalIgnoreCase));
                item.BackColor = match ? Color.Yellow : lvImportFiles.BackColor;
            }
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtImportFolder.Text))
            {
                LoadImportFiles(txtImportFolder.Text);
            }
        }

        private void btnFindText_Click(object sender, EventArgs e)
        {
            string term = txtFindText.Text.Trim();

            foreach (ListViewItem item in lvImportFiles.Items)
            {
                bool match = false;
                if (!string.IsNullOrEmpty(term))
                {
                    var tagObj = item.Tag;
                    string? path = null;
                    if (tagObj is ImportFileTag tag)
                        path = tag.Path;
                    else
                        path = tagObj as string;

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        try
                        {
                            var text = File.ReadAllText(path);
                            match = text.Contains(term, StringComparison.OrdinalIgnoreCase);
                        }
                        catch
                        {
                            match = false;
                        }
                    }
                }

                item.BackColor = match ? Color.Yellow : lvImportFiles.BackColor;
            }
        }

        private void btnArchive_Click(object sender, EventArgs e)
        {
            if (lvImportFiles.Items.Count == 0)
            {
                MessageBox.Show("There are no files to archive.", "Archive Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var uniqueTypes = lvImportFiles.Items
                .Cast<ListViewItem>()
                .Select(item => item.SubItems[5].Text) // Assuming 'Type' is at index 5
                .Distinct()
                .ToList();

            using var dialog = new ArchiveDialog(_settings, uniqueTypes);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                // Save the chosen settings for next time
                _settings.ArchiveOlderThanDays = dialog.DaysOlder;
                _settings.ArchiveForceUnimported = dialog.ForceArchive;
                _settings.ArchiveLastFileType = dialog.FileType;
                _settings.Save();

                int movedFiles = 0;
                var importPath = txtImportFolder.Text;
                var archiveDate = DateTime.Now.AddDays(-dialog.DaysOlder);

                SetImportUiState(false);
                _pbImport.Visible = true;

                try
                {
                    foreach (ListViewItem item in lvImportFiles.Items)
                    {
                        var fileInfo = new FileInfo(((ImportFileTag)item.Tag).Path);
                        var imported = item.SubItems[7].Text.Equals("Yes", StringComparison.OrdinalIgnoreCase); // 'Imported' is at index 7
                        var fileType = item.SubItems[5].Text; // 'Type' is at index 5

                        // Criterion 1: Age (if DaysOlder > 0)
                        if (dialog.DaysOlder > 0 && fileInfo.LastWriteTime >= archiveDate)
                        {
                            continue;
                        }

                        // Criterion 2: Import Status
                        if (!dialog.ForceArchive && !imported)
                        {
                            continue;
                        }

                        // Criterion 3: File Type (only if forcing)
                        if (dialog.ForceArchive && dialog.FileType != "All File Types" && dialog.FileType != fileType)
                        {
                            continue;
                        }

                        // All criteria met, move the file
                        var targetFolder = Path.Combine(importPath, fileType);
                        Directory.CreateDirectory(targetFolder);
                        var targetFile = Path.Combine(targetFolder, fileInfo.Name);

                        try
                        {
                            File.Move(fileInfo.FullName, targetFile, true);
                            movedFiles++;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Could not move file {fileInfo.Name}.\n\nError: {ex.Message}", "Archive Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break; // Stop on first error
                        }
                    }

                    MessageBox.Show($"Successfully archived {movedFiles} file(s).", "Archive Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    _pbImport.Visible = false;
                    SetImportUiState(true);
                    LoadImportFiles(importPath);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Retrieves the latest counter values from the database and displays them.
        /// </summary>
        private void LoadCounters()
        {
            try
            {
                var dcDb = new DCService();
                int dcGen = dcDb.GetCurrentGenerationNumber();
                int dcDaily = dcDb.GetCurrentDailyCounter(DateTime.Today);

                var eftDb = new EFT_Collections.DatabaseService();
                int eftGen = eftDb.PeekGenerationNumber();
                int eftDaily = eftDb.PeekDailyCounter(DateTime.Today);

                lblDcGenerationNumber.Text = $"DC GenerationNumber: {dcGen}";
                lblDcDailyCounter.Text = $"DC DailyCounter: {dcDaily}";
                lblEftGenerationNumber.Text = $"EFT GenerationNumber: {eftGen}";
                lblEftDailyCounter.Text = $"EFT DailyCounter: {eftDaily}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async Task LoadLibraryPathsAsync()
        {
            var paths = await Task.Run(() => (_settings.LibraryPaths ?? new List<string>()).Where(Directory.Exists).ToList());
            _libraryPaths = paths;
            lbLibraryFolders.Items.Clear();
            foreach (var path in paths)
                lbLibraryFolders.Items.Add(path);
            lbLibraryFolders.Enabled = true;
            pnlLibraryButtons.Enabled = true;
            _pbLibrary.Visible = false;
            await ScanLibraryFilesAsync();
        }

        private void btnLibraryAdd_Click(object? sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (!_libraryPaths.Contains(fbd.SelectedPath))
                {
                    _libraryPaths.Add(fbd.SelectedPath);
                    lbLibraryFolders.Items.Add(fbd.SelectedPath);
                    _settings.LibraryPaths = _libraryPaths;
                    _settings.Save();
                    var _ = ScanLibraryFilesAsync();
                }
            }
        }

        private void btnLibraryRemove_Click(object? sender, EventArgs e)
        {
            if (lbLibraryFolders.SelectedItem is string path)
            {
                _libraryPaths.Remove(path);
                lbLibraryFolders.Items.Remove(path);
                _settings.LibraryPaths = _libraryPaths;
                _settings.Save();
                var _ = ScanLibraryFilesAsync();
            }
        }

        private void PopulateBillingDates()
        {
            cmbBillingDate.Items.Clear();
            var today = DateTime.Today;
            var first = new DateTime(today.Year, today.Month, 1);
            if (first < today.Date)
            {
                first = first.AddMonths(1);
            }
            for (int i = 0; i < 6; i++)
            {
                var date = first.AddMonths(i);
                cmbBillingDate.Items.Add(date.ToString("yyyy-MM-dd"));
            }
            if (cmbBillingDate.Items.Count > 0)
            {
                cmbBillingDate.SelectedIndex = 0;
                UpdateBillingWindowDates();
            }
        }

        private void cmbBillingDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateBillingWindowDates();
        }

        private void UpdateBillingWindowDates()
        {
            var text = cmbBillingDate.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(text))
                return;
            var bill = DateTime.ParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var start = new DateTime(bill.AddMonths(-1).Year, bill.AddMonths(-1).Month, 6);
            var end = new DateTime(bill.Year, bill.Month, 5);
            dtStartCollectionDate.Value = start;
            dtEndCollectionDate.Value = end;
        }
    }
}
