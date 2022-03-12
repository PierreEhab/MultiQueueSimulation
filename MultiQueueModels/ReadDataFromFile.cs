using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class ReadDataFromFile
    {
        public SimulationSystem read_test_case_data(SimulationSystem simSys) 
        {
            String fileName = "X:/FCIS/Fourth Year/Second Term/Modeling and Simulation/Labs/Lab 2/Lab 2 - Task 1/MultiQueueSimulation/MultiQueueSimulation/TestCases/TestCase2.txt";
            String[] file_lines = File.ReadAllLines(fileName);
            simSys.NumberOfServers = int.Parse(file_lines[1]);
            simSys.StoppingNumber = int.Parse(file_lines[4]);
            // Check on Stopping Criteria
            simSys.StoppingCriteria = file_lines[7] == "1" ? Enums.StoppingCriteria.NumberOfCustomers : Enums.StoppingCriteria.SimulationEndTime;
            // Check on Selection Method
            switch (file_lines[10]) {
                case "1":
                    simSys.SelectionMethod = Enums.SelectionMethod.HighestPriority;
                    break;
                case "2":
                    simSys.SelectionMethod = Enums.SelectionMethod.Random;
                    break;
                case "3":
                    simSys.SelectionMethod = Enums.SelectionMethod.LeastUtilization;
                    break;
            }
            int serversIndex = 0;
            // loop to get interarrival table.
            for (int i = 13; i < file_lines.Length; i++) { 
                // interarrival table is ended.
                if(file_lines[i] == "")
                {
                    serversIndex = i + 2;
                    break;
                }
                // there is more data in interarrival table.
                else
                {
                    // split the line to interarrival time and probability
                    String[] lineWords = file_lines[i].Split(',');
                    TimeDistribution customerTimeDistribution = new TimeDistribution();
                    // set interarrival time.
                    customerTimeDistribution.Time = int.Parse(lineWords[0]);
                    // set probability.
                    customerTimeDistribution.Probability = decimal.Parse(lineWords[1]);
                    // add the customer's timeDistribution to the interarrival Distribution of the system.
                    simSys.InterarrivalDistribution.Add(customerTimeDistribution);
                }
            }
            // loop to get servers data.
            for (int i = 0; i < simSys.NumberOfServers; i++) {
                // Create New Server.
                Server server = new Server();
                // Set Server Id.
                server.ID = i + 1;
                // get serve's time distribution.
                while (serversIndex < file_lines.Length && file_lines[serversIndex] != "")
                {
                    TimeDistribution serverTimeDistribution = new TimeDistribution();
                    // split the line to service time and probability
                    String[] lineWords = file_lines[serversIndex].Split(',');
                    // set service time.
                    serverTimeDistribution.Time = int.Parse(lineWords[0]);
                    // set probability.
                    serverTimeDistribution.Probability = decimal.Parse(lineWords[1]);
                    // add the time distribution to the server.
                    server.TimeDistribution.Add(serverTimeDistribution);
                    serversIndex++;
                }
                serversIndex += 2;
                // add the server to the system.
                simSys.Servers.Add(server);
            }
            
            return simSys;
        }
    }
}
