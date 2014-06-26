﻿using System.Windows.Forms;

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
            // By default, use selected text
            var sql = editor.SelectedText;

            // Or, auto-select text according to cursor position
            if (sql.Length == 0)
            {
                var nl = "\n\n";
                // Find query start
                var start = editor.SelectionStart;
                if (start > 0) start--;
                start = editor.Text.LastIndexOf(nl, start);
                start = (start == -1) ? 0 : start + nl.Length;
                // Find query end
                var end = editor.Text.IndexOf(nl, start);
                if (end == -1) end = editor.Text.Length;
                // Select text
                editor.SelectionStart = start;
                editor.SelectionLength = end - start;
                // Get query to execute
                sql = editor.SelectedText;
            }

            return sql.Trim();
        }

        // Remove all formatting (in case of copy/paste)
        public static void RemoveFormatting(this RichTextBox editor)
        {
            var start = editor.SelectionStart;
            var length = editor.SelectionLength;
            var rtf = editor.Rtf;
            var text = editor.Text;
            editor.Rtf = "";
            editor.Text = text;
            editor.SelectionStart = start;
            editor.SelectionLength = length;

            if (editor.Rtf != rtf) editor.ScrollToCaret();
        }
    }
}
