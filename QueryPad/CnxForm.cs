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
            CnxParameters = CnxParameter.GetList();
            List.DataSource = CnxParameters;
        }

        private void CnxForm_Load(object sender, EventArgs e)
        {
            // Load connections parameters

            CnxParameters = CnxParameter.GetList();
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
