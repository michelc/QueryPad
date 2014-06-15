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

            List.Top = 10;
            List.Left = 10;
            List.Width = this.ClientSize.Width - 20;
            List.Height = this.ClientSize.Height - 20;
        }

        private void CnxForm_Activated(object sender, System.EventArgs e)
        {
            // Reload connections parameters

            CnxParameters = CnxParameter.GetList();
            List.DataSource = CnxParameters;
        }

        private void SelectConnection(object sender, DataGridViewCellEventArgs e)
        {
            // Double-click on a connection
            // => open this connection (via MdiForm)

            var CnxParameter = CnxParameters[e.RowIndex];
            var Mdi = (MdiForm)this.MdiParent;
            Mdi.OpenConnection(CnxParameter);
        }
    }
}
