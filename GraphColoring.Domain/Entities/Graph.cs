using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GraphColoring.Domain.Entities
{
    public class Graph
    {
        public int Id { get; set; }
        public List<List<int>> AdjacencyMatrix { get; set; }
        public List<AlgorithmResult> Results { get; set; } // FK
        public string Name { get; set; }

        public Graph(List<List<int>> adjacencyMatrix, string name)
        {
            Name = name;
            //check if adjacencyMatrix is Matrix
            foreach (List<int> array in adjacencyMatrix)
            {
                if (array.Count != adjacencyMatrix.Count)
                    throw new ValidationException("adjacencyMatrix is not a Matrix!");
            }

            //binarize if ints are > 1
            AdjacencyMatrix = adjacencyMatrix.Select
            (
                x => x.Select
                (
                    c => c == 0 ? 0 : 1 
                ).ToList()
            ).ToList();
        }

        public Graph()
        {
        }
    }
}
