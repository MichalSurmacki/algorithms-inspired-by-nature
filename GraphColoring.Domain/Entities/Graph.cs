using System.Collections.Generic;

namespace GraphColoring.Domain.Entities
{
    public class Graph
    {
        public int Id { get; }
        public List<List<int>> AdjacencyMatrix { get; }
        public List<AlgorithmResult> Results { get; } = null!; // FK
        public string Name { get; }

        public Graph(List<List<int>> adjacencyMatrix, string name)
        {
            Name = name;
            AdjacencyMatrix = adjacencyMatrix;
        }
    }
}
