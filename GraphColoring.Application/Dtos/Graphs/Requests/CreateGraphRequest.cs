using System.Collections.Generic;

namespace GraphColoring.Application.Dtos.Graphs.Requests
{
    public class CreateGraphRequest
    {
        public List<List<int>> AdjacencyMatrix { get; set; }
        public string GraphName { get; set; }
    }
}