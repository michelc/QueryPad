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
    }
}
