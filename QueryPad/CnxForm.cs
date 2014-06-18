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

        private void CnxForm_Load(object sender, EventArgs e)
        {
            // Load connections parameters

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
            List.DataSource = CnxParameters;
        }

        private void List_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Double-click on a connection
            // => open this connection (via MainForm)

            var CnxParameter = CnxParameters[e.RowIndex];
            var Main = (MainForm)this.Parent.FindForm();
            Main.OpenConnection(CnxParameter);
        }
    }
}
