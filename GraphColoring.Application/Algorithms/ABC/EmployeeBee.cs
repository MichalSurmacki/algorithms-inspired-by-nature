using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphColoring.Application.Extensions;

namespace GraphColoring.Application.Algorithms.ABC
{
    public class EmployeeBee
    {
        private readonly List<List<int>> _adjacencyMatrix;
        public int NumberOfNeighborsToLookup { get; }
        public int[] InitialSolution { get; private set; }
        public int InitialSolutionColorsCount { get; private set; }
        
        
        public int[] StartingSolution { get; private set; }
        public int StartingSolutionColorsCount { get; private set; }
        
        
        
        public int[] Solution { get; private set; }
        public int SolutionColorsCount { get; private set; }
        
        
        public float Score { get; private set; }
        public bool IsChangeToScoutNecessary { get; private set; } = true;
        
        public EmployeeBee(int numberOfNeighborsToLookup, List<List<int>> adjacencyMatrix, int[] initialSolution)
        {
            NumberOfNeighborsToLookup = numberOfNeighborsToLookup;
            _adjacencyMatrix = adjacencyMatrix;
            InitialSolution = initialSolution;
            InitialSolutionColorsCount = initialSolution.Distinct().ToList().Count;
            Solution = initialSolution;
            SolutionColorsCount = InitialSolutionColorsCount;
        }
        
        public Task Action()
        {
            for (var i = 0; i < NumberOfNeighborsToLookup; i++)
            {
                var newSolution = KempeChainNeighborhood.GetNeighbor(_adjacencyMatrix, Solution);
                var newSolutionColorsCount = newSolution.Distinct().ToList().Count;
                if (newSolutionColorsCount <= StartingSolutionColorsCount)
                {
                    if (newSolution.Matching(Solution)) continue;
                    Solution = newSolution;
                    SolutionColorsCount = newSolutionColorsCount;
                    
                    var rnd = new Random();
                    var chance = (float) rnd.NextDouble();
                    Score = chance / SolutionColorsCount;

                    IsChangeToScoutNecessary = false;
                }
            }
            return Task.CompletedTask;
        }
    }
}