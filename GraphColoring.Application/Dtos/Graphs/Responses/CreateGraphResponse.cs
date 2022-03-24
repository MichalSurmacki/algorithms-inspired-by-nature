using System.Collections.Generic;
using System.Net;

namespace GraphColoring.Application.Dtos.Graphs.Responses
{
    public class CreateGraphResponse
    {
        public int Id { get; set; }
        public HttpStatusCode Status { get; set; }
        public string ErrorMessage { get; set; }
        public CreateGraphResponse(int id)
        {
            Id = id;
            Status = HttpStatusCode.OK;
        }
        public CreateGraphResponse(HttpStatusCode statusCode, string ErrorMessage = "")
        {
            Status = statusCode;
            ErrorMessage = ErrorMessage;
        }        
    }
}