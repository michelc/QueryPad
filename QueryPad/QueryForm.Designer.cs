namespace QueryPad
{
    partial class QueryForm
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
            this.SplitVertical = new System.Windows.Forms.SplitContainer();
            this.Tables = new System.Windows.Forms.ListBox();
            this.Title = new System.Windows.Forms.Panel();
            this.ConnexionName = new System.Windows.Forms.Label();
            this.SplitHorizontal = new System.Windows.Forms.SplitContainer();
            this.Query = new System.Windows.Forms.TextBox();
            this.Toolbar = new System.Windows.Forms.Panel();
            this.Informations = new System.Windows.Forms.Label();
            this.Run = new System.Windows.Forms.Button();
            this.Execute = new System.Windows.Forms.Button();
            this.Grid = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.SplitVertical)).BeginInit();
            this.SplitVertical.Panel1.SuspendLayout();
            this.SplitVertical.Panel2.SuspendLayout();
            this.SplitVertical.SuspendLayout();
            this.Title.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitHorizontal)).BeginInit();
            this.SplitHorizontal.Panel1.SuspendLayout();
            this.SplitHorizontal.Panel2.SuspendLayout();
            this.SplitHorizontal.SuspendLayout();
            this.Toolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).BeginInit();
            this.SuspendLayout();
            // 
            // SplitVertical
            // 
            this.SplitVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitVertical.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SplitVertical.Location = new System.Drawing.Point(0, 0);
            this.SplitVertical.Margin = new System.Windows.Forms.Padding(0);
            this.SplitVertical.Name = "SplitVertical";
            // 
            // SplitVertical.Panel1
            // 
            this.SplitVertical.Panel1.Controls.Add(this.Tables);
            this.SplitVertical.Panel1.Controls.Add(this.Title);
            this.SplitVertical.Panel1.Padding = new System.Windows.Forms.Padding(8, 8, 2, 8);
            // 
            // SplitVertical.Panel2
            // 
            this.SplitVertical.Panel2.Controls.Add(this.SplitHorizontal);
            this.SplitVertical.Panel2.Padding = new System.Windows.Forms.Padding(2, 8, 8, 8);
            this.SplitVertical.Size = new System.Drawing.Size(805, 399);
            this.SplitVertical.SplitterDistance = 268;
            this.SplitVertical.TabIndex = 0;
            // 
            // Tables
            // 
            this.Tables.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Tables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tables.FormattingEnabled = true;
            this.Tables.IntegralHeight = false;
            this.Tables.Location = new System.Drawing.Point(8, 23);
            this.Tables.Name = "Tables";
            this.Tables.Size = new System.Drawing.Size(258, 368);
            this.Tables.TabIndex = 1;
            this.Tables.DoubleClick += new System.EventHandler(this.Tables_DoubleClick);
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Controls.Add(this.ConnexionName);
            this.Title.Dock = System.Windows.Forms.DockStyle.Top;
            this.Title.Location = new System.Drawing.Point(8, 8);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(258, 15);
            this.Title.TabIndex = 0;
            // 
            // ConnexionName
            // 
            this.ConnexionName.AutoSize = true;
            this.ConnexionName.BackColor = System.Drawing.Color.Transparent;
            this.ConnexionName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnexionName.Location = new System.Drawing.Point(0, 0);
            this.ConnexionName.Margin = new System.Windows.Forms.Padding(0);
            this.ConnexionName.Name = "ConnexionName";
            this.ConnexionName.Size = new System.Drawing.Size(96, 15);
            this.ConnexionName.TabIndex = 2;
            this.ConnexionName.Text = "ConnexionName";
            this.ConnexionName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SplitHorizontal
            // 
            this.SplitHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitHorizontal.Location = new System.Drawing.Point(2, 8);
            this.SplitHorizontal.Margin = new System.Windows.Forms.Padding(0);
            this.SplitHorizontal.Name = "SplitHorizontal";
            this.SplitHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitHorizontal.Panel1
            // 
            this.SplitHorizontal.Panel1.Controls.Add(this.Query);
            this.SplitHorizontal.Panel1.Controls.Add(this.Toolbar);
            // 
            // SplitHorizontal.Panel2
            // 
            this.SplitHorizontal.Panel2.Controls.Add(this.Grid);
            this.SplitHorizontal.Panel2.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.SplitHorizontal.Size = new System.Drawing.Size(523, 383);
            this.SplitHorizontal.SplitterDistance = 133;
            this.SplitHorizontal.TabIndex = 0;
            // 
            // Query
            // 
            this.Query.BackColor = System.Drawing.Color.White;
            this.Query.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Query.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Query.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Query.Location = new System.Drawing.Point(0, 0);
            this.Query.Multiline = true;
            this.Query.Name = "Query";
            this.Query.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Query.Size = new System.Drawing.Size(523, 98);
            this.Query.TabIndex = 0;
            // 
            // Toolbar
            // 
            this.Toolbar.Controls.Add(this.Informations);
            this.Toolbar.Controls.Add(this.Run);
            this.Toolbar.Controls.Add(this.Execute);
            this.Toolbar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Toolbar.Location = new System.Drawing.Point(0, 98);
            this.Toolbar.Name = "Toolbar";
            this.Toolbar.Size = new System.Drawing.Size(523, 35);
            this.Toolbar.TabIndex = 1;
            // 
            // Informations
            // 
            this.Informations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Informations.AutoSize = true;
            this.Informations.BackColor = System.Drawing.Color.Transparent;
            this.Informations.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Informations.ForeColor = System.Drawing.Color.DimGray;
            this.Informations.Location = new System.Drawing.Point(443, 12);
            this.Informations.Name = "Informations";
            this.Informations.Size = new System.Drawing.Size(80, 16);
            this.Informations.TabIndex = 7;
            this.Informations.Text = "Informations";
            this.Informations.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Run
            // 
            this.Run.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Run.BackColor = System.Drawing.Color.LightGray;
            this.Run.FlatAppearance.BorderSize = 0;
            this.Run.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Run.Location = new System.Drawing.Point(100, 9);
            this.Run.Name = "Run";
            this.Run.Size = new System.Drawing.Size(75, 23);
            this.Run.TabIndex = 6;
            this.Run.Text = "Run Script";
            this.Run.UseVisualStyleBackColor = false;
            // 
            // Execute
            // 
            this.Execute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Execute.BackColor = System.Drawing.Color.LightGray;
            this.Execute.FlatAppearance.BorderSize = 0;
            this.Execute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Execute.Location = new System.Drawing.Point(0, 9);
            this.Execute.Name = "Execute";
            this.Execute.Size = new System.Drawing.Size(75, 23);
            this.Execute.TabIndex = 5;
            this.Execute.Text = "&Execute";
            this.Execute.UseVisualStyleBackColor = false;
            this.Execute.Click += new System.EventHandler(this.ExecuteSql);
            // 
            // Grid
            // 
            this.Grid.AllowUserToAddRows = false;
            this.Grid.AllowUserToDeleteRows = false;
            this.Grid.BackgroundColor = System.Drawing.Color.White;
            this.Grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Grid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.Grid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.Grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grid.Location = new System.Drawing.Point(0, 2);
            this.Grid.Name = "Grid";
            this.Grid.ReadOnly = true;
            this.Grid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.Grid.RowHeadersWidth = 20;
            this.Grid.Size = new System.Drawing.Size(523, 244);
            this.Grid.TabIndex = 0;
            // 
            // QueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 399);
            this.Controls.Add(this.SplitVertical);
            this.Name = "QueryForm";
            this.Text = "QueryForm";
            this.SplitVertical.Panel1.ResumeLayout(false);
            this.SplitVertical.Panel1.PerformLayout();
            this.SplitVertical.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitVertical)).EndInit();
            this.SplitVertical.ResumeLayout(false);
            this.Title.ResumeLayout(false);
            this.Title.PerformLayout();
            this.SplitHorizontal.Panel1.ResumeLayout(false);
            this.SplitHorizontal.Panel1.PerformLayout();
            this.SplitHorizontal.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitHorizontal)).EndInit();
            this.SplitHorizontal.ResumeLayout(false);
            this.Toolbar.ResumeLayout(false);
            this.Toolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer SplitVertical;
        private System.Windows.Forms.Panel Title;
        private System.Windows.Forms.SplitContainer SplitHorizontal;
        private System.Windows.Forms.TextBox Query;
        private System.Windows.Forms.Panel Toolbar;
        private System.Windows.Forms.DataGridView Grid;
        private System.Windows.Forms.ListBox Tables;
        private System.Windows.Forms.Label ConnexionName;
        private System.Windows.Forms.Label Informations;
        private System.Windows.Forms.Button Run;
        private System.Windows.Forms.Button Execute;
    }
}