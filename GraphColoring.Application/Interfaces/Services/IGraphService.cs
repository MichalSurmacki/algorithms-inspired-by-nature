using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GraphColoring.Application.Interfaces.Services
{
    public interface IGraphService
    {
        Task<int> CreateGraph(List<List<int>> adjacencyMatrix, string name);
        Task<int> LoadGraphFromDIMACS(StreamReader streamReaderFileDIMACS, string graphName);
        Task<List<List<int>>> GetGraphById(int id);
        Task<int> GenereteRandomGraph(int size, int density);
    }
}