﻿using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
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
            }

            return sql.Trim();
        }

        // Split sql script in command lines
        public static string[] SplitCommands(this string script)
        {
            // Simplify end of lines
            script = script.Replace("\r\n", "\n");

            // Check for script start
            var check = script.Length < 6 ? "" : script.Substring(0, 6).ToUpper();
            if ((check == "BEGIN\n") || (check == "BEGIN "))
            {
                script = "BEGIN\nGO\n" + script.Substring(6);
            }

            // Complete comma + newline with a GO separator
            script = Regex.Replace(script, @";\s*$", ";\nGO", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // Complete double newline with a GO separator
            script = Regex.Replace(script, @"^\s*$", "GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // Split commands on GO separator
            var commands = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // Remove empty commands
            commands = commands.Where(x => x.Trim() != "").Select(x => x.Trim()).ToArray();

            // Micro-optimization
            if (commands[commands.Length - 1].ToUpper() == "END;") commands[commands.Length - 1] = "END;";

            return commands;
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
