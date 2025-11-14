namespace Ejercicio1
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
            this.btnMostrar = new System.Windows.Forms.Button();
            this.lstProcesos = new System.Windows.Forms.ListBox();
            this.txtFiltro = new System.Windows.Forms.TextBox();
            this.btnDetener = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMostrar
            // 
            this.btnMostrar.Location = new System.Drawing.Point(22, 24);
            this.btnMostrar.Name = "btnMostrar";
            this.btnMostrar.Size = new System.Drawing.Size(75, 23);
            this.btnMostrar.TabIndex = 0;
            this.btnMostrar.Text = "Mostrar";
            this.btnMostrar.UseVisualStyleBackColor = true;
            this.btnMostrar.Click += new System.EventHandler(this.BtnMostrar_Click);
            // 
            // lstProcesos
            // 
            this.lstProcesos.FormattingEnabled = true;
            this.lstProcesos.ItemHeight = 16;
            this.lstProcesos.Location = new System.Drawing.Point(22, 79);
            this.lstProcesos.Name = "lstProcesos";
            this.lstProcesos.Size = new System.Drawing.Size(609, 324);
            this.lstProcesos.TabIndex = 1;
            // 
            // txtFiltro
            // 
            this.txtFiltro.Location = new System.Drawing.Point(119, 24);
            this.txtFiltro.Name = "txtFiltro";
            this.txtFiltro.Size = new System.Drawing.Size(457, 22);
            this.txtFiltro.TabIndex = 2;
            // 
            // btnDetener
            // 
            this.btnDetener.Location = new System.Drawing.Point(22, 415);
            this.btnDetener.Name = "btnDetener";
            this.btnDetener.Size = new System.Drawing.Size(75, 23);
            this.btnDetener.TabIndex = 3;
            this.btnDetener.Text = "Detener";
            this.btnDetener.UseVisualStyleBackColor = true;
            this.btnDetener.Click += new System.EventHandler(this.BtnDetener_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnDetener);
            this.Controls.Add(this.txtFiltro);
            this.Controls.Add(this.lstProcesos);
            this.Controls.Add(this.btnMostrar);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnMostrar;
        private System.Windows.Forms.ListBox lstProcesos;
        private System.Windows.Forms.TextBox txtFiltro;
        private System.Windows.Forms.Button btnDetener;
    }
}

