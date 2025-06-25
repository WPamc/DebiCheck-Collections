using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;
using System.Data;
using RMCollectionProcessor.Models;

namespace DCCollections.Gui
{
    public partial class MainForm : Form
    {
        private readonly RMCollectionProcessor.CollectionService _service;
        private readonly IConfiguration _config;
        private object[]? _parsedRecords;
        private FileType _currentFileType;
        private readonly UserSettings _settings;

        private class FileListItem
        {
            public string Path { get; }
            public FileListItem(string path) => Path = path;
            public override string ToString()
            {
                var info = new FileInfo(Path);
                return $"{info.Name} | C:{info.CreationTime:yyyy-MM-dd} M:{info.LastWriteTime:yyyy-MM-dd} | {info.Length} bytes";
            }
        }

        public MainForm()
        {
            InitializeComponent();

            _config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            // Display the connection string (without the password) to the user
            var connStr = _config.GetConnectionString("DefaultConnection");
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

        private void btnParse_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var result = _service.ParseFile(ofd.FileName, _config);
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
                var file = _service.GenerateFile(day, _config, test, outFolder);
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

        private void btnFolderBrowse_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtFolder.Text = fbd.SelectedPath;
                _settings.ParseFolderPath = fbd.SelectedPath;
                LoadFolderFiles(fbd.SelectedPath);
            }
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

        private void LoadFolderFiles(string path)
        {
            try
            {
                lstFolderFiles.Items.Clear();
                foreach (var file in Directory.GetFiles(path))
                {
                    lstFolderFiles.Items.Add(new FileListItem(file));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void lstFolderFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnParseSelected.Enabled = lstFolderFiles.SelectedItem != null;
        }

        private void btnParseSelected_Click(object sender, EventArgs e)
        {
            if (lstFolderFiles.SelectedItem is FileListItem item)
            {
                try
                {
                    var result = _service.ParseFile(item.Path,_config);
                    _parsedRecords = result.Records;
                    _currentFileType = result.FileType;
                    txtRaw.Text = File.ReadAllText(item.Path);
                    MessageBox.Show($"Parsed {_parsedRecords.Length} records (Type: {_currentFileType}).", "Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        private void btnLookup_Click(object sender, EventArgs e)
        {
            try
            {
                var reference = txtReference.Text.Trim();
                if (string.IsNullOrEmpty(reference))
                {
                    MessageBox.Show("Enter a reference", "Info");
                    return;
                }

                if (_parsedRecords == null)
                {
                    MessageBox.Show("Parse a file first", "Info");
                    return;
                }

                foreach (var obj in _parsedRecords)
                {
                    if (obj is RMCollectionProcessor.Models.CollectionTxLine02 l2 && l2.MandateReference.Trim() == reference)
                    {
                        MessageBox.Show($"Found mandate reference in sequence {l2.RecordSequenceNumber}", "Result");
                        return;
                    }
                    if (obj is RMCollectionProcessor.Models.CollectionTxLine03 l3 && l3.ContractReference.Trim() == reference)
                    {
                        MessageBox.Show($"Found contract reference in sequence {l3.RecordSequenceNumber}", "Result");
                        return;
                    }
                }

                MessageBox.Show("No record found", "Info");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnOpenCsv_Click(object sender, EventArgs e)
        {
            try
            {
                var csv = Path.Combine(AppContext.BaseDirectory, "transactions.csv");
                if (File.Exists(csv))
                {
                    Process.Start(new ProcessStartInfo(csv) { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show($"File not found: {csv}", "Error");
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
                var table = _service.GetDuplicateCollections(day, _config);
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
            if (!string.IsNullOrWhiteSpace(_settings.ParseFolderPath) && Directory.Exists(_settings.ParseFolderPath))
            {
                txtFolder.Text = _settings.ParseFolderPath;
                LoadFolderFiles(_settings.ParseFolderPath);
            }

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
                LoadImportFiles(fbd.SelectedPath);
            }
        }

        private void LoadImportFiles(string path)
        {
            try
            {
                lvImportFiles.Items.Clear();
                foreach (var file in Directory.GetFiles(path))
                {
                    var info = new FileInfo(file);
                    var size = info.Length > 1024 ? $"{info.Length / 1024} KB" : $"{info.Length} bytes";
                    var item = new ListViewItem(info.Name)
                    {
                        Tag = info.FullName
                    };
                    item.SubItems.Add(size);
                    item.SubItems.Add(info.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
                    lvImportFiles.Items.Add(item);
                }
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

        private void btnImportParse_Click(object sender, EventArgs e)
        {
            if (lvImportFiles.SelectedItems.Count == 0)
                return;

            var path = lvImportFiles.SelectedItems[0].Tag as string;
            if (string.IsNullOrWhiteSpace(path))
                return;

            try
            {
                var result = _service.ParseFile(path, _config);
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

            var path = lvImportFiles.SelectedItems[0].Tag as string;
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _settings.Save();
        }
    }
}
