using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EjerciciosGardeta
{
    public partial class Ejercicio3 : Form
    {
        public Ejercicio3()
        {
            InitializeComponent();
        }

        private void BtnAbrir_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                 string a = openFileDialog1.FileName;
                txtFichero.Text = File.ReadAllText(a);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            DialogResult res = saveFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                string a = saveFileDialog1.FileName;

               File.WriteAllText(a, txtFichero.Text);
                
            }
        }

        private void BtnGuardar2_Click(object sender, EventArgs e)
        {
                string a = openFileDialog1.FileName;
                File.WriteAllText(a, txtFichero.Text);

            }
        }
    }

