using AutoMapper;
using GraphColoring.Application.Algorithms;
using GraphColoring.Application.Algorithms.ABC;
using GraphColoring.Application.Dtos.Algorithms.Responses;
using GraphColoring.Application.Dtos.Graphs;
using GraphColoring.Application.Interfaces;
using GraphColoring.Application.Interfaces.Services;
using GraphColoring.Domain.Entities;
using GraphColoring.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GraphColoring.Application.Algorithms.SimulatedAnnealing;
using GraphColoring.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GraphColoring.Application.Services
{
    public class AlgorithmService : IAlgorithmService
    {
        private readonly IGraphColoringContext _context;

        public AlgorithmService(IGraphColoringContext context)
        {
            _context = context;
        }

        public async Task<AlgorithmResponse> PerformABCAlgorithm(int graphId, int employeeBeesSize, int employeeBeesNeighborSize, int onLookerBeesSize,
            int onLookerBeesNeighborSize, int scoutBeesSize, int maxCycles, int onLookerBeesFavouredSolutionsNumber = 0)
        {
            if(onLookerBeesFavouredSolutionsNumber > onLookerBeesSize)
            {
                throw new BadRequestException("Liczba pszczół przeszukiwaczek (onLookerBee) musi być większa lub równa ilości rozwiązań przeszukiwanych przez pszczoły tego typu.");
            }
            if(onLookerBeesFavouredSolutionsNumber < 1)
            {
                throw new BadRequestException("The number of favoured solutions by OnLookerBees cannot be less than 1...");
            }

            var g = await _context.Graphs.Where(g => g.Id.Equals(graphId)).SingleAsync();
            var gg = new GraphDto(g.AdjacencyMatrix);
            
            var watch = Stopwatch.StartNew();
            gg.GraphColors = await ABC.Start(gg.AdjacencyMatrix, employeeBeesSize, employeeBeesNeighborSize, onLookerBeesSize, onLookerBeesNeighborSize, scoutBeesSize, maxCycles, onLookerBeesFavouredSolutionsNumber);
            watch.Stop();
            var elapsedTime = watch.ElapsedMilliseconds;

            var dict = new Dictionary<string, string>();
            dict.Add("EmployeeBeesSize", employeeBeesSize.ToString());
            dict.Add("EmployeeBeesNeighborhoodSize", employeeBeesNeighborSize.ToString());
            dict.Add("OnLookerBeesSize", onLookerBeesSize.ToString());
            dict.Add("OnLookerBeesNeighborhoodSize", onLookerBeesNeighborSize.ToString());
            dict.Add("OnLookerBeesFavouredSolutionsNumber", onLookerBeesFavouredSolutionsNumber.ToString());
            dict.Add("ScoutBeesSize", scoutBeesSize.ToString());
            dict.Add("MaxCycles", maxCycles.ToString());
            var jsonInfo = JsonSerializer.Serialize(dict);
            
            SaveResultToDatabase(
                name: AlgorithmName.ABC,
                coloredNodes: gg.GraphColors.ToList(),
                time: elapsedTime,
                jsonInfo: jsonInfo,
                graph: g,
                numberOfColors: gg.GraphColors.Distinct().Count());

            return new AlgorithmResponse("ABC", gg.GraphColors);
        }

        public async Task<AlgorithmResponse> PerformSimulatedAnnealing(int graphId, int coolingParameter, int maxCycles, int neighbourhoodLookupNumber)
        {
            var g = await _context.Graphs.Where(g => g.Id.Equals(graphId)).SingleAsync();
            var gg = new GraphDto(g.AdjacencyMatrix);
            
            var watch = Stopwatch.StartNew();
            gg.GraphColors = SimulatedAnnealing.Start(gg.AdjacencyMatrix, coolingParameter, maxCycles, neighbourhoodLookupNumber);
            watch.Stop();
            var elapsedTime = watch.ElapsedMilliseconds;
            
            var dict = new Dictionary<string, string>();
            dict.Add("CoolingParameter", coolingParameter.ToString());
            dict.Add("NeighbourhoodLookupNumber", neighbourhoodLookupNumber.ToString());
            dict.Add("MaxCycles", maxCycles.ToString());
            var jsonInfo = JsonSerializer.Serialize(dict);
            
            SaveResultToDatabase(
                name: AlgorithmName.SimulatedAnnealing,
                coloredNodes: gg.GraphColors.ToList(),
                time: elapsedTime,
                jsonInfo: jsonInfo,
                graph: g,
                numberOfColors: gg.GraphColors.Distinct().Count());
            return new AlgorithmResponse("SA", gg.GraphColors);
        }
        
        //TODO delete
        public async Task<List<float>> GetGraphInfo(int graphId)
        {
            var g = await _context.Graphs.Where(g => g.Id.Equals(graphId)).SingleAsync();
            var neighboursCountedList = g.AdjacencyMatrix
                .Select(row => row.Where(i => i == 1).ToList().Count)
                .ToList();
            var ttt = new List<float>();
            ttt.Add(neighboursCountedList.Max());
            ttt.Add(neighboursCountedList.Min());
            ttt.Add((float)neighboursCountedList.Average());
            //liczba wszystkich do liczby mozliwych
            var xd = (float)(neighboursCountedList.Count * (neighboursCountedList.Count - 1)) / 2;
            var gg = (float)neighboursCountedList.Sum();
            ttt.Add((gg / xd));
            return (ttt);
        }
        
        //TODO delete
        public async Task Work()
        {
            for (var i = 220; i < 251; i++)
            {
                var g = await _context.Graphs
                                .Where(g => g.Id.Equals(i))
                                .SingleAsync();
                var gp = "D:/Desktop/ss/Greedy.txt";
                var maxp = "D:/Desktop/ss/Max.txt";
                var minp = "D:/Desktop/ss/Min.txt";
                using (StreamWriter sw = File.AppendText(gp))
                {
                    sw.Write(g.Name);
                    sw.Write(',');
                }
                using (StreamWriter sw = File.AppendText(maxp))
                {
                    sw.Write(g.Name);
                    sw.Write(',');
                }
                using (StreamWriter sw = File.AppendText(minp))
                {
                    sw.Write(g.Name);
                    sw.Write(',');
                }
                var fff = _context.AlgorithmResults.Where(x => x.Graph.Id == i).ToList();
                foreach (var r in fff)
                {
                    if (r.Name == AlgorithmName.Greedy)
                    {
                        using (StreamWriter sw = File.AppendText(gp))
                        {
                            var ddd = r.NumberOfColors.ToString();
                            sw.Write(ddd);
                            sw.Write(',');
                        }    
                    }
                    if(r.Name == AlgorithmName.LargestFirst)
                    {
                        using (StreamWriter sw = File.AppendText(maxp))
                        {
                            var ddd = r.NumberOfColors.ToString();
                            sw.Write(ddd);
                            sw.Write(',');
                        }
                    }
                    if (r.Name == AlgorithmName.LowestFirst)
                    {
                        using (StreamWriter sw = File.AppendText(minp))
                        {
                            var ddd = r.NumberOfColors.ToString();
                            sw.Write(ddd);
                            sw.Write(',');
                        }
                    }
                }
                using (StreamWriter sw = File.AppendText(gp))
                {
                    sw.Write("\n");
                }
                using (StreamWriter sw = File.AppendText(maxp))
                {
                    sw.Write("\n");
                }
                using (StreamWriter sw = File.AppendText(minp))
                {
                    sw.Write("\n");
                }
            }
        }

        public async Task<AlgorithmResponse> PerformGreedyAlgorithm(int graphId)
        {
            var g = await _context.Graphs
                .Where(g => g.Id.Equals(graphId))
                .SingleAsync();
            var gg = new GraphDto(g.AdjacencyMatrix);
            
            var watch = Stopwatch.StartNew();
            gg.GraphColors = Greedy.Start(gg.AdjacencyMatrix);
            watch.Stop();
            var elapsedTime = watch.ElapsedMilliseconds;

            SaveResultToDatabase(
                name: AlgorithmName.Greedy,
                coloredNodes: gg.GraphColors.ToList(),
                time: elapsedTime,
                jsonInfo: "",
                graph: g,
                numberOfColors: gg.GraphColors.Distinct().Count());

            var response = new AlgorithmResponse("Greedy", gg.GraphColors);
            return response;
        }

        public async Task<AlgorithmResponse> PerformLargestFirstAlgorithm(int graphId)
        {
            var g = await _context.Graphs
                .Where(g => g.Id.Equals(graphId))
                .SingleAsync();
            //TODO refactor mapping for new GraphDto
            var gg = new GraphDto(g.AdjacencyMatrix);
            
            var watch = Stopwatch.StartNew();
            gg.GraphColors = LargestFirst.Start(gg.AdjacencyMatrix);
            watch.Stop();
            var elapsedTime = watch.ElapsedMilliseconds;

            SaveResultToDatabase(
                name: AlgorithmName.LargestFirst,
                coloredNodes: gg.GraphColors.ToList(),
                time: elapsedTime,
                jsonInfo: "",
                graph: g,
                numberOfColors: gg.GraphColors.Distinct().Count());

            var response = new AlgorithmResponse("GreedyLargestFirst", gg.GraphColors);
            return response;
        }
        
        public async Task<AlgorithmResponse> PerformLowestFirstAlgorithm(int graphId)
        {
            var g = await _context.Graphs
                .Where(g => g.Id.Equals(graphId))
                .SingleAsync();
            //TODO refactor mapping for new GraphDto
            var gg = new GraphDto(g.AdjacencyMatrix);
            
            var watch = Stopwatch.StartNew();
            gg.GraphColors = LowestFirst.Start(gg.AdjacencyMatrix);
            watch.Stop();
            var elapsedTime = watch.ElapsedMilliseconds;

            SaveResultToDatabase(
                name: AlgorithmName.LowestFirst,
                coloredNodes: gg.GraphColors.ToList(),
                time: elapsedTime,
                jsonInfo: "",
                graph: g,
                numberOfColors: gg.GraphColors.Distinct().Count());

            var response = new AlgorithmResponse("GreedyLowestFirst", gg.GraphColors);
            return response;
        }

        private void SaveResultToDatabase(AlgorithmName name, List<int> coloredNodes, long time, string jsonInfo, Graph graph, int numberOfColors)
        {
            var algorithmResult = new AlgorithmResult()
            {
                Name = name,
                ColoredNodes = coloredNodes,
                TimeInMiliseconds = time,
                JsonInfo = jsonInfo,
                NumberOfColors = numberOfColors,
                Graph = graph
            };
            _context.AlgorithmResults.Add(algorithmResult);
            _context.SaveChanges();
        }
    }
}
