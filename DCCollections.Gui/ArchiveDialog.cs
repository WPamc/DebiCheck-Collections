using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DCCollections.Gui
{
    public class ArchiveDialog : Form
    {
        private readonly NumericUpDown _nudDays;
        private readonly RadioButton _rbImportedOnly;
        private readonly RadioButton _rbForce;
        private readonly ComboBox _cmbFileType;

        public int DaysOlder => (int)_nudDays.Value;
        public bool ForceArchive => _rbForce.Checked;
        public string FileType => _cmbFileType.SelectedItem?.ToString() ?? "All File Types";

        public ArchiveDialog(UserSettings settings, List<string> fileTypes)
        {
            Text = "Archive Old Files";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(350, 220);

            // Age Filter
            var lblDays = new Label { Text = "Archive files older than:", Location = new Point(10, 15), AutoSize = true };
            _nudDays = new NumericUpDown { Value = settings.ArchiveOlderThanDays, Location = new Point(150, 13), Width = 60 };
            var lblDaysSuffix = new Label { Text = "days", Location = new Point(215, 15), AutoSize = true };
            var lblHelpText = new Label { Text = "(Set to 0 to archive all files regardless of age)", ForeColor = SystemColors.GrayText, Location = new Point(10, 40), AutoSize = true };

            // Import Status Filter
            var grpStatus = new GroupBox { Text = "Archive Scope", Location = new Point(10, 70), Size = new Size(330, 80) };
            _rbImportedOnly = new RadioButton { Text = "Imported files only (Recommended)", Location = new Point(10, 20), Checked = !settings.ArchiveForceUnimported, AutoSize = true };
            _rbForce = new RadioButton { Text = "Force archive un-imported files", Location = new Point(10, 50), Checked = settings.ArchiveForceUnimported, AutoSize = true };
            grpStatus.Controls.Add(_rbImportedOnly);
            grpStatus.Controls.Add(_rbForce);

            // File Type Filter
            var lblFileType = new Label { Text = "For file type:", Location = new Point(10, 160), AutoSize = true };
            _cmbFileType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(110, 157), Width = 230 };
            _cmbFileType.Items.Add("All File Types");
            fileTypes.ForEach(ft => _cmbFileType.Items.Add(ft));
            _cmbFileType.SelectedItem = settings.ArchiveLastFileType;
            if (!_cmbFileType.Items.Contains(_cmbFileType.SelectedItem))
            {
                _cmbFileType.SelectedItem = "All File Types";
            }


            // Buttons
            var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(184, 190) };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(265, 190) };
            AcceptButton = btnOk;
            CancelButton = btnCancel;

            // Event Handlers
            _rbForce.CheckedChanged += (sender, args) => _cmbFileType.Enabled = _rbForce.Checked;
            _cmbFileType.Enabled = _rbForce.Checked;

            Controls.AddRange(new Control[] { lblDays, _nudDays, lblDaysSuffix, lblHelpText, grpStatus, lblFileType, _cmbFileType, btnOk, btnCancel });
        }
    }
}
