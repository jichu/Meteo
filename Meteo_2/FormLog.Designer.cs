namespace Meteo
{
    partial class FormLog
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
            this.log = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // log
            // 
            this.log.BackColor = System.Drawing.Color.MistyRose;
            this.log.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.log.Location = new System.Drawing.Point(0, 0);
            this.log.Name = "log";
            this.log.ReadOnly = true;
            this.log.Size = new System.Drawing.Size(284, 266);
            this.log.TabIndex = 0;
            this.log.Text = "";
            // 
            // FormLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSalmon;
            this.ClientSize = new System.Drawing.Size(284, 266);
            this.Controls.Add(this.log);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Výpis logu";
            this.Load += new System.EventHandler(this.FormLog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.RichTextBox log;
    }
}