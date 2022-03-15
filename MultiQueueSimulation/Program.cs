using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
//using MultiQueueTesting;
using MultiQueueModels;

namespace MultiQueueSimulation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // new System is required to run the form
            SimulationSystem system = new SimulationSystem();
            
            // string result = TestingManager.Test(system, Constants.FileNames.TestCase1);
            // MessageBox.Show(result);

            //running the form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 System_Form = new Form1(system);
            Application.Run(System_Form);
            
        }
    }
}
