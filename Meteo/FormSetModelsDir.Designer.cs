namespace Meteo
{
    partial class FormSetModelsDir
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
            this.comboBoxModels = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBoxModels
            // 
            this.comboBoxModels.FormattingEnabled = true;
            this.comboBoxModels.Location = new System.Drawing.Point(100, 12);
            this.comboBoxModels.Name = "comboBoxModels";
            this.comboBoxModels.Size = new System.Drawing.Size(197, 21);
            this.comboBoxModels.TabIndex = 14;
            this.comboBoxModels.SelectedIndexChanged += new System.EventHandler(this.comboBoxModels_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Vyberte modely:";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(222, 48);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 15;
            this.buttonSave.Text = "Potvrdit";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // FormSetModelsDir
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 83);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.comboBoxModels);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSetModelsDir";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Adresář modelů";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormSetModelsDir_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ComboBox comboBoxModels;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSave;
    }
}