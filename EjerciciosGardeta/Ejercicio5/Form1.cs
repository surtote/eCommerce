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

namespace Ejercicio5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {


            buscar("E:\\OceanoAtlantico");

        }
        void buscar(string directorio)
        {
            try
            {
                string[] directorios = Directory.GetFiles(directorio);

                foreach (String dir in directorios)
                {


                    if (dir.Contains(txtBuscar.Text))
                    {
                        lstProcesos.Items.Add(dir);
                        lstProcesos.Refresh();
                    }

                }
                foreach (string dir in Directory.GetDirectories(directorio))
                {
                    buscar(dir);
                }
            }
            catch(Exception e2)
            {

            }
        }
    }
}
