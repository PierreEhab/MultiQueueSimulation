using System;
using System.Collections.Generic;



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
            system.queueSeconds = new Dictionary<int, List<int>>();
            system.InterarrivalDistribution = calculator.calculateCommulativeProbability(system.InterarrivalDistribution);
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                system.Servers[i].TimeDistribution = calculator.calculateCommulativeProbability(system.Servers[i].TimeDistribution);
            }
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
                            simulationCase.AssignedServer.TotalWorkingTime += simulationCase.ServiceTime;
                            simulationCase.TimeInQueue = 0;
                            simulationCase.StartTime = simulationCase.ArrivalTime;
                            simulationCase.EndTime = simulationCase.ArrivalTime + simulationCase.ServiceTime;
                        }
                        else
                        {
                            chosenServerNumber = calculator.GetFirstFreeServer(system, simulationCase);
                            int delay = system.Servers[chosenServerNumber - 1].FinishTime - simulationCase.ArrivalTime;
                            simulationCase.TimeInQueue = delay;
                            for (int j = 0; j < simulationCase.TimeInQueue; j++) {
                                if (system.queueSeconds.ContainsKey(simulationCase.ArrivalTime + j))
                                {
                                    system.queueSeconds[simulationCase.ArrivalTime + j].Add(simulationCase.CustomerNumber);
                                }
                                else {
                                    List<int> customerNumber = new List<int>();
                                    customerNumber.Add(simulationCase.CustomerNumber);
                                    system.queueSeconds.Add(simulationCase.ArrivalTime + j,customerNumber);
                                }
                            }
                            simulationCase.AssignedServer = system.Servers[chosenServerNumber - 1];
                            simulationCase.ServiceTime = calculator.GetTimeForRandomValue(table: simulationCase.AssignedServer.TimeDistribution, randomValue: simulationCase.RandomService);
                            simulationCase.AssignedServer.TotalWorkingTime += simulationCase.ServiceTime;
                            simulationCase.AssignedServer.FinishTime = simulationCase.AssignedServer.FinishTime + simulationCase.ServiceTime;
                            simulationCase.StartTime = simulationCase.ArrivalTime + simulationCase.TimeInQueue;
                            simulationCase.EndTime = simulationCase.StartTime + simulationCase.ServiceTime;
                        }
                        system.SimulationTable.Add(simulationCase);
                    }
                }
            }
            int maxQueueLen = 0;
            foreach (KeyValuePair<int, List<int>> entry in system.queueSeconds)
            {
                if (entry.Value.Count > maxQueueLen) {
                   maxQueueLen = entry.Value.Count;
                }
                
            }
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine(maxQueueLen);
            system.PerformanceMeasures.MaxQueueLength = maxQueueLen;
            Console.WriteLine(system.PerformanceMeasures.MaxQueueLength);
            system.PerformanceMeasures.AverageWaitingTime = calculator.calculateAverageWaitingTime(system);
            Console.WriteLine(system.PerformanceMeasures.AverageWaitingTime);
            system.PerformanceMeasures.WaitingProbability = calculator.calculateProbabilityOfWaiting(system);
            Console.WriteLine(system.PerformanceMeasures.WaitingProbability);

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
        public Dictionary<int,List<int>> queueSeconds { get; set; }

    }
}
