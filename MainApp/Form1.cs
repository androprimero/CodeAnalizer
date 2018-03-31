using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainApp
{
    public partial class Form1 : Form
    {
        Controller controller;
        public Form1()
        {
            InitializeComponent();
            controller = new Controller();
        }

        private void SolutionButton_Click(object sender, EventArgs e)
        {
            int projects = 0;
            if(SolutionFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextSolution.Text = SolutionFileDialog.FileName;
                projects =controller.OpenSolution(TextSolution.Text);
                if(projects < 0)
                {
                    Console.WriteLine("Solution couldn't be open");
                }
                else
                {
                    Console.WriteLine("Projects: " + projects.ToString());
                }
            }
        }

        private void ButtonAnalize_Click(object sender, EventArgs e)
        {

        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
