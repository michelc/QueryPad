using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Altrr;

namespace QueryPad
{
    public partial class QueryForm : Form
    {
        private Connexion Cnx { get; set; }
        private DataGridViewCellEventArgs PreviousCellClick;

        private CancellationTokenSource Cancellation;

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
            Editor.ConfigureTabs();

            Execute.Text = char.ConvertFromUtf32(9654) + " " + Execute.Text;
            Stop.Text = char.ConvertFromUtf32(9632) + " " + Stop.Text;
            Commit.Text = char.ConvertFromUtf32(8730) + " " + Commit.Text;
            Rollback.Text = char.ConvertFromUtf32(9587) + " " + Rollback.Text;
            FreezeToolbar(false);
        }

        protected override void OnClosed(EventArgs e)
        {
            // Close connection when form is closed

            Cnx.Close();
            base.OnClosed(e);
        }

        private async void ExecuteSql(object sender, EventArgs e)
        {
            // Get query to run
            var sql = ExecuteSql_Prepare();
            if (sql == "") return;

            var count = -1;
            var total = 0;
            var start = DateTime.Now;

            Cancellation = new CancellationTokenSource();

            // Run query
            try
            {
                var check = sql.Substring(0, 6).ToUpper();
                if (check == "SELECT")
                {
                    // Read data from DB
                    var dt = Cnx.ExecuteDataTable(sql);

                    // Display DB access statistics
                    total = dt.Rows.Count;
                    ExecuteSql_Message(0, total, start);

                    // Add data to Grid
                    count = ExecuteSql_Load(dt);
                }
                else if (check.StartsWith("DESC"))
                {
                    // Read informations from schema
                    sql = sql.Substring(4).Trim();
                    Grid.DataSource = new SortableBindingList<Column>(Cnx.GetColumns(sql));
                    Grid.AutoResizeColumns();

                    // Display columns statistics
                    if (Grid.RowCount > 0)
                    {
                        ShowInformations(Grid.RowCount.ToString() + " columns");
                    }
                    else
                    {
                        ShowInformations("No such table");
                    }
                    return;
                }
                else
                {
                    // Update DB
                    var task_write = ExecuteSql_NonQueryAsync(sql);
                    await task_write;

                    count = task_write.Result;

                    Commit.Visible = true;
                    Rollback.Visible = true;
                }
            }
            catch (TaskCanceledException)
            {
                ShowInformations("Stopped");
                return;
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
            ExecuteSql_Message(count, total, start);
        }

        private string ExecuteSql_Prepare()
        {
            // Clear results
            Grid.DataSource = null;
            Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            PreviousCellClick = null;
            ShowInformations("");

            // Get current query to execute
            var sql = Editor.CurrentQuery();

            // Check if query is empty
            if (sql == "")
            {
                ShowInformations("No query !");
                return "";
            }

            // Update display
            FreezeToolbar(true);
            ShowInformations("Executing...");

            // Return the query to run
            return sql;
        }

        private int ExecuteSql_Load(DataTable dt)
        {
            Cursor = Cursors.WaitCursor;

            // Initialize grid
            Grid.DataSource = dt;
            var count = Grid.RowCount;

            // Title case Oracle columns header
            if (Cnx.CnxParameter.Provider.Contains("Oracle"))
            {
                var ti = CultureInfo.CurrentCulture.TextInfo;
                for (var i = 0; i < Grid.Columns.Count; i++)
                {
                    var text = Grid.Columns[i].HeaderText.ToLower();
                    Grid.Columns[i].HeaderText = ti.ToTitleCase(text);
                }
            }

            // Auto-resize columns width
            var mode = (count < 250) ? DataGridViewAutoSizeColumnsMode.AllCells : DataGridViewAutoSizeColumnsMode.DisplayedCells;
            Grid.AutoResizeColumns(mode);

            // Check for big widths
            Grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            for (var i = 0; i < Grid.Columns.Count; i++)
            {
                if (Grid.Columns[i].Width > 750) Grid.Columns[i].Width = 750;
            }

            // Return loaded rows count
            return Grid.RowCount;
        }

        private async Task<int> ExecuteSql_NonQueryAsync(string sql)
        {
            var count = Cnx.ExecuteNonQueryAsync(sql, Cancellation.Token);
            await count;

            return count.Result;
        }

        private void ExecuteSql_Message(int count, int total, DateTime? start)
        {
            // Display statistics

            var message = string.Format("{0} rows", count);
            if (count < total) message = string.Format("{0}/{1} rows", count, total);
            if (start != null)
            {
                var duration = DateTime.Now.Subtract(start.Value);
                message += string.Format(" ({0:00}:{1:00}.{2:000})"
                                       , duration.Minutes, duration.Seconds, duration.Milliseconds);
            }
            if (count < 2) message = message.Replace(" rows ", " row ");
            ShowInformations(message);
        }

        private void ShowInformations(string message)
        {
            // Force informations display
            Informations.Text = last_message + Environment.NewLine + message;
            last_message = message;
            Informations.Left = Toolbar.ClientSize.Width - Informations.Width;
            this.Refresh();
        }
        private string last_message = "";

        private void FreezeToolbar(bool is_busy)
        {
            // Enable or disable [Execute] button
            Execute.Enabled = !is_busy;
            if (!is_busy)
            {
                Execute.BackColor = Color.FromKnownColor(KnownColor.Highlight);
                Execute.ForeColor = Color.FromKnownColor(KnownColor.HighlightText);
            }
            else
            {
                Execute.BackColor = Color.LightGray;
                Execute.ForeColor = Color.WhiteSmoke;
            }

            // Enable or disable [Stop] button
            Stop.Enabled = is_busy;
            if (is_busy)
            {
                Stop.BackColor = Color.FromKnownColor(KnownColor.Highlight);
                Stop.ForeColor = Color.FromKnownColor(KnownColor.HighlightText);
            }
            else
            {
                Stop.BackColor = Color.LightGray;
                Stop.ForeColor = Color.WhiteSmoke;
            }

            // Enable or disable [Commit] and [Rollback] buttons
            Commit.Enabled = !is_busy;
            Rollback.Enabled = !is_busy;

            // Change cursor to hourglass during working
            Cursor = is_busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private void Tables_DoubleClick(object sender, EventArgs e)
        {
            // Double-click a table name

            if (Tables.Tag == null)
            {
                // Simple Double-click
                // => generate & run select query to display its content
                Editor.AppendQuery("SELECT * FROM " + Tables.SelectedValue);
            }
            else
            {
                // Control Double-click
                // => generate & run desc query to display its structure
                Editor.AppendQuery("DESC " + Tables.SelectedValue);
            }
            ExecuteSql(null, null);
        }

        private void Tables_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if Control key is pressed

            if (e.Control) Tables.Tag = "Control";
        }

        private void Tables_KeyUp(object sender, KeyEventArgs e)
        {
            // No Control key is pressed

            Tables.Tag = null;
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
                var test = Cnx.ExecuteDataTable("SELECT * FROM " + table_name + " WHERE 1 = 2");
                column_name = test.Columns[0].ColumnName;
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

        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Double-click a cell

            // Don't track column header
            if (e.RowIndex < 0) return;

            // Checks if it's an ID column
            var cell = Grid[e.ColumnIndex, e.RowIndex];
            var data = QuickNavigation(cell.OwningColumn.HeaderText);
            if (data == null) return;

            // Yes => generate & run select query to display related data
            var value = cell.Value.ToString();
            if (cell.ValueType == typeof(string)) value = "'" + value + "'";
            var sql = string.Format("SELECT * FROM {0} WHERE {1} = {2}"
                                    , data[0]
                                    , data[1]
                                    , value);
            Editor.AppendQuery(sql);
            ExecuteSql(null, null);
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Set Yellow background for null values

            if (e.Value == DBNull.Value)
            {
                e.CellStyle.BackColor = Color.FromArgb(0xFF, 0xDC, 0x00); // Yellow
            }
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

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            // Detect a keyboard event

            // Paste can be Ctrl+V (or Shift+Ins for old schools)
            var ControlV = (e.Control && (e.KeyCode == Keys.V));
            var ShiftInsert = (e.Shift && (e.KeyCode == Keys.Insert));

            // Only paste plain text
            if (ControlV || ShiftInsert)
            {
                if (Clipboard.ContainsText())
                {
                    Editor.Paste(DataFormats.GetFormat("Text"));
                }
                e.Handled = true;
            }
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            // [Stop] button has been used
            // => cancel current query

            Cancellation.Cancel();
        }

        private void Commit_Click(object sender, EventArgs e)
        {
            // [Commit] button has been used
            // => commit current transaction

            Editor.AppendQuery("COMMIT");
            ExecuteSql(null, null);

            Commit.Visible = false;
            Rollback.Visible = false;
            ShowInformations("commit");
        }

        private void Rollback_Click(object sender, EventArgs e)
        {
            // [Rollback] button has been used
            // => rollback current transaction

            Editor.AppendQuery("ROLLBACK");
            ExecuteSql(null, null);

            Commit.Visible = false;
            Rollback.Visible = false;
            ShowInformations("rollback");
        }
    }
}
