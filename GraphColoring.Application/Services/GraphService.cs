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
            var adjacencyMatrix = new List<List<int>>();
            string filename;
            var allEdges = -1;
            var edgesCount = 0;
            var readEdgesCount = false;
            // processing loop
            while(streamReaderFileDIMACS.ReadLine() is { } line)
            {
                var splits = line.Split(' ');
                if(splits[0] != "c" && splits[0] != "p" && splits[0] != "e" && splits[0] != " ")
                {
                    throw new UnprocessableEntityException("Cannot process file due to problem with it...");
                }
                else if (splits[0] == "c")
                {
                    if (splits.Length >= 3 && splits[1].ToLower() == "file:")
                    {
                        filename = splits[2];
                    }
                    continue;
                }
                else if (splits.Length == 4 && splits[0] == "p" && readEdgesCount == false)
                {
                    readEdgesCount = true;
                    try
                    {
                        var allNodes = Int32.Parse(splits[2]);
                        for(int i = 0; i < allNodes; i++) // initialize matrix with empty values
                        {
                            adjacencyMatrix.Add(new List<int>(new int[allNodes])); // default value of int is 0
                        }
                        allEdges = Int32.Parse(splits[3]);
                    }
                    catch(Exception e)
                    {
                        throw new UnprocessableEntityException("Cannot process file due to problem with it...");
                    }
                }
                else if (splits.Length == 3 && splits[0] == "e")
                {
                    edgesCount++;
                    try
                    {
                        var i = Int32.Parse(splits[1]) - 1;
                        var j = Int32.Parse(splits[2]) - 1;
                        if (adjacencyMatrix[i][j] == 0)
                        {
                            adjacencyMatrix[j][i] = 1;
                        }
                    }
                    catch(Exception e)
                    {
                        throw new UnprocessableEntityException("Cannot process file due to problem with it...");
                    }
                }
                else
                {
                    throw new UnprocessableEntityException("Cannot process file due to problem with it...");
                }
            }
            // validate matrix
            if (edgesCount == 0 || edgesCount != allEdges)
            {
                throw new UnprocessableEntityException("Cannot process file due to problem with it...");
            }

            return await CreateGraph(adjacencyMatrix, graphName);
        }
    }
}