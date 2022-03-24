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
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraphColoring.Application.Services
{
    public class AlgorithmService : IAlgorithmService
    {
        private readonly IGraphColoringContext _context;
        private readonly IMapper _mapper;

        public AlgorithmService(IGraphColoringContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AlgorithmResponse> PerformABCAlgorithm(int graphId, int employeeBeesSize, int employeeBeesNeighborSize, int onLookerBeesSize,
            int onLookerBeesNeighborSize, int scoutBeesSize, int maxCicles, int onLookerBeesFavouredSolutionsNumber)
        {
            if(onLookerBeesFavouredSolutionsNumber > onLookerBeesSize)
            {
                return new AlgorithmResponse(System.Net.HttpStatusCode.UnprocessableEntity, $"Nie uda sie przydzielić ({onLookerBeesSize}) pszczół OnLooker do ({onLookerBeesFavouredSolutionsNumber}) grup...");
            }

            var g = _context.Graphs.Where(g => g.Id.Equals(graphId)).FirstOrDefault();
            var graph = _mapper.Map<GraphReadDto>(g);
            var abcAlgorithm = new ABC(graph, employeeBeesSize, employeeBeesNeighborSize, onLookerBeesSize, 
                onLookerBeesNeighborSize, scoutBeesSize, maxCicles, onLookerBeesFavouredSolutionsNumber);
            
            var watch = Stopwatch.StartNew();
            var solution = await abcAlgorithm.Start();
            watch.Stop();
            var elapsedTime = watch.ElapsedMilliseconds;

            // dodatkowe sprawdzenie rozwiązania
            var conflicts = solution.Nodes.Where(n => n.Neighbors.Any(nn => nn.ColorNumber == n.ColorNumber)).Select(n => n.Id).ToList();
            if (conflicts.Count > 0)
            {
                throw new Exception("Graph wasn't colored properly... Something went wrong...");
            }

            string jsonInfo = $"{elapsedTime},{AlgorithmName.ABC},{graphId},{solution.NumberOfColorsInGraph},{employeeBeesSize},{employeeBeesNeighborSize},{onLookerBeesSize},{onLookerBeesNeighborSize},{scoutBeesSize},{maxCicles},{onLookerBeesFavouredSolutionsNumber}";
            /*using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    writer.WriteStartObject();
                    writer.WriteNumber("graphId", graphId);
                    writer.WriteNumber("employeeBeesSize", employeeBeesSize);
                    writer.WriteNumber("employeeBeesNeighborSize", employeeBeesNeighborSize);
                    writer.WriteNumber("onLookerBeesSize", onLookerBeesSize);
                    writer.WriteNumber("onLookerBeesNeighborSize", onLookerBeesNeighborSize);
                    writer.WriteNumber("scoutBeesSize", scoutBeesSize);
                    writer.WriteNumber("maxCicles", maxCicles);
                    writer.WriteNumber("onLookerBeesFavouredSolutionsNumber", onLookerBeesFavouredSolutionsNumber);
                    writer.WriteEndObject();
                }
                jsonInfo = Encoding.UTF8.GetString(stream.ToArray());
            }*/

            var coloredNodesList = solution.GetColoredNodesList();
            // save result to database
            SaveResultToDatabase(AlgorithmName.ABC, coloredNodesList, elapsedTime, jsonInfo, g, solution.NumberOfColorsInGraph);

            return new AlgorithmResponse(coloredNodesList);
        }

        public Task<AlgorithmResponse> PerformGreedyAlgorithm(int graphId)
        {
            var g = _context.Graphs.Where(g => g.Id.Equals(graphId)).FirstOrDefault();
            var graph = _mapper.Map<GraphReadDto>(g);

            var watch = Stopwatch.StartNew();
            Greedy.Start(ref graph);
            watch.Stop();
            var elapsedTime = watch.ElapsedMilliseconds;

            // dodatkowe sprawdzenie rozwiązania
            var conflicts = graph.Nodes.Where(n => n.Neighbors.Any(nn => nn.ColorNumber == n.ColorNumber)).Select(n => n.Id).ToList();
            if (conflicts.Count > 0)
            {
                throw new Exception("Graph wasn't colored properly... Something went wrong...");
            }

            string jsonInfo = $"{elapsedTime},{AlgorithmName.Greedy},{graphId},{graph.NumberOfColorsInGraph}";
            var coloredNodesList = graph.GetColoredNodesList();
            // save result to database
            SaveResultToDatabase(AlgorithmName.Greedy, coloredNodesList, elapsedTime, jsonInfo, g, graph.NumberOfColorsInGraph);

            var response = new AlgorithmResponse(coloredNodesList);
            return Task.FromResult(response);
        }

        public Task<AlgorithmResponse> PerformLargestFirstAlgorithm(int graphId)
        {
            var g = _context.Graphs.Where(g => g.Id.Equals(graphId)).FirstOrDefault();
            var graph = _mapper.Map<GraphReadDto>(g);

            var watch = Stopwatch.StartNew();
            LargestFirst.Start(ref graph);
            watch.Stop();
            var elapsedTime = watch.ElapsedMilliseconds;

            // dodatkowe sprawdzenie rozwiązania
            var conflicts = graph.Nodes.Where(n => n.Neighbors.Any(nn => nn.ColorNumber == n.ColorNumber)).Select(n => n.Id).ToList();
            if (conflicts.Count > 0)
            {
                throw new Exception("Graph wasn't colored properly... Something went wrong...");
            }

            string jsonInfo = $"{elapsedTime},{AlgorithmName.LargestFirst},{graphId},{graph.NumberOfColorsInGraph}";
            var coloredNodesList = graph.GetColoredNodesList();
            // save result to database
            SaveResultToDatabase(AlgorithmName.LargestFirst, coloredNodesList, elapsedTime, jsonInfo, g, graph.NumberOfColorsInGraph);

            var response = new AlgorithmResponse(coloredNodesList);
            return Task.FromResult(response);
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
