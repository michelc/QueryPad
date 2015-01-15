using System.Drawing;
using System.Windows.Forms;

namespace QueryPad
{
    public static class EditorHelpers
    {
        // Add query to the editor area
        public static void AppendQuery(this RichTextBox editor, string sql)
        {
            // Append query at the end
            var start = editor.Text.Length;
            if (editor.Text.Length > 0)
            {
                sql = "\n\n" + sql;
                start += 2;
            }
            editor.AppendText(sql);

            // Auto-select new query
            editor.SelectionStart = start;
            editor.SelectionLength = sql.Length;
        }

        // Get query to execute
        public static string CurrentQuery(this RichTextBox editor)
        {
            // Empty editor !
            if (editor.Text.Trim() == "") return "";

            // By default, use selected text
            var sql = editor.SelectedText;

            // Or, auto-select text according to cursor position
            if (sql.Length == 0)
            {
                var nlnl = "\n\n";  // newline + newline
                var scnl = ";\n";   // semicolon + newline
                var text = editor.Text.Replace(scnl, nlnl);
                // Find query start
                var start = editor.GetFirstCharIndexOfCurrentLine();
                var line = editor.GetLineFromCharIndex(start);
                while (editor.Lines[line].Trim() == "")
                {
                    if (line == 0) break;
                    line--;
                }
                start = editor.GetFirstCharIndexFromLine(line);
                start = text.LastIndexOf(nlnl, start);
                start = (start == -1) ? 0 : start + nlnl.Length;
                // Find query end
                var end = text.IndexOf(nlnl, start);
                if (end == -1) end = text.Length;
                if ((end < text.Length) && (editor.Text[end] == ';')) end++;
                // Select text
                editor.SelectionStart = start;
                editor.SelectionLength = end - start;
                // Get query to execute
                sql = editor.SelectedText;
            }

            return sql.Trim(" \t\n\r;".ToCharArray());
        }

        // Set tabs every 4 characters
        public static void ConfigureTabs(this RichTextBox editor)
        {
            // Get outer size for 4 characters
            var size1 = TextRenderer.MeasureText("1234", editor.Font, editor.Size, TextFormatFlags.NoClipping);
            double width1 = size1.Width;

            // Get outer size for 8 characters
            var size2 = TextRenderer.MeasureText("12345678", editor.Font, editor.Size, TextFormatFlags.NoClipping);
            double width2 = size2.Width;

            // Get inner size for 4 characters
            double width = width2 - width1;

            // Define all tabs
            var tabs = new int[32];
            for (int i = 1; i < tabs.Length; i++)
            {
                tabs[i - 1] = (int)(width * i);
            }
            editor.SelectionTabs = tabs;
        }

        public static void Enable(this Button button, bool onoff)
        {
            // Enable or disable button
            button.Enabled = onoff;

            // Update button color
            if (onoff)
            {
                button.BackColor = Color.FromKnownColor(KnownColor.Highlight);
                button.ForeColor = Color.FromKnownColor(KnownColor.HighlightText);
            }
            else
            {
                button.BackColor = Color.LightGray;
                button.ForeColor = Color.WhiteSmoke;
            }
        }
    }
}
