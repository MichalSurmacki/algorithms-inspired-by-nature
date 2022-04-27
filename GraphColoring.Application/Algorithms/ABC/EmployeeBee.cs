using GraphColoring.Application.Dtos.Graphs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphColoring.Application.Algorithms.ABC
{
    public class EmployeeBee
    {
        public string Id { get; set; }
        public string LogInfo { get; set; }
        public GraphReadDto BestSolution { get; private set; }
        public GraphReadDto InitialSolution { get; private set; }
        public Task TaskAction { get; private set; }
        public float OnLookerPropability { get; private set; }
        // p-stwo tego, że rozwiązanie będzie preferowane przez pszczoły obserwatorki
        public float OverallScore { get => OnLookerPropability / BestSolution.NumberOfColorsInGraph; }
        public int NumberOfNeighborsToLookup { get; private set; }
        public int CiclesCountWithoutImprove { get; private set; }
        public bool IsChangeNecessary { get; private set; }

        public EmployeeBee(GraphReadDto initialSolution, int numberOfNeighborsToLookup, float chance, string id)
        {
            Id = id;
            IsChangeNecessary = false;
            CiclesCountWithoutImprove = 0;
            InitialSolution = initialSolution;
            BestSolution = initialSolution;
            NumberOfNeighborsToLookup = numberOfNeighborsToLookup;
            OnLookerPropability = chance;
            TaskAction = new Task(async () => await BeeAction(this));
        }

        public void SetTaskAction(string logInfo)
        {
            LogInfo = logInfo;
            TaskAction = new Task(async () => await BeeAction(this));
        }

        public bool SetSolutionIfBetter(GraphReadDto solution)
        {
            if (solution.NumberOfColorsInGraph < BestSolution.NumberOfColorsInGraph)
            {
                BestSolution = solution;
                CiclesCountWithoutImprove = 0;
                return true;
            }
            if (solution.NumberOfColorsInGraph == BestSolution.NumberOfColorsInGraph)
            {
                CiclesCountWithoutImprove = 0;
            }
            CiclesCountWithoutImprove++;
            if (CiclesCountWithoutImprove == NumberOfNeighborsToLookup)
            {
                IsChangeNecessary = true;
            }
            return false;
        }

        private static Task BeeAction(EmployeeBee b)
        {
            for (int i = 0; i < b.NumberOfNeighborsToLookup; i++)
            {
                var neighbor = KempeChainNeighborhood.GetNeighbor(b.InitialSolution);
                b.SetSolutionIfBetter(neighbor);
            }
            return Task.CompletedTask;
        }
    }
}
