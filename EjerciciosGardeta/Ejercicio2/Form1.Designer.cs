namespace Ejercicio2
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
            this.txtProcesos = new System.Windows.Forms.TextBox();
            this.lstProcesos = new System.Windows.Forms.ListBox();
            this.txtHilo = new System.Windows.Forms.TextBox();
            this.txtConsumo = new System.Windows.Forms.TextBox();
            this.btnConsumo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMostrar
            // 
            this.btnMostrar.Location = new System.Drawing.Point(13, 28);
            this.btnMostrar.Name = "btnMostrar";
            this.btnMostrar.Size = new System.Drawing.Size(166, 23);
            this.btnMostrar.TabIndex = 0;
            this.btnMostrar.Text = "Mostrar Procesos";
            this.btnMostrar.UseVisualStyleBackColor = true;
            this.btnMostrar.Click += new System.EventHandler(this.BtnMostrar_Click);
            // 
            // txtProcesos
            // 
            this.txtProcesos.Location = new System.Drawing.Point(209, 28);
            this.txtProcesos.Name = "txtProcesos";
            this.txtProcesos.Size = new System.Drawing.Size(147, 22);
            this.txtProcesos.TabIndex = 1;
            // 
            // lstProcesos
            // 
            this.lstProcesos.FormattingEnabled = true;
            this.lstProcesos.ItemHeight = 16;
            this.lstProcesos.Location = new System.Drawing.Point(13, 72);
            this.lstProcesos.Name = "lstProcesos";
            this.lstProcesos.Size = new System.Drawing.Size(343, 308);
            this.lstProcesos.TabIndex = 2;
            this.lstProcesos.SelectedIndexChanged += new System.EventHandler(this.LstProcesos_SelectedIndexChanged);
            // 
            // txtHilo
            // 
            this.txtHilo.Enabled = false;
            this.txtHilo.Location = new System.Drawing.Point(433, 128);
            this.txtHilo.Name = "txtHilo";
            this.txtHilo.Size = new System.Drawing.Size(168, 22);
            this.txtHilo.TabIndex = 3;
            // 
            // txtConsumo
            // 
            this.txtConsumo.Enabled = false;
            this.txtConsumo.Location = new System.Drawing.Point(614, 175);
            this.txtConsumo.Name = "txtConsumo";
            this.txtConsumo.Size = new System.Drawing.Size(161, 22);
            this.txtConsumo.TabIndex = 4;
            // 
            // btnConsumo
            // 
            this.btnConsumo.Location = new System.Drawing.Point(433, 173);
            this.btnConsumo.Name = "btnConsumo";
            this.btnConsumo.Size = new System.Drawing.Size(168, 23);
            this.btnConsumo.TabIndex = 5;
            this.btnConsumo.Text = "Monitorizar consumo";
            this.btnConsumo.UseVisualStyleBackColor = true;
            this.btnConsumo.Click += new System.EventHandler(this.BtnConsumo_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnConsumo);
            this.Controls.Add(this.txtConsumo);
            this.Controls.Add(this.txtHilo);
            this.Controls.Add(this.lstProcesos);
            this.Controls.Add(this.txtProcesos);
            this.Controls.Add(this.btnMostrar);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnMostrar;
        private System.Windows.Forms.TextBox txtProcesos;
        private System.Windows.Forms.ListBox lstProcesos;
        private System.Windows.Forms.TextBox txtHilo;
        private System.Windows.Forms.TextBox txtConsumo;
        private System.Windows.Forms.Button btnConsumo;
    }
}

