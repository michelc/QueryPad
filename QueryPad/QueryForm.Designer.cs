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
            this.components = new System.ComponentModel.Container();
            this.SplitVertical = new System.Windows.Forms.SplitContainer();
            this.Tables = new System.Windows.Forms.ListBox();
            this.Title = new System.Windows.Forms.Panel();
            this.Filter = new System.Windows.Forms.TextBox();
            this.ConnexionName = new System.Windows.Forms.Label();
            this.SplitHorizontal = new System.Windows.Forms.SplitContainer();
            this.Editor = new System.Windows.Forms.RichTextBox();
            this.Toolbar = new System.Windows.Forms.Panel();
            this.Rotate = new System.Windows.Forms.Button();
            this.Rollback = new System.Windows.Forms.Button();
            this.Commit = new System.Windows.Forms.Button();
            this.Informations = new System.Windows.Forms.Label();
            this.Stop = new System.Windows.Forms.Button();
            this.Execute = new System.Windows.Forms.Button();
            this.Grid = new System.Windows.Forms.DataGridView();
            this.RunTime = new System.Windows.Forms.Timer(this.components);
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
            this.SplitVertical.SplitterDistance = 209;
            this.SplitVertical.TabIndex = 0;
            // 
            // Tables
            // 
            this.Tables.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Tables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tables.FormattingEnabled = true;
            this.Tables.IntegralHeight = false;
            this.Tables.Location = new System.Drawing.Point(8, 39);
            this.Tables.Name = "Tables";
            this.Tables.Size = new System.Drawing.Size(199, 352);
            this.Tables.TabIndex = 1;
            this.Tables.DoubleClick += new System.EventHandler(this.Tables_DoubleClick);
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Controls.Add(this.Filter);
            this.Title.Controls.Add(this.ConnexionName);
            this.Title.Dock = System.Windows.Forms.DockStyle.Top;
            this.Title.Location = new System.Drawing.Point(8, 8);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(199, 31);
            this.Title.TabIndex = 0;
            // 
            // Filter
            // 
            this.Filter.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Filter.Location = new System.Drawing.Point(0, 15);
            this.Filter.Name = "Filter";
            this.Filter.Size = new System.Drawing.Size(196, 13);
            this.Filter.TabIndex = 3;
            this.Filter.TabStop = false;
            this.Filter.WordWrap = false;
            this.Filter.TextChanged += new System.EventHandler(this.Filter_TextChanged);
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
            this.ConnexionName.DoubleClick += new System.EventHandler(this.ConnexionName_DoubleClick);
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
            this.SplitHorizontal.Panel1.Controls.Add(this.Editor);
            this.SplitHorizontal.Panel1.Controls.Add(this.Toolbar);
            // 
            // SplitHorizontal.Panel2
            // 
            this.SplitHorizontal.Panel2.Controls.Add(this.Grid);
            this.SplitHorizontal.Panel2.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.SplitHorizontal.Size = new System.Drawing.Size(582, 383);
            this.SplitHorizontal.SplitterDistance = 133;
            this.SplitHorizontal.TabIndex = 0;
            // 
            // Editor
            // 
            this.Editor.AcceptsTab = true;
            this.Editor.BackColor = System.Drawing.Color.White;
            this.Editor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Editor.DetectUrls = false;
            this.Editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Editor.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Editor.HideSelection = false;
            this.Editor.Location = new System.Drawing.Point(0, 0);
            this.Editor.Name = "Editor";
            this.Editor.Size = new System.Drawing.Size(582, 98);
            this.Editor.TabIndex = 0;
            this.Editor.Text = "";
            this.Editor.WordWrap = false;
            this.Editor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyDown);
            // 
            // Toolbar
            // 
            this.Toolbar.Controls.Add(this.Rotate);
            this.Toolbar.Controls.Add(this.Rollback);
            this.Toolbar.Controls.Add(this.Commit);
            this.Toolbar.Controls.Add(this.Informations);
            this.Toolbar.Controls.Add(this.Stop);
            this.Toolbar.Controls.Add(this.Execute);
            this.Toolbar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Toolbar.Location = new System.Drawing.Point(0, 98);
            this.Toolbar.Name = "Toolbar";
            this.Toolbar.Size = new System.Drawing.Size(582, 35);
            this.Toolbar.TabIndex = 1;
            // 
            // Rotate
            // 
            this.Rotate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Rotate.AutoSize = true;
            this.Rotate.BackColor = System.Drawing.Color.LightGray;
            this.Rotate.FlatAppearance.BorderSize = 0;
            this.Rotate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Rotate.Location = new System.Drawing.Point(321, 9);
            this.Rotate.Name = "Rotate";
            this.Rotate.Size = new System.Drawing.Size(65, 23);
            this.Rotate.TabIndex = 10;
            this.Rotate.Text = "R&otate";
            this.Rotate.UseVisualStyleBackColor = false;
            this.Rotate.Visible = false;
            this.Rotate.Click += new System.EventHandler(this.Rotate_Click);
            // 
            // Rollback
            // 
            this.Rollback.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Rollback.AutoSize = true;
            this.Rollback.BackColor = System.Drawing.Color.LightGray;
            this.Rollback.FlatAppearance.BorderSize = 0;
            this.Rollback.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Rollback.Location = new System.Drawing.Point(240, 9);
            this.Rollback.Name = "Rollback";
            this.Rollback.Size = new System.Drawing.Size(65, 23);
            this.Rollback.TabIndex = 9;
            this.Rollback.Text = "Rollbac&k";
            this.Rollback.UseVisualStyleBackColor = false;
            this.Rollback.Visible = false;
            this.Rollback.Click += new System.EventHandler(this.Rollback_Click);
            // 
            // Commit
            // 
            this.Commit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Commit.AutoSize = true;
            this.Commit.BackColor = System.Drawing.Color.LightGray;
            this.Commit.FlatAppearance.BorderSize = 0;
            this.Commit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Commit.Location = new System.Drawing.Point(160, 9);
            this.Commit.Name = "Commit";
            this.Commit.Size = new System.Drawing.Size(65, 23);
            this.Commit.TabIndex = 8;
            this.Commit.Text = "&Commit";
            this.Commit.UseVisualStyleBackColor = false;
            this.Commit.Visible = false;
            this.Commit.Click += new System.EventHandler(this.Commit_Click);
            // 
            // Informations
            // 
            this.Informations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Informations.AutoSize = true;
            this.Informations.BackColor = System.Drawing.Color.Transparent;
            this.Informations.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Informations.ForeColor = System.Drawing.Color.DimGray;
            this.Informations.Location = new System.Drawing.Point(502, 12);
            this.Informations.Name = "Informations";
            this.Informations.Size = new System.Drawing.Size(80, 16);
            this.Informations.TabIndex = 7;
            this.Informations.Text = "Informations";
            this.Informations.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Stop
            // 
            this.Stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Stop.AutoSize = true;
            this.Stop.BackColor = System.Drawing.Color.LightGray;
            this.Stop.FlatAppearance.BorderSize = 0;
            this.Stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Stop.Location = new System.Drawing.Point(80, 9);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(65, 23);
            this.Stop.TabIndex = 6;
            this.Stop.Text = "Sto&p";
            this.Stop.UseVisualStyleBackColor = false;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // Execute
            // 
            this.Execute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Execute.AutoSize = true;
            this.Execute.BackColor = System.Drawing.Color.LightGray;
            this.Execute.FlatAppearance.BorderSize = 0;
            this.Execute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Execute.Location = new System.Drawing.Point(0, 9);
            this.Execute.Name = "Execute";
            this.Execute.Size = new System.Drawing.Size(65, 23);
            this.Execute.TabIndex = 5;
            this.Execute.Text = "&Execute";
            this.Execute.UseVisualStyleBackColor = false;
            this.Execute.Click += new System.EventHandler(this.Execute_Click);
            // 
            // Grid
            // 
            this.Grid.AllowUserToAddRows = false;
            this.Grid.AllowUserToDeleteRows = false;
            this.Grid.AllowUserToOrderColumns = true;
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
            this.Grid.RowHeadersWidth = 23;
            this.Grid.Size = new System.Drawing.Size(582, 244);
            this.Grid.TabIndex = 0;
            this.Grid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Grid_CellClick);
            this.Grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Grid_CellDoubleClick);
            this.Grid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Grid_CellFormatting);
            this.Grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            this.Grid.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Grid_KeyUp);
            // 
            // RunTime
            // 
            this.RunTime.Tick += new System.EventHandler(this.RunTime_Tick);
            // 
            // QueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 399);
            this.Controls.Add(this.SplitVertical);
            this.KeyPreview = true;
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
        private System.Windows.Forms.RichTextBox Editor;
        private System.Windows.Forms.Panel Toolbar;
        private System.Windows.Forms.DataGridView Grid;
        private System.Windows.Forms.ListBox Tables;
        private System.Windows.Forms.Label ConnexionName;
        private System.Windows.Forms.Label Informations;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.Button Execute;
        private System.Windows.Forms.Button Commit;
        private System.Windows.Forms.Button Rollback;
        private System.Windows.Forms.Button Rotate;
        private System.Windows.Forms.TextBox Filter;
        private System.Windows.Forms.Timer RunTime;
    }
}