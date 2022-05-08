using System.Collections.Generic;

namespace GraphColoring.Application.Dtos.Graphs
{
    public class CreateGraphRequest
    {
        public List<List<int>> AdjacencyMatrix { get; set; }
        public string Name { get; set; }
    }
}