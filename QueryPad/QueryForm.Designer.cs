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
            this.ConnexionName = new System.Windows.Forms.Label();
            this.Tables = new System.Windows.Forms.ListBox();
            this.SplitHorizontal = new System.Windows.Forms.SplitContainer();
            this.Run = new System.Windows.Forms.Button();
            this.Execute = new System.Windows.Forms.Button();
            this.Query = new System.Windows.Forms.TextBox();
            this.Grid = new System.Windows.Forms.DataGridView();
            this.Informations = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.SplitVertical)).BeginInit();
            this.SplitVertical.Panel1.SuspendLayout();
            this.SplitVertical.Panel2.SuspendLayout();
            this.SplitVertical.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitHorizontal)).BeginInit();
            this.SplitHorizontal.Panel1.SuspendLayout();
            this.SplitHorizontal.Panel2.SuspendLayout();
            this.SplitHorizontal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).BeginInit();
            this.SuspendLayout();
            // 
            // SplitVertical
            // 
            this.SplitVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitVertical.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SplitVertical.Location = new System.Drawing.Point(0, 0);
            this.SplitVertical.Name = "SplitVertical";
            // 
            // SplitVertical.Panel1
            // 
            this.SplitVertical.Panel1.Controls.Add(this.ConnexionName);
            this.SplitVertical.Panel1.Controls.Add(this.Tables);
            // 
            // SplitVertical.Panel2
            // 
            this.SplitVertical.Panel2.Controls.Add(this.SplitHorizontal);
            this.SplitVertical.Size = new System.Drawing.Size(824, 417);
            this.SplitVertical.SplitterDistance = 250;
            this.SplitVertical.TabIndex = 0;
            // 
            // ConnexionName
            // 
            this.ConnexionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConnexionName.BackColor = System.Drawing.Color.Transparent;
            this.ConnexionName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnexionName.Location = new System.Drawing.Point(5, 10);
            this.ConnexionName.Margin = new System.Windows.Forms.Padding(0);
            this.ConnexionName.Name = "ConnexionName";
            this.ConnexionName.Size = new System.Drawing.Size(245, 23);
            this.ConnexionName.TabIndex = 1;
            this.ConnexionName.Text = "ConnexionName";
            this.ConnexionName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Tables
            // 
            this.Tables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Tables.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Tables.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tables.FormattingEnabled = true;
            this.Tables.IntegralHeight = false;
            this.Tables.ItemHeight = 21;
            this.Tables.Location = new System.Drawing.Point(10, 35);
            this.Tables.Margin = new System.Windows.Forms.Padding(0);
            this.Tables.Name = "Tables";
            this.Tables.Size = new System.Drawing.Size(235, 370);
            this.Tables.TabIndex = 0;
            this.Tables.DoubleClick += new System.EventHandler(this.Tables_DoubleClick);
            // 
            // SplitHorizontal
            // 
            this.SplitHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitHorizontal.Location = new System.Drawing.Point(0, 0);
            this.SplitHorizontal.Name = "SplitHorizontal";
            this.SplitHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitHorizontal.Panel1
            // 
            this.SplitHorizontal.Panel1.Controls.Add(this.Informations);
            this.SplitHorizontal.Panel1.Controls.Add(this.Run);
            this.SplitHorizontal.Panel1.Controls.Add(this.Execute);
            this.SplitHorizontal.Panel1.Controls.Add(this.Query);
            // 
            // SplitHorizontal.Panel2
            // 
            this.SplitHorizontal.Panel2.Controls.Add(this.Grid);
            this.SplitHorizontal.Size = new System.Drawing.Size(570, 417);
            this.SplitHorizontal.SplitterDistance = 200;
            this.SplitHorizontal.TabIndex = 0;
            // 
            // Run
            // 
            this.Run.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Run.BackColor = System.Drawing.Color.LightGray;
            this.Run.FlatAppearance.BorderSize = 0;
            this.Run.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Run.Location = new System.Drawing.Point(100, 175);
            this.Run.Name = "Run";
            this.Run.Size = new System.Drawing.Size(75, 23);
            this.Run.TabIndex = 3;
            this.Run.Text = "Run Script";
            this.Run.UseVisualStyleBackColor = false;
            // 
            // Execute
            // 
            this.Execute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Execute.BackColor = System.Drawing.Color.LightGray;
            this.Execute.FlatAppearance.BorderSize = 0;
            this.Execute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Execute.Location = new System.Drawing.Point(5, 175);
            this.Execute.Name = "Execute";
            this.Execute.Size = new System.Drawing.Size(75, 23);
            this.Execute.TabIndex = 1;
            this.Execute.Text = "&Execute";
            this.Execute.UseVisualStyleBackColor = false;
            this.Execute.Click += new System.EventHandler(this.ExecuteSql);
            // 
            // Query
            // 
            this.Query.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Query.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Query.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Query.Location = new System.Drawing.Point(5, 10);
            this.Query.Margin = new System.Windows.Forms.Padding(0);
            this.Query.Multiline = true;
            this.Query.Name = "Query";
            this.Query.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Query.Size = new System.Drawing.Size(555, 160);
            this.Query.TabIndex = 0;
            this.Query.WordWrap = false;
            // 
            // Grid
            // 
            this.Grid.AllowUserToAddRows = false;
            this.Grid.AllowUserToDeleteRows = false;
            this.Grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.Grid.BackgroundColor = System.Drawing.Color.White;
            this.Grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Grid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.Grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Grid.Location = new System.Drawing.Point(5, 5);
            this.Grid.Name = "Grid";
            this.Grid.ReadOnly = true;
            this.Grid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.Grid.RowHeadersWidth = 20;
            this.Grid.Size = new System.Drawing.Size(555, 196);
            this.Grid.TabIndex = 0;
            // 
            // Informations
            // 
            this.Informations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Informations.BackColor = System.Drawing.Color.Transparent;
            this.Informations.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Informations.ForeColor = System.Drawing.Color.DimGray;
            this.Informations.Location = new System.Drawing.Point(196, 175);
            this.Informations.Name = "Informations";
            this.Informations.Size = new System.Drawing.Size(364, 23);
            this.Informations.TabIndex = 4;
            this.Informations.Text = "Informations";
            this.Informations.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // QueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(824, 417);
            this.ControlBox = false;
            this.Controls.Add(this.SplitVertical);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QueryForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "QueryForm";
            this.SplitVertical.Panel1.ResumeLayout(false);
            this.SplitVertical.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitVertical)).EndInit();
            this.SplitVertical.ResumeLayout(false);
            this.SplitHorizontal.Panel1.ResumeLayout(false);
            this.SplitHorizontal.Panel1.PerformLayout();
            this.SplitHorizontal.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitHorizontal)).EndInit();
            this.SplitHorizontal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer SplitVertical;
        private System.Windows.Forms.SplitContainer SplitHorizontal;
        private System.Windows.Forms.TextBox Query;
        private System.Windows.Forms.ListBox Tables;
        private System.Windows.Forms.DataGridView Grid;
        private System.Windows.Forms.Label ConnexionName;
        private System.Windows.Forms.Button Execute;
        private System.Windows.Forms.Button Run;
        private System.Windows.Forms.Label Informations;

    }
}