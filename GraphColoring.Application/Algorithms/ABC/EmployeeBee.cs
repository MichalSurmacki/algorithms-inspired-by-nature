using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphColoring.Application.Algorithms.ABC
{
    public class EmployeeBee
    {
        private readonly List<List<int>> _adjacencyMatrix;
        public int NumberOfNeighborsToLookup { get; }
        public int[] InitialSolution { get; private set; }
        public int InitialSolutionColorsCount { get; private set; }
        public int[] Solution { get; private set; }
        public float Score { get; private set; }
        public bool IsChangeToScoutNecessary { get; private set; }
        
        public EmployeeBee(int numberOfNeighborsToLookup, List<List<int>> adjacencyMatrix, int[] initialSolution)
        {
            NumberOfNeighborsToLookup = numberOfNeighborsToLookup;
            _adjacencyMatrix = adjacencyMatrix;
            InitialSolution = initialSolution;
            InitialSolutionColorsCount = initialSolution.Distinct().ToList().Count;
        }
        
        public Task Action()
        {
            for (var i = 0; i < NumberOfNeighborsToLookup; i++)
            {
                var newSolution = KempeChainNeighborhood.GetNeighbor(_adjacencyMatrix, InitialSolution);
                var newSolutionColorsCount = newSolution.Distinct().ToList().Count;
                if (newSolutionColorsCount < InitialSolutionColorsCount)
                {
                    Solution = newSolution;
                    var rnd = new Random();
                    InitialSolution = Solution;
                    InitialSolutionColorsCount = Solution.Distinct().ToList().Count;
                    var chance = (float) rnd.NextDouble();
                    Score = chance / InitialSolutionColorsCount;
                }
            }

            if (Solution == null)
            {
                IsChangeToScoutNecessary = true;
            }
            
            return Task.CompletedTask;
        }
    }
}