using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphColoring.Application.Algorithms.ABC
{
    public class OnLookerBee
    {
        private readonly List<List<int>> _adjacencyMatrix;
        private readonly int _numberOfNeighborsToLookup;
        public int[] Solution { get; private set; }

        public OnLookerBee(int numberOfNeighborsToLookup, List<List<int>> adjacencyMatrix)
        {
            _numberOfNeighborsToLookup = numberOfNeighborsToLookup;
            _adjacencyMatrix = adjacencyMatrix;
        }
        
        public Task Action(int[] initialSolution)
        {
            var initialSolutionColorsCount = initialSolution.Distinct().ToList().Count;
            for (var i = 0; i < _numberOfNeighborsToLookup; i++)
            {
                var newSolution = KempeChainNeighborhood.GetNeighbor(_adjacencyMatrix, initialSolution);
                var newSolutionColorsCount = newSolution.Distinct().ToList().Count;
                if (newSolutionColorsCount < initialSolutionColorsCount)
                {
                    Solution = newSolution;
                }
            }
            return Task.CompletedTask;
        }
    }
}