using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphColoring.Application.Extensions;

namespace GraphColoring.Application.Algorithms.ABC
{
    public static class ABC
    {
        public static int[] BestSolution = null;

        public static async Task<int[]> Start(List<List<int>> adjacencyMatrix, int employeeBeesSize, int employeeNeighLookNmb, 
            int onLookerBeesSize, int onLookerNeighLookNmb, int scoutBeesSize, int maxCycles, int onLookerFavouredSolutionsNumber = 0)
        {
            var employeeBees = new List<EmployeeBee>();
            for (var i = 0; i < employeeBeesSize; i++)
            {
                var initialSolution = Greedy.Start(adjacencyMatrix);
                employeeBees.Add(new EmployeeBee(employeeNeighLookNmb, adjacencyMatrix, initialSolution));
            }
            
            var onLookerBees = new List<OnLookerBee>();
            for (var i = 0; i < onLookerBeesSize; i++)
            {
                onLookerBees.Add(new OnLookerBee(onLookerNeighLookNmb, adjacencyMatrix));
            }
            
            var scoutBees = new List<ScoutBee>();
            for (var i = 0; i < scoutBeesSize; i++)
            {
                scoutBees.Add(new ScoutBee());
            }
            
            for (var i = 0; i < maxCycles; i++)
            {
                //Start employeeBees
                var employeeBeeTasks = employeeBees
                    .Select(employeeBee => employeeBee.Action())
                    .ToList();
                employeeBeeTasks.ForEach(t => t.Start());
                await Task.WhenAll(employeeBeeTasks);
                var employeeBeesChangesCount = 0;
                for (var j = employeeBees.Count - 1; j >= 0 ; j--)
                {
                    if (employeeBees[j].IsChangeToScoutNecessary)
                    {
                        employeeBees.RemoveAt(j);
                        employeeBeesChangesCount++;
                    }
                }
                var solutions = employeeBees
                    .Select(eb => new
                    {
                        eb.Solution, eb.Score
                    })
                    .OrderByDescending(x => x.Score)
                    .Select(x => x.Solution)
                    .ToList();

                var firstSolution = solutions.First();
                if (firstSolution.Distinct().Count() < BestSolution.Distinct().Count() || BestSolution == null)
                {
                    BestSolution = firstSolution;
                }
                //End employeeBees

                //Start onLookerBees
                var onLookerBeesChunks = new List<List<OnLookerBee>>();
                if (onLookerFavouredSolutionsNumber > 1)
                {
                    onLookerBeesChunks = onLookerBees.Split(onLookerFavouredSolutionsNumber);
                }
                else
                {
                    onLookerBeesChunks.Add(onLookerBees);
                }
                if (onLookerBeesChunks.Count > solutions.Count)
                {
                    onLookerBeesChunks = onLookerBees.Split(solutions.Count);
                }

                var onLookerBeeTasks = new List<Task>();
                for (var j = 0; j < onLookerBeesChunks.Count; j++)
                {
                    onLookerBeeTasks.AddRange(onLookerBeesChunks[j].Select(onLookerBee => onLookerBee.Action(solutions[j])));
                    onLookerBeeTasks.ForEach(t => t.Start());
                }
                await Task.WhenAll(onLookerBeeTasks);

                var onLookerBeesSolutions = onLookerBees
                    .Select(o => o.Solution)
                    .OrderBy(o => o.Distinct().Count())
                    .ToList();
                var onLookerBeesFirstSolutions = onLookerBeesSolutions.First();
                if (BestSolution.Distinct().Count() > onLookerBeesFirstSolutions.Distinct().Count())
                {
                    BestSolution = onLookerBeesFirstSolutions;
                }
                //End onLookerBees

                //Start scoutBees
                var scoutBeesTasks = scoutBees.Select(scoutBee => scoutBee.Action(adjacencyMatrix)).ToList();
                scoutBeesTasks.ForEach(t => t.Start());
                await Task.WhenAll(scoutBeesTasks);
                foreach (var scout in scoutBees)
                {
                    if (scout.Solution.Distinct().Count() <= BestSolution.Distinct().Count())
                    {
                        employeeBees.Add(new EmployeeBee(employeeNeighLookNmb, adjacencyMatrix, scout.Solution));
                        BestSolution = scout.Solution;
                    }
                }
                //End scoutBees

                //Add scoutBees from employeeBees changes
                for (var j = 0; j < employeeBeesChangesCount; j++)
                {
                    scoutBees.Add(new ScoutBee());
                }
            }
            return BestSolution;
        }
    }
}