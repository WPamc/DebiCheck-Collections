using System.Diagnostics;
using DbConnection;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using RMCollectionProcessor.Models;
using RMCollectionProcessor;

namespace DCCollections.Gui
{
    public partial class MainForm : Form
    {
        private readonly RMCollectionProcessor.CollectionService _service;
        private object[]? _parsedRecords;
        private FileType _currentFileType;
        private readonly UserSettings _settings;
        private int _importSortColumn;
        private bool _importSortDescending;

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

            // Display the connection string (without the password) to the user
            var connStr = DbConnection.AppConfig.ConnectionString;
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                var safeConn = RemovePassword(connStr);
                MessageBox.Show(safeConn, "Database Connection");
                Text = $"Collections - {safeConn}";
            }

            _service = new RMCollectionProcessor.CollectionService();
            _settings = UserSettings.Load();
            WindowState = FormWindowState.Maximized;
            MaximizeBox = true;
            chkTest.Checked = true;
            LoadInitialPaths();
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
                    var result = _service.ParseFile(ofd.FileName);
                    _parsedRecords = result.Records;
                    _currentFileType = result.FileType;
                    MessageBox.Show($"Parsed {_parsedRecords.Length} records (Type: {_currentFileType}).", "Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                int day = (int)nudDay.Value;
                bool test = chkTest.Checked;
                string? outFolder = test ? _settings.TestOutputFolderPath : _settings.LiveOutputFolderPath;
                var file = _service.GenerateFile(day, test, outFolder);
                MessageBox.Show($"File generated: {file}", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
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
                var table = _service.GetDuplicateCollections(day);
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
                        if (records.Length > 0 && records[0] is TransmissionHeader000 th)
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
            if (hit.Item != null && hit.Item.SubItems.IndexOf(hit.SubItem) == 0)
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

        private void btnImportParse_Click(object sender, EventArgs e)
        {
            if (lvImportFiles.SelectedItems.Count == 0)
                return;

            var tagObj = lvImportFiles.SelectedItems[0].Tag;
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
                return;

            if (status.Equals("T", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Test Files cannot be imported.");
                return;
            }

            try
            {
                var result = _service.ParseFile(path);
                _parsedRecords = result.Records;
                _currentFileType = result.FileType;
                MessageBox.Show($"Imported {_parsedRecords.Length} records (Type: {_currentFileType}).", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
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
    }
}
