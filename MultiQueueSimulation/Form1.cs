using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueModels;
//using MultiQueueTesting;

namespace MultiQueueSimulation
{
    public partial class Form1 : Form
    {
        SimulationSystem Sim_System = new SimulationSystem();
        String Path;
        public Form1(SimulationSystem system)
        {
            Sim_System = system;
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox1.SelectedItem.ToString() == "Test Case 1")
            {
                MessageBox.Show("Test Case 1 uploaded");
                // shafai path
                Path = "C:/Users/moham/Documents/GitHub/MultiQueueSimulation/MultiQueueSimulation/TestCases/TestCase1.txt";
                // pierre path
                //Path = "X:/FCIS/Fourth Year/Second Term/Modeling and Simulation/Labs/Lab 2/Lab 2 - Task 1/MultiQueueSimulation/MultiQueueSimulation/TestCases/TestCase1.txt";
            }
            else if (comboBox1.SelectedItem.ToString() == "Test Case 2")
            {
                MessageBox.Show("Test Case 2 Uploaded");
                // shafai path
                Path = "C:/Users/moham/Documents/GitHub/MultiQueueSimulation/MultiQueueSimulation/TestCases/TestCase2.txt";
                // pierre
                //Path = "X:/FCIS/Fourth Year/Second Term/Modeling and Simulation/Labs/Lab 2/Lab 2 - Task 1/MultiQueueSimulation/MultiQueueSimulation/TestCases/TestCase2.txt";
            }
            else if (comboBox1.SelectedItem.ToString() == "Test Case 3")
            {
                MessageBox.Show("Test Case 3 uploaded");
                // shafai path
                Path = "C:/Users/moham/Documents/GitHub/MultiQueueSimulation/MultiQueueSimulation/TestCases/TestCase3.txt";
                // pierre
                //Path = "X:/FCIS/Fourth Year/Second Term/Modeling and Simulation/Labs/Lab 2/Lab 2 - Task 1/MultiQueueSimulation/MultiQueueSimulation/TestCases/TestCase3.txt";
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //simulation running is required
            Sim_System.Run_Simulation(Sim_System, Path);

            MessageBox.Show(Sim_System.SimulationTable.Count().ToString());

            //writing the table
            for (int i = 0; i < Sim_System.SimulationTable.Count(); i++)
            {
                //getting new row
                int n = dataGridView1.Rows.Add();

                //completing the table
                dataGridView1.Rows[n].Cells[0].Value = Sim_System.SimulationTable[n].CustomerNumber;
                dataGridView1.Rows[n].Cells[1].Value = Sim_System.SimulationTable[n].RandomInterArrival;
                dataGridView1.Rows[n].Cells[2].Value = Sim_System.SimulationTable[n].InterArrival;
                dataGridView1.Rows[n].Cells[3].Value = Sim_System.SimulationTable[n].ArrivalTime;
                dataGridView1.Rows[n].Cells[4].Value = Sim_System.SimulationTable[n].RandomService;
                dataGridView1.Rows[n].Cells[5].Value = Sim_System.SimulationTable[n].AssignedServer.ID;
                dataGridView1.Rows[n].Cells[6].Value = Sim_System.SimulationTable[n].StartTime;
                dataGridView1.Rows[n].Cells[7].Value = Sim_System.SimulationTable[n].EndTime;
                dataGridView1.Rows[n].Cells[8].Value = Sim_System.SimulationTable[n].ServiceTime;
                dataGridView1.Rows[n].Cells[9].Value = Sim_System.SimulationTable[n].TimeInQueue;
            }

            //filling text boxes of performance measures
            textBox1.Text = Sim_System.PerformanceMeasures.AverageWaitingTime.ToString();
            textBox2.Text = Sim_System.PerformanceMeasures.WaitingProbability.ToString();
            textBox3.Text = Sim_System.PerformanceMeasures.MaxQueueLength.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
