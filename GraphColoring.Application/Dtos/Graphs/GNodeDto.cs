using System;
using System.Collections.Generic;
using System.Text;

namespace GraphColoring.Application.Dtos.Graphs
{
    public class GNodeDto
    {
        // id is index in graph --- counted from 0
        public int Id { get; set; }
        public int ColorNumber { get; set; }
        public List<GNodeDto> Neighbors { get; set; }

        public GNodeDto(int id)
        {
            Id = id;
            ColorNumber = -1;
            Neighbors = new List<GNodeDto>();
        }

        public bool HasNeighborWithColor(int colorNumber)
        {
            foreach(GNodeDto node in Neighbors)
                if (node.ColorNumber == colorNumber)
                    return true;
            return false;
        }

        public int CountNeighborsWithColor(int colorNumber)
        {
            int count = 0;
            foreach (GNodeDto node in Neighbors)
                if (node.ColorNumber == colorNumber)
                    count++;
            return count;
        }
    }
}
