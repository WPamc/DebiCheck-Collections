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
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.NumericUpDown nudDay;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnShowCurrent;
        private System.Windows.Forms.ListBox lstFiles;
        private System.Windows.Forms.CheckBox chkTest;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnFolderBrowse;
        private System.Windows.Forms.ListBox lstFolderFiles;
        private System.Windows.Forms.Button btnParseSelected;
        private System.Windows.Forms.TextBox txtRaw;
        private System.Windows.Forms.TextBox txtReference;
        private System.Windows.Forms.Button btnLookup;

        private void InitializeComponent()
        {
            tabMain = new TabControl();
            tabOperations = new TabPage();
            lstFiles = new ListBox();
            chkTest = new CheckBox();
            btnBrowse = new Button();
            btnShowCurrent = new Button();
            btnGenerate = new Button();
            nudDay = new NumericUpDown();
            btnParse = new Button();
            tabParse = new TabPage();
            txtRaw = new TextBox();
            btnParseSelected = new Button();
            lstFolderFiles = new ListBox();
            btnFolderBrowse = new Button();
            txtFolder = new TextBox();
            txtReference = new TextBox();
            btnLookup = new Button();
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
            tabMain.Margin = new Padding(4, 5, 4, 5);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(1698, 1111);
            tabMain.TabIndex = 0;
            // 
            // tabOperations
            // 
            tabOperations.Controls.Add(lstFiles);
            tabOperations.Controls.Add(chkTest);
            tabOperations.Controls.Add(btnBrowse);
            tabOperations.Controls.Add(btnShowCurrent);
            tabOperations.Controls.Add(btnGenerate);
            tabOperations.Controls.Add(txtReference);
            tabOperations.Controls.Add(btnLookup);
            tabOperations.Controls.Add(nudDay);
            tabOperations.Controls.Add(btnParse);
            tabOperations.Location = new Point(4, 34);
            tabOperations.Margin = new Padding(4, 5, 4, 5);
            tabOperations.Name = "tabOperations";
            tabOperations.Padding = new Padding(4, 5, 4, 5);
            tabOperations.Size = new Size(949, 534);
            tabOperations.TabIndex = 0;
            tabOperations.Text = "Operations";
            tabOperations.UseVisualStyleBackColor = true;
            // 
            // lstFiles
            // 
            lstFiles.FormattingEnabled = true;
            lstFiles.ItemHeight = 25;
            lstFiles.Location = new Point(214, 58);
            lstFiles.Margin = new Padding(4, 5, 4, 5);
            lstFiles.Name = "lstFiles";
            lstFiles.Size = new Size(731, 479);
            lstFiles.TabIndex = 5;
            // 
            // chkTest
            // 
            chkTest.AutoSize = true;
            chkTest.Location = new Point(9, 203);
            chkTest.Margin = new Padding(4, 5, 4, 5);
            chkTest.Name = "chkTest";
            chkTest.Size = new Size(99, 29);
            chkTest.TabIndex = 6;
            chkTest.Text = "Test File";
            chkTest.UseVisualStyleBackColor = true;
            //
            // txtReference
            //
            txtReference.Location = new Point(9, 252);
            txtReference.Margin = new Padding(4, 5, 4, 5);
            txtReference.Name = "txtReference";
            txtReference.Size = new Size(171, 31);
            txtReference.TabIndex = 7;
            //
            // btnLookup
            //
            btnLookup.Location = new Point(9, 293);
            btnLookup.Margin = new Padding(4, 5, 4, 5);
            btnLookup.Name = "btnLookup";
            btnLookup.Size = new Size(171, 38);
            btnLookup.TabIndex = 8;
            btnLookup.Text = "Lookup Ref";
            btnLookup.UseVisualStyleBackColor = true;
            btnLookup.Click += btnLookup_Click;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(214, 10);
            btnBrowse.Margin = new Padding(4, 5, 4, 5);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(171, 38);
            btnBrowse.TabIndex = 3;
            btnBrowse.Text = "Browse Folder";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnShowCurrent
            // 
            btnShowCurrent.Location = new Point(9, 155);
            btnShowCurrent.Margin = new Padding(4, 5, 4, 5);
            btnShowCurrent.Name = "btnShowCurrent";
            btnShowCurrent.Size = new Size(171, 38);
            btnShowCurrent.TabIndex = 4;
            btnShowCurrent.Text = "Show App Files";
            btnShowCurrent.UseVisualStyleBackColor = true;
            btnShowCurrent.Click += btnShowCurrent_Click;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(9, 107);
            btnGenerate.Margin = new Padding(4, 5, 4, 5);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(171, 38);
            btnGenerate.TabIndex = 2;
            btnGenerate.Text = "Generate File";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // nudDay
            // 
            nudDay.Location = new Point(9, 58);
            nudDay.Margin = new Padding(4, 5, 4, 5);
            nudDay.Maximum = new decimal(new int[] { 31, 0, 0, 0 });
            nudDay.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudDay.Name = "nudDay";
            nudDay.Size = new Size(171, 31);
            nudDay.TabIndex = 1;
            nudDay.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btnParse
            // 
            btnParse.Location = new Point(9, 10);
            btnParse.Margin = new Padding(4, 5, 4, 5);
            btnParse.Name = "btnParse";
            btnParse.Size = new Size(171, 38);
            btnParse.TabIndex = 0;
            btnParse.Text = "Parse File";
            btnParse.UseVisualStyleBackColor = true;
            btnParse.Click += btnParse_Click;
            // 
            // tabParse
            // 
            tabParse.Controls.Add(txtRaw);
            tabParse.Controls.Add(btnParseSelected);
            tabParse.Controls.Add(lstFolderFiles);
            tabParse.Controls.Add(btnFolderBrowse);
            tabParse.Controls.Add(txtFolder);
            tabParse.Location = new Point(4, 34);
            tabParse.Margin = new Padding(4, 5, 4, 5);
            tabParse.Name = "tabParse";
            tabParse.Padding = new Padding(4, 5, 4, 5);
            tabParse.Size = new Size(1690, 1073);
            tabParse.TabIndex = 1;
            tabParse.Text = "Parse Files";
            tabParse.UseVisualStyleBackColor = true;
            // 
            // txtRaw
            // 
            txtRaw.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtRaw.Location = new Point(815, 58);
            txtRaw.Margin = new Padding(4, 5, 4, 5);
            txtRaw.Multiline = true;
            txtRaw.Name = "txtRaw";
            txtRaw.ScrollBars = ScrollBars.Vertical;
            txtRaw.Size = new Size(863, 1005);
            txtRaw.TabIndex = 4;
            // 
            // btnParseSelected
            // 
            btnParseSelected.Enabled = false;
            btnParseSelected.Location = new Point(511, 10);
            btnParseSelected.Margin = new Padding(4, 5, 4, 5);
            btnParseSelected.Name = "btnParseSelected";
            btnParseSelected.Size = new Size(171, 38);
            btnParseSelected.TabIndex = 3;
            btnParseSelected.Text = "Parse File";
            btnParseSelected.UseVisualStyleBackColor = true;
            btnParseSelected.Click += btnParseSelected_Click;
            // 
            // lstFolderFiles
            // 
            lstFolderFiles.FormattingEnabled = true;
            lstFolderFiles.ItemHeight = 25;
            lstFolderFiles.Location = new Point(9, 58);
            lstFolderFiles.Margin = new Padding(4, 5, 4, 5);
            lstFolderFiles.Name = "lstFolderFiles";
            lstFolderFiles.Size = new Size(798, 329);
            lstFolderFiles.TabIndex = 2;
            lstFolderFiles.SelectedIndexChanged += lstFolderFiles_SelectedIndexChanged;
            // 
            // btnFolderBrowse
            // 
            btnFolderBrowse.Location = new Point(360, 10);
            btnFolderBrowse.Margin = new Padding(4, 5, 4, 5);
            btnFolderBrowse.Name = "btnFolderBrowse";
            btnFolderBrowse.Size = new Size(143, 38);
            btnFolderBrowse.TabIndex = 1;
            btnFolderBrowse.Text = "Browse";
            btnFolderBrowse.UseVisualStyleBackColor = true;
            btnFolderBrowse.Click += btnFolderBrowse_Click;
            // 
            // txtFolder
            // 
            txtFolder.Location = new Point(9, 10);
            txtFolder.Margin = new Padding(4, 5, 4, 5);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new Size(341, 31);
            txtFolder.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1698, 1111);
            Controls.Add(tabMain);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
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
