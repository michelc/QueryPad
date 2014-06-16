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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.List = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.List)).BeginInit();
            this.SuspendLayout();
            // 
            // List
            // 
            this.List.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.List.BackgroundColor = System.Drawing.Color.White;
            this.List.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.List.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.List.ColumnHeadersHeight = 48;
            this.List.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.List.Dock = System.Windows.Forms.DockStyle.Fill;
            this.List.Location = new System.Drawing.Point(0, 0);
            this.List.Margin = new System.Windows.Forms.Padding(10);
            this.List.MultiSelect = false;
            this.List.Name = "List";
            this.List.ReadOnly = true;
            this.List.RowHeadersVisible = false;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.List.RowsDefaultCellStyle = dataGridViewCellStyle12;
            this.List.RowTemplate.Height = 44;
            this.List.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.List.Size = new System.Drawing.Size(284, 262);
            this.List.TabIndex = 0;
            this.List.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.List_CellDoubleClick);
            // 
            // CnxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.List);
            this.Name = "CnxForm";
            this.Text = "CnxForm";
            this.Load += new System.EventHandler(this.CnxForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.List)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView List;
    }
}