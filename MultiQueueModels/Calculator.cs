using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class Calculator
    {
        public List<TimeDistribution> calculateCommulativeProbability(List<TimeDistribution> table)
        {
            for (int i = 0; i < table.Count; i++)
            {
                if (i == 0)
                {
                    table[i].CummProbability = table[i].Probability;
                    table[i].MinRange = 0;
                    table[i].MaxRange = (int)(table[i].CummProbability * 100);
                }
                else
                {
                    table[i].CummProbability = table[i - 1].CummProbability + table[i].Probability;
                    table[i].MinRange = (int)(table[i - 1].CummProbability * 100) + 1;
                    table[i].MaxRange = (int)(table[i].CummProbability * 100);
                }
            }
            return table;
        }
        public int GetTimeForRandomValue(List<TimeDistribution> table, int randomValue)
        {
            int serviceTime = -1;
            for (int i = 0; i < table.Count; i++)
            {
                if (randomValue > table[i].MinRange && randomValue < table[i].MaxRange)
                {
                    serviceTime = table[i].Time;
                }
            }
            return serviceTime;
        }
        public int GetRandomServerID(SimulationSystem simSys,SimulationCase simCase) {
            int serverId = -1;
            List<Server> availableServers = new List<Server>();
            // get all available servers;
            for (int i = 0; i < simSys.Servers.Count; i++) {

                if (simCase.ArrivalTime > simSys.Servers[i].FinishTime) { 
                    availableServers.Add(simSys.Servers[i]);
                }
            }
            if (availableServers.Count > 0) {
                Random rnd = new Random();
                serverId = availableServers[rnd.Next(1, availableServers.Count)].ID;
                
            }
            else
            {
                Debug.Assert(false);
            }
            return serverId;
        }
        public int GetHighestPriorityServerID(SimulationSystem simSys, SimulationCase simCase)
        {
            int serverId = -1;
            List<Server> availableServers = new List<Server>();
            // get all available servers;
            for (int i = 0; i < simSys.Servers.Count; i++)
            {

                if (simCase.ArrivalTime > simSys.Servers[i].FinishTime)
                {
                    availableServers.Add(simSys.Servers[i]);
                }
            }
            if (availableServers.Count > 0)
            {
                serverId = availableServers[0].ID;

            }
            return serverId;
        }
        public int GetFirstFreeServer(SimulationSystem simSys, SimulationCase simCase) {
            int nextFinishTime = 10000;
            int serverId = -1;
            for (int i = 0; i < simSys.NumberOfServers; i++) {
                if (simSys.Servers[i].FinishTime < nextFinishTime) {
                    nextFinishTime = simSys.Servers[i].FinishTime;
                    serverId = simSys.Servers[i].ID;
                }
            }
            return serverId;
        }

    }
}
