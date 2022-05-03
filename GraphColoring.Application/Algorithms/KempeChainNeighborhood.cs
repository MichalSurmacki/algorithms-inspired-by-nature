using GraphColoring.Application.Dtos.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace GraphColoring.Application.Algorithms
{
    public static class KempeChainNeighborhood
    {
        public static int[] GetNeighbor(List<List<int>> adjacencyMatrix, IEnumerable<int> graphColors)
        {
            var graphColorsCopy = graphColors.ToArray();
            
            //get random index of node
            var rnd = new Random();
            var nodeIndexForColorChange = rnd.Next(adjacencyMatrix.Count);

            //get color of random node
            var currentColorForChange = graphColorsCopy[nodeIndexForColorChange];
            
            //get list of random node neighbours indexes
            var neighbourIndexes = adjacencyMatrix[nodeIndexForColorChange]
                .Select((r, i) => new {r, i})
                .Where(x => x.r == 1)
                .Select(x => x.i)
                .OrderBy(x => Guid.NewGuid())
                .ToList();
            
            //get node random neighbour
            var neighbourIndexForColorChange = neighbourIndexes.First();
            
            //get color of random neighbour for change with random node
            var nextColorForChange = graphColorsCopy[neighbourIndexForColorChange];

            //change nodes colors
            graphColorsCopy[nodeIndexForColorChange] = nextColorForChange;
            graphColorsCopy[neighbourIndexForColorChange] = currentColorForChange;
            
            //get conflicts 
            var neighboursIndexesWithColorConflict =
                neighbourIndexes.Where(ni => graphColorsCopy[ni] == nextColorForChange)
                    .ToList();

            var count = neighboursIndexesWithColorConflict.Count;
            while (count > 0)
            {
                var newNeighboursList = new List<int>();
                foreach (var neighbourIndex in neighboursIndexesWithColorConflict)
                {
                    //resolve conflicts
                    graphColorsCopy[neighbourIndex] = currentColorForChange;
                    //add new neighbours with conflict if exists
                    newNeighboursList.AddRange(
                        adjacencyMatrix[neighbourIndex]
                            .Select((r, i) => new {r, i})
                            .Where(x => x.r == 1 && graphColorsCopy[x.i] == currentColorForChange)
                            .Select(x => x.i)
                            .ToList());
                }

                count = newNeighboursList.Count;
                (currentColorForChange, nextColorForChange) = (nextColorForChange, currentColorForChange);
            }
            return graphColorsCopy;
        }
    }
}
