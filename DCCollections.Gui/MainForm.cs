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
        private ListView lvLibraryFolders;
        private Button btnLibraryAdd;
        private Button btnLibraryRemove;
        private FlowLayoutPanel pnlLibraryButtons;
        private ProgressBar _pbLibrary;
        private List<UserSettings.LibraryPathEntry> _libraryPaths = new();
        private Dictionary<string, string> _libraryFiles = new();
        private readonly Dictionary<TabPage, Panel> _tabLocks = new();
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
            lvImportFiles.FullRowSelect = true;
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
            lvLibraryFolders = new ListView { Dock = DockStyle.Fill, Enabled = false, CheckBoxes = true, View = View.List };
            lvLibraryFolders.ItemChecked += lvLibraryFolders_ItemChecked;
            pnlLibraryButtons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Enabled = false };
            btnLibraryAdd = new Button { Text = "Add" };
            btnLibraryAdd.Click += btnLibraryAdd_Click;
            btnLibraryRemove = new Button { Text = "Remove" };
            btnLibraryRemove.Click += btnLibraryRemove_Click;
            pnlLibraryButtons.Controls.Add(btnLibraryAdd);
            pnlLibraryButtons.Controls.Add(btnLibraryRemove);
            _pbLibrary = new ProgressBar { Dock = DockStyle.Bottom, Style = ProgressBarStyle.Marquee, Visible = true };
            tabLibrary.Controls.Add(lvLibraryFolders);
            tabLibrary.Controls.Add(pnlLibraryButtons);
            tabLibrary.Controls.Add(_pbLibrary);
            tabMain.Controls.Add(tabLibrary);
            PopulateBillingDates();
            LoadInitialDataAsync();
        }

        /// <summary>
        /// Orchestrates the asynchronous and parallel loading of all initial data to prevent blocking the UI thread on startup.
        /// </summary>
        private async void LoadInitialDataAsync()
        {
            try
            {
                var countersTask = LoadCountersAsync();
                var bankFilesTask = LoadBankFilesAsync();
                var initialPathsTask = LoadInitialPathsAsync();
                var libraryPathsTask = LoadLibraryPathsAsync();

                await Task.WhenAll(countersTask, bankFilesTask, initialPathsTask, libraryPathsTask);
            }
            catch (Exception ex)
            {
                ShowError($"An error occurred during initial data load: {ex.Message}");
            }
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

        /// <summary>
        /// Displays an error message box with a standardized caption.
        /// </summary>
        /// <param name="message">The message to display.</param>
        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Error");
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
                ShowError(ex.Message);
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
                ShowError(ex.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// Overlays the specified tab with a spinner to prevent interaction.
        /// </summary>
        private void LockTab(TabPage tab)
        {
            if (_tabLocks.ContainsKey(tab))
                return;
            var overlay = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(128, Color.Gray) };
            var spinner = new ProgressBar { Style = ProgressBarStyle.Marquee, Width = 100, Height = 20 };
            spinner.Anchor = AnchorStyles.None;
            spinner.Location = new Point((overlay.Width - spinner.Width) / 2, (overlay.Height - spinner.Height) / 2);
            overlay.Controls.Add(spinner);
            overlay.Resize += (s, _) =>
            {
                var pb = (ProgressBar)((Panel)s).Controls[0];
                pb.Location = new Point(((Panel)s).Width / 2 - pb.Width / 2, ((Panel)s).Height / 2 - pb.Height / 2);
            };
            tab.Controls.Add(overlay);
            overlay.BringToFront();
            _tabLocks[tab] = overlay;
        }

        /// <summary>
        /// Removes the spinner overlay from the specified tab.
        /// </summary>
        private void UnlockTab(TabPage tab)
        {
            if (_tabLocks.TryGetValue(tab, out var overlay))
            {
                tab.Controls.Remove(overlay);
                overlay.Dispose();
                _tabLocks.Remove(tab);
            }
        }
        /// <summary>
        /// Asynchronously loads EDI bank file records from the database into the grid.
        /// It then attempts to match these records against files found in the Library.
        /// </summary>
        private async Task LoadBankFilesAsync()
        {
            LockTab(tabBankFiles);
            try
            {
                var db = new DCService();
                var table = await Task.Run(() => db.GetEdiBankFiles());
                dgvBankFiles.DataSource = table;
                if (dgvBankFiles.Columns.Contains("FileName"))
                    dgvBankFiles.Columns["FileName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                if (!dgvBankFiles.Columns.Contains("Path"))
                {
                    var pathCol = new DataGridViewTextBoxColumn { Name = "Path", HeaderText = "Path", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
                    dgvBankFiles.Columns.Add(pathCol);
                }
                var counts = table.AsEnumerable().GroupBy(r => r.Field<int>("GenerationNumber")).ToDictionary(g => g.Key, g => g.Count());
                foreach (DataGridViewRow row in dgvBankFiles.Rows)
                {
                    bool isUnlinked = row.Cells["FileID"].Value == DBNull.Value;
                    int gen = Convert.ToInt32(row.Cells["GenerationNumber"].Value);
                    if (isUnlinked)
                    {
                        row.DefaultCellStyle.BackColor = Color.Orange;
                    }
                    else if (counts.ContainsKey(gen) && counts[gen] > 1)
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
                ShowError(ex.Message);
            }
            finally
            {
                UnlockTab(tabBankFiles);
            }
        }
        /// <summary>
        /// Updates the bank files grid with information about library files.
        /// </summary>
        private void MatchBankFilesToLibrary()
        {
            if (!dgvBankFiles.Columns.Contains(ImportColumnName) || !dgvBankFiles.Columns.Contains("Path"))
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
                row.Cells["Path"].Value = hasFile ? path : string.Empty;
            }
        }

        /// <summary>
        /// Scans all library paths and records found files.
        /// </summary>
        private async Task ScanLibraryFilesAsync()
        {
            LockTab(tabLibrary);
            LockTab(tabBankFiles);
            try
            {
                var files = await Task.Run(() => _libraryPaths.Where(p => Directory.Exists(p.Path))
                    .SelectMany(p => Directory.EnumerateFiles(p.Path, "*", p.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                    .GroupBy(Path.GetFileName)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase));
                _libraryFiles = files;
                MatchBankFilesToLibrary();
            }
            finally
            {
                UnlockTab(tabLibrary);
                UnlockTab(tabBankFiles);
            }
        }

        /// <summary>
        /// Handles the click event for cells in the bank files grid.
        /// If the 'Import' button is clicked, it retrieves the file path and calls the database service to link the physical file to the bank file record.
        /// It first checks the cell's Tag property for the path, and if not present, attempts a real-time lookup in the library file cache as a fallback.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A DataGridViewCellEventArgs that contains the event data.</param>
        private async void dgvBankFiles_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != dgvBankFiles.Columns[ImportColumnName].Index)
                return;
            var row = dgvBankFiles.Rows[e.RowIndex];
            var cell = row.Cells[e.ColumnIndex];
            var path = cell.Tag as string;
            var fileName = row.Cells["FileName"].Value?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                if (string.IsNullOrEmpty(fileName) || !_libraryFiles.TryGetValue(fileName, out path))
                {
                    return;
                }
            }
            try
            {
                var db = new DCService();
                int rowId = db.GetBankFileRowId(fileName);
                if (rowId > 0)
                {
                    db.LinkFileToBankFile(rowId, path);
                    await LoadBankFilesAsync();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
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
                    ShowError(ex.Message);
                }
            }
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            LockTab(tabOperations);
            SetOperationsUiState(false);
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
                ShowError(ex.Message);
            }
            finally
            {
                SetOperationsUiState(true);
                UnlockTab(tabOperations);
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
                ShowError(ex.Message);
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
                ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Asynchronously loads initial paths from settings and populates the relevant UI controls.
        /// </summary>
        private async Task LoadInitialPathsAsync()
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
                await LoadImportFilesAsync(_settings.ImportFolderPath);
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
                ShowError(ex.Message);
            }
        }

        private async void btnImportBrowse_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtImportFolder.Text = fbd.SelectedPath;
                _settings.ImportFolderPath = fbd.SelectedPath;
                _settings.Save();
                await LoadImportFilesAsync(fbd.SelectedPath);
            }
        }

        private async void chkHideTestFiles_CheckedChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(txtImportFolder.Text))
            {
                await LoadImportFilesAsync(txtImportFolder.Text);
            }
        }

        /// <summary>
        /// Asynchronously loads files from the specified path into the Import Files list view without blocking the UI.
        /// </summary>
        /// <param name="path">The folder to scan for files.</param>
        private async Task LoadImportFilesAsync(string path)
        {
            LockTab(tabImportFiles);
            SetImportUiState(false);
            try
            {
                bool hideTests = chkHideTestFiles.Checked;
                string nameFilter = txtFileFilter.Text.Trim();

                var items = await Task.Run(() =>
                {
                    var newItems = new List<ListViewItem>();
                    var processor = new FileProcessor();
                    var eftIdentifier = new EftFileIdentifier();
                    var db = new DCService();

                    foreach (var file in Directory.GetFiles(path))
                    {
                        var info = new FileInfo(file);
                        if (!string.IsNullOrEmpty(nameFilter) &&
                            !info.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string recordStatus = string.Empty;
                        EftFileType eftType = eftIdentifier.IdentifyFileType(file);
                        if (eftType != EftFileType.Unknown)
                        {
                            recordStatus = GetEftRecordStatus(file);
                        }
                        else
                        {
                            var fileProcessor = new FileProcessor();
                            var records = fileProcessor.ProcessFile(file);
                            if (records.Length > 0 && records[0] is RMCollectionProcessor.Models.TransmissionHeader000 th)
                                recordStatus = th.RecordStatus?.Trim() ?? string.Empty;
                        }

                        bool isLive = recordStatus.Length == 0 || recordStatus.Equals("L", StringComparison.OrdinalIgnoreCase);
                        if (hideTests && !isLive)
                            continue;

                        var size = info.Length > 1024 ? $"{info.Length / 1024} KB" : $"{info.Length} bytes";
                        var (genDate, genTime) = ExtractGenerationInfo(info.Name);
                        var item = new ListViewItem(info.Name)
                        {
                            Tag = new ImportFileTag(info.FullName, recordStatus)
                        };
                        item.SubItems.Add(genDate);
                        item.SubItems.Add(genTime);
                        item.SubItems.Add(size);
                        item.SubItems.Add(info.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));

                        var parsed = processor.ProcessFile(info.FullName);
                        var ft = DCFileTypeIdentifier.Identify(parsed);
                        var desc = ft != DCFileType.Unknown ? ft.ToString() : eftType.ToString();
                        if (!isLive && recordStatus.Length > 0)
                            desc += " (Test)";
                        item.SubItems.Add(desc);

                        item.SubItems.Add(isLive ? "No" : "Yes");
                        bool imported = db.GetBankFileRowId(info.Name) > 0;
                        item.SubItems.Add(imported ? "Yes" : "No");

                        newItems.Add(item);
                    }
                    return newItems;
                });

                lvImportFiles.BeginUpdate();
                lvImportFiles.Items.Clear();
                lvImportFiles.Items.AddRange(items.ToArray());
                lvImportFiles.ListViewItemSorter = new ListViewItemComparer(_importSortColumn, _importSortDescending);
                lvImportFiles.Sort();
                lvImportFiles.EndUpdate();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                SetImportUiState(true);
                UnlockTab(tabImportFiles);
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
                ShowError(ex.Message);
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

            LockTab(tabImportFiles);
            SetImportUiState(false);

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
                        ShowError(ex.Message);
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
                ShowError(ex.Message);
            }
            finally
            {
                SetImportUiState(true);
                UnlockTab(tabImportFiles);
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
                ShowError(ex.Message);
            }
        }

        private void SetImportUiState(bool enabled)
        {
            pnlImportTop.Enabled = enabled;
            lvImportFiles.Enabled = enabled;
        }

        private void SetOperationsUiState(bool enabled)
        {
            grpConfig.Enabled = enabled;
            btnGenerate.Enabled = enabled;
            btnCheckDuplicates.Enabled = enabled && rdoDebiCheck.Checked;
            chkTest.Enabled = enabled;
            nudDay.Enabled = enabled;
            dgvPossibleDuplicates.Enabled = enabled;
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

        private async void btnApplyFilter_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtImportFolder.Text))
            {
                await LoadImportFilesAsync(txtImportFolder.Text);
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

        private async void btnArchive_Click(object sender, EventArgs e)
        {
            LockTab(tabImportFiles);
            try
            {
                if (lvImportFiles.Items.Count == 0)
                {
                    MessageBox.Show("There are no files to archive.", "Archive Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var uniqueTypes = lvImportFiles.Items
                    .Cast<ListViewItem>()
                    .Select(item => item.SubItems[5].Text)
                    .Distinct()
                    .ToList();

                using var dialog = new ArchiveDialog(_settings, uniqueTypes);

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _settings.ArchiveOlderThanDays = dialog.DaysOlder;
                    _settings.ArchiveForceUnimported = dialog.ForceArchive;
                    _settings.ArchiveLastFileType = dialog.FileType;
                    _settings.Save();

                    int movedFiles = 0;
                    var importPath = txtImportFolder.Text;
                    var archiveDate = DateTime.Now.AddDays(-dialog.DaysOlder);

                    SetImportUiState(false);

                    try
                    {
                        foreach (ListViewItem item in lvImportFiles.Items)
                        {
                            var fileInfo = new FileInfo(((ImportFileTag)item.Tag).Path);
                            var imported = item.SubItems[7].Text.Equals("Yes", StringComparison.OrdinalIgnoreCase);
                            var fileType = item.SubItems[5].Text;

                            if (dialog.DaysOlder > 0 && fileInfo.LastWriteTime >= archiveDate)
                            {
                                continue;
                            }

                            if (!dialog.ForceArchive && !imported)
                            {
                                continue;
                            }

                            if (dialog.ForceArchive && dialog.FileType != "All File Types" && dialog.FileType != fileType)
                            {
                                continue;
                            }

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
                                ShowError($"Could not move file {fileInfo.Name}.\n\nError: {ex.Message}");
                                break;
                            }
                        }

                        MessageBox.Show($"Successfully archived {movedFiles} file(s).", "Archive Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    finally
                    {
                        SetImportUiState(true);
                        await LoadImportFilesAsync(importPath);
                    }
                }
            }
            finally
            {
                UnlockTab(tabImportFiles);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Asynchronously retrieves the latest counter values from the database and displays them.
        /// </summary>
        private async Task LoadCountersAsync()
        {
            LockTab(tabOperations);
            try
            {
                var (dcGen, dcDaily, eftGen, eftDaily) = await Task.Run(() =>
                {
                    var dcDb = new DCService();
                    int dcG = dcDb.GetCurrentGenerationNumber();
                    int dcD = dcDb.GetCurrentDailyCounter(DateTime.Today);

                    var eftDb = new EFT_Collections.DatabaseService();
                    int eftG = eftDb.PeekGenerationNumber();
                    int eftD = eftDb.PeekDailyCounter(DateTime.Today);

                    return (dcG, dcD, eftG, eftD);
                });

                lblDcGenerationNumber.Text = $"DC GenerationNumber: {dcGen}";
                lblDcDailyCounter.Text = $"DC DailyCounter: {dcDaily}";
                lblEftGenerationNumber.Text = $"EFT GenerationNumber: {eftGen}";
                lblEftDailyCounter.Text = $"EFT DailyCounter: {eftDaily}";
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                UnlockTab(tabOperations);
            }
        }

        /// <summary>
        /// Asynchronously loads the library folder paths from user settings,
        /// populates the UI list box, and initiates the file scan.
        /// </summary>
        private async Task LoadLibraryPathsAsync()
        {
            LockTab(tabLibrary);
            try
            {
                var paths = await Task.Run(() => (_settings.LibraryPaths ?? new List<UserSettings.LibraryPathEntry>()).Where(p => Directory.Exists(p.Path)).ToList());
                _libraryPaths = paths;
                lvLibraryFolders.Items.Clear();
                foreach (var path in paths)
                {
                    var item = new ListViewItem(path.Path) { Checked = path.IncludeSubfolders };
                    lvLibraryFolders.Items.Add(item);
                }
                lvLibraryFolders.Enabled = true;
                pnlLibraryButtons.Enabled = true;
                await ScanLibraryFilesAsync();
            }
            finally
            {
                UnlockTab(tabLibrary);
            }
        }

        /// <summary>
        /// Handles the event for adding a new folder to the Library.
        /// It updates the settings and triggers a rescan of library files.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private async void btnLibraryAdd_Click(object? sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (!_libraryPaths.Any(p => p.Path == fbd.SelectedPath))
                {
                    var entry = new UserSettings.LibraryPathEntry { Path = fbd.SelectedPath, IncludeSubfolders = true };
                    _libraryPaths.Add(entry);
                    var item = new ListViewItem(entry.Path) { Checked = entry.IncludeSubfolders };
                    lvLibraryFolders.Items.Add(item);
                    _settings.LibraryPaths = _libraryPaths;
                    _settings.Save();
                    await ScanLibraryFilesAsync();
                }
            }
        }

        /// <summary>
        /// Handles the event for removing a selected folder from the Library.
        /// It updates the settings and triggers a rescan of library files.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private async void btnLibraryRemove_Click(object? sender, EventArgs e)
        {
            if (lvLibraryFolders.SelectedItems.Count > 0)
            {
                var path = lvLibraryFolders.SelectedItems[0].Text;
                var entry = _libraryPaths.FirstOrDefault(p => p.Path == path);
                if (entry != null)
                    _libraryPaths.Remove(entry);
                lvLibraryFolders.Items.Remove(lvLibraryFolders.SelectedItems[0]);
                _settings.LibraryPaths = _libraryPaths;
                _settings.Save();
                await ScanLibraryFilesAsync();
            }
        }

        private async void lvLibraryFolders_ItemChecked(object? sender, ItemCheckedEventArgs e)
        {
            var entry = _libraryPaths.FirstOrDefault(p => p.Path == e.Item.Text);
            if (entry != null)
            {
                entry.IncludeSubfolders = e.Item.Checked;
                _settings.LibraryPaths = _libraryPaths;
                _settings.Save();
                await ScanLibraryFilesAsync();
            }
        }

        private void PopulateBillingDates()
        {
            cmbBillingDate.Items.Clear();
            var today = DateTime.Today;
            var first = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
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