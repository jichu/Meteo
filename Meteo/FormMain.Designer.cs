namespace Meteo
{
    partial class FormMain
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
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.souborToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.modelyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemLoadModels = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExplore = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemPalette = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.panelLayout = new System.Windows.Forms.Panel();
            this.backgroundWorkerEnumerationModels = new System.ComponentModel.BackgroundWorker();
            this.menuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.souborToolStripMenuItem,
            this.modelyToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(1008, 24);
            this.menuMain.TabIndex = 2;
            this.menuMain.Text = "menuStrip1";
            // 
            // souborToolStripMenuItem
            // 
            this.souborToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemClose});
            this.souborToolStripMenuItem.Name = "souborToolStripMenuItem";
            this.souborToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.souborToolStripMenuItem.Text = "Soubor";
            // 
            // menuItemClose
            // 
            this.menuItemClose.Name = "menuItemClose";
            this.menuItemClose.Size = new System.Drawing.Size(107, 22);
            this.menuItemClose.Text = "Konec";
            this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // modelyToolStripMenuItem
            // 
            this.modelyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemLoadModels,
            this.menuItemExplore,
            this.menuItemPalette,
            this.menuItemOutput});
            this.modelyToolStripMenuItem.Name = "modelyToolStripMenuItem";
            this.modelyToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.modelyToolStripMenuItem.Text = "Modely";
            // 
            // menuItemLoadModels
            // 
            this.menuItemLoadModels.Name = "menuItemLoadModels";
            this.menuItemLoadModels.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuItemLoadModels.Size = new System.Drawing.Size(277, 22);
            this.menuItemLoadModels.Text = "Načíst modely a ORP masky ...";
            this.menuItemLoadModels.Click += new System.EventHandler(this.menuItemLoadModels_Click);
            // 
            // menuItemExplore
            // 
            this.menuItemExplore.Name = "menuItemExplore";
            this.menuItemExplore.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.menuItemExplore.Size = new System.Drawing.Size(277, 22);
            this.menuItemExplore.Text = "Prohlídnout";
            this.menuItemExplore.Click += new System.EventHandler(this.menuItemExplore_Click);
            // 
            // menuItemPalette
            // 
            this.menuItemPalette.Name = "menuItemPalette";
            this.menuItemPalette.Size = new System.Drawing.Size(277, 22);
            this.menuItemPalette.Text = "Paleta masky";
            this.menuItemPalette.Click += new System.EventHandler(this.menuItemPalette_Click);
            // 
            // menuItemOutput
            // 
            this.menuItemOutput.Name = "menuItemOutput";
            this.menuItemOutput.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuItemOutput.Size = new System.Drawing.Size(277, 22);
            this.menuItemOutput.Text = "Výstup";
            this.menuItemOutput.Click += new System.EventHandler(this.menuItemOutput_Click);
            // 
            // panelLayout
            // 
            this.panelLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLayout.Location = new System.Drawing.Point(13, 37);
            this.panelLayout.Name = "panelLayout";
            this.panelLayout.Size = new System.Drawing.Size(983, 681);
            this.panelLayout.TabIndex = 3;
            // 
            // backgroundWorkerEnumerationModels
            // 
            this.backgroundWorkerEnumerationModels.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerEnumerationModels_DoWork);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.panelLayout);
            this.Controls.Add(this.menuMain);
            this.MainMenuStrip = this.menuMain;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MeteoŠ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyUp);
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem souborToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemClose;
        private System.Windows.Forms.ToolStripMenuItem modelyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemLoadModels;
        private System.Windows.Forms.ToolStripMenuItem menuItemExplore;
        public System.Windows.Forms.Panel panelLayout;
        private System.Windows.Forms.ToolStripMenuItem menuItemPalette;
        private System.Windows.Forms.ToolStripMenuItem menuItemOutput;
        private System.ComponentModel.BackgroundWorker backgroundWorkerEnumerationModels;
    }
}

