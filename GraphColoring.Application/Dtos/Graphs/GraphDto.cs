using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphColoring.Application.Dtos.Graphs
{
    public class GraphDto
    {                                                   
        public bool IsFullyColored { get; set; }
        //matrix containing nodes and information if they are connected
        public readonly List<List<int>> AdjacencyMatrix;
        public int[] GraphColors { get; set; }
        public int CurrentHighestColor { get; set; }
        
        public GraphDto(List<List<int>> adjacencyMatrix)
        {
            CurrentHighestColor = 0;
            AdjacencyMatrix = adjacencyMatrix;
        }

        public bool isColoringEqual(int[] collorsArray)
        {
            throw new NotImplementedException();
        }
    }
}