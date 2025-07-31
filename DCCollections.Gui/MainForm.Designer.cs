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
            cmbBillingDate = new ComboBox();
            label4 = new Label();
            rdoDebiCheck = new RadioButton();
            rdoEft = new RadioButton();
            btnGenerate = new Button();
            nudDay = new NumericUpDown();
            tabImportFiles = new TabPage();
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
            btnArchive = new Button();
            lblLiveOutput = new Label();
            lblTestOutput = new Label();
            cmsImportFiles = new ContextMenuStrip(components);
            previewToolStripMenuItem = new ToolStripMenuItem();
            groupBox1 = new GroupBox();
            tabMain.SuspendLayout();
            tabOperations.SuspendLayout();
            grpCounters.SuspendLayout();
            grpConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPossibleDuplicates).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDay).BeginInit();
            tabImportFiles.SuspendLayout();
            pnlImportTop.SuspendLayout();
            cmsImportFiles.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // btnSearchFiles
            // 
            btnSearchFiles.Location = new Point(1563, 7);
            btnSearchFiles.Margin = new Padding(4, 5, 4, 5);
            btnSearchFiles.Name = "btnSearchFiles";
            btnSearchFiles.Size = new Size(107, 38);
            btnSearchFiles.TabIndex = 6;
            btnSearchFiles.Text = "Filter List";
            btnSearchFiles.UseVisualStyleBackColor = true;
            btnSearchFiles.Click += btnSearchFiles_Click;
            // 
            // btnTestOutputOpen
            // 
            btnTestOutputOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTestOutputOpen.Location = new Point(699, 83);
            btnTestOutputOpen.Margin = new Padding(4, 5, 4, 5);
            btnTestOutputOpen.Name = "btnTestOutputOpen";
            btnTestOutputOpen.Size = new Size(73, 38);
            btnTestOutputOpen.TabIndex = 19;
            btnTestOutputOpen.Text = "Open";
            btnTestOutputOpen.UseVisualStyleBackColor = true;
            btnTestOutputOpen.Click += btnTestOutputOpen_Click;
            // 
            // btnLiveOutputOpen
            // 
            btnLiveOutputOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLiveOutputOpen.Location = new Point(699, 35);
            btnLiveOutputOpen.Margin = new Padding(4, 5, 4, 5);
            btnLiveOutputOpen.Name = "btnLiveOutputOpen";
            btnLiveOutputOpen.Size = new Size(73, 38);
            btnLiveOutputOpen.TabIndex = 18;
            btnLiveOutputOpen.Text = "Open";
            btnLiveOutputOpen.UseVisualStyleBackColor = true;
            btnLiveOutputOpen.Click += btnLiveOutputOpen_Click;
            // 
            // tabMain
            // 
            tabMain.Controls.Add(tabOperations);
            tabMain.Controls.Add(tabImportFiles);
            tabMain.Dock = DockStyle.Fill;
            tabMain.Location = new Point(0, 0);
            tabMain.Margin = new Padding(4, 5, 4, 5);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(1699, 1062);
            tabMain.TabIndex = 0;
            // 
            // tabOperations
            // 
            tabOperations.Controls.Add(groupBox1);
            tabOperations.Controls.Add(grpCounters);
            tabOperations.Controls.Add(label3);
            tabOperations.Controls.Add(grpConfig);
            tabOperations.Controls.Add(dgvPossibleDuplicates);
            tabOperations.Controls.Add(btnCheckDuplicates);
            tabOperations.Controls.Add(chkTest);
            tabOperations.Controls.Add(rdoDebiCheck);
            tabOperations.Controls.Add(rdoEft);
            tabOperations.Controls.Add(btnGenerate);
            tabOperations.Controls.Add(nudDay);
            tabOperations.Location = new Point(4, 34);
            tabOperations.Margin = new Padding(4, 5, 4, 5);
            tabOperations.Name = "tabOperations";
            tabOperations.Padding = new Padding(4, 5, 4, 5);
            tabOperations.Size = new Size(1691, 1024);
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
            grpCounters.Location = new Point(11, 437);
            grpCounters.Margin = new Padding(4, 5, 4, 5);
            grpCounters.Name = "grpCounters";
            grpCounters.Padding = new Padding(4, 5, 4, 5);
            grpCounters.Size = new Size(777, 562);
            grpCounters.TabIndex = 20;
            grpCounters.TabStop = false;
            grpCounters.Text = "Counters";
            // 
            // lblEftDailyCounter
            // 
            lblEftDailyCounter.AutoSize = true;
            lblEftDailyCounter.Location = new Point(23, 162);
            lblEftDailyCounter.Margin = new Padding(4, 0, 4, 0);
            lblEftDailyCounter.Name = "lblEftDailyCounter";
            lblEftDailyCounter.Size = new Size(165, 25);
            lblEftDailyCounter.TabIndex = 3;
            lblEftDailyCounter.Text = "EFT DailyCounter: 0";
            // 
            // lblEftGenerationNumber
            // 
            lblEftGenerationNumber.AutoSize = true;
            lblEftGenerationNumber.Location = new Point(23, 120);
            lblEftGenerationNumber.Margin = new Padding(4, 0, 4, 0);
            lblEftGenerationNumber.Name = "lblEftGenerationNumber";
            lblEftGenerationNumber.Size = new Size(214, 25);
            lblEftGenerationNumber.TabIndex = 2;
            lblEftGenerationNumber.Text = "EFT GenerationNumber: 0";
            // 
            // lblDcDailyCounter
            // 
            lblDcDailyCounter.AutoSize = true;
            lblDcDailyCounter.Location = new Point(23, 78);
            lblDcDailyCounter.Margin = new Padding(4, 0, 4, 0);
            lblDcDailyCounter.Name = "lblDcDailyCounter";
            lblDcDailyCounter.Size = new Size(162, 25);
            lblDcDailyCounter.TabIndex = 1;
            lblDcDailyCounter.Text = "DC DailyCounter: 0";
            // 
            // lblDcGenerationNumber
            // 
            lblDcGenerationNumber.AutoSize = true;
            lblDcGenerationNumber.Location = new Point(23, 37);
            lblDcGenerationNumber.Margin = new Padding(4, 0, 4, 0);
            lblDcGenerationNumber.Name = "lblDcGenerationNumber";
            lblDcGenerationNumber.Size = new Size(211, 25);
            lblDcGenerationNumber.TabIndex = 0;
            lblDcGenerationNumber.Text = "DC GenerationNumber: 0";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(110, 92);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(130, 25);
            label3.TabIndex = 15;
            label3.Text = "Deduction Day";
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
            grpConfig.Location = new Point(11, 277);
            grpConfig.Margin = new Padding(4, 5, 4, 5);
            grpConfig.Name = "grpConfig";
            grpConfig.Padding = new Padding(4, 5, 4, 5);
            grpConfig.Size = new Size(777, 150);
            grpConfig.TabIndex = 15;
            grpConfig.TabStop = false;
            grpConfig.Text = "Config";
            // 
            // txtLiveOutputFolder
            // 
            txtLiveOutputFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLiveOutputFolder.Location = new Point(160, 35);
            txtLiveOutputFolder.Margin = new Padding(4, 5, 4, 5);
            txtLiveOutputFolder.Name = "txtLiveOutputFolder";
            txtLiveOutputFolder.Size = new Size(447, 31);
            txtLiveOutputFolder.TabIndex = 8;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(1, 40);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(159, 25);
            label2.TabIndex = 14;
            label2.Text = "Live Output Folder";
            // 
            // btnLiveOutputBrowse
            // 
            btnLiveOutputBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLiveOutputBrowse.Location = new Point(617, 35);
            btnLiveOutputBrowse.Margin = new Padding(4, 5, 4, 5);
            btnLiveOutputBrowse.Name = "btnLiveOutputBrowse";
            btnLiveOutputBrowse.Size = new Size(73, 38);
            btnLiveOutputBrowse.TabIndex = 9;
            btnLiveOutputBrowse.Text = "Browse";
            btnLiveOutputBrowse.UseVisualStyleBackColor = true;
            btnLiveOutputBrowse.Click += btnLiveOutputBrowse_Click;
            // 
            // txtTestOutputFolder
            // 
            txtTestOutputFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTestOutputFolder.Location = new Point(160, 83);
            txtTestOutputFolder.Margin = new Padding(4, 5, 4, 5);
            txtTestOutputFolder.Name = "txtTestOutputFolder";
            txtTestOutputFolder.Size = new Size(447, 31);
            txtTestOutputFolder.TabIndex = 11;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(1, 90);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(159, 25);
            label1.TabIndex = 13;
            label1.Text = "Test Output Folder";
            // 
            // btnTestOutputBrowse
            // 
            btnTestOutputBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTestOutputBrowse.Location = new Point(617, 83);
            btnTestOutputBrowse.Margin = new Padding(4, 5, 4, 5);
            btnTestOutputBrowse.Name = "btnTestOutputBrowse";
            btnTestOutputBrowse.Size = new Size(73, 38);
            btnTestOutputBrowse.TabIndex = 12;
            btnTestOutputBrowse.Text = "Browse";
            btnTestOutputBrowse.UseVisualStyleBackColor = true;
            btnTestOutputBrowse.Click += btnTestOutputBrowse_Click;
            // 
            // dgvPossibleDuplicates
            // 
            dgvPossibleDuplicates.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPossibleDuplicates.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPossibleDuplicates.Location = new Point(797, 10);
            dgvPossibleDuplicates.Margin = new Padding(4, 5, 4, 5);
            dgvPossibleDuplicates.Name = "dgvPossibleDuplicates";
            dgvPossibleDuplicates.RowHeadersWidth = 62;
            dgvPossibleDuplicates.Size = new Size(881, 988);
            dgvPossibleDuplicates.TabIndex = 17;
            // 
            // btnCheckDuplicates
            // 
            btnCheckDuplicates.Location = new Point(136, 227);
            btnCheckDuplicates.Margin = new Padding(4, 5, 4, 5);
            btnCheckDuplicates.Name = "btnCheckDuplicates";
            btnCheckDuplicates.Size = new Size(171, 38);
            btnCheckDuplicates.TabIndex = 16;
            btnCheckDuplicates.Text = "Check Duplicates";
            btnCheckDuplicates.UseVisualStyleBackColor = true;
            btnCheckDuplicates.Click += btnCheckDuplicates_Click;
            // 
            // chkTest
            // 
            chkTest.AutoSize = true;
            chkTest.Location = new Point(110, 122);
            chkTest.Margin = new Padding(4, 5, 4, 5);
            chkTest.Name = "chkTest";
            chkTest.Size = new Size(99, 29);
            chkTest.TabIndex = 6;
            chkTest.Text = "Test File";
            chkTest.UseVisualStyleBackColor = true;
            // 
            // cmbBillingDate
            // 
            cmbBillingDate.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBillingDate.FormattingEnabled = true;
            cmbBillingDate.Location = new Point(160, 32);
            cmbBillingDate.Margin = new Padding(4, 5, 4, 5);
            cmbBillingDate.Name = "cmbBillingDate";
            cmbBillingDate.Size = new Size(171, 33);
            cmbBillingDate.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(40, 35);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(101, 25);
            label4.TabIndex = 21;
            label4.Text = "Billing Date";
            // 
            // rdoDebiCheck
            // 
            rdoDebiCheck.AutoSize = true;
            rdoDebiCheck.Checked = true;
            rdoDebiCheck.Location = new Point(136, 40);
            rdoDebiCheck.Margin = new Padding(4, 5, 4, 5);
            rdoDebiCheck.Name = "rdoDebiCheck";
            rdoDebiCheck.Size = new Size(121, 29);
            rdoDebiCheck.TabIndex = 18;
            rdoDebiCheck.TabStop = true;
            rdoDebiCheck.Text = "DebiCheck";
            rdoDebiCheck.UseVisualStyleBackColor = true;
            rdoDebiCheck.CheckedChanged += rdoFileType_CheckedChanged;
            // 
            // rdoEft
            // 
            rdoEft.AutoSize = true;
            rdoEft.Location = new Point(257, 40);
            rdoEft.Margin = new Padding(4, 5, 4, 5);
            rdoEft.Name = "rdoEft";
            rdoEft.Size = new Size(64, 29);
            rdoEft.TabIndex = 19;
            rdoEft.Text = "EFT";
            rdoEft.UseVisualStyleBackColor = true;
            rdoEft.CheckedChanged += rdoFileType_CheckedChanged;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(136, 178);
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
            nudDay.Location = new Point(253, 88);
            nudDay.Margin = new Padding(4, 5, 4, 5);
            nudDay.Maximum = new decimal(new int[] { 31, 0, 0, 0 });
            nudDay.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudDay.Name = "nudDay";
            nudDay.Size = new Size(54, 31);
            nudDay.TabIndex = 1;
            nudDay.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // tabImportFiles
            // 
            tabImportFiles.Controls.Add(lvImportFiles);
            tabImportFiles.Controls.Add(pnlImportTop);
            tabImportFiles.Location = new Point(4, 34);
            tabImportFiles.Margin = new Padding(4, 5, 4, 5);
            tabImportFiles.Name = "tabImportFiles";
            tabImportFiles.Padding = new Padding(4, 5, 4, 5);
            tabImportFiles.Size = new Size(1691, 1024);
            tabImportFiles.TabIndex = 2;
            tabImportFiles.Text = "Import Files";
            tabImportFiles.UseVisualStyleBackColor = true;
            // 
            // lvImportFiles
            // 
            lvImportFiles.Columns.AddRange(new ColumnHeader[] { chName, chGenDate, chGenTime, chSize, chModified, chType, chTest, chImported });
            lvImportFiles.Dock = DockStyle.Fill;
            lvImportFiles.FullRowSelect = true;
            lvImportFiles.Location = new Point(4, 105);
            lvImportFiles.Margin = new Padding(4, 5, 4, 5);
            lvImportFiles.Name = "lvImportFiles";
            lvImportFiles.Size = new Size(1683, 914);
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
            pnlImportTop.Controls.Add(btnArchive);
            pnlImportTop.Dock = DockStyle.Top;
            pnlImportTop.Location = new Point(4, 5);
            pnlImportTop.Margin = new Padding(4, 5, 4, 5);
            pnlImportTop.Name = "pnlImportTop";
            pnlImportTop.Size = new Size(1683, 100);
            pnlImportTop.TabIndex = 0;
            // 
            // btnImportParse
            // 
            btnImportParse.Enabled = false;
            btnImportParse.Location = new Point(1030, 7);
            btnImportParse.Margin = new Padding(4, 5, 4, 5);
            btnImportParse.Name = "btnImportParse";
            btnImportParse.Size = new Size(143, 38);
            btnImportParse.TabIndex = 3;
            btnImportParse.Text = "Import";
            btnImportParse.UseVisualStyleBackColor = true;
            btnImportParse.Click += btnImportParse_Click;
            // 
            // btnImportRead
            // 
            btnImportRead.Enabled = false;
            btnImportRead.Location = new Point(879, 7);
            btnImportRead.Margin = new Padding(4, 5, 4, 5);
            btnImportRead.Name = "btnImportRead";
            btnImportRead.Size = new Size(143, 38);
            btnImportRead.TabIndex = 2;
            btnImportRead.Text = "Read File";
            btnImportRead.UseVisualStyleBackColor = true;
            btnImportRead.Click += btnImportRead_Click;
            // 
            // txtSearchFiles
            // 
            txtSearchFiles.Location = new Point(1340, 8);
            txtSearchFiles.Margin = new Padding(4, 5, 4, 5);
            txtSearchFiles.Name = "txtSearchFiles";
            txtSearchFiles.Size = new Size(213, 31);
            txtSearchFiles.TabIndex = 5;
            // 
            // btnFindText
            // 
            btnFindText.Location = new Point(1173, 57);
            btnFindText.Margin = new Padding(4, 5, 4, 5);
            btnFindText.Name = "btnFindText";
            btnFindText.Size = new Size(143, 38);
            btnFindText.TabIndex = 10;
            btnFindText.Text = "Search Content";
            btnFindText.UseVisualStyleBackColor = true;
            btnFindText.Click += btnFindText_Click;
            // 
            // txtFindText
            // 
            txtFindText.Location = new Point(879, 57);
            txtFindText.Margin = new Padding(4, 5, 4, 5);
            txtFindText.Name = "txtFindText";
            txtFindText.Size = new Size(284, 31);
            txtFindText.TabIndex = 9;
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.Location = new Point(727, 55);
            btnApplyFilter.Margin = new Padding(4, 5, 4, 5);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(143, 38);
            btnApplyFilter.TabIndex = 8;
            btnApplyFilter.Text = "Apply";
            btnApplyFilter.UseVisualStyleBackColor = true;
            btnApplyFilter.Click += btnApplyFilter_Click;
            // 
            // txtFileFilter
            // 
            txtFileFilter.Location = new Point(4, 57);
            txtFileFilter.Margin = new Padding(4, 5, 4, 5);
            txtFileFilter.Name = "txtFileFilter";
            txtFileFilter.Size = new Size(713, 31);
            txtFileFilter.TabIndex = 7;
            // 
            // btnImportBrowse
            // 
            btnImportBrowse.Location = new Point(727, 7);
            btnImportBrowse.Margin = new Padding(4, 5, 4, 5);
            btnImportBrowse.Name = "btnImportBrowse";
            btnImportBrowse.Size = new Size(143, 38);
            btnImportBrowse.TabIndex = 1;
            btnImportBrowse.Text = "Browse";
            btnImportBrowse.UseVisualStyleBackColor = true;
            btnImportBrowse.Click += btnImportBrowse_Click;
            // 
            // txtImportFolder
            // 
            txtImportFolder.Location = new Point(4, 8);
            txtImportFolder.Margin = new Padding(4, 5, 4, 5);
            txtImportFolder.Name = "txtImportFolder";
            txtImportFolder.Size = new Size(713, 31);
            txtImportFolder.TabIndex = 0;
            // 
            // chkHideTestFiles
            // 
            chkHideTestFiles.AutoSize = true;
            chkHideTestFiles.Location = new Point(1181, 12);
            chkHideTestFiles.Margin = new Padding(4, 5, 4, 5);
            chkHideTestFiles.Name = "chkHideTestFiles";
            chkHideTestFiles.Size = new Size(149, 29);
            chkHideTestFiles.TabIndex = 4;
            chkHideTestFiles.Text = "Hide Test Files";
            chkHideTestFiles.UseVisualStyleBackColor = true;
            chkHideTestFiles.CheckedChanged += chkHideTestFiles_CheckedChanged;
            // 
            // btnArchive
            // 
            btnArchive.Location = new Point(1527, 52);
            btnArchive.Margin = new Padding(4, 5, 4, 5);
            btnArchive.Name = "btnArchive";
            btnArchive.Size = new Size(143, 38);
            btnArchive.TabIndex = 11;
            btnArchive.Text = "Archive Files";
            btnArchive.UseVisualStyleBackColor = true;
            btnArchive.Click += btnArchive_Click;
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
            cmsImportFiles.ImageScalingSize = new Size(24, 24);
            cmsImportFiles.Items.AddRange(new ToolStripItem[] { previewToolStripMenuItem });
            cmsImportFiles.Name = "cmsImportFiles";
            cmsImportFiles.Size = new Size(145, 36);
            // 
            // previewToolStripMenuItem
            // 
            previewToolStripMenuItem.Name = "previewToolStripMenuItem";
            previewToolStripMenuItem.Size = new Size(144, 32);
            previewToolStripMenuItem.Text = "Preview";
            previewToolStripMenuItem.Click += previewToolStripMenuItem_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cmbBillingDate);
            groupBox1.Controls.Add(label4);
            groupBox1.Location = new Point(331, 40);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(452, 238);
            groupBox1.TabIndex = 22;
            groupBox1.TabStop = false;
            groupBox1.Text = "Billing Window";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1699, 1062);
            Controls.Add(tabMain);
            Margin = new Padding(4, 5, 4, 5);
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
            tabImportFiles.ResumeLayout(false);
            pnlImportTop.ResumeLayout(false);
            pnlImportTop.PerformLayout();
            cmsImportFiles.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabPage tabImportFiles;
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
        private Button btnArchive;
        private GroupBox groupBox1;
    }
}
