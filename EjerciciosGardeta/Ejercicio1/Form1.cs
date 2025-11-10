using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ejercicio1
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();
            funcProcesos();
        }

        private void BtnMostrar_Click(object sender, EventArgs e)
        {
            funcProcesos();
            }
        public void funcProcesos()
        {
            lstProcesos.Items.Clear();
            Process[] procesos = Process.GetProcesses();
            foreach (Process lmn in procesos)
            {

                if (lmn.ProcessName.ToUpper().Contains(txtFiltro.Text.ToUpper()))
                {

                    lstProcesos.Items.Add(lmn.Id + "," + lmn.ProcessName + "," + lmn.BasePriority);
                }

            }
        }

        private void BtnDetener_Click(object sender, EventArgs e)
        {
            Process[] procesos = Process.GetProcesses();
            string lista = lstProcesos.SelectedItem.ToString();
            int pid = int.Parse(lista.Split(',') [0]);
            foreach (Process lmn in procesos)
            {
                if(pid == lmn.Id)
                {
                    lmn.Kill();
                }

            }
        }
        }
    }
    


    