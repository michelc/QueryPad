using System;
using System.Drawing;
using System.Windows.Forms;

namespace QueryPad
{
    public partial class QueryForm : Form
    {
        private Connexion Cnx { get; set; }

        public QueryForm(CnxParameter CnxParameter)
        {
            InitializeComponent();

            Name = CnxParameter.Name;
            Text = CnxParameter.Name;

            switch (CnxParameter.Environment)
            {
                case "Debug":
                    BackColor = Color.FromArgb(0xDD, 0xDD, 0xDD); // Silver
                    break;
                case "Test":
                    BackColor = Color.FromArgb(0xFF, 0x85, 0x1B); // Orange
                    break;
                case "Release":
                    BackColor = Color.FromArgb(0xFF, 0x41, 0x36); // Red
                    break;
            }

            ConnexionName.Text = CnxParameter.Name.ToUpper();
            ShowInformations("");

            Cnx = new Connexion(CnxParameter);
            try
            {
                Cnx.Open();
            }
            catch (Exception ex)
            {
                var caption = "Error " + ex.HResult.ToString("x");
                var text = string.Format("{0}\n\n({1})", ex.Message, ex.Source);
                MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Text = "";
                return;
            }

            Tables.DataSource = Cnx.GetTables();
        }

        protected override void OnClosed(EventArgs e)
        {
            // Close connection when form is closed

            Cnx.Close();
            base.OnClosed(e);
        }

        private void ExecuteSql(object sender, EventArgs e)
        {
            // Clear results
            Grid.DataSource = null;
            ShowInformations("");

            // Get query to execute
            var sql = Query.SelectedText.Length == 0 ? Query.Text : Query.SelectedText;
            if (sql.Trim() == "")
            {
                ShowInformations("No query !");
                return;
            }

            // Update display
            FreezeToolbar(true);
            ShowInformations("Executing...");

            // Run query
            TimeSpan duration;
            try
            {
                var start = DateTime.Now;
                Grid.DataSource = Cnx.ExecuteDataSet(sql).Tables[0];
                Grid.AutoResizeColumns();
                duration = DateTime.Now.Subtract(start);
            }
            catch (Exception ex)
            {
                var caption = "Error " + ex.HResult.ToString("x");
                var text = string.Format("{0}\n\n({1})", ex.Message, ex.Source);
                MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ShowInformations("Error");
                return;
            }
            finally
            {
                FreezeToolbar(false);
            }

            // Display statistics
            var message = string.Format("{0} rows ({1:00}:{2:00}.{3:000})"
                                        , Grid.RowCount
                                        , duration.Minutes, duration.Seconds, duration.Milliseconds);
            if (Grid.RowCount < 2) message = message.Replace(" rows ", " row ");
            ShowInformations(message);
        }

        private void ShowInformations(string message)
        {
            // Force informations display
            Informations.Text = message;
            Informations.Left = Toolbar.ClientSize.Width - Informations.Width;
            this.Refresh();
        }

        private void FreezeToolbar(bool is_busy)
        {
            // Enable or disable [Execute] button
            Execute.Enabled = !is_busy;
            Execute.ForeColor = is_busy ? Color.LightGray : Color.Black;

            // Enable or disable [Run script] button
            Run.Enabled = !is_busy;
            Run.ForeColor = is_busy ? Color.LightGray : Color.Black;

            // Change cursor to hourglass during working
            Cursor = is_busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private void Tables_DoubleClick(object sender, EventArgs e)
        {
            // Double-click a table name
            // => generate select query to display its content

            if (Query.Text.Length > 0)
            {
                Query.Text += Environment.NewLine + Environment.NewLine;
            }

            var list = (ListControl)sender;
            Query.Text += "SELECT * FROM " + list.SelectedValue;
        }
    }
}
