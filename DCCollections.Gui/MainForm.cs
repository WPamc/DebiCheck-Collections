using System.Diagnostics;
using DbConnection;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using RMCollectionProcessor.Models;
using RMCollectionProcessor;
using EFT_Collections;
using DCService = global::DatabaseService;

namespace DCCollections.Gui
{
    public partial class MainForm : Form
    {
        private readonly RMCollectionProcessor.CollectionService _dcCollectionservice;
        private object[]? _parsedRecords;
        private FileType _currentFileType;
        private readonly UserSettings _settings;
        private int _importSortColumn;
        private bool _importSortDescending;
        private ProgressBar _pbImport;
        private ProgressBar _pbOperations;

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

            var connStr = DbConnection.AppConfig.ConnectionString;
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                var safeConn = RemovePassword(connStr);
                MessageBox.Show(safeConn, "Database Connection");
                Text = $"Collections - {safeConn}";
            }

            _dcCollectionservice = new RMCollectionProcessor.CollectionService();
            _settings = UserSettings.Load();
            WindowState = FormWindowState.Maximized;
            MaximizeBox = true;
            chkTest.Checked = true;
            chkHideTestFiles.Checked = true;
            _pbImport = new ProgressBar { Dock = DockStyle.Bottom, Style = ProgressBarStyle.Marquee, Visible = false };
            tpImportFiles.Controls.Add(_pbImport);
            _pbOperations = new ProgressBar { Dock = DockStyle.Bottom, Style = ProgressBarStyle.Marquee, Visible = false };
            tabOperations.Controls.Add(_pbOperations);
            lvImportFiles.MultiSelect = true;
            LoadInitialPaths();
            LoadCounters();
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
            catch { }
            return (string.Empty, string.Empty);
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
                    if (_currentFileType == FileType.StatusReport)
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
                string file;
                if (rdoDebiCheck.Checked)
                {
                    file = await Task.Run(() => _dcCollectionservice.GenerateDCFile(day, test, outFolder));
                }
                else
                {
                    var date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, day);
                    string folder = outFolder ?? AppContext.BaseDirectory;
                    file = await Task.Run(() => EFTService.GenerateEFTFile(date, test, folder));
                }
                MessageBox.Show($"File generated: {file}", "Success");
                using var form = new FilePreviewForm(file);
                form.ShowDialog(this);
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
            }
        }

        private void btnTestOutputBrowse_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtTestOutputFolder.Text = fbd.SelectedPath;
                _settings.TestOutputFolderPath = fbd.SelectedPath;
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
                var table = _dcCollectionservice.GetDuplicateCollections(day);
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
                //lstFiles.Items.Clear();
                foreach (var file in Directory.GetFiles(path))
                {
                  //  lstFiles.Items.Add(new FileListItem(file));
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

        private void LoadImportFiles(string path)
        {
            try
            {
                lvImportFiles.Items.Clear();

                bool hideTests = chkHideTestFiles.Checked;

                var processor = new FileProcessor();

                foreach (var file in Directory.GetFiles(path))
                {
                    var info = new FileInfo(file);
                    var size = info.Length > 1024 ? $"{info.Length / 1024} KB" : $"{info.Length} bytes";
                    FileType type = FileType.Unknown;
                    string recordStatus = string.Empty;
                    try
                    {
                        var fileProcessor = new FileProcessor();
                        var records = fileProcessor.ProcessFile(file);
                        type = FileTypeIdentifier.Identify(records);
                        if (records.Length > 0 && records[0] is RMCollectionProcessor.Models.TransmissionHeader000 th)
                            recordStatus = th.RecordStatus?.Trim() ?? string.Empty;
                    }
                    catch { }

                    bool isLive = recordStatus.Equals("L", StringComparison.OrdinalIgnoreCase);
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

                    var desc = type.ToString();
                    if (!isLive && recordStatus.Length > 0)
                        desc += " (Test)";
                    item.SubItems.Add(desc);

                    try
                    {
                        var parsed = processor.ProcessFile(info.FullName);
                        var ft = FileTypeIdentifier.Identify(parsed);
                        item.SubItems.Add(ft.ToString());
                    }
                    catch
                    {
                        item.SubItems.Add(FileType.Unknown.ToString());
                    }

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
                _importSortDescending = !_importSortDescending;
            else
            {
                _importSortColumn = e.Column;
                _importSortDescending = false;
            }

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
                foreach (ListViewItem item in lvImportFiles.SelectedItems)
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

                    var result = await Task.Run(() => _dcCollectionservice.ParseFile(path));
                    if (result.FileType == FileType.StatusReport)
                    {
                        totalFound += result.StatusRecordsFound;
                        totalInserted += result.StatusRecordsInserted;
                    }
                }

                var msg = "Import complete.";
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _settings.ImportSortColumn = _importSortColumn;
            _settings.ImportSortDescending = _importSortDescending;
            _settings.Save();
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
    }
}
