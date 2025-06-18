using Microsoft.Extensions.Configuration;
using System.IO;

namespace DCCollections.Gui
{
    public partial class MainForm : Form
    {
        private readonly RMCollectionProcessor.CollectionService _service;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

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
                    var result = _service.ParseFile(ofd.FileName);
                    MessageBox.Show($"Parsed {result.Length} records.", "Success");
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
                    var result = _service.ParseFile(item.Path);
                    txtRaw.Text = File.ReadAllText(item.Path);
                    MessageBox.Show($"Parsed {result.Length} records.", "Success");
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

                var result = _service.GetRequestByReference(reference, _config);
                if (result == null)
                {
                    MessageBox.Show("No record found", "Info");
                }
                else
                {
                    MessageBox.Show($"Row ID: {result.RowId}\nAmount: {result.AmountRequested}", "Result");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
