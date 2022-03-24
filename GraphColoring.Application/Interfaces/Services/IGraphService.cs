using System.IO;
using System.Threading.Tasks;
using GraphColoring.Application.Dtos.Graphs.Requests;
using GraphColoring.Application.Dtos.Graphs.Responses;

namespace GraphColoring.Application.Interfaces.Services
{
    public interface IGraphService
    {
        Task<CreateGraphResponse> LoadGraph(CreateGraphRequest request);
        Task<CreateGraphResponse> LoadGraphFromDIMACS(StreamReader streamReaderFileDIMACS, string graphName);
        Task<GetGraphByIdResponse> GetGraphById(int id);
    }
}