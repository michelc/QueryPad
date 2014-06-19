using System;
using System.Drawing;
using System.Windows.Forms;

namespace QueryPad
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            // Main form is closed
            // => explicitly close each "tab" form 

            foreach (TabPage tab in Tabs.TabPages)
            {
                var form = (Form)tab.Controls[0];
                form.Close();
            }
            base.OnClosed(e);
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
            TabConnections.Controls[0].Select();
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

            // Remove the tab when connexion failed
            if (query.Text == "")
            {
                Tabs.TabPages.Remove(tab);
                return;
            }

            // Or activate the tab
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
            if (tab != TabConnections) tab.Text += "   x ";
            tab.Controls.Add(form);
            form.Show();
        }

        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            // A tab has been selected
            // => update title bar

            this.Text = App.Title + " // " + Tabs.SelectedTab.Text.Replace("   x ", "");
        }

        private void Tabs_MouseUp(object sender, MouseEventArgs e)
        {
            // A tab has been clicked
            // => check if tab should be closed

            // Connections tab has no close button
            if (Tabs.SelectedIndex == 0) return;

            // Check if click was over the "close" button
            var r = Tabs.GetTabRect(Tabs.SelectedIndex);
            var btn = new Rectangle(r.Right - 20, r.Top + 2, 18, 18);
            if (btn.Contains(e.Location))
            {
                // Yes => close the form
                var form = (Form)Tabs.SelectedTab.Controls[0];
                form.Close();
                // And remove the tab
                var index = Tabs.SelectedIndex;
                Tabs.TabPages.Remove(Tabs.SelectedTab);
                Tabs.SelectedIndex = (index < Tabs.TabPages.Count) ? index : index - 1;
            }
        }
    }
}
