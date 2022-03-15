using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            this.Servers = new List<Server>();
            this.InterarrivalDistribution = new List<TimeDistribution>();
            this.PerformanceMeasures = new PerformanceMeasures();
            this.SimulationTable = new List<SimulationCase>();
        }

        public void Run_Simulation(SimulationSystem system , String Path)
        {
            //sending the test case location to the system
            String testCasePath = Path;
            ReadDataFromFile file_reader = new ReadDataFromFile();
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

        ///////////// INPUTS ///////////// 
        public int NumberOfServers { get; set; }
        public int StoppingNumber { get; set; }
        public List<Server> Servers { get; set; }
        public List<TimeDistribution> InterarrivalDistribution { get; set; }
        public Enums.StoppingCriteria StoppingCriteria { get; set; }
        public Enums.SelectionMethod SelectionMethod { get; set; }

        ///////////// OUTPUTS /////////////
        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }

    }
}
