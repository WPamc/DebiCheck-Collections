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
        private System.Windows.Forms.Label lblLiveOutput;
        private System.Windows.Forms.TextBox txtLiveOutputFolder;
        private System.Windows.Forms.Button btnLiveOutputBrowse;
        private System.Windows.Forms.Label lblTestOutput;
        private System.Windows.Forms.TextBox txtTestOutputFolder;
        private System.Windows.Forms.Button btnTestOutputBrowse;
        private System.Windows.Forms.Button btnLiveOutputOpen;
        private System.Windows.Forms.Button btnTestOutputOpen;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnFolderBrowse;
        private System.Windows.Forms.ListBox lstFolderFiles;
        private System.Windows.Forms.Button btnParseSelected;
        private System.Windows.Forms.TextBox txtRaw;
        private System.Windows.Forms.TextBox txtReference;
        private System.Windows.Forms.Button btnLookup;
        private System.Windows.Forms.Button btnOpenCsv;
        private System.Windows.Forms.Panel pnlImportTop;
        private System.Windows.Forms.TextBox txtImportFolder;
        private System.Windows.Forms.Button btnImportBrowse;
        private System.Windows.Forms.Button btnImportRead;
        private System.Windows.Forms.Button btnImportParse;
        private System.Windows.Forms.ListView lvImportFiles;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chSize;
        private System.Windows.Forms.ColumnHeader chModified;
        private System.Windows.Forms.ColumnHeader chType;

        private void InitializeComponent()
        {
            btnTestOutputOpen = new Button();
            btnLiveOutputOpen = new Button();
            tabMain = new TabControl();
            tabOperations = new TabPage();
            label3 = new Label();
            groupBox1 = new GroupBox();
            txtLiveOutputFolder = new TextBox();
            label2 = new Label();
            btnLiveOutputBrowse = new Button();
            txtTestOutputFolder = new TextBox();
            label1 = new Label();
            btnTestOutputBrowse = new Button();
            dgvPossibleDuplicates = new DataGridView();
            btnCheckDuplicates = new Button();
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
            tpImportFiles = new TabPage();
            lvImportFiles = new ListView();
            chName = new ColumnHeader();
            chGenDate = new ColumnHeader();
            chGenTime = new ColumnHeader();
            chSize = new ColumnHeader();
            chModified = new ColumnHeader();
            chType = new ColumnHeader();
            pnlImportTop = new Panel();
            btnImportParse = new Button();
            btnImportRead = new Button();
            btnImportBrowse = new Button();
            txtImportFolder = new TextBox();
            lblLiveOutput = new Label();
            lblTestOutput = new Label();
            tabMain.SuspendLayout();
            tabOperations.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPossibleDuplicates).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDay).BeginInit();
            tabParse.SuspendLayout();
            tpImportFiles.SuspendLayout();
            pnlImportTop.SuspendLayout();
            SuspendLayout();
            // 
            // btnTestOutputOpen
            // 
            btnTestOutputOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTestOutputOpen.Location = new Point(489, 87);
            btnTestOutputOpen.Name = "btnTestOutputOpen";
            btnTestOutputOpen.Size = new Size(51, 23);
            btnTestOutputOpen.TabIndex = 19;
            btnTestOutputOpen.Text = "Open";
            btnTestOutputOpen.UseVisualStyleBackColor = true;
            btnTestOutputOpen.Click += btnTestOutputOpen_Click;
            // 
            // btnLiveOutputOpen
            // 
            btnLiveOutputOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLiveOutputOpen.Location = new Point(489, 58);
            btnLiveOutputOpen.Name = "btnLiveOutputOpen";
            btnLiveOutputOpen.Size = new Size(51, 23);
            btnLiveOutputOpen.TabIndex = 18;
            btnLiveOutputOpen.Text = "Open";
            btnLiveOutputOpen.UseVisualStyleBackColor = true;
            btnLiveOutputOpen.Click += btnLiveOutputOpen_Click;
            // 
            // tabMain
            // 
            tabMain.Controls.Add(tabOperations);
            tabMain.Controls.Add(tabParse);
            tabMain.Controls.Add(tpImportFiles);
            tabMain.Dock = DockStyle.Fill;
            tabMain.Location = new Point(0, 0);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(1189, 637);
            tabMain.TabIndex = 0;
            // 
            // tabOperations
            // 
            tabOperations.Controls.Add(label3);
            tabOperations.Controls.Add(groupBox1);
            tabOperations.Controls.Add(dgvPossibleDuplicates);
            tabOperations.Controls.Add(btnCheckDuplicates);
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
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(77, 55);
            label3.Name = "label3";
            label3.Size = new Size(85, 15);
            label3.TabIndex = 15;
            label3.Text = "Deduction Day";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtLiveOutputFolder);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(btnLiveOutputBrowse);
            groupBox1.Controls.Add(btnLiveOutputOpen);
            groupBox1.Controls.Add(txtTestOutputFolder);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(btnTestOutputBrowse);
            groupBox1.Controls.Add(btnTestOutputOpen);
            groupBox1.Location = new Point(8, 166);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(544, 253);
            groupBox1.TabIndex = 15;
            groupBox1.TabStop = false;
            groupBox1.Text = "Config";
            // 
            // txtLiveOutputFolder
            // 
            txtLiveOutputFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLiveOutputFolder.Location = new Point(112, 58);
            txtLiveOutputFolder.Name = "txtLiveOutputFolder";
            txtLiveOutputFolder.Size = new Size(314, 23);
            txtLiveOutputFolder.TabIndex = 8;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(1, 61);
            label2.Name = "label2";
            label2.Size = new Size(105, 15);
            label2.TabIndex = 14;
            label2.Text = "Live Output Folder";
            // 
            // btnLiveOutputBrowse
            // 
            btnLiveOutputBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLiveOutputBrowse.Location = new Point(432, 58);
            btnLiveOutputBrowse.Name = "btnLiveOutputBrowse";
            btnLiveOutputBrowse.Size = new Size(51, 23);
            btnLiveOutputBrowse.TabIndex = 9;
            btnLiveOutputBrowse.Text = "Browse";
            btnLiveOutputBrowse.UseVisualStyleBackColor = true;
            btnLiveOutputBrowse.Click += btnLiveOutputBrowse_Click;
            // 
            // txtTestOutputFolder
            // 
            txtTestOutputFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTestOutputFolder.Location = new Point(112, 87);
            txtTestOutputFolder.Name = "txtTestOutputFolder";
            txtTestOutputFolder.Size = new Size(314, 23);
            txtTestOutputFolder.TabIndex = 11;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(1, 91);
            label1.Name = "label1";
            label1.Size = new Size(105, 15);
            label1.TabIndex = 13;
            label1.Text = "Test Output Folder";
            // 
            // btnTestOutputBrowse
            // 
            btnTestOutputBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTestOutputBrowse.Location = new Point(432, 87);
            btnTestOutputBrowse.Name = "btnTestOutputBrowse";
            btnTestOutputBrowse.Size = new Size(51, 23);
            btnTestOutputBrowse.TabIndex = 12;
            btnTestOutputBrowse.Text = "Browse";
            btnTestOutputBrowse.UseVisualStyleBackColor = true;
            btnTestOutputBrowse.Click += btnTestOutputBrowse_Click;
            // 
            // dgvPossibleDuplicates
            // 
            dgvPossibleDuplicates.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPossibleDuplicates.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPossibleDuplicates.Location = new Point(558, 6);
            dgvPossibleDuplicates.Name = "dgvPossibleDuplicates";
            dgvPossibleDuplicates.Size = new Size(617, 593);
            dgvPossibleDuplicates.TabIndex = 17;
            // 
            // btnCheckDuplicates
            // 
            btnCheckDuplicates.Location = new Point(95, 136);
            btnCheckDuplicates.Name = "btnCheckDuplicates";
            btnCheckDuplicates.Size = new Size(120, 23);
            btnCheckDuplicates.TabIndex = 16;
            btnCheckDuplicates.Text = "Check Duplicates";
            btnCheckDuplicates.UseVisualStyleBackColor = true;
            btnCheckDuplicates.Click += btnCheckDuplicates_Click;
            // 
            // chkTest
            // 
            chkTest.AutoSize = true;
            chkTest.Location = new Point(147, 82);
            chkTest.Name = "chkTest";
            chkTest.Size = new Size(68, 19);
            chkTest.TabIndex = 6;
            chkTest.Text = "Test File";
            chkTest.UseVisualStyleBackColor = true;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(95, 107);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(120, 23);
            btnGenerate.TabIndex = 2;
            btnGenerate.Text = "Generate File";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // nudDay
            // 
            nudDay.Location = new Point(177, 53);
            nudDay.Maximum = new decimal(new int[] { 31, 0, 0, 0 });
            nudDay.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudDay.Name = "nudDay";
            nudDay.Size = new Size(38, 23);
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
            // tpImportFiles
            // 
            tpImportFiles.Controls.Add(lvImportFiles);
            tpImportFiles.Controls.Add(pnlImportTop);
            tpImportFiles.Location = new Point(4, 24);
            tpImportFiles.Name = "tpImportFiles";
            tpImportFiles.Padding = new Padding(3);
            tpImportFiles.Size = new Size(1181, 609);
            tpImportFiles.TabIndex = 2;
            tpImportFiles.Text = "Import Files";
            tpImportFiles.UseVisualStyleBackColor = true;
            // 
            // lvImportFiles
            // 
            lvImportFiles.Columns.AddRange(new ColumnHeader[] { chName, chGenDate, chGenTime, chSize, chModified, chType });
            lvImportFiles.Dock = DockStyle.Fill;
            lvImportFiles.FullRowSelect = true;
            lvImportFiles.Location = new Point(3, 35);
            lvImportFiles.Name = "lvImportFiles";
            lvImportFiles.Size = new Size(1175, 571);
            lvImportFiles.TabIndex = 1;
            lvImportFiles.UseCompatibleStateImageBehavior = false;
            lvImportFiles.View = View.Details;
            lvImportFiles.SelectedIndexChanged += lvImportFiles_SelectedIndexChanged;
            lvImportFiles.ColumnClick += lvImportFiles_ColumnClick;
            // 
            // chName
            //
            chName.Text = "Name";
            chName.Width = 400;

            // chGenDate
            //
            chGenDate.Text = "Gen Date";
            chGenDate.Width = 100;

            // chGenTime
            //
            chGenTime.Text = "Gen Time";
            chGenTime.Width = 80;
            //
            // chSize
            //
            chSize.Text = "Size";
            chSize.Width = 100;
            // 
            // chModified
            // 
            chModified.Text = "Modified";
            chModified.Width = 200;
            //
            // chType
            //
            chType.Text = "Type";
            chType.Width = 120;
            //
            // pnlImportTop
            // 
            pnlImportTop.Controls.Add(btnImportParse);
            pnlImportTop.Controls.Add(btnImportRead);
            pnlImportTop.Controls.Add(btnImportBrowse);
            pnlImportTop.Controls.Add(txtImportFolder);
            pnlImportTop.Dock = DockStyle.Top;
            pnlImportTop.Location = new Point(3, 3);
            pnlImportTop.Name = "pnlImportTop";
            pnlImportTop.Size = new Size(1175, 32);
            pnlImportTop.TabIndex = 0;
            // 
            // btnImportParse
            // 
            btnImportParse.Enabled = false;
            btnImportParse.Location = new Point(461, 4);
            btnImportParse.Name = "btnImportParse";
            btnImportParse.Size = new Size(100, 23);
            btnImportParse.TabIndex = 3;
            btnImportParse.Text = "Import";
            btnImportParse.UseVisualStyleBackColor = true;
            btnImportParse.Click += btnImportParse_Click;
            // 
            // btnImportRead
            // 
            btnImportRead.Enabled = false;
            btnImportRead.Location = new Point(355, 4);
            btnImportRead.Name = "btnImportRead";
            btnImportRead.Size = new Size(100, 23);
            btnImportRead.TabIndex = 2;
            btnImportRead.Text = "Read File";
            btnImportRead.UseVisualStyleBackColor = true;
            btnImportRead.Click += btnImportRead_Click;
            // 
            // btnImportBrowse
            // 
            btnImportBrowse.Location = new Point(249, 4);
            btnImportBrowse.Name = "btnImportBrowse";
            btnImportBrowse.Size = new Size(100, 23);
            btnImportBrowse.TabIndex = 1;
            btnImportBrowse.Text = "Browse";
            btnImportBrowse.UseVisualStyleBackColor = true;
            btnImportBrowse.Click += btnImportBrowse_Click;
            // 
            // txtImportFolder
            // 
            txtImportFolder.Location = new Point(3, 5);
            txtImportFolder.Name = "txtImportFolder";
            txtImportFolder.Size = new Size(240, 23);
            txtImportFolder.TabIndex = 0;
            // 
            // lblLiveOutput
            // 
            lblLiveOutput.AutoSize = true;
            lblLiveOutput.Location = new Point(6, 157);
            lblLiveOutput.Name = "lblLiveOutput";
            lblLiveOutput.Size = new Size(115, 15);
            lblLiveOutput.TabIndex = 7;
            lblLiveOutput.Text = "Live Output Folder";
            // 
            // lblTestOutput
            // 
            lblTestOutput.AutoSize = true;
            lblTestOutput.Location = new Point(6, 204);
            lblTestOutput.Name = "lblTestOutput";
            lblTestOutput.Size = new Size(113, 15);
            lblTestOutput.TabIndex = 10;
            lblTestOutput.Text = "Test Output Folder";
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
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPossibleDuplicates).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDay).EndInit();
            tabParse.ResumeLayout(false);
            tabParse.PerformLayout();
            tpImportFiles.ResumeLayout(false);
            pnlImportTop.ResumeLayout(false);
            pnlImportTop.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabPage tpImportFiles;
        private Label label3;
        private GroupBox groupBox1;
        private Label label2;
        private Label label1;
        private Button btnCheckDuplicates;
        private DataGridView dgvPossibleDuplicates;
        private ColumnHeader chGenDate;
        private ColumnHeader chGenTime;
    }
}
