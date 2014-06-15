namespace QueryPad
{
    partial class MdiForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.TabConnections = new System.Windows.Forms.TabPage();
            this.Tabs = new System.Windows.Forms.TabControl();
            this.Tabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabConnections
            // 
            this.TabConnections.Location = new System.Drawing.Point(4, 30);
            this.TabConnections.Name = "TabConnections";
            this.TabConnections.Padding = new System.Windows.Forms.Padding(3);
            this.TabConnections.Size = new System.Drawing.Size(811, 66);
            this.TabConnections.TabIndex = 0;
            this.TabConnections.Text = "Connections";
            this.TabConnections.UseVisualStyleBackColor = true;
            this.TabConnections.Enter += new System.EventHandler(this.ListConnections);
            // 
            // Tabs
            // 
            this.Tabs.Controls.Add(this.TabConnections);
            this.Tabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.Tabs.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tabs.Location = new System.Drawing.Point(0, 0);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(819, 100);
            this.Tabs.TabIndex = 3;
            // 
            // MdiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(819, 487);
            this.Controls.Add(this.Tabs);
            this.IsMdiContainer = true;
            this.Name = "MdiForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QueryPad";
            this.Resize += new System.EventHandler(this.MdiForm_Resize);
            this.Tabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage TabConnections;
        private System.Windows.Forms.TabControl Tabs;
    }
}
