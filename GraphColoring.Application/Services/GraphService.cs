using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GraphColoring.Application.Exceptions;
using GraphColoring.Application.Interfaces;
using GraphColoring.Application.Interfaces.Services;
using GraphColoring.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphColoring.Application.Services
{
    public class GraphService : IGraphService
    {
        private readonly IGraphColoringContext _context;

        public GraphService(IGraphColoringContext context)
        {
            _context = context;
        }

        public async Task<List<List<int>>> GetGraphById(int id)
        {
            var graph = await _context.Graphs
                .Where(g => g.Id.Equals(id))
                .SingleAsync();
            return graph.AdjacencyMatrix;
        }

        public Task<int> GenereteRandomGraph(int size, int density)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateGraph(List<List<int>> adjacencyMatrix, string name)
        {
            //check if adjacencyMatrix is Matrix 
            if (adjacencyMatrix.Any(row => row.Count != adjacencyMatrix.Count))
                throw new BadRequestException("AdjacencyMatrix is not a Matrix...");
            
            //check if adjacencyMatrix has only 0 and 1
            if (adjacencyMatrix.Select(row => row.Select(item => item < 0 || item > 1).First()).First())
                throw new BadRequestException("AdjacencyMatrix has values that are not binary...");
            
            var graph = new Graph(adjacencyMatrix, name);
            
            await _context.Graphs.AddAsync(graph);
            await _context.SaveChangesAsync();

            return graph.Id;
        }

        public async Task<int> LoadGraphFromDIMACS(StreamReader streamReaderFileDIMACS, string graphName)
        {
            throw new NotImplementedException();
            // List<List<int>> adjacencyMatrix = new List<List<int>>();
            // string filename;
            // string line;
            // int allEdges = -1;
            // int edgesCount = 0;
            // int allNodes = -1;
            // bool readEdgesCount = false;
            // // processing loop
            // while((line = streamReaderFileDIMACS.ReadLine()) != null)
            // {
            //     var splits = line.Split(' ');
            //     if(splits[0] != "c" && splits[0] != "p" && splits[0] != "e" && splits[0] != " ")
            //     {
            //         return new CreateGraphResponse(System.Net.HttpStatusCode.UnprocessableEntity); // 422 understood but unable to process the contained file
            //     }
            //     else if (splits[0] == "c")
            //     {
            //         if (splits.Length >= 3 && splits[1].ToLower() == "file:")
            //         {
            //             filename = splits[2];
            //         }
            //         continue;
            //     }
            //     else if (splits.Length == 4 && splits[0] == "p" && readEdgesCount == false)
            //     {
            //         readEdgesCount = true;
            //         try
            //         {
            //             allNodes = Int32.Parse(splits[2]);
            //             for(int i = 0; i < allNodes; i++) // initialize matrix with empty values
            //             {
            //                 adjacencyMatrix.Add(new List<int>(new int[allNodes])); // default value of int is 0
            //             }
            //             allEdges = Int32.Parse(splits[3]);
            //         }
            //         catch(Exception e)
            //         {
            //             return new CreateGraphResponse(System.Net.HttpStatusCode.UnprocessableEntity, e.Message); 
            //         }
            //     }
            //     else if (splits.Length == 3 && splits[0] == "e")
            //     {
            //         edgesCount++;
            //         try
            //         {
            //             int i = Int32.Parse(splits[1]) - 1;
            //             int j = Int32.Parse(splits[2]) - 1;
            //             if (adjacencyMatrix[i][j] == 0)
            //             {
            //                 adjacencyMatrix[j][i] = 1;
            //             }
            //         }
            //         catch(Exception e)
            //         {
            //             return new CreateGraphResponse(System.Net.HttpStatusCode.UnprocessableEntity, e.Message);
            //         }
            //     }
            //     else
            //     {
            //         return new CreateGraphResponse(System.Net.HttpStatusCode.UnprocessableEntity);
            //     }
            // }
            // // validate matrix
            // if (edgesCount == 0 || edgesCount != allEdges)
            // {
            //     return new CreateGraphResponse(System.Net.HttpStatusCode.UnprocessableEntity);
            // }
            //
            // Graph graph = new Graph(adjacencyMatrix, graphName);
            // await _context.Graphs.AddAsync(graph);
            // _context.SaveChanges();
            //
            // var g = _mapper.Map<GraphReadDto>(graph);
            // var response = new CreateGraphResponse(g.Id);
            //
            // return response;
        }
    }
}