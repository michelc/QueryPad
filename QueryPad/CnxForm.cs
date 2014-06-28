using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
    }
}
