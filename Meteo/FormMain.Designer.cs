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
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBoxModel = new System.Windows.Forms.PictureBox();
            this.pictureBoxORP = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxModel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxORP)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(28, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // pictureBoxModel
            // 
            this.pictureBoxModel.Location = new System.Drawing.Point(48, 60);
            this.pictureBoxModel.Name = "pictureBoxModel";
            this.pictureBoxModel.Size = new System.Drawing.Size(100, 50);
            this.pictureBoxModel.TabIndex = 1;
            this.pictureBoxModel.TabStop = false;
            // 
            // pictureBoxORP
            // 
            this.pictureBoxORP.Location = new System.Drawing.Point(48, 138);
            this.pictureBoxORP.Name = "pictureBoxORP";
            this.pictureBoxORP.Size = new System.Drawing.Size(100, 50);
            this.pictureBoxORP.TabIndex = 2;
            this.pictureBoxORP.TabStop = false;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1065, 513);
            this.Controls.Add(this.pictureBoxORP);
            this.Controls.Add(this.pictureBoxModel);
            this.Controls.Add(this.button1);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MeteoŠ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxModel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxORP)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.PictureBox pictureBoxModel;
        public System.Windows.Forms.PictureBox pictureBoxORP;
    }
}

