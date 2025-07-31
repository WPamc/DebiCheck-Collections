using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RMCollectionProcessor.Models;

namespace DCCollections.Gui
{
    public class ConfirmCollectionsForm : Form
    {
        private readonly DataGridView _grid;
        private readonly Button _btnOk;
        private readonly Button _btnCancel;
        private readonly List<DebtorCollectionData> _collections;

        public List<DebtorCollectionData> SelectedCollections { get; } = new();

        public ConfirmCollectionsForm(List<DebtorCollectionData> collections, Dictionary<string, List<BillingCollectionRequest>> existing)
        {
            _collections = collections;
            _grid = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false };
            _grid.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Include", Width = 60 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SubSSN" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Reference" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Amount" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Date" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Previous" });

            _btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Dock = DockStyle.Bottom, Height = 30 };
            _btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Dock = DockStyle.Bottom, Height = 30 };
            _btnOk.Click += BtnOk_Click;

            Controls.Add(_grid);
            Controls.Add(_btnCancel);
            Controls.Add(_btnOk);
            Text = "Confirm Collections";
            StartPosition = FormStartPosition.CenterParent;
            Width = 800;
            Height = 400;

            foreach (var col in collections)
            {
                string sub = "MGS" + col.ContractReference;
                string hist = string.Empty;
                if (existing.TryGetValue(sub, out var list) && list.Any())
                {
                    hist = string.Join(", ", list.Select(r => r.DateRequested.ToString("yyyy-MM-dd")));
                }
                _grid.Rows.Add(true, sub, col.ContractReference, col.InstructedAmount.ToString("F2"), col.RequestedCollectionDate.ToString("yyyy-MM-dd"), hist);
            }
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            for (int i = 0; i < _grid.Rows.Count; i++)
            {
                if (Convert.ToBoolean(_grid.Rows[i].Cells[0].Value))
                    SelectedCollections.Add(_collections[i]);
            }
        }
    }
}
