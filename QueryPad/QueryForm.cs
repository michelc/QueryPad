using System;
using System.Drawing;
using System.Windows.Forms;

namespace QueryPad
{
    public partial class QueryForm : Form
    {
        private Connexion Cnx { get; set; }
        private DataGridViewCellEventArgs PreviousCellClick;

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
            Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            PreviousCellClick = null;
            ShowInformations("");

            // Remove all formatting
            {
                var text = Query.Text;
                var start = Query.SelectionStart;
                var length = Query.SelectionLength;
                Query.Rtf = "";
                Query.Text = text;
                Query.SelectionStart = start;
                Query.SelectionLength = length;
                Query.Focus();
            }

            // Get query to execute (selected text by default)
            var sql = Query.SelectedText;
            if (sql.Length == 0)
            {
                // Or, auto-select text according to cursor position
                var nl = "\n\n";
                // Find query start
                var start = Query.SelectionStart;
                if (start > 0) start--;
                start = Query.Text.LastIndexOf(nl, start);
                start = (start == -1) ? 0 : start + nl.Length;
                // Find query end
                var end = Query.Text.IndexOf(nl, start);
                if (end == -1) end = Query.Text.Length;
                // Select text
                Query.SelectionStart = start;
                Query.SelectionLength = end - start;
                Query.Focus();
                // Get query to execute
                sql = Query.SelectedText;
            }

            // Check if query is empty
            sql = sql.Trim();
            if (sql == "")
            {
                ShowInformations("No query !");
                return;
            }

            // Update display
            FreezeToolbar(true);
            ShowInformations("Executing...");

            // Run query
            TimeSpan duration;
            var count = -1;
            var read = true;
            try
            {
                var start = DateTime.Now;
                if (sql.ToUpper().StartsWith("SELECT"))
                {
                    sql = Cnx.SelectTop(sql);
                    Grid.DataSource = Cnx.ExecuteDataSet(sql).Tables[0];
                    Grid.AutoResizeColumns();
                    count = Grid.RowCount;
                }
                else
                {
                    read = false;
                    count = Cnx.ExecuteNonQuery(sql);
                }
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
                                        , count
                                        , duration.Minutes, duration.Seconds, duration.Milliseconds);
            if (count < 2) message = message.Replace(" rows ", " row ");
            if (read) message = message.Replace("10000 ", "top 10000 ");
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

            var list = (ListControl)sender;
            NewQuery("SELECT * FROM " + list.SelectedValue);
        }

        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Double-click a cell

            // Don't track column header
            if (e.RowIndex < 0) return;

            // Checks if it's an ID column
            var cell = Grid[e.ColumnIndex, e.RowIndex];
            var data = QuickNavigation(cell.OwningColumn.HeaderText);
            if (data == null) return;

            // Yes => generate select query to display related data
            var sql = string.Format("SELECT * FROM {0} WHERE {1} = {2}"
                                    , data[0]
                                    , data[1]
                                    , cell.Value);
            NewQuery(sql);
        }

        private string[] QuickNavigation(string foreign_col)
        {
            // Guess foreign table name (from foreign column name)
            var column_name = foreign_col.ToUpper();
            var table_name = "";
            if (column_name.EndsWith("_ID"))
            {
                // Foreign key = Table_ID
                table_name = foreign_col.Substring(0, foreign_col.Length - 3);
            }
            else if (column_name.EndsWith("ID"))
            {
                // Foreign key = TableID
                table_name = foreign_col.Substring(0, foreign_col.Length - 2);
            }
            else if (column_name.StartsWith("ID_"))
            {
                // Foreign key = ID_Table
                table_name = foreign_col.Substring(3);
            }
            else if (column_name.StartsWith("ID"))
            {
                // Foreign key = IDTable
                table_name = foreign_col.Substring(2);
            }
            else
            {
                return null;
            }

            // Check if foreign table exists
            for (var i = 0; i < Tables.Items.Count; i++)
            {
                if (string.Compare(Tables.Items[i].ToString(), table_name, true) == 0)
                {
                    table_name = Tables.Items[i].ToString();
                    break;
                }
                else if (string.Compare(Tables.Items[i].ToString(), table_name + "s", true) == 0)
                {
                    table_name = Tables.Items[i].ToString();
                    break;
                }
                else if (string.Compare(Tables.Items[i].ToString() + "s", table_name, true) == 0)
                {
                    table_name = Tables.Items[i].ToString();
                    break;
                }
            }
            if (table_name == "") return null;

            // Get table first column name (should be the primary key)
            try
            {
                var test = Cnx.ExecuteDataSet("SELECT * FROM " + table_name + " WHERE 1 = 2");
                column_name = test.Tables[0].Columns[0].ColumnName;
            }
            catch { return null; }

            // Return table name and pk name
            return new[] { table_name, column_name };
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // A cell has been clicked

            // Don't track column header
            if (e.RowIndex < 0) return;

            // Select the cell after 2 clicks
            if (PreviousCellClick != null)
            {
                if (e.RowIndex == PreviousCellClick.RowIndex)
                {
                    if (e.ColumnIndex == PreviousCellClick.ColumnIndex)
                    {
                        Grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
                        var cell = Grid[e.ColumnIndex, e.RowIndex];
                        cell.Selected = true;
                        return;
                    }
                }
            }

            // Select the full row otherwise
            PreviousCellClick = e;
            Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Grid.Rows[e.RowIndex].Selected = true;
        }

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Don't track data errors for binary columns

            if (Grid.Columns[e.ColumnIndex].ValueType == typeof(Byte[]))
            {
                e.Cancel = true;
                e.ThrowException = false;
            }
            else
            {
                e.Cancel = false;
                e.ThrowException = true;
            }
        }

        private void NewQuery(string sql)
        {
            // Add query to the editor area
            var start = Query.Text.Length;
            if (Query.Text.Length > 0)
            {
                sql = "\n\n" + sql;
                start += 2;
            }
            Query.AppendText(sql);

            // Auto-select new query
            Query.SelectionStart = start;
            Query.SelectionLength = sql.Length;
            Query.Focus();

            // Auto-run new query
            ExecuteSql(null, null);
        }
    }
}
