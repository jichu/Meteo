namespace Meteo
{
    partial class UserControlModel
    {
        /// <summary> 
        /// Vyžaduje se proměnná návrháře.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Uvolněte všechny používané prostředky.
        /// </summary>
        /// <param name="disposing">hodnota true, když by se měl spravovaný prostředek odstranit; jinak false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kód vygenerovaný pomocí Návrháře komponent

        /// <summary> 
        /// Metoda vyžadovaná pro podporu Návrháře - neupravovat
        /// obsah této metody v editoru kódu.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.treeViewModel = new System.Windows.Forms.TreeView();
            this.pictureBoxMap = new System.Windows.Forms.PictureBox();
            this.checkBoxShowORP = new System.Windows.Forms.CheckBox();
            this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            this.checkBoxShoweRain = new System.Windows.Forms.CheckBox();
            this.checkBoxShowOutput = new System.Windows.Forms.CheckBox();
            this.labelRegionName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMap)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Model";
            // 
            // treeViewModel
            // 
            this.treeViewModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeViewModel.Location = new System.Drawing.Point(3, 28);
            this.treeViewModel.Name = "treeViewModel";
            this.treeViewModel.Size = new System.Drawing.Size(185, 258);
            this.treeViewModel.TabIndex = 3;
            // 
            // pictureBoxMap
            // 
            this.pictureBoxMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxMap.Location = new System.Drawing.Point(194, 89);
            this.pictureBoxMap.Name = "pictureBoxMap";
            this.pictureBoxMap.Size = new System.Drawing.Size(473, 197);
            this.pictureBoxMap.TabIndex = 4;
            this.pictureBoxMap.TabStop = false;
            // 
            // checkBoxShowORP
            // 
            this.checkBoxShowORP.AutoSize = true;
            this.checkBoxShowORP.Enabled = false;
            this.checkBoxShowORP.Location = new System.Drawing.Point(194, 28);
            this.checkBoxShowORP.Name = "checkBoxShowORP";
            this.checkBoxShowORP.Size = new System.Drawing.Size(124, 17);
            this.checkBoxShowORP.TabIndex = 5;
            this.checkBoxShowORP.Text = "Zobrazit ORP masku";
            this.checkBoxShowORP.UseVisualStyleBackColor = true;
            this.checkBoxShowORP.CheckedChanged += new System.EventHandler(this.checkBoxShowORP_CheckedChanged);
            // 
            // richTextBoxOutput
            // 
            this.richTextBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxOutput.Location = new System.Drawing.Point(514, 28);
            this.richTextBoxOutput.Name = "richTextBoxOutput";
            this.richTextBoxOutput.Size = new System.Drawing.Size(153, 258);
            this.richTextBoxOutput.TabIndex = 6;
            this.richTextBoxOutput.Text = "";
            this.richTextBoxOutput.Visible = false;
            // 
            // checkBoxShoweRain
            // 
            this.checkBoxShoweRain.AutoSize = true;
            this.checkBoxShoweRain.Enabled = false;
            this.checkBoxShoweRain.Location = new System.Drawing.Point(194, 51);
            this.checkBoxShoweRain.Name = "checkBoxShoweRain";
            this.checkBoxShoweRain.Size = new System.Drawing.Size(97, 17);
            this.checkBoxShoweRain.TabIndex = 7;
            this.checkBoxShoweRain.Text = "Zobrazit srážky";
            this.checkBoxShoweRain.UseVisualStyleBackColor = true;
            this.checkBoxShoweRain.CheckedChanged += new System.EventHandler(this.checkBoxShoweRain_CheckedChanged);
            // 
            // checkBoxShowOutput
            // 
            this.checkBoxShowOutput.AutoSize = true;
            this.checkBoxShowOutput.Enabled = false;
            this.checkBoxShowOutput.Location = new System.Drawing.Point(324, 28);
            this.checkBoxShowOutput.Name = "checkBoxShowOutput";
            this.checkBoxShowOutput.Size = new System.Drawing.Size(98, 17);
            this.checkBoxShowOutput.TabIndex = 8;
            this.checkBoxShowOutput.Text = "Zobrazit výstup";
            this.checkBoxShowOutput.UseVisualStyleBackColor = true;
            this.checkBoxShowOutput.CheckedChanged += new System.EventHandler(this.checkBoxShowOutput_CheckedChanged);
            // 
            // labelRegionName
            // 
            this.labelRegionName.AutoSize = true;
            this.labelRegionName.Location = new System.Drawing.Point(191, 71);
            this.labelRegionName.Name = "labelRegionName";
            this.labelRegionName.Size = new System.Drawing.Size(0, 13);
            this.labelRegionName.TabIndex = 9;
            // 
            // UserControlModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelRegionName);
            this.Controls.Add(this.checkBoxShowOutput);
            this.Controls.Add(this.checkBoxShoweRain);
            this.Controls.Add(this.richTextBoxOutput);
            this.Controls.Add(this.checkBoxShowORP);
            this.Controls.Add(this.pictureBoxMap);
            this.Controls.Add(this.treeViewModel);
            this.Controls.Add(this.label1);
            this.Name = "UserControlModel";
            this.Size = new System.Drawing.Size(670, 289);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TreeView treeViewModel;
        public System.Windows.Forms.PictureBox pictureBoxMap;
        public System.Windows.Forms.CheckBox checkBoxShowORP;
        public System.Windows.Forms.RichTextBox richTextBoxOutput;
        public System.Windows.Forms.CheckBox checkBoxShoweRain;
        public System.Windows.Forms.CheckBox checkBoxShowOutput;
        private System.Windows.Forms.Label labelRegionName;
    }
}
