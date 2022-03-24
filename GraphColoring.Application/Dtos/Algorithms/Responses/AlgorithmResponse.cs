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
        public List<int> ColoredNodes { get; set; }
        public HttpStatusCode Status { get; set; }
        public string ErrorMessage { get; set; }

        public AlgorithmResponse(HttpStatusCode statusCode, string errorMessage = "")
        {
            Status = statusCode;
            ErrorMessage = errorMessage;
        }

        public AlgorithmResponse(List<int> coloredNodes)
        {
            ColoredNodes = coloredNodes;
            ColorsCount = coloredNodes.Distinct().Count();
            Status = HttpStatusCode.OK;
        }
    }
}
