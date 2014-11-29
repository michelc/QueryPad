using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Altrr;

namespace QueryPad
{
    public partial class QueryForm : Form
    {
        private Connexion Cnx { get; set; }

        private bool ControlKey = false;
        private CancellationTokenSource Cancellation;
        private DataTableResult QueryResult;

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
            QueryResult = new DataTableResult();

            Execute.Text = char.ConvertFromUtf32(9654) + " " + Execute.Text;
            Stop.Text = char.ConvertFromUtf32(9632) + " " + Stop.Text;
            Commit.Text = char.ConvertFromUtf32(8730) + " " + Commit.Text;
            Rollback.Text = char.ConvertFromUtf32(9587) + " " + Rollback.Text;
            Rotate.Text = char.ConvertFromUtf32(8984) + " " + Rotate.Text;
            FreezeToolbar(false);
        }

        protected override void OnClosed(EventArgs e)
        {
            // Close connection when form is closed

            Cnx.Close();
            base.OnClosed(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Check if Control key is pressed

            ControlKey = e.Control;
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            // No Control key is pressed

            ControlKey = false;
            base.OnKeyUp(e);
        }

        private async void ExecuteSql(object sender, EventArgs e)
        {
            // Get query to run
            var sql = Editor.CurrentQuery();
            if (sql == "") return;
            if (!sql.StartsWith("FORMAT ")) sql = ExecuteSql_Prepare();

            var index = -1;
            var count = -1;
            var start = DateTime.Now;

            Cancellation = new CancellationTokenSource();

            // Run query
            var check = "";
            try
            {
                check = sql.Substring(0, 6).ToUpper();
                if (check == "SELECT")
                {
                    // Read data from DB
                    QueryResult = Cnx.ExecuteDataTable(sql);

                    // Display DB access statistics
                    count = QueryResult.RowCount;
                    ExecuteSql_Message(index, count, start);
                    last_message = Informations.Text;

                    // Add data to Grid
                    Display_List(QueryResult.DataTable, QueryResult.IsSlow);
                    if (QueryResult.RowCount > 0) index = 0;
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
                    Grid.Select();
                    return;
                }
                else if (check == "FORMAT")
                {
                    Format_List(sql.Substring(7).Trim(), QueryResult);
                    ShowInformations(Grid.RowCount.ToString() + " lines");
                    Grid.Select();
                    return;
                }
                else
                {
                    // Update DB
                    check = "";
                    var task_write = ExecuteSql_NonQueryAsync(sql);
                    await task_write;

                    count = task_write.Result;

                    Commit.Visible = Cnx.UseTransaction;
                    Rollback.Visible = Cnx.UseTransaction;
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
            ExecuteSql_Message(index, count, start);
            if (check == "") Editor.Select(); else Grid.Select();
        }

        private string ExecuteSql_Prepare()
        {
            // Clear results
            QueryResult = new DataTableResult();
            Grid.DataSource = null;
            Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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

        private void Format_List(string format, DataTableResult result)
        {
            // Check if we use single or double quotation marks
            var single_quote = format.Contains("'{") && format.Contains("}'");
            var double_quote = format.Contains("\"{") && format.Contains("}\"");

            // Check columns with string values
            var count = Grid.ColumnCount;
            var is_string = new bool[count];
            foreach (DataGridViewColumn c in Grid.Columns)
            {
                if (object.ReferenceEquals(c.ValueType, typeof(string))) is_string[c.Index] = true;
                if (object.ReferenceEquals(c.GetType(), typeof(string))) is_string[c.Index] = true;
            }

            // Format data in a new DataTable
            var dt = new DataTable();
            dt.Columns.Add(" ", typeof(String));
            var data = new object[count];
            foreach (DataGridViewRow row in Grid.Rows)
            {
                for (var i = 0; i < count; i++)
                {
                    data[i] = row.Cells[i].Value;
                    if (data[i] == DBNull.Value)
                    {
                        // Explicit null values
                        data[i] = "null";
                    }
                    else if (is_string[i])
                    {
                        // Escape string values
                        if (single_quote)
                        {
                            data[i] = data[i].ToString().Replace("'", "''");
                        }
                        else if (double_quote)
                        {
                            data[i] = data[i].ToString().Replace("\"", "\"\"");
                        }
                    }
                }
                var line = string.Format(format, data).Replace("'null'", "null").Replace("\"null\"", "\"\"");
                dt.Rows.Add(line);
            }

            // Initialize grid
            Grid.DataSource = dt;
            result.RowCount = 0; // avoid Rotate

            // Resize value column width
            Grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void Display_List(DataTable dt, bool slow)
        {
            Cursor = Cursors.WaitCursor;

            // Initialize grid
            Grid.SuspendLayout();
            Grid.ColumnHeadersVisible = false;
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
            if (slow) mode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            if (count < 50) Grid.ColumnHeadersVisible = true;
            Grid.AutoResizeColumns(mode);

            // Check for big widths
            Grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            for (var i = 0; i < Grid.Columns.Count; i++)
            {
                if (Grid.Columns[i].Width > 750) Grid.Columns[i].Width = 750;
            }

            // Return loaded rows count
            Grid.ColumnHeadersVisible = true;
            Grid.ResumeLayout();
        }

        private void Display_RowDetail()
        {
            Cursor = Cursors.WaitCursor;

            // Create a datatable with current row pivoted
            var headers = QueryResult.DataTable.Columns.Cast<DataColumn>().ToList();
            var datas = QueryResult.DataTable.Rows[QueryResult.RowIndex].ItemArray;
            var dt = new DataTable();
            dt.Columns.Add("#", typeof(Int32));
            dt.Columns.Add("Type", typeof(String));
            dt.Columns.Add("Column", typeof(String));
            dt.Columns.Add("Value");
            for (var x = 0; x < headers.Count; x++)
            {
                dt.Rows.Add(new object[] { x
                                         , headers[x].DataType.ToString().Replace("System.", "")
                                         , headers[x].Caption
                                         , datas[x] });
            }

            // Initialize grid
            Grid.DataSource = dt;

            // Title case Oracle columns name
            if (Cnx.CnxParameter.Provider.Contains("Oracle"))
            {
                var ti = CultureInfo.CurrentCulture.TextInfo;
                for (var i = 0; i < Grid.Rows.Count; i++)
                {
                    var text = Grid.Rows[i].Cells[2].Value.ToString().ToLower();
                    Grid.Rows[i].Cells[2].Value = ti.ToTitleCase(text);
                }
            }

            // Resize value column width
            Grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            Grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            for (var i = 0; i < Grid.Rows.Count; i++)
            {
                var size = Grid.Rows[i].Cells[3].Value.ToString().Length;
                if (size > 100)
                {
                    Grid.Rows[i].Height = Grid.Rows[i].Height * ((size / 100) + 1);
                }
            }
            Grid.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Adjust columns styles
            Grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
            Grid.Columns[0].DefaultCellStyle.ForeColor = Color.Gray;
            Grid.Columns[1].DefaultCellStyle.ForeColor = Color.Gray;
            Grid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopRight;
        }

        private async Task<int> ExecuteSql_NonQueryAsync(string sql)
        {
            var count = Cnx.ExecuteNonQueryAsync(sql, Cancellation.Token);
            await count;

            return count.Result;
        }

        private void ExecuteSql_Message(int index, int count, DateTime? start)
        {
            // Display statistics

            var message = string.Format("{0} rows", count);
            if ((index != -1) && (count > 1)) message = string.Format("{0}/{1} rows", index + 1, count);
            if (count == -10001) message = "commit";
            if (count == -10002) message = "rollback";
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
            if (!string.IsNullOrEmpty(last_message))
            {
                Informations.Text = last_message + Environment.NewLine + message;
                last_message = "";
            }
            else
            {
                Informations.Text = message;
            }
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

            // Enable or disable [Rotate] button
            Rotate.Enabled = (is_busy == false) && (QueryResult.RowCount > 0);
            if (Rotate.Enabled)
            {
                Rotate.BackColor = Color.FromKnownColor(KnownColor.Highlight);
                Rotate.ForeColor = Color.FromKnownColor(KnownColor.HighlightText);
                Rotate.Visible = true;
            }
            else
            {
                Rotate.BackColor = Color.LightGray;
                Rotate.ForeColor = Color.WhiteSmoke;
                Rotate.Visible = false;
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

            if (!ControlKey)
            {
                // Simple Double-click
                // => generate & run select query to display its content
                Editor.AppendQuery("SELECT * FROM " + Tables.SelectedValue);
            }
            else
            {
                // Control + Double-click
                // => generate & run desc query to display its structure
                Editor.AppendQuery("DESC " + Tables.SelectedValue);
            }
            ExecuteSql(null, null);
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
                else if (string.Compare(Tables.Items[i].ToString(), "dir_" + table_name + "s", true) == 0)
                {
                    // PI.MDB very specific
                    table_name = Tables.Items[i].ToString();
                    break;
                }
            }
            if (table_name == "") return null;

            // Get table first column name (should be the primary key)
            try
            {
                var test = Cnx.ExecuteDataTable("SELECT * FROM " + table_name + " WHERE 1 = 2");
                column_name = test.DataTable.Columns[0].ColumnName;
            }
            catch { return null; }

            // Return table name and pk name
            return new[] { table_name, column_name };
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // A cell has been clicked

            // Don't track headers
            if (e.ColumnIndex < 0) return;
            if (e.RowIndex < 0) return;

            // Reset full row select
            if (Grid.SelectionMode != DataGridViewSelectionMode.FullRowSelect)
            {
                Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                Grid.Rows[e.RowIndex].Selected = true;
            }

            // Update status
            if (QueryResult.RowCount <= 1) return;
            ExecuteSql_Message(e.RowIndex, Grid.RowCount, null);
        }

        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Double-click a cell

            // Don't track headers
            if (e.ColumnIndex < 0) return;
            if (e.RowIndex < 0) return;

            // Select current cell if simple double click
            if (!ControlKey)
            {
                Grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
                Grid[e.ColumnIndex, e.RowIndex].Selected = true;
                return;
            }

            // Control + Double-click
            // => checks if it's an ID column
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

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (QueryResult.RowCount <= 1) return;
            if (QueryResult.RowIndex != -1)
            {
                // Check left and right arrows to browse rows
                Browse_RowDetail(e);
                ExecuteSql_Message(QueryResult.RowIndex, QueryResult.RowCount, null);
            }
            else
            {
                // Check page down and down arrow to load new page
                Browse_NextPage(e);
                ExecuteSql_Message(Grid.CurrentRow.Index, QueryResult.RowCount, null);
            }
        }

        private void Browse_RowDetail(KeyEventArgs e)
        {
            // Use left and right arrows to browse rows

            // Get previous or next row index
            if (e.KeyCode == Keys.Left)
            {
                QueryResult.RowIndex = (QueryResult.RowIndex == 0) ? QueryResult.RowCount - 1 : QueryResult.RowIndex - 1;
            }
            else if (e.KeyCode == Keys.Right)
            {
                QueryResult.RowIndex = (QueryResult.RowIndex == QueryResult.RowCount - 1) ? 0 : QueryResult.RowIndex + 1;
            }
            else
            {
                return;
            }

            var state = GridGetState(Grid);
            Display_RowDetail();
            GridSetState(Grid, state);
            Cursor = Cursors.Default;
        }

        private DataGridState GridGetState(DataGridView grid)
        {
            var state = new DataGridState
            {
                CurrentRow = grid.CurrentRow.Index,
                SortedColumn = (grid.SortedColumn == null) ? -1 : grid.SortedColumn.Index,
                SortDirection = (grid.SortOrder == SortOrder.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending,
                Widths = grid.Columns.Cast<DataGridViewColumn>().Select(c => c.Width).ToArray()
            };

            return state;
        }

        private void GridSetState(DataGridView grid, DataGridState state)
        {
            grid.CurrentCell = grid.Rows[state.CurrentRow].Cells[0];
            if (state.SortedColumn != -1) grid.Sort(Grid.Columns[state.SortedColumn], state.SortDirection);
            foreach (DataGridViewColumn c in grid.Columns)
            {
                c.Width = state.Widths[c.Index];
            }
        }

        private void Browse_NextPage(KeyEventArgs e)
        {
            // Check if we need to load more rows from current query

            if ((e.KeyCode == Keys.PageDown) || (e.KeyCode == Keys.Down))
            {
                // There must be more rows to load
                if (QueryResult.IsFull) return;

                // Just page down or down arrow without Ctrl, Alt or Shift
                if (e.Control) return;
                if (e.Alt) return;
                if (e.Shift) return;

                // Current row must be the last row
                if (Grid.CurrentRow == null) return;
                if (Grid.CurrentRow.Index != Grid.RowCount - 1) return;

                // Load a new page of data
                try
                {
                    QueryResult = Cnx.ExecuteNextPage(QueryResult);
                    Grid.SuspendLayout();
                    Grid.DataSource = QueryResult.DataTable;
                    Grid.ResumeLayout();
                }
                catch (Exception ex)
                {
                    var caption = "Error " + ex.HResult.ToString("x");
                    var text = string.Format("{0}\n\n({1})", ex.Message, ex.Source);
                    MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ShowInformations("Error");
                }

                Cursor = Cursors.Default;
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

        private void Rotate_Click(object sender, EventArgs e)
        {
            // [Rotate] button has been used
            // => alternate between row or list view

            if (QueryResult.RowIndex == -1)
            {
                // Set DataTable order to DataGridView order
                QueryResult.GridState = GridGetState(Grid);
                if (QueryResult.GridState.SortedColumn != -1)
                {
                    var sort = Grid.SortedColumn.Name;
                    if (Grid.SortOrder == SortOrder.Descending) sort += " DESC";
                    QueryResult.DataTable = QueryResult.DataTable.Select("", sort).CopyToDataTable();
                }
                // Display as row detail
                QueryResult.RowIndex = Grid.CurrentRow.Index;
                Display_RowDetail();
            }
            else
            {
                // Display as rows list
                Display_List(QueryResult.DataTable, false);
                QueryResult.GridState.CurrentRow = QueryResult.RowIndex;
                GridSetState(Grid, QueryResult.GridState);
                QueryResult.RowIndex = -1;
            }

            Grid.Select();
            FreezeToolbar(false);
        }

        private void Commit_Click(object sender, EventArgs e)
        {
            // [Commit] button has been used
            // => commit current transaction

            Editor.AppendQuery("COMMIT");
            ExecuteSql(null, null);
        }

        private void Rollback_Click(object sender, EventArgs e)
        {
            // [Rollback] button has been used
            // => rollback current transaction

            Editor.AppendQuery("ROLLBACK");
            ExecuteSql(null, null);
        }

        private void ConnexionName_DoubleClick(object sender, EventArgs e)
        {
            // Connexion name was double clicked
            // => reload table list

            Tables.DataSource = Cnx.GetTables();
        }
    }
}
