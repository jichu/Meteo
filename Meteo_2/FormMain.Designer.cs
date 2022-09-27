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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.souborToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.modelyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExplore = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemLoadModels = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemLoadInputs = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemAlgorithm = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemForecast = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemPalette = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemStatForecasting = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDoAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemWind = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSettingWind = new System.Windows.Forms.ToolStripMenuItem();
            this.panelLayout = new System.Windows.Forms.Panel();
            this.backgroundWorkerEnumerationModels = new System.ComponentModel.BackgroundWorker();
            this.menuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMain
            // 
            this.menuMain.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.souborToolStripMenuItem,
            this.modelyToolStripMenuItem,
            this.menuItemWind});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(2279, 33);
            this.menuMain.TabIndex = 2;
            this.menuMain.Text = "menuStrip1";
            // 
            // souborToolStripMenuItem
            // 
            this.souborToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemClose});
            this.souborToolStripMenuItem.Name = "souborToolStripMenuItem";
            this.souborToolStripMenuItem.Size = new System.Drawing.Size(87, 29);
            this.souborToolStripMenuItem.Text = "Soubor";
            // 
            // menuItemClose
            // 
            this.menuItemClose.Name = "menuItemClose";
            this.menuItemClose.Size = new System.Drawing.Size(162, 34);
            this.menuItemClose.Text = "Konec";
            this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // modelyToolStripMenuItem
            // 
            this.modelyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemExplore,
            this.menuItemLoadModels,
            this.menuItemLoadInputs,
            this.MenuItemAlgorithm,
            this.menuItemOutput,
            this.menuItemForecast,
            this.menuItemPalette,
            this.menuItemStatForecasting,
            this.menuDoAllToolStripMenuItem});
            this.modelyToolStripMenuItem.Name = "modelyToolStripMenuItem";
            this.modelyToolStripMenuItem.Size = new System.Drawing.Size(88, 29);
            this.modelyToolStripMenuItem.Text = "Modely";
            // 
            // menuItemExplore
            // 
            this.menuItemExplore.Name = "menuItemExplore";
            this.menuItemExplore.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.menuItemExplore.Size = new System.Drawing.Size(514, 34);
            this.menuItemExplore.Text = "Prohlídnout modely";
            this.menuItemExplore.Visible = false;
            this.menuItemExplore.Click += new System.EventHandler(this.menuItemExplore_Click);
            // 
            // menuItemLoadModels
            // 
            this.menuItemLoadModels.Name = "menuItemLoadModels";
            this.menuItemLoadModels.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuItemLoadModels.Size = new System.Drawing.Size(514, 34);
            this.menuItemLoadModels.Text = "Načíst modely a ORP masky ...";
            this.menuItemLoadModels.Click += new System.EventHandler(this.menuItemLoadModels_Click);
            // 
            // menuItemLoadInputs
            // 
            this.menuItemLoadInputs.Name = "menuItemLoadInputs";
            this.menuItemLoadInputs.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.menuItemLoadInputs.Size = new System.Drawing.Size(514, 34);
            this.menuItemLoadInputs.Text = "Načíst vstupy";
            this.menuItemLoadInputs.Click += new System.EventHandler(this.menuItemLoadInputs_Click);
            // 
            // MenuItemAlgorithm
            // 
            this.MenuItemAlgorithm.Name = "MenuItemAlgorithm";
            this.MenuItemAlgorithm.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
            this.MenuItemAlgorithm.Size = new System.Drawing.Size(514, 34);
            this.MenuItemAlgorithm.Text = "Spustit algoritmus předpovědi bouří";
            this.MenuItemAlgorithm.Visible = false;
            this.MenuItemAlgorithm.Click += new System.EventHandler(this.MenuItemAlgorithm_Click);
            // 
            // menuItemOutput
            // 
            this.menuItemOutput.Name = "menuItemOutput";
            this.menuItemOutput.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuItemOutput.Size = new System.Drawing.Size(514, 34);
            this.menuItemOutput.Text = "Zobrazit výstup";
            this.menuItemOutput.Visible = false;
            this.menuItemOutput.Click += new System.EventHandler(this.menuItemOutput_Click);
            // 
            // menuItemForecast
            // 
            this.menuItemForecast.Name = "menuItemForecast";
            this.menuItemForecast.Size = new System.Drawing.Size(514, 34);
            this.menuItemForecast.Text = "Obecné informace z webu";
            this.menuItemForecast.Visible = false;
            this.menuItemForecast.Click += new System.EventHandler(this.menuItemForecast_Click);
            // 
            // menuItemPalette
            // 
            this.menuItemPalette.Name = "menuItemPalette";
            this.menuItemPalette.Size = new System.Drawing.Size(514, 34);
            this.menuItemPalette.Text = "Paleta masky";
            this.menuItemPalette.Visible = false;
            this.menuItemPalette.Click += new System.EventHandler(this.menuItemPalette_Click);
            // 
            // menuItemStatForecasting
            // 
            this.menuItemStatForecasting.Name = "menuItemStatForecasting";
            this.menuItemStatForecasting.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuItemStatForecasting.Size = new System.Drawing.Size(514, 34);
            this.menuItemStatForecasting.Text = "Statistická předpověď konvektivních srážek";
            this.menuItemStatForecasting.Click += new System.EventHandler(this.menuItemStatForecasting_Click);
            // 
            // menuDoAllToolStripMenuItem
            // 
            this.menuDoAllToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.menuDoAllToolStripMenuItem.Name = "menuDoAllToolStripMenuItem";
            this.menuDoAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.menuDoAllToolStripMenuItem.Size = new System.Drawing.Size(514, 34);
            this.menuDoAllToolStripMenuItem.Text = "Proveď CELÝ výpočet předpovědí ...";
            this.menuDoAllToolStripMenuItem.ToolTipText = "Načíst modely a ORP masky, načíst vstupy, algoritmus předpovědí";
            this.menuDoAllToolStripMenuItem.Click += new System.EventHandler(this.menuDoAllToolStripMenuItem_Click);
            // 
            // menuItemWind
            // 
            this.menuItemWind.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSettingWind});
            this.menuItemWind.Name = "menuItemWind";
            this.menuItemWind.Size = new System.Drawing.Size(105, 29);
            this.menuItemWind.Text = "Nastavení";
            // 
            // menuSettingWind
            // 
            this.menuSettingWind.Name = "menuSettingWind";
            this.menuSettingWind.Size = new System.Drawing.Size(391, 34);
            this.menuSettingWind.Text = "Zahrnout do výpočtů i Směry větru";
            this.menuSettingWind.Click += new System.EventHandler(this.menuSettingWind_Click);
            // 
            // panelLayout
            // 
            this.panelLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLayout.Location = new System.Drawing.Point(18, 42);
            this.panelLayout.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelLayout.Name = "panelLayout";
            this.panelLayout.Size = new System.Drawing.Size(2241, 2116);
            this.panelLayout.TabIndex = 3;
            // 
            // backgroundWorkerEnumerationModels
            // 
            this.backgroundWorkerEnumerationModels.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerEnumerationModels_DoWork);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(2279, 2130);
            this.Controls.Add(this.panelLayout);
            this.Controls.Add(this.menuMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMain;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FormMain";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Aplikace předpovědi meteorologických modelových výstupů";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FormMain_MouseClick);
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
        private System.Windows.Forms.ToolStripMenuItem menuItemLoadInputs;
        private System.Windows.Forms.ToolStripMenuItem MenuItemAlgorithm;
        private System.Windows.Forms.ToolStripMenuItem menuItemForecast;
        private System.Windows.Forms.ToolStripMenuItem menuDoAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemWind;
        internal System.Windows.Forms.ToolStripMenuItem menuSettingWind;
        private System.Windows.Forms.ToolStripMenuItem menuItemStatForecasting;
    }
}

