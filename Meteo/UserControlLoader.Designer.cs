namespace Meteo
{
    partial class UserControlLoader
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
            this.infoText = new System.Windows.Forms.RichTextBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // infoText
            // 
            this.infoText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoText.BackColor = System.Drawing.Color.WhiteSmoke;
            this.infoText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.infoText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.infoText.ForeColor = System.Drawing.Color.Navy;
            this.infoText.Location = new System.Drawing.Point(7, 154);
            this.infoText.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.infoText.Name = "infoText";
            this.infoText.ReadOnly = true;
            this.infoText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.infoText.Size = new System.Drawing.Size(1440, 640);
            this.infoText.TabIndex = 3;
            this.infoText.Text = "";
            this.infoText.UseWaitCursor = true;
            // 
            // labelMessage
            // 
            this.labelMessage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.labelMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelMessage.ForeColor = System.Drawing.Color.Black;
            this.labelMessage.Location = new System.Drawing.Point(0, 0);
            this.labelMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(1451, 149);
            this.labelMessage.TabIndex = 2;
            this.labelMessage.Text = "Zpracování...";
            this.labelMessage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.labelMessage.UseWaitCursor = true;
            // 
            // UserControlLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.infoText);
            this.Controls.Add(this.labelMessage);
            this.Name = "UserControlLoader";
            this.Size = new System.Drawing.Size(1451, 799);
            this.Load += new System.EventHandler(this.UserControlLoader_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox infoText;
        private System.Windows.Forms.Label labelMessage;
    }
}
