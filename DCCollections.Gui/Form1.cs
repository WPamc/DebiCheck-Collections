using Microsoft.Extensions.Configuration;
using System.IO;

namespace DCCollections.Gui
{
    public partial class Form1 : Form
    {
        private readonly RMCollectionProcessor.CollectionService _service;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        public Form1()
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
                var file = _service.GenerateFile(day, _config);
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
    }
}
