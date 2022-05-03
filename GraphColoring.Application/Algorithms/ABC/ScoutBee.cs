using System.Collections.Generic;
using System.Threading.Tasks;

namespace GraphColoring.Application.Algorithms.ABC
{
    public class ScoutBee
    {
        public int[] Solution { get; private set; }

        public Task Action(List<List<int>> adjacencyMatrix)
        {
            Solution = Greedy.Start(adjacencyMatrix);
            return Task.CompletedTask;
        }
    }
}