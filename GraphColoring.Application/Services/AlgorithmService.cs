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
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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

            // dodatkowe sprawdzenie rozwiązania
            // var conflicts = solution.Nodes.Where(n => n.Neighbors.Any(nn => nn.ColorNumber == n.ColorNumber)).Select(n => n.Id).ToList();
            // if (conflicts.Count > 0)
            // {
            //     throw new Exception("Graph wasn't colored properly... Something went wrong...");
            // }
            
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

        public Task<AlgorithmResponse> PerformSimulatedAnnealing()
        {
            throw new NotImplementedException();
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

            // dodatkowe sprawdzenie rozwiązania
            // var conflicts = graph.Nodes.Where(n => n.Neighbors.Any(nn => nn.ColorNumber == n.ColorNumber)).Select(n => n.Id).ToList();
            // if (conflicts.Count > 0)
            // {
            //     throw new Exception("Graph wasn't colored properly... Something went wrong...");
            // }
            
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

            // dodatkowe sprawdzenie rozwiązania
            // var conflicts = graph.Nodes.Where(n => n.Neighbors.Any(nn => nn.ColorNumber == n.ColorNumber)).Select(n => n.Id).ToList();
            // if (conflicts.Count > 0)
            // {
            //     throw new Exception("Graph wasn't colored properly... Something went wrong...");
            // }
            
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
