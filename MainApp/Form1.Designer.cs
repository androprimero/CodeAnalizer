namespace MainApp
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.TextSolution = new System.Windows.Forms.TextBox();
            this.SolutionButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TextResult = new System.Windows.Forms.TextBox();
            this.ResultButton = new System.Windows.Forms.Button();
            this.SolutionFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ResultFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ButtonAnalize = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Solution File:";
            // 
            // TextSolution
            // 
            this.TextSolution.Location = new System.Drawing.Point(99, 20);
            this.TextSolution.Name = "TextSolution";
            this.TextSolution.Size = new System.Drawing.Size(275, 20);
            this.TextSolution.TabIndex = 1;
            // 
            // SolutionButton
            // 
            this.SolutionButton.Location = new System.Drawing.Point(390, 20);
            this.SolutionButton.Name = "SolutionButton";
            this.SolutionButton.Size = new System.Drawing.Size(75, 23);
            this.SolutionButton.TabIndex = 2;
            this.SolutionButton.Text = "Browse";
            this.SolutionButton.UseVisualStyleBackColor = true;
            this.SolutionButton.Click += new System.EventHandler(this.SolutionButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Result File:";
            // 
            // TextResult
            // 
            this.TextResult.Location = new System.Drawing.Point(99, 60);
            this.TextResult.Name = "TextResult";
            this.TextResult.Size = new System.Drawing.Size(275, 20);
            this.TextResult.TabIndex = 4;
            // 
            // ResultButton
            // 
            this.ResultButton.Location = new System.Drawing.Point(390, 63);
            this.ResultButton.Name = "ResultButton";
            this.ResultButton.Size = new System.Drawing.Size(75, 23);
            this.ResultButton.TabIndex = 5;
            this.ResultButton.Text = "Browse";
            this.ResultButton.UseVisualStyleBackColor = true;
            // 
            // SolutionFileDialog
            // 
            this.SolutionFileDialog.Filter = "\"Solution File .sln\"|*.sln|All Files|*.*";
            // 
            // ButtonAnalize
            // 
            this.ButtonAnalize.Location = new System.Drawing.Point(299, 144);
            this.ButtonAnalize.Name = "ButtonAnalize";
            this.ButtonAnalize.Size = new System.Drawing.Size(75, 23);
            this.ButtonAnalize.TabIndex = 6;
            this.ButtonAnalize.Text = "Analyze";
            this.ButtonAnalize.UseVisualStyleBackColor = true;
            this.ButtonAnalize.Click += new System.EventHandler(this.ButtonAnalize_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Location = new System.Drawing.Point(390, 144);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 7;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 179);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonAnalize);
            this.Controls.Add(this.ResultButton);
            this.Controls.Add(this.TextResult);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SolutionButton);
            this.Controls.Add(this.TextSolution);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextSolution;
        private System.Windows.Forms.Button SolutionButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextResult;
        private System.Windows.Forms.Button ResultButton;
        private System.Windows.Forms.OpenFileDialog SolutionFileDialog;
        private System.Windows.Forms.SaveFileDialog ResultFileDialog;
        private System.Windows.Forms.Button ButtonAnalize;
        private System.Windows.Forms.Button ButtonCancel;
    }
}

