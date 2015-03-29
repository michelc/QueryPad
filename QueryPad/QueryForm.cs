using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private DateTime? StartTime;
        private DataTableResult QueryResult;
        private CancellationTokenSource Cancellation;

        public QueryForm(CnxParameter CnxParameter)
        {
            InitializeComponent();

            Name = CnxParameter.Name;
            Text = CnxParameter.Name;

            switch (CnxParameter.Environment)
            {
                case "Test":
                    Execute.Tag = Commit.Tag = Color.Orange;
                    break;
                case "Release":
                    Execute.Tag = Commit.Tag = Color.Red;
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

            Grid.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            Grid.DefaultCellStyle.ForeColor = Tables.ForeColor = Color.FromArgb(0x45, 0x44, 0x41);

            Tables.DataSource = Cnx.GetTables(false);
            Filter.Top = 16;
            Filter.Width = Tables.Width;
            Filter.Visible = Tables.Items.Count > 20;

            Editor.SyntaxHighlighter.StatementsStyle = Editor.SyntaxHighlighter.KeywordStyle
                                                     = Editor.SyntaxHighlighter.TypesStyle
                                                     = Editor.SyntaxHighlighter.BlueStyle;

            QueryResult = new DataTableResult();

            Execute.Text = char.ConvertFromUtf32(9654) + " " + Execute.Text;
            Stop.Text = char.ConvertFromUtf32(9632) + " " + Stop.Text;
            Commit.Text = char.ConvertFromUtf32(8730) + " " + Commit.Text;
            Rollback.Text = char.ConvertFromUtf32(9587) + " " + Rollback.Text;
            Rotate.Text = char.ConvertFromUtf32(8984) + " " + Rotate.Text;
            EnableUI(true);
            RunTime_Enable(false);
            RunTime_Tick(null, null);
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

        private void Execute_Click(object sender, EventArgs e)
        {
            // Get query to run
            var sql = Editor.CurrentQuery();
            if (sql == "") return;

            // Clear results
            var type = Execute_Type(sql);
            if (type != SqlType.Format)
            {
                QueryResult = new DataTableResult();
                Grid.DataSource = null;
                Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }

            // Update display
            EnableUI(false);
            StartTime = DateTime.Now;
            RunTime_Enable(true);

            // Execute SQL
            switch (type)
            {
                case SqlType.Query:
                    // Read data from DB
                    Execute_QueryAsync(sql);
                    break;
                case SqlType.Desc:
                    // Describe a DB table
                    Execute_Desc(sql.Substring(4).Trim());
                    break;
                case SqlType.Format:
                    // Format DB data
                    Execute_Format(sql.Substring(7).Trim());
                    break;
                case SqlType.NonQuery:
                    // Update DB
                    Execute_NonQueryAsync(sql, type);
                    break;
                case SqlType.Script:
                    // Run Script
                    Execute_NonQueryAsync(sql, type);
                    break;
            }
        }

        private enum SqlType { Query, NonQuery, Desc, Format, Script };

        private SqlType Execute_Type(string sql)
        {
            var check = sql.Length < 6 ? "" : sql.Substring(0, 6).ToUpper();

            // Describe
            if (check.StartsWith("DESC ")) return SqlType.Desc;

            // Formatting
            if (check == "FORMAT") return SqlType.Format;

            // Script
            var commands = sql.SplitCommands();
            // It's a script when it starts with "BEGIN" and ends with "END;"
            if (commands.First() + commands.Last() == "BEGINEND;") return SqlType.Script;
            // It's a script when there is more than 2 statements
            if (commands.Length > 1) return SqlType.Script;

            // Query
            if (check == "SELECT") return SqlType.Query;
            if (check.StartsWith("EXEC ")) return SqlType.Query;

            // NonQuery
            return SqlType.NonQuery;
        }

        private async void Execute_QueryAsync(string sql)
        {
            Cancellation = new CancellationTokenSource();
            var infos = "";

            try
            {
                // Execute query
                QueryResult = await Task.Run(() => Cnx.ExecuteDataTable(sql, Cancellation.Token));

                // Display DB access statistics
                var count = QueryResult.RowCount;
                var index = -1;
                Execute_Message(index, count, StartTime);
                last_message = Informations.Text;

                // Add data to Grid
                Display_List(QueryResult.DataTable, QueryResult.IsSlow, false);
                if (QueryResult.RowCount > 0) index = 0;
                Execute_Message(index, count, StartTime);
            }
            catch (OperationCanceledException)
            {
                infos = "Stopped";
            }
            catch (Exception ex)
            {
                infos = Execute_Error(ex);
            }
            finally
            {
                Execute_End(infos, SqlType.Query);
            }
        }

        private void Execute_Desc(string sql)
        {
            var informations = "";
            try
            {
                // Read informations from schema
                Grid.DataSource = new SortableBindingList<Column>(Cnx.GetColumns(sql));
                Grid.AutoResizeColumns();
                if (Grid.RowCount > 0)
                {
                    informations = Grid.RowCount.ToString() + " columns";
                }
                else
                {
                    informations = "No such table";
                }
            }
            catch (Exception ex)
            {
                informations = Execute_Error(ex);
            }

            // Reset display
            Execute_End(informations, SqlType.Desc);
        }

        private void Execute_Format(string sql)
        {
            // Avoid error when there is no data to format
            if (Grid.RowCount == 0)
            {
                Execute_End("no data", SqlType.Format);
                return;
            }

            // Reset Grid with original datas
            // (after a previous Format command)
            if (QueryResult.GridState.Widths != null)
            {
                Display_List(QueryResult.DataTable, false, true);
                GridSetState(Grid, QueryResult.GridState);
                Grid.ColumnHeadersVisible = true;
            }
            else
            {
                QueryResult.GridState = GridGetState(Grid);
            }

            var informations = "";
            try
            {
                // Format DB data
                if (sql.ToLower() == "list")
                {
                    Format_Text(sql.ToLower());
                }
                else if (sql.ToLower() == "grid")
                {
                    Format_Text(sql.ToLower());
                }
                else
                {
                    Format_List(sql, QueryResult);
                }
                informations = Grid.RowCount.ToString() + " lines";
            }
            catch (Exception ex)
            {
                informations = Execute_Error(ex);
            }

            // Reset display
            Execute_End(informations, SqlType.Format);
        }

        private void Execute_NonQueryAsync(string sql, SqlType type)
        {
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Cancellation = new CancellationTokenSource();

            // Update DB
            var run = type == SqlType.NonQuery
                    ? new Task<int>(() => Cnx.ExecuteNonQuery(sql))
                    : new Task<int>(() => Cnx.ExecuteNonQueries(sql));

            // Success => display results
            var ok = run.ContinueWith<string>((t) =>
            {
                // Show task result
                var index = type == SqlType.NonQuery ? -1 : -2;
                Execute_Message(index, t.Result, StartTime);

                // Show transaction buttons if necessary
                Commit.Visible = Cnx.UseTransaction;
                Rollback.Visible = Cnx.UseTransaction;

                return "";
            }, Cancellation.Token, TaskContinuationOptions.OnlyOnRanToCompletion, scheduler);

            // Failure => show error
            var ko = run.ContinueWith<string>((t) =>
            {
                return Execute_Error(t.Exception.InnerException);
            }
            , Cancellation.Token, TaskContinuationOptions.OnlyOnFaulted, scheduler);

            // Reset display at the end
            var end_ok = ok.ContinueWith((t) => Execute_End(t.Result, SqlType.NonQuery), scheduler);
            var end_ko = ko.ContinueWith((t) => Execute_End(t.Result, SqlType.NonQuery), scheduler);

            // Start nonquery task
            run.Start();
        }

        private string Execute_Error(Exception ex)
        {
            ControlKey = false;
            RunTime_Enable(false);

            var caption = "Error " + ex.HResult.ToString("x");
            var text = string.Format("{0}\n\n({1})", ex.Message, ex.Source);
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "Error";
        }

        private void Execute_End(string informations, SqlType type)
        {
            // Reset display
            RunTime_Enable(false);
            if (informations != "") ShowInformations(informations);

            EnableUI(true);
            if ((Grid.DataSource != null) && (Grid.RowCount > 0))
            {
                if (type == SqlType.Query)
                {
                    Grid.Columns[Grid.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                Grid.Select();
            }
            else
            {
                Editor.Select();
            }
        }

        private void Execute_Message(int index, int count, DateTime? start)
        {
            // Display statistics

            var message = string.Format("{0} rows", count);
            if ((index >= 0) && (count > 1)) message = string.Format("{0}/{1} rows", index + 1, count);
            if (count == -10001) message = "commit";
            if (count == -10002) message = "rollback";
            if (start != null)
            {
                var duration = DateTime.Now.Subtract(start.Value);
                message += string.Format(" ({0:0}:{1:00}.{2:000})"
                                       , duration.Minutes, duration.Seconds, duration.Milliseconds);
            }
            if (count < 2) message = message.Replace(" rows ", " row ");
            if (index == -2) message = message.Replace(" row", " command");
            ShowInformations(message);
        }

        private void Format_List(string format, DataTableResult result)
        {
            // Unescapes literal tabs or new lines
            format = format.Replace("\\t", "\t");
            format = format.Replace("\\r", "\r");
            format = format.Replace("\\n", "\n");

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
                format = Regex.Replace(format, "\\{" + c.Name + "\\}", "{" + c.Index + "}", RegexOptions.IgnoreCase);
                format = Regex.Replace(format, "\\{" + c.Name + "\\:", "{" + c.Index + ":", RegexOptions.IgnoreCase);
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

        private void Format_Text(string mode)
        {
            // Get columns alignment (left or right)
            var count = Grid.ColumnCount;
            var is_left = new bool[count];
            foreach (DataGridViewColumn c in Grid.Columns)
            {
                if (object.ReferenceEquals(c.ValueType, typeof(string))) is_left[c.Index] = true;
                if (object.ReferenceEquals(c.GetType(), typeof(string))) is_left[c.Index] = true;
                if (object.ReferenceEquals(c.ValueType, typeof(DateTime))) is_left[c.Index] = true;
                if (object.ReferenceEquals(c.GetType(), typeof(DateTime))) is_left[c.Index] = true;
            }

            // Get columns maxlength
            var maxlengths = new int[count];
            foreach (DataGridViewRow row in Grid.Rows)
            {
                for (var i = 0; i < count; i++)
                {
                    var length = row.Cells[i].FormattedValue.ToString().Length;
                    if (maxlengths[i] < length) maxlengths[i] = length;
                }
            }

            // Build export format and headers
            var format = "|";
            var header = "|";
            var hbreak = "|";
            for (var i = 0; i < count; i++)
            {
                var caption = Grid.Columns[i].HeaderText;
                maxlengths[i] = Math.Max(maxlengths[i], caption.Length);
                maxlengths[i] = Math.Min(maxlengths[i], 100);
                header += string.Format(" {0} |", caption.PadRight(maxlengths[i]));
                hbreak += string.Format("-{0}-|", "".PadRight(maxlengths[i], '-'));
                if (is_left[i]) maxlengths[i] = -maxlengths[i];
                format += string.Format(" {{{0},{1}}} |", i, maxlengths[i]);
            }
            if (mode == "list")
            {
                format = format.Replace("|", "");
                header = header.Replace("|", "");
                hbreak = hbreak.Replace("-|-", "  ").Replace("|-", "").Replace("-|", "");
            }
            format = format.Trim();

            // Export data as text
            var text = new StringBuilder();
            text.AppendLine(header.Trim());
            text.AppendLine(hbreak.Trim());
            var data = new string[count];
            foreach (DataGridViewRow row in Grid.Rows)
            {
                for (var i = 0; i < count; i++)
                {
                    data[i] = row.Cells[i].FormattedValue
                                          .ToString()
                                          .Replace(Environment.NewLine, " ")
                                          .Replace('\r', ' ')
                                          .Replace('\n', ' ')
                                          .Replace('\t', ' ');
                    if (data[i].Length > 100) data[i] = data[i].Substring(0, 99) + "…";
                }
                var line = string.Format(format, data).TrimEnd();
                text.AppendLine(line);
            }

            // Export is available via clipboard
            Clipboard.SetText(text.ToString());
        }

        private void Display_List(DataTable dt, bool slow, bool reload)
        {
            Cursor = Cursors.WaitCursor;

            // Initialize grid
            Grid.SuspendLayout();
            Grid.ColumnHeadersVisible = false;
            Grid.DataSource = dt;

            // Set columns title
            for (var i = 0; i < Grid.ColumnCount; i++)
            {
                Grid.Columns[i].HeaderText = QueryResult.Titles[i];
            }

            // Just refresh grid display
            if (reload)
            {
                Grid.ResumeLayout();
                return;
            }

            // Auto-resize columns width
            var count = dt.Rows.Count;
            var mode = (count < 250) ? DataGridViewAutoSizeColumnsMode.AllCells : DataGridViewAutoSizeColumnsMode.DisplayedCells;
            if (slow)
            {
                mode = DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;
            }
            else
            {
                Grid.ColumnHeadersVisible = true;
            }
            Grid.AutoResizeColumns(mode);
            
            // Check for big widths
            Grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            for (var i = 0; i < Grid.ColumnCount; i++)
            {
                if (Grid.Columns[i].Width > 750) Grid.Columns[i].Width = 750;
            }

            // Refresh grid display
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

            // Set columns title
            for (var i = 0; i < Grid.RowCount; i++)
            {
                Grid.Rows[i].Cells[2].Value = QueryResult.Titles[i];
            }

            // Resize value column width
            Grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            Grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            for (var i = 0; i < Grid.RowCount; i++)
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

        private void EnableUI(bool onoff)
        {
            // Table list is enabled when toolbar is
            Tables.Enabled = onoff;
            Filter.Enabled = onoff;
            ConnexionName.Enabled = onoff;

            // [Execute] button is enabled when toolbar is
            Execute.Enable(onoff);

            // [Stop] button is disabled when toolbar is enabled
            Stop.Enable(!onoff);

            // [Rotate] button is enabled when toolbar is enabled and result is not empty
            Rotate.Enable(onoff && (QueryResult.RowCount > 0));
            Rotate.Visible = Rotate.Enabled;

            // [Commit] and [Rollback] buttons are enabled when toolbar is
            Commit.Enable(onoff);
            Rollback.Enable(onoff);

            // Cursor is hourglass when toolbar is disabled
            Cursor = onoff ? Cursors.Default : Cursors.WaitCursor;
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
            Execute_Click(null, null);
        }

        private void Tables_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Draw a table name

            // http://stackoverflow.com/questions/3663704/how-to-change-listbox-selection-background-color
            // but need to set ItemHeight to 21?

            if (e.Index < 0) return;
            if (e.Index >= Tables.Items.Count) return;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                // Just set BackColor for selected table name
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          e.ForeColor,
                                          Execute.BackColor);
            }

            e.DrawBackground();
            e.Graphics.DrawString(Tables.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds);
            // (hide focus effect) e.DrawFocusRectangle();
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
                var test = Cnx.ExecuteDataTable("SELECT * FROM " + table_name + " WHERE 1 = 2", new CancellationTokenSource().Token);
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
            if (QueryResult.RowIndex != -1) return;
            Execute_Message(e.RowIndex, Grid.RowCount, null);
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
            Execute_Click(null, null);
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Set Yellow background for null values

            if (e.Value == DBNull.Value)
            {
                e.CellStyle.BackColor = Color.Yellow;
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
            if ((e.Control) && (e.KeyCode == Keys.C)) Grid_Copy();
            if (QueryResult.RowCount <= 1) return;
            if (QueryResult.RowIndex != -1)
            {
                // Check left and right arrows to browse rows
                Browse_RowDetail(e);
                Execute_Message(QueryResult.RowIndex, QueryResult.RowCount, null);
            }
            else
            {
                // Check page down and down arrow to load new page
                Browse_NextPage(e);
                Execute_Message(Grid.CurrentRow.Index, QueryResult.RowCount, null);
            }
        }

        private void Grid_Copy()
        {
            // When Grid contains FORMAT data
            if (Grid.Columns[0].Name == " ")
            {
                // Manual clipboard copy to keep potential tabs
                Clipboard.Clear();
                var raw = new StringBuilder();
                foreach (DataGridViewRow row in Grid.SelectedRows)
                {
                    raw.AppendLine(row.Cells[0].Value.ToString());
                }
                Clipboard.SetText(raw.ToString());
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

        private void Stop_Click(object sender, EventArgs e)
        {
            // [Stop] button has been used
            // => cancel current query

            Cancellation.Cancel();
            Stop.Enable(false);
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
                Display_List(QueryResult.DataTable, false, true);
                QueryResult.GridState.CurrentRow = QueryResult.RowIndex;
                GridSetState(Grid, QueryResult.GridState);
                Grid.ColumnHeadersVisible = true;
                QueryResult.RowIndex = -1;
            }

            Grid.Select();
            EnableUI(true);
        }

        private void Commit_Click(object sender, EventArgs e)
        {
            // [Commit] button has been used
            // => commit current transaction

            Editor.AppendQuery("COMMIT");
            Execute_Click(null, null);
        }

        private void Rollback_Click(object sender, EventArgs e)
        {
            // [Rollback] button has been used
            // => rollback current transaction

            Editor.AppendQuery("ROLLBACK");
            Execute_Click(null, null);
        }

        private void ConnexionName_DoubleClick(object sender, EventArgs e)
        {
            // Connexion name was double clicked
            // => reload table list

            Tables.DataSource = Cnx.GetTables(false);
            if (Filter.Visible) Filter_TextChanged(null, null);
        }

        private void Filter_TextChanged(object sender, EventArgs e)
        {
            // Filter text has changed
            // => update table list

            var filter = Filter.Text.ToLowerInvariant();
            var tables = Cnx.GetTables(true).Where(t => t.ToLowerInvariant().Contains(filter)).ToArray();
            Tables.DataSource = tables;
        }

        private void RunTime_Tick(object sender, EventArgs e)
        {
            if (RunTime.Tag.ToString() == "True")
            {
                // Display duration during SQL execution
                var duration = DateTime.Now.Subtract(StartTime.Value);
                Informations.Text = string.Format("({0}:{1:00}.{2:000})", duration.Minutes, duration.Seconds, duration.Milliseconds);
                Informations.Left = Toolbar.ClientSize.Width - Informations.Width;
            }
            else if (RunTime.Tag.ToString() == "Oracle")
            {
                // Keep Oracle connection alive
                Cnx.ExecuteNonQuery("SELECT SYSDATE FROM DUAL");
            }
        }

        private void RunTime_Enable(bool onoff)
        {
            // Activate or deactivate duration display
            RunTime.Tag = onoff.ToString();
            RunTime.Interval = 100;
            RunTime.Enabled = onoff;

            // Nothing more when duration activated
            if (onoff) return;

            // Activate dummy sql every 5 minutes to refresh Oracle session
            if (Cnx.CnxParameter.IsOracle)
            {
                RunTime.Tag = "Oracle";
                RunTime.Interval = (int)new TimeSpan(0, 5, 0).TotalMilliseconds;
                RunTime.Enabled = true;
            }
        }
    }
}
