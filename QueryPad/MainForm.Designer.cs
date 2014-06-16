namespace QueryPad
{
    partial class MainForm
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
            this.TabConnections.Size = new System.Drawing.Size(801, 428);
            this.TabConnections.TabIndex = 0;
            this.TabConnections.Text = "Connections";
            this.TabConnections.UseVisualStyleBackColor = true;
            this.TabConnections.Enter += new System.EventHandler(this.TabConnections_Enter);
            // 
            // Tabs
            // 
            this.Tabs.Controls.Add(this.TabConnections);
            this.Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabs.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tabs.Location = new System.Drawing.Point(0, 0);
            this.Tabs.Margin = new System.Windows.Forms.Padding(0);
            this.Tabs.Name = "Tabs";
            this.Tabs.Padding = new System.Drawing.Point(0, 0);
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(809, 462);
            this.Tabs.TabIndex = 0;
            this.Tabs.SelectedIndexChanged += new System.EventHandler(this.Tabs_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 462);
            this.Controls.Add(this.Tabs);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Tabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage TabConnections;
        private System.Windows.Forms.TabControl Tabs;

    }
}