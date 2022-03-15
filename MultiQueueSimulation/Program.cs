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
            ReadDataFromFile file_reader = new ReadDataFromFile();
            // string result = TestingManager.Test(system, Constants.FileNames.TestCase1);
            // MessageBox.Show(result);

            //running the form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 System_Form = new Form1(system);
            Application.Run(System_Form);

            //sending thr test case location to the system
            String testCasePath = System_Form.ComboBox_Return();

            file_reader.read_test_case_data(system, testCasePath);
            Calculator calculator = new Calculator();

            system.InterarrivalDistribution = calculator.calculateCommulativeProbability(system.InterarrivalDistribution);
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                system.Servers[i].TimeDistribution = calculator.calculateCommulativeProbability(system.Servers[i].TimeDistribution);
            }


            //MessageBox.Show(system.InterarrivalDistribution[1].MinRange.ToString());
            //MessageBox.Show(system.InterarrivalDistribution[1].MaxRange.ToString());
            Random rnd = new Random();
            if (system.StoppingCriteria == Enums.StoppingCriteria.NumberOfCustomers)
            {
                for (int i = 0; i < system.StoppingNumber; i++)
                {
                    SimulationCase simulationCase = new SimulationCase();
                    // Set customer ID.
                    simulationCase.CustomerNumber = i + 1;

                    if (i == 0)
                    {
                        simulationCase.RandomInterArrival = 1;
                        simulationCase.InterArrival = 0;
                        simulationCase.ArrivalTime = 0;
                        // Generate Random Service time number.
                        simulationCase.RandomService = rnd.Next(1, 100);
                        // check on server selection
                        int chosenServerNumber = -1;
                        switch (system.SelectionMethod)
                        {
                            case Enums.SelectionMethod.Random:
                                chosenServerNumber = calculator.GetRandomServerID(system, simulationCase);
                                Console.WriteLine(chosenServerNumber);
                                break;
                            case Enums.SelectionMethod.HighestPriority:
                                chosenServerNumber = 1;
                                Console.WriteLine(chosenServerNumber);
                                break;
                            case Enums.SelectionMethod.LeastUtilization:
                                break;
                        }
                        simulationCase.AssignedServer = system.Servers[chosenServerNumber - 1];
                        simulationCase.ServiceTime = calculator.GetTimeForRandomValue(table: simulationCase.AssignedServer.TimeDistribution, randomValue: simulationCase.RandomService);
                        simulationCase.AssignedServer.FinishTime = simulationCase.ArrivalTime + simulationCase.ServiceTime;
                        simulationCase.TimeInQueue = 0;
                        simulationCase.StartTime = 0;
                        simulationCase.EndTime = simulationCase.ArrivalTime + simulationCase.ServiceTime;
                        system.SimulationTable.Add(simulationCase);
                    }
                    else
                    {
                        // Generate Random interarival number.
                        simulationCase.RandomInterArrival = rnd.Next(1, 100);
                        // Get Interarrival time from generated random number.
                        simulationCase.InterArrival = calculator.GetTimeForRandomValue(table: system.InterarrivalDistribution, simulationCase.RandomInterArrival);
                        // set arrival time.
                        simulationCase.ArrivalTime = system.SimulationTable[i - 1].ArrivalTime + simulationCase.InterArrival;
                        // Generate Random Service time number.
                        simulationCase.RandomService = rnd.Next(1, 100);
                        // check on server selection
                        int chosenServerNumber = -1;
                        switch (system.SelectionMethod)
                        {
                            case Enums.SelectionMethod.Random:
                                chosenServerNumber = calculator.GetRandomServerID(system, simulationCase);
                                Console.WriteLine(chosenServerNumber);
                                break;
                            case Enums.SelectionMethod.HighestPriority:
                                chosenServerNumber = calculator.GetHighestPriorityServerID(system, simulationCase);
                                Console.WriteLine(chosenServerNumber);
                                break;
                            case Enums.SelectionMethod.LeastUtilization:
                                break;
                        }
                        if (chosenServerNumber != -1)
                        {
                            simulationCase.AssignedServer = system.Servers[chosenServerNumber - 1];
                            simulationCase.ServiceTime = calculator.GetTimeForRandomValue(table: simulationCase.AssignedServer.TimeDistribution, randomValue: simulationCase.RandomService);
                            simulationCase.AssignedServer.FinishTime = simulationCase.ArrivalTime + simulationCase.ServiceTime;
                            simulationCase.TimeInQueue = 0;
                            simulationCase.StartTime = simulationCase.ArrivalTime;
                            simulationCase.EndTime = simulationCase.ArrivalTime + simulationCase.ServiceTime;
                        }
                        else
                        {
                            chosenServerNumber = calculator.GetFirstFreeServer(system, simulationCase);
                            int delay = system.Servers[chosenServerNumber - 1].FinishTime - simulationCase.ArrivalTime;
                            simulationCase.TimeInQueue = delay;
                            simulationCase.AssignedServer = system.Servers[chosenServerNumber - 1];
                            simulationCase.ServiceTime = calculator.GetTimeForRandomValue(table: simulationCase.AssignedServer.TimeDistribution, randomValue: simulationCase.RandomService);
                            simulationCase.AssignedServer.FinishTime = simulationCase.AssignedServer.FinishTime + simulationCase.ServiceTime;
                            simulationCase.StartTime = simulationCase.ArrivalTime + simulationCase.TimeInQueue;
                            simulationCase.EndTime = simulationCase.StartTime + simulationCase.ServiceTime;
                        }

                        system.SimulationTable.Add(simulationCase);
                    }
                }
            }
        }
    }
}
