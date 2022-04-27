using GraphColoring.Application.Dtos.Graphs;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GraphColoring.Application.Algorithms.ABC
{
    public class ScoutBee
    {
        public string Id { get; set; }
        public string LogInfo { get; set; }
        public GraphReadDto FoundSolution { get; private set; }
        // można by nie przechowywać instancji grafu w poszczególnej pszczole oszczędziło by to część pamięci
        public GraphReadDto RawGraph { get; private set; }
        public Task TaskAction { get; private set; }

        public ScoutBee(GraphReadDto rawGraph, string id)
        {
            Id = id;
            RawGraph = rawGraph;
            TaskAction = new Task(async () => await BeeAction(this));
        }

        public void SetTaskAction(string logInfo)
        {
            LogInfo = logInfo;
            TaskAction = new Task(async () => await BeeAction(this));
        }

        private static Task BeeAction(ScoutBee b)
        {
            if (b.RawGraph == null)
            {
                throw new TaskCanceledException("RawGraph cant be null...");
            }
            var initGraphCp = b.RawGraph.MakeACopy();
            Greedy.Start(ref initGraphCp);
            b.FoundSolution = initGraphCp;
            return Task.CompletedTask;
        }
    }
}
