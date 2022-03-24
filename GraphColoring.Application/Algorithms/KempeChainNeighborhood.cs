using GraphColoring.Application.Dtos.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphColoring.Application.Algorithms
{
    public static class KempeChainNeighborhood
    {
        public static GraphReadDto GetNeighbor(GraphReadDto graph)
        {
            GraphReadDto graphCopy = graph.MakeACopy();

            Random rnd = new Random();
            int id = rnd.Next(graphCopy.Nodes.Count);
            GNodeDto nroot = graphCopy.Nodes.Where(n => n.Id == id).First();
            int currentColorForChange = nroot.ColorNumber;
            int nextColorForChange = nroot.Neighbors[0].ColorNumber;

            //zmiana koloru korzenia łańcucha
            nroot.ColorNumber = nextColorForChange;
            //wszyscy sąsiedzi korzenia łańsicha o kolorze colorSecond - szukanie nowopowstałych konfliktów
            var neighbors = nroot.Neighbors
                                .Where(n => n.ColorNumber.Equals(nextColorForChange))
                                .ToList();
            int count = neighbors.Count;
            while (count != 0)
            {
                if(currentColorForChange == nextColorForChange)
                {
                    throw new Exception("Ups coś poszło nie tak, kolory sie nie zgadzają");
                }

                var newNeighbors = neighbors.ToList();
                neighbors = new List<GNodeDto>();
                foreach (var Node in newNeighbors)
                {
                    Node.ColorNumber = currentColorForChange;
                }
                //szukanie konfliktów
                foreach (var Node in newNeighbors)
                {
                    var n = Node.Neighbors
                            .Where(n => n.ColorNumber.Equals(currentColorForChange))
                            .ToList();
                    neighbors.AddRange(n);
                }
                

                neighbors = neighbors.GroupBy(x => x.Id)
                     .Select(g => g.OrderBy(y => y.Id).First()).ToList();


                count = neighbors.Count;
                var buff = currentColorForChange;
                currentColorForChange = nextColorForChange;
                nextColorForChange = buff;
            }

            graphCopy.UpdateColorClassesConut();
            return graphCopy;
        }
    }
}
