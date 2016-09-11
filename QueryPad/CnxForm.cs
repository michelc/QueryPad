using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Altrr;

namespace QueryPad
{
    public partial class CnxForm : Form
    {
        private List<CnxParameter> CnxParameters;

        public CnxForm()
        {
            InitializeComponent();
        }

        protected override void OnEnter(EventArgs e)
        {
            // Reload connections parameters

            try
            {
                CnxParameters = CnxParameter.Load();
            }
            catch (Exception ex)
            {
                var caption = "Error " + ex.HResult.ToString("x");
                var text = string.Format("{0}\n\n({1})", ex.Message, ex.Source);
                MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            List.DataSource = new SortableBindingList<CnxParameter>(CnxParameters);

            List.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            List.DefaultCellStyle.ForeColor = Color.Gray;
            List.Columns[1].DefaultCellStyle.ForeColor = Color.Black;
            List.Columns[0].DefaultCellStyle.ForeColor = Color.FromArgb(0x45, 0x44, 0x41);
            List.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Cursor = Cursors.Default;
            if (List.RowCount > 0) List.CurrentCell = List.Rows[0].Cells[1];
            List.Select();

            base.OnEnter(e);
        }

        private void List_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Double-click on a connection
            // => open this connection (via MainForm)

            // Don't track column header
            if (e.RowIndex < 0) return;

            Cursor = Cursors.WaitCursor;
            var CnxParameter = CnxParameters[e.RowIndex];
            CnxParameter.LastUse = DateTime.Now.ToString("s");
            CnxParameter.Save(CnxParameters);
            var Main = (MainForm)this.Parent.FindForm();
            Main.OpenConnection(CnxParameter);
            Cursor = Cursors.Default;
        }

        private void List_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                switch (e.Value.ToString())
                {
                    case "Debug":
                        e.CellStyle.BackColor = Color.FromKnownColor(KnownColor.Control);
                        break;
                    case "Test":
                        e.CellStyle.BackColor = Color.Orange;
                        break;
                    case "Release":
                        e.CellStyle.BackColor = Color.Red;
                        break;
                }
            }
        }

        private void List_KeyDown(object sender, KeyEventArgs e)
        {
            // Return, Space or Delete over a connection
            if (List.RowCount == 0) return;

            if ((e.KeyCode == Keys.Return) || (e.KeyCode == Keys.Space))
            {
                // Open this connection
                Cursor = Cursors.WaitCursor;
                List_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, List.CurrentRow.Index));
            }
            else if (e.KeyCode == Keys.Delete)
            {
                // Ask to remove the connection
                if (MessageBox.Show("Ok to remove this connection (files will not be deleted)?"
                                  , List.CurrentRow.Cells[1].Value.ToString()
                                  , MessageBoxButtons.OKCancel) == DialogResult.Cancel) return;

                // Remove the connection
                var index = List.CurrentRow.Index;
                var CnxParameter = CnxParameters[index];
                CnxParameters.RemoveAt(index);
                CnxParameter.Save(CnxParameters);
                List.DataSource = new SortableBindingList<CnxParameter>(CnxParameters);

                // Reselect current connection
                if (List.RowCount == 0) return;
                if (index == List.RowCount) index--;
                List.CurrentCell = List.Rows[index].Cells[1];
            }
        }

        private void List_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Press a key
            // => select next connection starting with this key
            if (List.RowCount == 0) return;

            var letter = e.KeyChar.ToString().ToUpper();
            if (letter.CompareTo("A") < 0) return;
            if (letter.CompareTo("Z") > 0) return;

            // Search to the end
            for (var i = List.CurrentRow.Index + 1; i < List.RowCount; i++)
            {
                var name = List.Rows[i].Cells[1].Value.ToString().ToUpper();
                if (name.Substring(0, 1) == letter)
                {
                    List.CurrentCell = List.Rows[i].Cells[0];
                    return;
                }
            }

            // Search from the beginning
            for (var i = 0; i < List.CurrentRow.Index; i++)
            {
                var name = List.Rows[i].Cells[1].Value.ToString().ToUpper();
                if (name.Substring(0, 1) == letter)
                {
                    List.CurrentCell = List.Rows[i].Cells[0];
                    return;
                }
            }
        }

        private void CnxForm_DragDrop(object sender, DragEventArgs e)
        {
            // Check if drop is valid
            var file = GetDropFileName(e);
            if (file == "") return;

            // Just select corresponding connection if exist
            foreach (DataGridViewRow r in List.Rows)
            {
                if (r.Cells[3].Value.ToString().ToLower().Contains(file.ToLower()))
                {
                    List.CurrentCell = r.Cells[1];
                    return;
                }
            }

            // Get connection infos for this file
            var CnxParameter = new CnxParameter(file);
            if (string.IsNullOrEmpty(CnxParameter.Name)) return;

            // Check for name duplicate
            var name = CnxParameter.Name;
            var index = 0;
            var duplicate = true;
            while (duplicate)
            {
                duplicate = false;
                foreach (DataGridViewRow r in List.Rows)
                {
                    if (r.Cells[1].Value.ToString().ToLower() == CnxParameter.Name.ToLower())
                    {
                        duplicate = true;
                        CnxParameter.Name = string.Format("{0} ({1})", name, ++index);
                        break;
                    }
                }
            }

            // Add a connection for this file
            CnxParameters.Add(CnxParameter);
            List.DataSource = new SortableBindingList<CnxParameter>(CnxParameters);

            // Directly open this new connection
            Cursor = Cursors.WaitCursor;
            List_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, List.RowCount - 1));
        }

        private void CnxForm_DragEnter(object sender, DragEventArgs e)
        {
            // Show if drop is valid
            var file = GetDropFileName(e);
            e.Effect = file == "" ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private string GetDropFileName(DragEventArgs e)
        {
            // Accept drag & drop for files only
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return "";

            // Accept only 1 file
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files.Length != 1) return "";

            // Accept only known extensions
            var file = files[0];
            var extension = file.Substring(1 + file.LastIndexOf(".")).ToLower();
            if (!CnxParameter.DropExtensions.Contains(extension)) return "";

            // This file is dropable
            return file;
        }
    }
}
