namespace Meteo
{
    partial class FormLoader
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
            this.infoText = new System.Windows.Forms.RichTextBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // infoText
            // 
            this.infoText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoText.BackColor = System.Drawing.Color.LightCyan;
            this.infoText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.infoText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.infoText.ForeColor = System.Drawing.Color.DimGray;
            this.infoText.Location = new System.Drawing.Point(12, 100);
            this.infoText.Name = "infoText";
            this.infoText.ReadOnly = true;
            this.infoText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.infoText.Size = new System.Drawing.Size(777, 260);
            this.infoText.TabIndex = 1;
            this.infoText.Text = "";
            // 
            // labelMessage
            // 
            this.labelMessage.BackColor = System.Drawing.Color.Transparent;
            this.labelMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelMessage.ForeColor = System.Drawing.Color.Black;
            this.labelMessage.Location = new System.Drawing.Point(0, 0);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(801, 97);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "Zpracování...";
            this.labelMessage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.labelMessage.UseWaitCursor = true;
            this.labelMessage.Click += new System.EventHandler(this.labelMessage_Click);
            // 
            // FormLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.LightCyan;
            this.ClientSize = new System.Drawing.Size(801, 360);
            this.Controls.Add(this.infoText);
            this.Controls.Add(this.labelMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLoader";
            this.Opacity = 0.8D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormLoader";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.White;
            this.UseWaitCursor = true;
            this.Load += new System.EventHandler(this.FormLoader_Load);
            this.Shown += new System.EventHandler(this.FormLoader_Shown);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox infoText;
        private System.Windows.Forms.Label labelMessage;
    }
}