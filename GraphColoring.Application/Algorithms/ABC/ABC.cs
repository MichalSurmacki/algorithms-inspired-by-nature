using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphColoring.Application.Extensions;

namespace GraphColoring.Application.Algorithms.ABC
{
    public static class ABC
    {
        public static async Task<int[]> Start(List<List<int>> adjacencyMatrix, int employeeBeesSize, int employeeNeighLookNmb, 
            int onLookerBeesSize, int onLookerNeighLookNmb, int scoutBeesSize, int maxCycles, int onLookerFavouredSolutionsNumber = 0)
        {
            int[] bestSolution = null;
            var employeeBees = new List<EmployeeBee>();
            for (var i = 0; i < employeeBeesSize; i++)
            {
                var initialSolution = Greedy.Start(adjacencyMatrix);
                if (bestSolution == null || initialSolution.Distinct().Count() < bestSolution.Distinct().Count())
                {
                    bestSolution = initialSolution.ToArray();
                }
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
                    .Select(e => new Task( () => e.Action()))
                    .ToList();
                employeeBeeTasks.ForEach(t => t.Start());
                // employeeBeeTasks[0].Start();
                await Task.WhenAll(employeeBeeTasks);
                
                var solutions = employeeBees
                    .Select(eb => new
                    {
                        eb.Solution, eb.Score
                    })
                    .OrderByDescending(x => x.Score)
                    .Select(x => x.Solution)
                    .ToList();

                //when employeed bees didnt fount better solutions than initial
                // if (solutions.Count == 0)
                // {
                //     solutions = employeeBees
                //         .Select(e => new
                //         {
                //             e.InitialSolutionColorsCount,
                //             e.InitialSolution
                //         })
                //         .OrderBy(x => x.InitialSolutionColorsCount)
                //         .Select(x => x.InitialSolution)
                //         .ToList();
                // }
                
                var firstSolution = solutions.First();
                if (firstSolution.Distinct().Count() <= bestSolution.Distinct().Count())
                {
                    bestSolution = firstSolution;
                }
                
                var employeeBeesChangesCount = 0;
                for (var j = employeeBees.Count - 1; j >= 0 ; j--)
                {
                    if (employeeBees[j].IsChangeToScoutNecessary)
                    {
                        employeeBees.RemoveAt(j);
                        employeeBeesChangesCount++;
                    }
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
                    var index = j;
                    var tasks = onLookerBeesChunks[j].Select(o => new Task(() => o.Action(solutions[index]))).ToList();
                    onLookerBeeTasks.AddRange(tasks);
                    tasks.ForEach(t => t.Start());
                }
                await Task.WhenAll(onLookerBeeTasks);

                var onLookerBeesSolutions = onLookerBees
                    .Select(o => o.Solution)
                    .OrderBy(o => o.Distinct().Count())
                    .ToList();
                var onLookerBeesFirstSolution = onLookerBeesSolutions.FirstOrDefault();
                if (onLookerBeesFirstSolution != null && bestSolution.Distinct().Count() >= onLookerBeesFirstSolution.Distinct().Count())
                {
                    bestSolution = onLookerBeesFirstSolution;
                }
                //End onLookerBees

                //Start scoutBees
                var scoutBeesTasks = scoutBees.Select(s => new Task(() => s.Action(adjacencyMatrix))).ToList();
                scoutBeesTasks.ForEach(t => t.Start());
                await Task.WhenAll(scoutBeesTasks);
                //Scouts becomes EmployeeBees
                for (var j = scoutBees.Count - 1; j >= 0 ; j--)
                {
                    employeeBees.Add(new EmployeeBee(employeeNeighLookNmb, adjacencyMatrix, scoutBees[j].Solution));
                    if (scoutBees[j].Solution.Distinct().Count() <= bestSolution.Distinct().Count())
                    {
                        bestSolution = scoutBees[j].Solution.ToArray();
                    }
                    scoutBees.RemoveAt(j);
                }
                // foreach (var scout in scoutBees)
                // {
                //     if (scout.Solution.Distinct().Count() <= bestSolution.Distinct().Count())
                //     {
                //         employeeBees.Add(new EmployeeBee(employeeNeighLookNmb, adjacencyMatrix, scout.Solution));
                //         bestSolution = scout.Solution.ToArray();
                //     }
                // }
                //End scoutBees

                //Add scoutBees from employeeBees changes
                for (var j = 0; j < employeeBeesChangesCount; j++)
                {
                    scoutBees.Add(new ScoutBee());
                }
            }
            return bestSolution;
        }
    }
}