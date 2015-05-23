namespace QueryPad
{
    partial class CnxForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.List = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.List)).BeginInit();
            this.SuspendLayout();
            // 
            // List
            // 
            this.List.AllowUserToAddRows = false;
            this.List.AllowUserToDeleteRows = false;
            this.List.AllowUserToOrderColumns = true;
            this.List.AllowUserToResizeColumns = false;
            this.List.AllowUserToResizeRows = false;
            this.List.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.List.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.List.BackgroundColor = System.Drawing.SystemColors.Control;
            this.List.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.List.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.List.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.List.ColumnHeadersHeight = 48;
            this.List.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.List.EnableHeadersVisualStyles = false;
            this.List.Location = new System.Drawing.Point(0, 0);
            this.List.Margin = new System.Windows.Forms.Padding(0);
            this.List.MultiSelect = false;
            this.List.Name = "List";
            this.List.ReadOnly = true;
            this.List.RowHeadersVisible = false;
            this.List.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.List.RowTemplate.Height = 44;
            this.List.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.List.Size = new System.Drawing.Size(284, 262);
            this.List.StandardTab = true;
            this.List.TabIndex = 0;
            this.List.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.List_CellDoubleClick);
            this.List.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.List_CellFormatting);
            this.List.KeyDown += new System.Windows.Forms.KeyEventHandler(this.List_KeyDown);
            this.List.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.List_KeyPress);
            // 
            // CnxForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.List);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CnxForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "CnxForm";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.CnxForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.CnxForm_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.List)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView List;
    }
}