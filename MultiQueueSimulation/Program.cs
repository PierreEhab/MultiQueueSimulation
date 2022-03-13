using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueTesting;
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
            SimulationSystem system = new SimulationSystem();
            ReadDataFromFile file_reader = new ReadDataFromFile();
            file_reader.read_test_case_data(system);
            Calculator calculator = new Calculator();
           
            system.InterarrivalDistribution = calculator.calculateCommulativeProbability(system.InterarrivalDistribution);

            MessageBox.Show(system.InterarrivalDistribution[1].MinRange.ToString());
            MessageBox.Show(system.InterarrivalDistribution[1].MaxRange.ToString());
            Random rnd = new Random();
            if (system.StoppingCriteria == Enums.StoppingCriteria.NumberOfCustomers)
            {
                for (int i = 0; i < system.StoppingNumber; i++) {
                    SimulationCase simulationCase = new SimulationCase();
                    // Set customer ID.
                    simulationCase.CustomerNumber = i + 1;

                    if (i == 0) {
                        simulationCase.RandomInterArrival = -1;
                        simulationCase.InterArrival = -1;
                        simulationCase.ArrivalTime = 0;
                        // Generate Random Service time number.
                        simulationCase.RandomService = rnd.Next(0, 100);
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
                        simulationCase.AssignedServer = system.Servers[chosenServerNumber - 1];
                        simulationCase.ServiceTime = calculator.GetTimeForRandomValue(table: simulationCase.AssignedServer.TimeDistribution, randomValue: simulationCase.RandomService);
                        system.SimulationTable.Add(simulationCase); 
                    }
                    else
                    {
                        // Generate Random interarival number.
                        simulationCase.RandomInterArrival = rnd.Next(0, 100);
                        // Get Interarrival time from generated random number.
                        simulationCase.InterArrival = calculator.GetTimeForRandomValue(table:system.InterarrivalDistribution,simulationCase.RandomInterArrival);
                        // set arrival time.
                        simulationCase.ArrivalTime = system.SimulationTable[i-1].ArrivalTime + simulationCase.InterArrival;
                        // Generate Random Service time number.
                        simulationCase.RandomService = rnd.Next(0, 100);
                        // check on server selection
                        int chosenServerNumber = -1;
                        switch (system.SelectionMethod) {
                            case Enums.SelectionMethod.Random:
                                chosenServerNumber = calculator.GetRandomServerID(system,simulationCase);
                                Console.WriteLine(chosenServerNumber);
                                break;
                            case Enums.SelectionMethod.HighestPriority:
                                chosenServerNumber = calculator.GetHighestPriorityServerID(system, simulationCase);
                                Console.WriteLine(chosenServerNumber);
                                break;
                            case Enums.SelectionMethod.LeastUtilization:
                                break;
                        }
                        simulationCase.AssignedServer = system.Servers[chosenServerNumber - 1];
                        simulationCase.ServiceTime = calculator.GetTimeForRandomValue(table:simulationCase.AssignedServer.TimeDistribution,randomValue:simulationCase.RandomService);
                        system.SimulationTable.Add(simulationCase);
                    }
                }
            }
            
                
            
            
            //string result = TestingManager.Test(system, Constants.FileNames.TestCase1);
            //MessageBox.Show(result);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

        }
    }
}
