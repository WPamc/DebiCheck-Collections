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

        private void InitializeComponent()
        {
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabOperations = new System.Windows.Forms.TabPage();
            this.tabParse = new System.Windows.Forms.TabPage();
            this.btnParse = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.nudDay = new System.Windows.Forms.NumericUpDown();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnShowCurrent = new System.Windows.Forms.Button();
            this.lstFiles = new System.Windows.Forms.ListBox();
            this.chkTest = new System.Windows.Forms.CheckBox();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.btnFolderBrowse = new System.Windows.Forms.Button();
            this.lstFolderFiles = new System.Windows.Forms.ListBox();
            this.btnParseSelected = new System.Windows.Forms.Button();
            this.txtRaw = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudDay)).BeginInit();
            this.tabMain.SuspendLayout();
            this.tabOperations.SuspendLayout();
            this.tabParse.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            //
            this.tabMain.Controls.Add(this.tabOperations);
            this.tabMain.Controls.Add(this.tabParse);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(382, 237);
            this.tabMain.TabIndex = 0;

            // tabOperations
            //
            this.tabOperations.Controls.Add(this.lstFiles);
            this.tabOperations.Controls.Add(this.chkTest);
            this.tabOperations.Controls.Add(this.btnBrowse);
            this.tabOperations.Controls.Add(this.btnShowCurrent);
            this.tabOperations.Controls.Add(this.btnGenerate);
            this.tabOperations.Controls.Add(this.nudDay);
            this.tabOperations.Controls.Add(this.btnParse);
            this.tabOperations.Location = new System.Drawing.Point(4, 24);
            this.tabOperations.Name = "tabOperations";
            this.tabOperations.Padding = new System.Windows.Forms.Padding(3);
            this.tabOperations.Size = new System.Drawing.Size(374, 209);
            this.tabOperations.TabIndex = 0;
            this.tabOperations.Text = "Operations";
            this.tabOperations.UseVisualStyleBackColor = true;

            // tabParse
            //
            this.tabParse.Controls.Add(this.txtRaw);
            this.tabParse.Controls.Add(this.btnParseSelected);
            this.tabParse.Controls.Add(this.lstFolderFiles);
            this.tabParse.Controls.Add(this.btnFolderBrowse);
            this.tabParse.Controls.Add(this.txtFolder);
            this.tabParse.Location = new System.Drawing.Point(4, 24);
            this.tabParse.Name = "tabParse";
            this.tabParse.Padding = new System.Windows.Forms.Padding(3);
            this.tabParse.Size = new System.Drawing.Size(374, 209);
            this.tabParse.TabIndex = 1;
            this.tabParse.Text = "Parse Files";
            this.tabParse.UseVisualStyleBackColor = true;

            // btnParse
            //
            this.btnParse.Location = new System.Drawing.Point(6, 6);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(120, 23);
            this.btnParse.TabIndex = 0;
            this.btnParse.Text = "Parse File";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // btnGenerate
            //
            this.btnGenerate.Location = new System.Drawing.Point(6, 64);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(120, 23);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Generate File";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            //
            // nudDay
            //
            this.nudDay.Location = new System.Drawing.Point(6, 35);
            this.nudDay.Maximum = new decimal(new int[] {31,0,0,0});
            this.nudDay.Minimum = new decimal(new int[] {1,0,0,0});
            this.nudDay.Name = "nudDay";
            this.nudDay.Size = new System.Drawing.Size(120, 23);
            this.nudDay.TabIndex = 1;
            this.nudDay.Value = new decimal(new int[] {1,0,0,0});

            //
            // btnBrowse
            //
            this.btnBrowse.Location = new System.Drawing.Point(150, 6);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(120, 23);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "Browse Folder";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            //
            // btnShowCurrent
            //
            this.btnShowCurrent.Location = new System.Drawing.Point(6, 93);
            this.btnShowCurrent.Name = "btnShowCurrent";
            this.btnShowCurrent.Size = new System.Drawing.Size(120, 23);
            this.btnShowCurrent.TabIndex = 4;
            this.btnShowCurrent.Text = "Show App Files";
            this.btnShowCurrent.UseVisualStyleBackColor = true;
            this.btnShowCurrent.Click += new System.EventHandler(this.btnShowCurrent_Click);

            //
            // lstFiles
            //
            this.lstFiles.FormattingEnabled = true;
            this.lstFiles.ItemHeight = 15;
            this.lstFiles.Location = new System.Drawing.Point(150, 35);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(220, 184);
            this.lstFiles.TabIndex = 5;

            //
            // chkTest
            //
            this.chkTest.AutoSize = true;
            this.chkTest.Location = new System.Drawing.Point(6, 122);
            this.chkTest.Name = "chkTest";
            this.chkTest.Size = new System.Drawing.Size(76, 19);
            this.chkTest.TabIndex = 6;
            this.chkTest.Text = "Test File";
            this.chkTest.UseVisualStyleBackColor = true;

            // txtFolder
            //
            this.txtFolder.Location = new System.Drawing.Point(6, 6);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(240, 23);
            this.txtFolder.TabIndex = 0;

            // btnFolderBrowse
            //
            this.btnFolderBrowse.Location = new System.Drawing.Point(252, 6);
            this.btnFolderBrowse.Name = "btnFolderBrowse";
            this.btnFolderBrowse.Size = new System.Drawing.Size(100, 23);
            this.btnFolderBrowse.TabIndex = 1;
            this.btnFolderBrowse.Text = "Browse";
            this.btnFolderBrowse.UseVisualStyleBackColor = true;
            this.btnFolderBrowse.Click += new System.EventHandler(this.btnFolderBrowse_Click);

            // lstFolderFiles
            //
            this.lstFolderFiles.FormattingEnabled = true;
            this.lstFolderFiles.ItemHeight = 15;
            this.lstFolderFiles.Location = new System.Drawing.Point(6, 35);
            this.lstFolderFiles.Name = "lstFolderFiles";
            this.lstFolderFiles.Size = new System.Drawing.Size(346, 94);
            this.lstFolderFiles.TabIndex = 2;
            this.lstFolderFiles.SelectedIndexChanged += new System.EventHandler(this.lstFolderFiles_SelectedIndexChanged);

            // btnParseSelected
            //
            this.btnParseSelected.Enabled = false;
            this.btnParseSelected.Location = new System.Drawing.Point(6, 135);
            this.btnParseSelected.Name = "btnParseSelected";
            this.btnParseSelected.Size = new System.Drawing.Size(120, 23);
            this.btnParseSelected.TabIndex = 3;
            this.btnParseSelected.Text = "Parse File";
            this.btnParseSelected.UseVisualStyleBackColor = true;
            this.btnParseSelected.Click += new System.EventHandler(this.btnParseSelected_Click);

            // txtRaw
            //
            this.txtRaw.Location = new System.Drawing.Point(6, 164);
            this.txtRaw.Multiline = true;
            this.txtRaw.Name = "txtRaw";
            this.txtRaw.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRaw.Size = new System.Drawing.Size(346, 39);
            this.txtRaw.TabIndex = 4;

            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 237);
            this.Controls.Add(this.tabMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Collections";
            ((System.ComponentModel.ISupportInitialize)(this.nudDay)).EndInit();
            this.tabParse.ResumeLayout(false);
            this.tabParse.PerformLayout();
            this.tabOperations.ResumeLayout(false);
            this.tabOperations.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
