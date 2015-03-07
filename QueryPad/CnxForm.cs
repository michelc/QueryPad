using System;
using System.Collections.Generic;
using System.Drawing;
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
            // Return or Space over a connection
            // => open this connection

            if ((e.KeyCode == Keys.Return) || (e.KeyCode == Keys.Space))
            {
                List_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, List.CurrentRow.Index));
            }
        }

        private void List_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Press a key
            // => select next connection starting with this key

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
    }
}
