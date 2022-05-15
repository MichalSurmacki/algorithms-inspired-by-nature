using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphColoring.Application.Algorithms.SimulatedAnnealing
{
    public static class SimulatedAnnealing
    {
        public static int[] Start(List<List<int>> adjacencyMatrix, int coolingParameter, int maxCycles, int neighbourhoodLookupNumber)
        {
            var solution = Greedy.Start(adjacencyMatrix);
            var solutionColorsCount = solution.Distinct().ToList().Count;
            
            float cooling = coolingParameter;
            var rand = new Random();
            
            for (var i = 0; i < maxCycles; i++)
            {
                for (var j = 0; j < neighbourhoodLookupNumber; j++)
                {
                    var neighbour = KempeChainNeighborhood.GetNeighbor(adjacencyMatrix, solution);    
                    var neighbourColorsCount = neighbour.Distinct().ToList().Count;

                    var delta = solutionColorsCount - neighbourColorsCount;
                    
                    //if neighbour solution is better - go there
                    if (delta > 0)
                    {
                        solution = neighbour;
                        solutionColorsCount = neighbourColorsCount;
                    }
                    //if neighbour solution is worse - count propability
                    else
                    {
                        if (Math.Exp(delta / cooling) > rand.NextDouble())
                        {
                            solution = neighbour;
                            solutionColorsCount = neighbourColorsCount;
                        }
                    }
                }
                cooling = (float)(cooling * 0.9);
            }
            
            return solution;
        }
    }
}