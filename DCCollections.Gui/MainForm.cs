using Microsoft.Extensions.Configuration;
using System.IO;

namespace DCCollections.Gui
{
    public partial class MainForm : Form
    {
        private readonly RMCollectionProcessor.CollectionService _service;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;
        private object[]? _parsedRecords;

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
            _service = new RMCollectionProcessor.CollectionService();
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _parsedRecords = _service.ParseFile(ofd.FileName);
                    MessageBox.Show($"Parsed {_parsedRecords.Length} records.", "Success");
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
                var file = _service.GenerateFile(day, _config, test);
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
                try
                {
                    lstFiles.Items.Clear();
                    foreach (var file in Directory.GetFiles(fbd.SelectedPath))
                    {
                        lstFiles.Items.Add(Path.GetFileName(file));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        private void btnShowCurrent_Click(object sender, EventArgs e)
        {
            try
            {
                lstFiles.Items.Clear();
                foreach (var file in Directory.GetFiles(AppContext.BaseDirectory))
                {
                    lstFiles.Items.Add(Path.GetFileName(file));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnFolderBrowse_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtFolder.Text = fbd.SelectedPath;
                LoadFolderFiles(fbd.SelectedPath);
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
                    _parsedRecords = _service.ParseFile(item.Path);
                    txtRaw.Text = File.ReadAllText(item.Path);
                    MessageBox.Show($"Parsed {_parsedRecords.Length} records.", "Success");
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
    }
}
