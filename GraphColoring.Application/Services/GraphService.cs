using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GraphColoring.Application.Dtos.Graphs;
using GraphColoring.Application.Dtos.Graphs.Requests;
using GraphColoring.Application.Dtos.Graphs.Responses;
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
        private readonly IMapper _mapper;

        public GraphService(IGraphColoringContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetGraphByIdResponse> GetGraphById(int id)
        {
            Graph graph = await _context.Graphs.Where(g => g.Id.Equals(id)).FirstAsync();

            var g = _mapper.Map<GraphReadDto>(graph);
            var response = new GetGraphByIdResponse(g.AdjacencyMatrix);

            return response;
        }

        public async Task<CreateGraphResponse> CreateGraph(CreateGraphRequest request)
        {
            //check if adjacencyMatrix is Matrix 
            if (request.AdjacencyMatrix.Any(row => row.Count != request.AdjacencyMatrix.Count))
                throw new GraphValidationException("AdjacencyMatrix is not a Matrix...");
            
            //check if adjacencyMatrix has only 0 and 1
            if (request.AdjacencyMatrix.Select(row => row.Select(item => item < 0 || item > 1).First()).First())
                throw new GraphValidationException("AdjacencyMatrix values that are not binary...");
            
            var graph = new Graph(request.AdjacencyMatrix, request.GraphName);
            
            await _context.Graphs.AddAsync(graph);
            await _context.SaveChangesAsync();

            var g = _mapper.Map<GraphReadDto>(graph);
            var response = new CreateGraphResponse(g.Id);
            
            return response;
        }

        public async Task<CreateGraphResponse> LoadGraphFromDIMACS(StreamReader streamReaderFileDIMACS, string graphName)
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