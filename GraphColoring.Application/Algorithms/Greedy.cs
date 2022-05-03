using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphColoring.Application.Algorithms
{
    public static class Greedy
    {
        public static int[] Start(List<List<int>> adjacencyMatrix)
        {
            var graphColors = new int[adjacencyMatrix.Count];
            
            //get randomized array of indexes of nodes
            var nodesIndexes = Enumerable
                .Range(0, adjacencyMatrix.Count)
                .OrderBy(x => Guid.NewGuid())
                .ToArray();

            foreach (var nodeIndex in nodesIndexes)
            {
                //get neighbours indexes of random node 
                var neighboursIndexes = adjacencyMatrix[nodeIndex]
                    .Select((r ,i) => new {r, i})
                    .Where(x => x.r == 1)
                    .Select(x => x.i).ToList();    
                
                //get list of colors of neighbours
                var neighboursColors = neighboursIndexes
                    .Where(x => graphColors[x] != 0)
                    .Select(x => graphColors[x])
                    .Distinct()
                    .ToList()
                    .OrderBy(x => x)
                    .ToList();

                //get lowest available color
                int lowest = 1;
                foreach (var color in neighboursColors)
                {
                    if (color == lowest)
                        lowest++;
                    else
                        break;
                }
                graphColors[nodeIndex] = lowest;
            }
            return graphColors;
        }
    }
}
