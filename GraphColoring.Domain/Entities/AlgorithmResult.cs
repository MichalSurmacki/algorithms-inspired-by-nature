using GraphColoring.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphColoring.Domain.Entities
{
    public class AlgorithmResult
    {
        public int Id { get; set; }
        public AlgorithmName Name { get; set; }
        public List<int> ColoredNodes { get; set; }
        public int NumberOfColors { get; set; }
        public long TimeInMiliseconds { get; set; }
        public string JsonInfo { get; set; }
        public Graph Graph { get; set; } //FK
    }
}
