﻿namespace DCCollections.Gui
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
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.NumericUpDown nudDay;
        private System.Windows.Forms.CheckBox chkTest;
        private System.Windows.Forms.RadioButton rdoDebiCheck;
        private System.Windows.Forms.RadioButton rdoEft;
        private System.Windows.Forms.Label lblLiveOutput;
        private System.Windows.Forms.TextBox txtLiveOutputFolder;
        private System.Windows.Forms.Button btnLiveOutputBrowse;
        private System.Windows.Forms.Label lblTestOutput;
        private System.Windows.Forms.TextBox txtTestOutputFolder;
        private System.Windows.Forms.Button btnTestOutputBrowse;
        private System.Windows.Forms.Button btnLiveOutputOpen;
        private System.Windows.Forms.Button btnTestOutputOpen;
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
        private System.Windows.Forms.ColumnHeader chTest;
        private System.Windows.Forms.ColumnHeader chImported;

        private System.Windows.Forms.ContextMenuStrip cmsImportFiles;
        private System.Windows.Forms.ToolStripMenuItem previewToolStripMenuItem;


        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            btnSearchFiles = new Button();
            btnTestOutputOpen = new Button();
            btnLiveOutputOpen = new Button();
            tabMain = new TabControl();
            tabOperations = new TabPage();
            grpCounters = new GroupBox();
            lblEftDailyCounter = new Label();
            lblEftGenerationNumber = new Label();
            lblDcDailyCounter = new Label();
            lblDcGenerationNumber = new Label();
            label3 = new Label();
            grpConfig = new GroupBox();
            txtLiveOutputFolder = new TextBox();
            label2 = new Label();
            btnLiveOutputBrowse = new Button();
            txtTestOutputFolder = new TextBox();
            label1 = new Label();
            btnTestOutputBrowse = new Button();
            dgvPossibleDuplicates = new DataGridView();
            btnCheckDuplicates = new Button();
            chkTest = new CheckBox();
            rdoDebiCheck = new RadioButton();
            rdoEft = new RadioButton();
            btnGenerate = new Button();
            nudDay = new NumericUpDown();
            cmbBillingDate = new ComboBox();
            label4 = new Label();
            tpImportFiles = new TabPage();
            lvImportFiles = new ListView();
            chName = new ColumnHeader();
            chGenDate = new ColumnHeader();
            chGenTime = new ColumnHeader();
            chSize = new ColumnHeader();
            chModified = new ColumnHeader();
            chType = new ColumnHeader();
            chTest = new ColumnHeader();
            chImported = new ColumnHeader();
            pnlImportTop = new Panel();
            btnImportParse = new Button();
            btnImportRead = new Button();
            txtSearchFiles = new TextBox();
            btnFindText = new Button();
            txtFindText = new TextBox();
            btnApplyFilter = new Button();
            txtFileFilter = new TextBox();
            btnImportBrowse = new Button();
            txtImportFolder = new TextBox();
            chkHideTestFiles = new CheckBox();
            lblLiveOutput = new Label();
            lblTestOutput = new Label();
            cmsImportFiles = new ContextMenuStrip(components);
            previewToolStripMenuItem = new ToolStripMenuItem();
            tabMain.SuspendLayout();
            tabOperations.SuspendLayout();
            grpCounters.SuspendLayout();
            grpConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPossibleDuplicates).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDay).BeginInit();
            tpImportFiles.SuspendLayout();
            pnlImportTop.SuspendLayout();
            cmsImportFiles.SuspendLayout();
            SuspendLayout();
            // 
            // btnSearchFiles
            // 
            btnSearchFiles.Location = new Point(1094, 4);
            btnSearchFiles.Name = "btnSearchFiles";
            btnSearchFiles.Size = new Size(75, 23);
            btnSearchFiles.TabIndex = 6;
            btnSearchFiles.Text = "Search";
            btnSearchFiles.UseVisualStyleBackColor = true;
            btnSearchFiles.Click += btnSearchFiles_Click;
            // 
            // btnTestOutputOpen
            // 
            btnTestOutputOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTestOutputOpen.Location = new Point(489, 50);
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
            btnLiveOutputOpen.Location = new Point(489, 21);
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
            tabOperations.Controls.Add(grpCounters);
            tabOperations.Controls.Add(label3);
            tabOperations.Controls.Add(grpConfig);
            tabOperations.Controls.Add(dgvPossibleDuplicates);
            tabOperations.Controls.Add(btnCheckDuplicates);
            tabOperations.Controls.Add(chkTest);
            tabOperations.Controls.Add(cmbBillingDate);
            tabOperations.Controls.Add(label4);
            tabOperations.Controls.Add(rdoDebiCheck);
            tabOperations.Controls.Add(rdoEft);
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
            // grpCounters
            // 
            grpCounters.Controls.Add(lblEftDailyCounter);
            grpCounters.Controls.Add(lblEftGenerationNumber);
            grpCounters.Controls.Add(lblDcDailyCounter);
            grpCounters.Controls.Add(lblDcGenerationNumber);
            grpCounters.Location = new Point(8, 262);
            grpCounters.Name = "grpCounters";
            grpCounters.Size = new Size(544, 337);
            grpCounters.TabIndex = 20;
            grpCounters.TabStop = false;
            grpCounters.Text = "Counters";
            // 
            // lblEftDailyCounter
            // 
            lblEftDailyCounter.AutoSize = true;
            lblEftDailyCounter.Location = new Point(16, 97);
            lblEftDailyCounter.Name = "lblEftDailyCounter";
            lblEftDailyCounter.Size = new Size(110, 15);
            lblEftDailyCounter.TabIndex = 3;
            lblEftDailyCounter.Text = "EFT DailyCounter: 0";
            // 
            // lblEftGenerationNumber
            // 
            lblEftGenerationNumber.AutoSize = true;
            lblEftGenerationNumber.Location = new Point(16, 72);
            lblEftGenerationNumber.Name = "lblEftGenerationNumber";
            lblEftGenerationNumber.Size = new Size(143, 15);
            lblEftGenerationNumber.TabIndex = 2;
            lblEftGenerationNumber.Text = "EFT GenerationNumber: 0";
            // 
            // lblDcDailyCounter
            // 
            lblDcDailyCounter.AutoSize = true;
            lblDcDailyCounter.Location = new Point(16, 47);
            lblDcDailyCounter.Name = "lblDcDailyCounter";
            lblDcDailyCounter.Size = new Size(107, 15);
            lblDcDailyCounter.TabIndex = 1;
            lblDcDailyCounter.Text = "DC DailyCounter: 0";
            // 
            // lblDcGenerationNumber
            // 
            lblDcGenerationNumber.AutoSize = true;
            lblDcGenerationNumber.Location = new Point(16, 22);
            lblDcGenerationNumber.Name = "lblDcGenerationNumber";
            lblDcGenerationNumber.Size = new Size(140, 15);
            lblDcGenerationNumber.TabIndex = 0;
            lblDcGenerationNumber.Text = "DC GenerationNumber: 0";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(77, 55);
            label3.Name = "label3";
            label3.Size = new Size(85, 15);
            label3.TabIndex = 15;
            label3.Text = "Deduction Day";
            label4.AutoSize = true;
            label4.Location = new Point(77, 82);
            label4.Name = "label4";
            label4.Size = new Size(73, 15);
            label4.TabIndex = 21;
            label4.Text = "Billing Date";
            // 
            // grpConfig
            // 
            grpConfig.Controls.Add(txtLiveOutputFolder);
            grpConfig.Controls.Add(label2);
            grpConfig.Controls.Add(btnLiveOutputBrowse);
            grpConfig.Controls.Add(btnLiveOutputOpen);
            grpConfig.Controls.Add(txtTestOutputFolder);
            grpConfig.Controls.Add(label1);
            grpConfig.Controls.Add(btnTestOutputBrowse);
            grpConfig.Controls.Add(btnTestOutputOpen);
            grpConfig.Location = new Point(8, 166);
            grpConfig.Name = "grpConfig";
            grpConfig.Size = new Size(544, 90);
            grpConfig.TabIndex = 15;
            grpConfig.TabStop = false;
            grpConfig.Text = "Config";
            // 
            // txtLiveOutputFolder
            // 
            txtLiveOutputFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLiveOutputFolder.Location = new Point(112, 21);
            txtLiveOutputFolder.Name = "txtLiveOutputFolder";
            txtLiveOutputFolder.Size = new Size(314, 23);
            txtLiveOutputFolder.TabIndex = 8;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(1, 24);
            label2.Name = "label2";
            label2.Size = new Size(105, 15);
            label2.TabIndex = 14;
            label2.Text = "Live Output Folder";
            // 
            // btnLiveOutputBrowse
            // 
            btnLiveOutputBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLiveOutputBrowse.Location = new Point(432, 21);
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
            txtTestOutputFolder.Location = new Point(112, 50);
            txtTestOutputFolder.Name = "txtTestOutputFolder";
            txtTestOutputFolder.Size = new Size(314, 23);
            txtTestOutputFolder.TabIndex = 11;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(1, 54);
            label1.Name = "label1";
            label1.Size = new Size(105, 15);
            label1.TabIndex = 13;
            label1.Text = "Test Output Folder";
            // 
            // btnTestOutputBrowse
            // 
            btnTestOutputBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTestOutputBrowse.Location = new Point(432, 50);
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
            // rdoDebiCheck
            // 
            rdoDebiCheck.AutoSize = true;
            rdoDebiCheck.Checked = true;
            rdoDebiCheck.Location = new Point(95, 24);
            rdoDebiCheck.Name = "rdoDebiCheck";
            rdoDebiCheck.Size = new Size(82, 19);
            rdoDebiCheck.TabIndex = 18;
            rdoDebiCheck.TabStop = true;
            rdoDebiCheck.Text = "DebiCheck";
            rdoDebiCheck.UseVisualStyleBackColor = true;
            rdoDebiCheck.CheckedChanged += rdoFileType_CheckedChanged;
            // 
            // rdoEft
            // 
            rdoEft.AutoSize = true;
            rdoEft.Location = new Point(180, 24);
            rdoEft.Name = "rdoEft";
            rdoEft.Size = new Size(44, 19);
            rdoEft.TabIndex = 19;
            rdoEft.Text = "EFT";
            rdoEft.UseVisualStyleBackColor = true;
            rdoEft.CheckedChanged += rdoFileType_CheckedChanged;
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
            cmbBillingDate.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBillingDate.FormattingEnabled = true;
            cmbBillingDate.Location = new Point(251, 79);
            cmbBillingDate.Name = "cmbBillingDate";
            cmbBillingDate.Size = new Size(121, 23);
            cmbBillingDate.TabIndex = 5;
            // tpImportFiles
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
            lvImportFiles.Columns.AddRange(new ColumnHeader[] { chName, chGenDate, chGenTime, chSize, chModified, chType, chTest, chImported });
            lvImportFiles.Dock = DockStyle.Fill;
            lvImportFiles.FullRowSelect = true;
            lvImportFiles.Location = new Point(3, 35);
            lvImportFiles.Name = "lvImportFiles";
            lvImportFiles.Size = new Size(1175, 571);
            lvImportFiles.TabIndex = 1;
            lvImportFiles.UseCompatibleStateImageBehavior = false;
            lvImportFiles.View = View.Details;
            lvImportFiles.ColumnClick += lvImportFiles_ColumnClick;
            lvImportFiles.SelectedIndexChanged += lvImportFiles_SelectedIndexChanged;
            lvImportFiles.MouseUp += lvImportFiles_MouseUp;
            // 
            // chName
            // 
            chName.Text = "Name";
            chName.Width = 400;
            // 
            // chGenDate
            // 
            chGenDate.Text = "Gen Date";
            chGenDate.Width = 100;
            // 
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
            // chTest
            //
            chTest.Text = "Test";
            chTest.Width = 60;
            //
            // chImported
            //
            chImported.Text = "Imported";
            chImported.Width = 80;
            //
            // pnlImportTop
            //
            pnlImportTop.Controls.Add(btnImportParse);
            pnlImportTop.Controls.Add(btnImportRead);
            pnlImportTop.Controls.Add(btnSearchFiles);
            pnlImportTop.Controls.Add(txtSearchFiles);
            pnlImportTop.Controls.Add(btnFindText);
            pnlImportTop.Controls.Add(txtFindText);
            pnlImportTop.Controls.Add(btnApplyFilter);
            pnlImportTop.Controls.Add(txtFileFilter);
            pnlImportTop.Controls.Add(btnImportBrowse);
            pnlImportTop.Controls.Add(txtImportFolder);
            pnlImportTop.Controls.Add(chkHideTestFiles);
            pnlImportTop.Dock = DockStyle.Top;
            pnlImportTop.Location = new Point(3, 3);
            pnlImportTop.Name = "pnlImportTop";
            pnlImportTop.Size = new Size(1175, 60);
            pnlImportTop.TabIndex = 0;
            // 
            // btnImportParse
            // 
            btnImportParse.Enabled = false;
            btnImportParse.Location = new Point(721, 4);
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
            btnImportRead.Location = new Point(615, 4);
            btnImportRead.Name = "btnImportRead";
            btnImportRead.Size = new Size(100, 23);
            btnImportRead.TabIndex = 2;
            btnImportRead.Text = "Read File";
            btnImportRead.UseVisualStyleBackColor = true;
            btnImportRead.Click += btnImportRead_Click;
            // 
            // txtSearchFiles
            // 
            txtSearchFiles.Location = new Point(938, 5);
            txtSearchFiles.Name = "txtSearchFiles";
            txtSearchFiles.Size = new Size(150, 23);
            txtSearchFiles.TabIndex = 5;
            btnFindText.Location = new Point(821, 33);
            btnFindText.Name = "btnFindText";
            btnFindText.Size = new Size(100, 23);
            btnFindText.TabIndex = 10;
            btnFindText.Text = "Find Text";
            btnFindText.UseVisualStyleBackColor = true;
            btnFindText.Click += btnFindText_Click;
            txtFindText.Location = new Point(615, 34);
            txtFindText.Name = "txtFindText";
            txtFindText.Size = new Size(200, 23);
            txtFindText.TabIndex = 9;
            btnApplyFilter.Location = new Point(509, 33);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(100, 23);
            btnApplyFilter.TabIndex = 8;
            btnApplyFilter.Text = "Apply";
            btnApplyFilter.UseVisualStyleBackColor = true;
            btnApplyFilter.Click += btnApplyFilter_Click;
            txtFileFilter.Location = new Point(3, 34);
            txtFileFilter.Name = "txtFileFilter";
            txtFileFilter.Size = new Size(500, 23);
            txtFileFilter.TabIndex = 7;
            // 
            // btnImportBrowse
            // 
            btnImportBrowse.Location = new Point(509, 4);
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
            txtImportFolder.Size = new Size(500, 23);
            txtImportFolder.TabIndex = 0;
            // 
            // chkHideTestFiles
            // 
            chkHideTestFiles.AutoSize = true;
            chkHideTestFiles.Location = new Point(827, 7);
            chkHideTestFiles.Name = "chkHideTestFiles";
            chkHideTestFiles.Size = new Size(101, 19);
            chkHideTestFiles.TabIndex = 4;
            chkHideTestFiles.Text = "Hide Test Files";
            chkHideTestFiles.UseVisualStyleBackColor = true;
            chkHideTestFiles.CheckedChanged += chkHideTestFiles_CheckedChanged;
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
            // cmsImportFiles
            // 
            cmsImportFiles.Items.AddRange(new ToolStripItem[] { previewToolStripMenuItem });
            cmsImportFiles.Name = "cmsImportFiles";
            cmsImportFiles.Size = new Size(116, 26);
            // 
            // previewToolStripMenuItem
            // 
            previewToolStripMenuItem.Name = "previewToolStripMenuItem";
            previewToolStripMenuItem.Size = new Size(115, 22);
            previewToolStripMenuItem.Text = "Preview";
            previewToolStripMenuItem.Click += previewToolStripMenuItem_Click;
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
            grpCounters.ResumeLayout(false);
            grpCounters.PerformLayout();
            grpConfig.ResumeLayout(false);
            grpConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPossibleDuplicates).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDay).EndInit();
            tpImportFiles.ResumeLayout(false);
            pnlImportTop.ResumeLayout(false);
            pnlImportTop.PerformLayout();
            cmsImportFiles.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabPage tpImportFiles;
        private Label label3;
        private GroupBox grpConfig;
        private Label label2;
        private Label label1;
        private Button btnCheckDuplicates;
        private DataGridView dgvPossibleDuplicates;
        private ColumnHeader chGenDate;
        private ColumnHeader chGenTime;
        private CheckBox chkHideTestFiles;
        private TextBox txtSearchFiles;
        private Button btnSearchFiles;
        private TextBox txtFindText;
        private Button btnFindText;
        private TextBox txtFileFilter;
        private Button btnApplyFilter;
        private GroupBox grpCounters;
        private Label lblDcGenerationNumber;
        private Label lblDcDailyCounter;
        private Label lblEftGenerationNumber;
        private Label lblEftDailyCounter;
        private ComboBox cmbBillingDate;
        private Label label4;
    }
}
