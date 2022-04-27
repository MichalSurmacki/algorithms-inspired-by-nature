using System.Collections.Generic;
using System.Net;

namespace GraphColoring.Application.Dtos.Graphs.Responses
{
    public class CreateGraphResponse
    {
        public int Id { get; set; }
        
        public CreateGraphResponse(int id)
        {
            Id = id;
        }
    }
}