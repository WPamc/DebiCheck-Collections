namespace DCCollections.Gui
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabOperations;
        private System.Windows.Forms.TabPage tabParse;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.NumericUpDown nudDay;
        private System.Windows.Forms.CheckBox chkTest;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnFolderBrowse;
        private System.Windows.Forms.ListBox lstFolderFiles;
        private System.Windows.Forms.Button btnParseSelected;
        private System.Windows.Forms.TextBox txtRaw;
        private System.Windows.Forms.TextBox txtReference;
        private System.Windows.Forms.Button btnLookup;
        private System.Windows.Forms.Button btnOpenCsv;

        private void InitializeComponent()
        {
            tabMain = new TabControl();
            tabOperations = new TabPage();
            chkTest = new CheckBox();
            btnGenerate = new Button();
            nudDay = new NumericUpDown();
            tabParse = new TabPage();
            txtRaw = new TextBox();
            btnParseSelected = new Button();
            lstFolderFiles = new ListBox();
            btnFolderBrowse = new Button();
            txtFolder = new TextBox();
            txtReference = new TextBox();
            btnLookup = new Button();
            btnOpenCsv = new Button();
            tabMain.SuspendLayout();
            tabOperations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudDay).BeginInit();
            tabParse.SuspendLayout();
            SuspendLayout();
            // 
            // tabMain
            // 
            tabMain.Controls.Add(tabOperations);
            tabMain.Controls.Add(tabParse);
            tabMain.Dock = DockStyle.Fill;
            tabMain.Location = new Point(0, 0);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(1189, 637);
            tabMain.TabIndex = 0;
            // 
            // tabOperations
            // 
            tabOperations.Controls.Add(chkTest);
            tabOperations.Controls.Add(btnGenerate);
            tabOperations.Controls.Add(nudDay);
            tabOperations.Location = new Point(4, 24);
            tabOperations.Name = "tabOperations";
            tabOperations.Padding = new Padding(3);
            tabOperations.Size = new Size(1181, 609);
            tabOperations.TabIndex = 0;
            tabOperations.Text = "Operations";
            tabOperations.UseVisualStyleBackColor = true;
            // 
            // chkTest
            // 
            chkTest.AutoSize = true;
            chkTest.Location = new Point(6, 122);
            chkTest.Name = "chkTest";
            chkTest.Size = new Size(68, 19);
            chkTest.TabIndex = 6;
            chkTest.Text = "Test File";
            chkTest.UseVisualStyleBackColor = true;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(6, 64);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(120, 23);
            btnGenerate.TabIndex = 2;
            btnGenerate.Text = "Generate File";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // nudDay
            // 
            nudDay.Location = new Point(6, 35);
            nudDay.Maximum = new decimal(new int[] { 31, 0, 0, 0 });
            nudDay.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudDay.Name = "nudDay";
            nudDay.Size = new Size(120, 23);
            nudDay.TabIndex = 1;
            nudDay.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // tabParse
            // 
            tabParse.Controls.Add(txtRaw);
            tabParse.Controls.Add(btnParseSelected);
            tabParse.Controls.Add(lstFolderFiles);
            tabParse.Controls.Add(btnFolderBrowse);
            tabParse.Controls.Add(txtFolder);
            tabParse.Controls.Add(txtReference);
            tabParse.Controls.Add(btnLookup);
            tabParse.Controls.Add(btnOpenCsv);
            tabParse.Location = new Point(4, 24);
            tabParse.Name = "tabParse";
            tabParse.Padding = new Padding(3);
            tabParse.Size = new Size(1181, 609);
            tabParse.TabIndex = 1;
            tabParse.Text = "Parse Files";
            tabParse.UseVisualStyleBackColor = true;
            // 
            // txtRaw
            // 
            txtRaw.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtRaw.Location = new Point(570, 35);
            txtRaw.Multiline = true;
            txtRaw.Name = "txtRaw";
            txtRaw.ScrollBars = ScrollBars.Vertical;
            txtRaw.Size = new Size(605, 575);
            txtRaw.TabIndex = 4;
            // 
            // btnParseSelected
            // 
            btnParseSelected.Enabled = false;
            btnParseSelected.Location = new Point(358, 6);
            btnParseSelected.Name = "btnParseSelected";
            btnParseSelected.Size = new Size(120, 23);
            btnParseSelected.TabIndex = 3;
            btnParseSelected.Text = "Parse File";
            btnParseSelected.UseVisualStyleBackColor = true;
            btnParseSelected.Click += btnParseSelected_Click;
            // 
            // lstFolderFiles
            // 
            lstFolderFiles.FormattingEnabled = true;
            lstFolderFiles.ItemHeight = 15;
            lstFolderFiles.Location = new Point(6, 35);
            lstFolderFiles.Name = "lstFolderFiles";
            lstFolderFiles.Size = new Size(560, 199);
            lstFolderFiles.TabIndex = 2;
            lstFolderFiles.SelectedIndexChanged += lstFolderFiles_SelectedIndexChanged;
            // 
            // btnFolderBrowse
            // 
            btnFolderBrowse.Location = new Point(252, 6);
            btnFolderBrowse.Name = "btnFolderBrowse";
            btnFolderBrowse.Size = new Size(100, 23);
            btnFolderBrowse.TabIndex = 1;
            btnFolderBrowse.Text = "Browse";
            btnFolderBrowse.UseVisualStyleBackColor = true;
            btnFolderBrowse.Click += btnFolderBrowse_Click;
            // 
            // txtFolder
            // 
            txtFolder.Location = new Point(6, 6);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new Size(240, 23);
            txtFolder.TabIndex = 0;
            // 
            // txtReference
            // 
            txtReference.Location = new Point(6, 238);
            txtReference.Name = "txtReference";
            txtReference.Size = new Size(121, 23);
            txtReference.TabIndex = 7;
            // 
            // btnLookup
            // 
            btnLookup.Location = new Point(6, 263);
            btnLookup.Name = "btnLookup";
            btnLookup.Size = new Size(120, 23);
            btnLookup.TabIndex = 8;
            btnLookup.Text = "Lookup Ref";
            btnLookup.UseVisualStyleBackColor = true;
            btnLookup.Click += btnLookup_Click;
            // 
            // btnOpenCsv
            // 
            btnOpenCsv.Location = new Point(132, 263);
            btnOpenCsv.Name = "btnOpenCsv";
            btnOpenCsv.Size = new Size(120, 23);
            btnOpenCsv.TabIndex = 9;
            btnOpenCsv.Text = "Open CSV";
            btnOpenCsv.UseVisualStyleBackColor = true;
            btnOpenCsv.Click += btnOpenCsv_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1189, 637);
            Controls.Add(tabMain);
            Name = "MainForm";
            Text = "Collections";
            tabMain.ResumeLayout(false);
            tabOperations.ResumeLayout(false);
            tabOperations.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudDay).EndInit();
            tabParse.ResumeLayout(false);
            tabParse.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}
