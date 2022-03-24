using System.Collections.Generic;
using System.Linq;

namespace GraphColoring.Application.Dtos.Graphs
{
    public class GraphReadDto
    {
        public int Id { get; private set; }
        public IList<GNodeDto> Nodes { get; set; }
        public List<List<int>> AdjacencyMatrix { get; private set; }

        /// <summary>
        /// Index = ColorNumber; Value = ColorNumber count in graph; List Count - number of colors in graph
        /// </summary>
        public List<int> ColorClassesCount { get; private set; } = new List<int>();
        public int NumberOfColorsInGraph { get => ColorClassesCount.Count; }

        public void ProcessMatrix()
        {
            if (Nodes != null)
                return;
            
            bool[][] adjacencyMatrix = AdjacencyMatrix.Select
            (
                s => s.Select
                (
                    c => c == 1
                ).ToArray()
            ).ToArray();

            //make graph nodes
            GNodeDto[] nodes = new GNodeDto[adjacencyMatrix.Length];
            for (int i = 0; i < adjacencyMatrix.Length; i++)
                nodes[i] = new GNodeDto(i);

            //deserialize matrix to GNode array
            for (int i = 0; i < adjacencyMatrix.Length; i++)
            {
                List<int> indexes = new List<int>();
                for (int j = 0; j < i; j++)
                {
                    if (adjacencyMatrix[i][j] == true)
                        indexes.Add(j);
                }
                for (int j = i; j < adjacencyMatrix.Length; j++)
                {
                    if (adjacencyMatrix[j][i] == true)
                        indexes.Add(j);
                }

                for (int j = 0; j < indexes.Count; j++)
                {
                    nodes[i].Neighbors.Add(nodes[indexes[j]]);
                }
            }
            Nodes = nodes;
        }
        
        public List<int> UpdateColorClassesConut()
        {
            int[] colors = new int[Nodes.Count];
            foreach(GNodeDto node in Nodes)
                colors[node.ColorNumber]++;
            ColorClassesCount = colors.ToList();

            int index = Nodes.Count - 1;
            for (int i = index; i >= 0; i--)
                if (ColorClassesCount[i] == 0)
                    ColorClassesCount.RemoveAt(i);

            return ColorClassesCount;
        }
    
        public GraphReadDto MakeACopy()
        {
            var copy = new GraphReadDto();
            copy.Id = Id;
            copy.AdjacencyMatrix = AdjacencyMatrixCopy();
            var copyNodes = new List<GNodeDto>();
            for(int i = 0; i < Nodes.Count; i++)
            {
                copyNodes.Add(new GNodeDto(Nodes[i].Id));
                copyNodes[i].ColorNumber = Nodes[i].ColorNumber;
            }
            for (int i = 0; i < Nodes.Count; i++)
            {
                foreach (GNodeDto n in Nodes[i].Neighbors)
                {
                    copyNodes[i].Neighbors.Add(copyNodes[n.Id]);
                }
            }
            copy.Nodes = copyNodes;
            for (int i = 0; i < ColorClassesCount.Count; i++)
            {
                copy.ColorClassesCount.Add(ColorClassesCount[i]);
            }

            return copy;
        }
    
        public List<List<int>> AdjacencyMatrixCopy()
        {
            var copy = new List<List<int>>();
            foreach (List<int> row in AdjacencyMatrix)
                copy.Add(row.ToList());
            return copy;
        }

        public List<int> GetColoredNodesList()
        {
            List<int> list = new List<int>();
            foreach(GNodeDto node in Nodes)
            {
                list.Add(node.ColorNumber);
            }
            return list;
        }
    }
}