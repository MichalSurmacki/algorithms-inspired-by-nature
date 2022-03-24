using GraphColoring.Application.Dtos.Graphs;
using GraphColoring.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GraphColoring.Application.Algorithms
{
    public static class Greedy
    {
        public static void Start(ref GraphReadDto graph)
        {
            //graph.Nodes.Shuffle();
            graph.Nodes = graph.Nodes.OrderBy(a => Guid.NewGuid()).ToList();
            foreach (GNodeDto node in graph.Nodes)
            {
                List<int> colors = new List<int>();
                //sprawdz czy nie polaczony z sasiadami
                foreach(GNodeDto neighbor in node.Neighbors)
                {
                    if (neighbor.ColorNumber != -1)
                        colors.Add(neighbor.ColorNumber);
                }
                //kolory sąsiadów
                colors.Sort();
                colors = colors.Distinct().ToList();
                int lowest = 0;                
                for(int i=0; i<colors.Count; i++)
                {
                    if (colors[i] == lowest)
                        lowest++;
                    else
                        break;
                }
                node.ColorNumber = lowest;
                //colorsClasses[lowest]++;
            }
            graph.Nodes = graph.Nodes.OrderBy(n => n.Id).ToList(); // reverse shuffle 
            graph.UpdateColorClassesConut();
            //graph.Nodes = graph.Nodes.OrderBy(n => n.Id).ToList();
        }
    }
}
