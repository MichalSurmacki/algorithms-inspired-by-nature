using GraphColoring.Application.Dtos.Graphs;
using GraphColoring.Application.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace GraphColoring.Application.Algorithms
{
    public static class LargestFirst
    {
        public static void Start(ref GraphReadDto graph)
        {
            // w sumie jedyna różnica w stosunku do greedy to priorytetowanie kolorowania wierzchołków o największym stopniu
            graph.Nodes = graph.Nodes.OrderByDescending(n => n.Neighbors.Count).ToList();
            foreach (GNodeDto node in graph.Nodes)
            {
                List<int> colors = new List<int>();
                //sprawdz czy nie polaczony z sasiadami
                foreach (GNodeDto neighbor in node.Neighbors)
                {
                    if (neighbor.ColorNumber != -1)
                        colors.Add(neighbor.ColorNumber);
                }
                //kolory sąsiadów
                colors.Sort();
                colors = colors.Distinct().ToList();
                int lowest = 0;
                for (int i = 0; i < colors.Count; i++)
                {
                    if (colors[i] == lowest)
                        lowest++;
                    else
                        break;
                }
                node.ColorNumber = lowest;
            }
            graph.UpdateColorClassesConut();
            //graph.Nodes = graph.Nodes.OrderBy(n => n.Id).ToList();
        }
    }
}
