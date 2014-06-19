using System;
using System.Windows.Forms;

namespace QueryPad
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void TabConnections_Enter(object sender, EventArgs e)
        {
            // Click on the [Connections] tab
            // => eventually create the connection form
            if (TabConnections.Controls.Count == 0)
            {
                var Cnx = new CnxForm();
                AddFormToTab(Cnx, TabConnections);
                this.Text = App.Title + " // " + Tabs.SelectedTab.Text;
            }
        }

        public void OpenConnection(CnxParameter CnxParameter)
        {
            // A connection has been selected in CnxForm
            // => show a (new) tab with this connection

            // Check if the connection already exist
            var index = Tabs.TabPages.IndexOfKey(CnxParameter.Name);
            // => Display the tab related to the connection
            if (index != -1)
            {
                Tabs.SelectTab(index);
                return;
            }

            // Create a new tab for the connection
            var tab = new TabPage(CnxParameter.Name);
            tab.Name = CnxParameter.Name;
            Tabs.TabPages.Add(tab);

            // Associate a new QueryForm to the tab
            var query = new QueryForm(CnxParameter);
            AddFormToTab(query, tab);
            Tabs.SelectedTab = tab;
        }

        private void AddFormToTab(Form form, TabPage tab)
        {
            // Embed a form inside a tab panel
            // http://hen.co.za/blog/2011/06/embedding-a-form-inside-another-control/

            form.TopLevel = false;
            form.MdiParent = null;
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            tab.Controls.Add(form);
            form.Show();
        }

        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            // A tab has been selected
            // => update title bar

            this.Text = App.Title + " // " + Tabs.SelectedTab.Text;
        }
    }
}
