namespace Meteo
{
    partial class FormSetOptions
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
            this.labelModel = new System.Windows.Forms.Label();
            this.radioButtonSum = new System.Windows.Forms.RadioButton();
            this.radioButtonAvarage = new System.Windows.Forms.RadioButton();
            this.groupBoxCountMethod = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.buttonSave = new System.Windows.Forms.Button();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.groupBoxCountMethod.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelModel
            // 
            this.labelModel.AutoSize = true;
            this.labelModel.Location = new System.Drawing.Point(12, 9);
            this.labelModel.Name = "labelModel";
            this.labelModel.Size = new System.Drawing.Size(105, 13);
            this.labelModel.TabIndex = 1;
            this.labelModel.Text = "{model} : {submodel}";
            // 
            // radioButtonSum
            // 
            this.radioButtonSum.AutoSize = true;
            this.radioButtonSum.Location = new System.Drawing.Point(21, 24);
            this.radioButtonSum.Name = "radioButtonSum";
            this.radioButtonSum.Size = new System.Drawing.Size(52, 17);
            this.radioButtonSum.TabIndex = 3;
            this.radioButtonSum.TabStop = true;
            this.radioButtonSum.Tag = "sum";
            this.radioButtonSum.Text = "Suma";
            this.radioButtonSum.UseVisualStyleBackColor = true;
            // 
            // radioButtonAvarage
            // 
            this.radioButtonAvarage.AutoSize = true;
            this.radioButtonAvarage.Location = new System.Drawing.Point(20, 47);
            this.radioButtonAvarage.Name = "radioButtonAvarage";
            this.radioButtonAvarage.Size = new System.Drawing.Size(58, 17);
            this.radioButtonAvarage.TabIndex = 4;
            this.radioButtonAvarage.TabStop = true;
            this.radioButtonAvarage.Tag = "average";
            this.radioButtonAvarage.Text = "Průměr";
            this.radioButtonAvarage.UseVisualStyleBackColor = true;
            // 
            // groupBoxCountMethod
            // 
            this.groupBoxCountMethod.Controls.Add(this.radioButton4);
            this.groupBoxCountMethod.Controls.Add(this.radioButton1);
            this.groupBoxCountMethod.Controls.Add(this.radioButtonSum);
            this.groupBoxCountMethod.Controls.Add(this.radioButtonAvarage);
            this.groupBoxCountMethod.Location = new System.Drawing.Point(12, 38);
            this.groupBoxCountMethod.Name = "groupBoxCountMethod";
            this.groupBoxCountMethod.Size = new System.Drawing.Size(382, 81);
            this.groupBoxCountMethod.TabIndex = 5;
            this.groupBoxCountMethod.TabStop = false;
            this.groupBoxCountMethod.Text = "Specifikujte metodu [countMethod]:";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(121, 24);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(62, 17);
            this.radioButton1.TabIndex = 5;
            this.radioButton1.TabStop = true;
            this.radioButton1.Tag = "majority";
            this.radioButton1.Text = "Majorita";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(15, 125);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "Uložit";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(121, 47);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(84, 17);
            this.radioButton4.TabIndex = 6;
            this.radioButton4.TabStop = true;
            this.radioButton4.Tag = "average_raw";
            this.radioButton4.Text = "Průměr (raw)";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // FormSetOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 160);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.groupBoxCountMethod);
            this.Controls.Add(this.labelModel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSetOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormSetOptions";
            this.Load += new System.EventHandler(this.FormSetOptions_Load);
            this.groupBoxCountMethod.ResumeLayout(false);
            this.groupBoxCountMethod.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label labelModel;
        private System.Windows.Forms.RadioButton radioButtonSum;
        private System.Windows.Forms.RadioButton radioButtonAvarage;
        private System.Windows.Forms.GroupBox groupBoxCountMethod;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton4;
    }
}