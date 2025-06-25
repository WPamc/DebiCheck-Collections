using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace DCCollections.Gui
{
    public class FilePreviewForm : Form
    {
        private readonly string _filePath;
        private Label lblFileName;
        private TextBox txtContent;
        private Button btnOpenLocation;

        public FilePreviewForm(string filePath)
        {
            _filePath = filePath;
            InitializeComponent();
            lblFileName.Text = Path.GetFileName(filePath);
            try
            {
                txtContent.Text = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                txtContent.Text = ex.Message;
            }
        }

        private void InitializeComponent()
        {
            lblFileName = new Label();
            txtContent = new TextBox();
            btnOpenLocation = new Button();
            SuspendLayout();
            // 
            // lblFileName
            // 
            lblFileName.AutoSize = true;
            lblFileName.Dock = DockStyle.Top;
            lblFileName.Padding = new Padding(5);
            lblFileName.Text = "File";
            // 
            // btnOpenLocation
            // 
            btnOpenLocation.Dock = DockStyle.Bottom;
            btnOpenLocation.Text = "Open File Location";
            btnOpenLocation.Height = 30;
            btnOpenLocation.Click += BtnOpenLocation_Click;
            // 
            // txtContent
            // 
            txtContent.Multiline = true;
            txtContent.ReadOnly = true;
            txtContent.Dock = DockStyle.Fill;
            txtContent.ScrollBars = ScrollBars.Both;
            txtContent.Font = new System.Drawing.Font("Consolas", 9F);
            // 
            // FilePreviewForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 600);
            Controls.Add(txtContent);
            Controls.Add(btnOpenLocation);
            Controls.Add(lblFileName);
            Text = "Preview";
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
            PerformLayout();
        }

        private void BtnOpenLocation_Click(object? sender, EventArgs e)
        {
            try
            {
                var dir = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrWhiteSpace(dir) && Directory.Exists(dir))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", dir) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
