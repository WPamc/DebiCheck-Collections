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
        private readonly Button _btnSelectAll;
        private readonly Button _btnSelectNone;
        private readonly TextBox _txtFilter;
        private readonly Label _lblColumns;
        private readonly FlowLayoutPanel _panelTop;
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
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total" });

            _btnSelectAll = new Button { Text = "Select All" };
            _btnSelectNone = new Button { Text = "Select None" };
            _txtFilter = new TextBox { Width = 100 };
            _lblColumns = new Label
            {
                Text = "Include, SubSSN, Reference, Amount, Date, Previous, Total",
                Dock = DockStyle.Top,
                AutoSize = true
            };
            _panelTop = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            _panelTop.Controls.Add(new Label { Text = "Filter SubSSN:", AutoSize = true });
            _panelTop.Controls.Add(_txtFilter);
            _panelTop.Controls.Add(_btnSelectAll);
            _panelTop.Controls.Add(_btnSelectNone);

            _btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Dock = DockStyle.Bottom, Height = 30 };
            _btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Dock = DockStyle.Bottom, Height = 30 };
            _btnOk.Click += BtnOk_Click;
            _btnSelectAll.Click += BtnSelectAll_Click;
            _btnSelectNone.Click += BtnSelectNone_Click;
            _txtFilter.TextChanged += FilterRows;

            Controls.Add(_grid);
            Controls.Add(_btnCancel);
            Controls.Add(_btnOk);
            Controls.Add(_lblColumns);
            Controls.Add(_panelTop);
            Text = "Confirm Collections";
            StartPosition = FormStartPosition.CenterParent;
            Width = 800;
            Height = 400;

            foreach (var col in collections)
            {
                string sub = "MGS" + col.ContractReference;
                string hist = string.Empty;
                decimal total = col.InstructedAmount;
                if (existing.TryGetValue(sub, out var list) && list.Any())
                {
                    hist = string.Join(", ", list.Select(r => r.DateRequested.ToString("yyyy-MM-dd")));
                    total += list.Sum(
                        r => 
                        decimal.TryParse(
                            r.AmountRequested, out var a
                            ) ? a : 0m);
                }
                _grid.Rows.Add(true, sub, col.ContractReference, col.InstructedAmount.ToString("F2"), col.RequestedCollectionDate.ToString("yyyy-MM-dd"), hist, total.ToString("F2"));
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

        private void BtnSelectAll_Click(object? sender, EventArgs e)
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.Visible)
                    row.Cells[0].Value = true;
            }
        }

        private void BtnSelectNone_Click(object? sender, EventArgs e)
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.Visible)
                    row.Cells[0].Value = false;
            }
        }

        private void FilterRows(object? sender, EventArgs e)
        {
            string text = _txtFilter.Text.Trim();
            foreach (DataGridViewRow row in _grid.Rows)
            {
                row.Visible = string.IsNullOrEmpty(text) || row.Cells[1].Value?.ToString()?.Contains(text, StringComparison.OrdinalIgnoreCase) == true;
            }
        }
    }
}
