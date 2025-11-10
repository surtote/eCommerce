using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ejercicio2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

                if (lmn.ProcessName.ToUpper().Contains(txtProcesos.Text.ToUpper()))
                {

                    lstProcesos.Items.Add(lmn.Id + "," + lmn.ProcessName + "," + lmn.BasePriority);
                }

            }
        }

        private delegate void SafeCallDelegate(String v);
        private void BtnConsumo_Click(object sender, EventArgs e)
        {

           // SafeCallDelegate = Ejecutar(txtHilo.Text);

                Thread t = new Thread(new ThreadStart(Ejecutar));
           
            t.Start();

        }

        public void ModificarSeguro()
        {

        }

        public void Ejecutar() 
        {
            PerformanceCounter count = new PerformanceCounter("Process", "% Processor Time", txtHilo.Text, true);

            txtConsumo.Invoke(new SafeCallDelegate(ModificarSeguro), new Object[] { txtHilo.Text });
            while (true)
            {
                double pct = count.NextValue();
                txtConsumo.Text = "" + pct;
                txtConsumo.Refresh();
                Thread.Sleep(250);
            }
        }

        private void LstProcesos_SelectedIndexChanged(object sender, EventArgs e)
        {
            Process[] procesos = Process.GetProcesses();
            string lista = lstProcesos.SelectedItem.ToString();
            string nombre = lista.Split(',')[1];
            txtHilo.Text = nombre;
        }
    }
    
}
