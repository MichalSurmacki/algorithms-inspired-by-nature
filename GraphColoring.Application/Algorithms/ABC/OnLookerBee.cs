using GraphColoring.Application.Dtos.Graphs;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GraphColoring.Application.Algorithms.ABC
{
    public class OnLookerBee
    {
        public string Id { get; set; }
        public string LogInfo { get; set; }
        public GraphReadDto BestSolution { get; private set; }
        public GraphReadDto InitialSolution { get; private set; }
        public Task TaskAction { get; private set; }
        public int NumberOfNeighborsToLookup { get; private set; }

        public OnLookerBee(int numberOfNeighborsToLookup, string id)
        {
            Id = id;
            NumberOfNeighborsToLookup = numberOfNeighborsToLookup;
            TaskAction = new Task(async () => await BeeAction(this));
        }

        public void SetTaskAction(string logInfo)
        {
            LogInfo = logInfo;
            TaskAction = new Task(async () => await BeeAction(this));
        }

        public void SetBestAndInitSolutions(GraphReadDto bestSolution, GraphReadDto initialSolution)
        {
            BestSolution = bestSolution;
            InitialSolution = initialSolution;
        }

        private bool SetSolutionIfBetter(GraphReadDto solution)
        {
            if (solution.NumberOfColorsInGraph < BestSolution.NumberOfColorsInGraph)
            {
                BestSolution = solution;
                return true;
            }
            return false;
        }

        private static Task BeeAction(OnLookerBee b)
        {
            if(b.InitialSolution == null)
            {
                throw new TaskCanceledException("InitailGraph cant be null...");
            }
            for (int i = 0; i < b.NumberOfNeighborsToLookup; i++)
            {
                var neighbor = KempeChainNeighborhood.GetNeighbor(b.InitialSolution);
                b.SetSolutionIfBetter(neighbor);
            }
            return Task.CompletedTask;
        }
    }
}
