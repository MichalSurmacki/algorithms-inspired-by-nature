using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace GraphColoring.Application.Dtos.Algorithms.Responses
{
    public class AlgorithmResponse
    {
        public string AlgorithmName { get; set; }
        public int ColorsCount { get; set; }
        public int[] ColoredNodes { get; set; }

        public AlgorithmResponse(string algorithmName, int[] coloredNodes)
        {
            AlgorithmName = algorithmName;
            ColoredNodes = coloredNodes;
            ColorsCount = coloredNodes.Distinct().Count();
        }
    }
}
