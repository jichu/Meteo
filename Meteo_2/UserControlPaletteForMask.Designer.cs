namespace Meteo
{
    partial class UserControlPaletteForMask
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
            this.palette = new System.Windows.Forms.PictureBox();
            this.buttonExport = new System.Windows.Forms.Button();
            this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.palette)).BeginInit();
            this.SuspendLayout();
            // 
            // palette
            // 
            this.palette.Location = new System.Drawing.Point(147, 33);
            this.palette.Name = "palette";
            this.palette.Size = new System.Drawing.Size(100, 50);
            this.palette.TabIndex = 0;
            this.palette.TabStop = false;
            // 
            // buttonExport
            // 
            this.buttonExport.Location = new System.Drawing.Point(147, 4);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(99, 23);
            this.buttonExport.TabIndex = 1;
            this.buttonExport.Text = "Export do BMP";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // richTextBoxOutput
            // 
            this.richTextBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxOutput.Location = new System.Drawing.Point(4, 33);
            this.richTextBoxOutput.Name = "richTextBoxOutput";
            this.richTextBoxOutput.Size = new System.Drawing.Size(122, 279);
            this.richTextBoxOutput.TabIndex = 2;
            this.richTextBoxOutput.Text = "";
            // 
            // UserControlPaletteForMask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.richTextBoxOutput);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.palette);
            this.Name = "UserControlPaletteForMask";
            this.Size = new System.Drawing.Size(563, 312);
            ((System.ComponentModel.ISupportInitialize)(this.palette)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox palette;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.RichTextBox richTextBoxOutput;
    }
}
