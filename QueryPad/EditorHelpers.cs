using System.Drawing;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace QueryPad
{
    public static class EditorHelpers
    {
        // Add query to the editor area
        public static void AppendQuery(this FastColoredTextBox editor, string sql)
        {
            // Append query at the end
            var start = editor.Text.Length;
            if (editor.Text.Length > 0)
            {
                sql = "\r\n\r\n" + sql;
                start += 4;
            }
            editor.AppendText(sql);

            // Auto-select new query
            editor.SelectionStart = start;
            editor.SelectionLength = sql.Length;

            // Show new end
            editor.GoEnd();
        }

        // Get query to execute
        public static string CurrentQuery(this FastColoredTextBox editor)
        {
            // Empty editor !
            if (editor.Text.Trim() == "") return "";

            // By default, use selected text
            var sql = editor.SelectedText;

            // Or, auto-select text according to cursor position
            if (sql.Length == 0)
            {
                // Cherche la 1° ligne de l'instruction en cours
                var first = editor.Selection.Start.iLine;
                
                // Si on est sur une ligne vide, on remonte jusqu'à l'instruction précédente
                while (editor.Lines[first].Trim() == "")
                {
                    if (first == 0) break;
                    first--;
                }

                // On remonte maintenant jusqu'au début de l'instruction
                while (first > 0)
                {
                    var l = editor.Lines[first - 1].Trim();
                    if (l == "") break;
                    if (l.EndsWith(";")) break;
                    first--;
                }

                // On cherche maintenant la fin de l'instruction
                var last = first;
                while (last < editor.LinesCount - 1)
                {
                    var l = editor.Lines[last + 1].Trim();
                    if (l == "") break;
                    if (editor.Lines[last].Trim().EndsWith(";")) break;
                    last++;
                }
                editor.Selection = new Range(editor, 0, first, editor.GetLineLength(last), last);
                // Get query to execute
                sql = editor.SelectedText;
                sql = sql.Trim();
            }

            return sql.Trim(" \t\n\r".ToCharArray());
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
                if (button.Tag != null) button.BackColor = (Color)button.Tag;
            }
            else
            {
                button.BackColor = Color.LightGray;
                button.ForeColor = Color.WhiteSmoke;
            }
        }
    }
}
