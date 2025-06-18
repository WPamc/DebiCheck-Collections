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
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.NumericUpDown nudDay;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnShowCurrent;
        private System.Windows.Forms.ListBox lstFiles;
        private System.Windows.Forms.CheckBox chkTest;

        private void InitializeComponent()
        {
            this.btnParse = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.nudDay = new System.Windows.Forms.NumericUpDown();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnShowCurrent = new System.Windows.Forms.Button();
            this.lstFiles = new System.Windows.Forms.ListBox();
            this.chkTest = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudDay)).BeginInit();
            this.SuspendLayout();
            // 
            // btnParse
            // 
            this.btnParse.Location = new System.Drawing.Point(12, 12);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(120, 23);
            this.btnParse.TabIndex = 0;
            this.btnParse.Text = "Parse File";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(12, 70);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(120, 23);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Generate File";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            //
            // nudDay
            // 
            this.nudDay.Location = new System.Drawing.Point(12, 41);
            this.nudDay.Maximum = new decimal(new int[] {31,0,0,0});
            this.nudDay.Minimum = new decimal(new int[] {1,0,0,0});
            this.nudDay.Name = "nudDay";
            this.nudDay.Size = new System.Drawing.Size(120, 23);
            this.nudDay.TabIndex = 1;
            this.nudDay.Value = new decimal(new int[] {1,0,0,0});

            //
            // btnBrowse
            //
            this.btnBrowse.Location = new System.Drawing.Point(150, 12);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(120, 23);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "Browse Folder";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            //
            // btnShowCurrent
            //
            this.btnShowCurrent.Location = new System.Drawing.Point(12, 99);
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
            this.lstFiles.Location = new System.Drawing.Point(150, 41);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(220, 184);
            this.lstFiles.TabIndex = 5;

            //
            // chkTest
            //
            this.chkTest.AutoSize = true;
            this.chkTest.Location = new System.Drawing.Point(12, 128);
            this.chkTest.Name = "chkTest";
            this.chkTest.Size = new System.Drawing.Size(76, 19);
            this.chkTest.TabIndex = 6;
            this.chkTest.Text = "Test File";
            this.chkTest.UseVisualStyleBackColor = true;

            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 237);
            this.Controls.Add(this.lstFiles);
            this.Controls.Add(this.chkTest);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnShowCurrent);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.nudDay);
            this.Controls.Add(this.btnParse);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Collections";
            ((System.ComponentModel.ISupportInitialize)(this.nudDay)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
