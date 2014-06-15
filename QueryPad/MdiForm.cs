using System;
using System.Windows.Forms;

namespace QueryPad
{
    public partial class MdiForm : Form
    {
        private string AppTitle = "QueryPad";

        public MdiForm()
        {
            InitializeComponent();

            Tabs.Height = 30;
        }

        private void ListConnections(object sender, EventArgs e)
        {
            // Click on the [Connections] tab
            // => display the form to select a connection

            // Eventually, create the connection form
            if (this.MdiChildren.Length == 0)
            {
                var child = new CnxForm();
                child.MdiParent = this;
            }

            ShowChildrenForm(this.MdiChildren[0]);
        }

        public void OpenConnection(CnxParameter CnxParameter)
        {
            // A connection has been selected in CnxForm
            // => show a (new) tab with this connection

            // Check if the connection already exist
            var index = Tabs.TabPages.IndexOfKey(CnxParameter.Name);
            if (index == -1)
            {
                // Create a new tab for the connection
                var tab = new TabPage(CnxParameter.Name);
                tab.Name = CnxParameter.Name;
                tab.Enter += new System.EventHandler(this.DisplayConnection);
                Tabs.TabPages.Add(tab);

                // Get the index of this new tab
                index = Tabs.TabPages.IndexOfKey(CnxParameter.Name);

                // Create a new QueryForm for the connection
                var query = new QueryForm(CnxParameter);
                query.MdiParent = this;
            }

            // Display the tab related to the connection
            Tabs.SelectTab(index);
        }

        private void ShowChildrenForm(Form form)
        {
            // Display the form as maximized
            form.Left = 0;
            form.Top = 0;
            form.Width = this.ClientSize.Width - 4;
            form.Height = this.ClientSize.Height - Tabs.Height - 4;
            form.Show();

            // Update title bar
            this.Text = AppTitle + " // " + form.Text;
        }

        private void DisplayConnection(object sender, EventArgs e)
        {
            // Click on a specific [Connection] tab
            // => display the form with this connection

            // Get witch tab was clicked
            var tab = (TabPage)sender;
            var index = Tabs.TabPages.IndexOfKey(tab.Name);

            // Hide all childrens except the one related to the tab
            for (var i = 0; i < this.MdiChildren.Length; i++)
            {
                if (i == index)
                {
                    ShowChildrenForm(this.MdiChildren[i]);
                }
                else
                {
                    this.MdiChildren[i].Hide();
                }
            }
        }

        private void MdiForm_Resize(object sender, EventArgs e)
        {
            // Force all childrens to maximize
            foreach (var form in this.MdiChildren)
            {
                form.Left = 0;
                form.Top = 0;
                form.Width = this.ClientSize.Width - 4;
                form.Height = this.ClientSize.Height - Tabs.Height - 4;
            }
        }
    }
}
