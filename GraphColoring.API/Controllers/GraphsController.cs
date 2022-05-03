using System;
using System.Collections.Generic;
using GraphColoring.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace GraphColoring.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphsController : ControllerBase
    {
        private readonly IGraphService _graphService;

        public GraphsController(IGraphService graphService)
        {
            _graphService = graphService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGraph([FromBody]List<List<int>> adjacencyMatrix, [FromBody] string name)
        {
            var response = await _graphService.CreateGraph(adjacencyMatrix, name);
            return Ok(response);
        }

        [HttpPost]
        [Route("File")]
        public async Task<IActionResult> CreateGraphFromDIMACS([Required] IFormFile fileDIMACS)
        {
            int response;
            await using(var fileSteam = fileDIMACS.OpenReadStream())
            {
                response = await _graphService.LoadGraphFromDIMACS(new StreamReader(fileSteam), fileDIMACS.FileName);
            }
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGraphById([FromRoute] int id)
        {
            var response = await _graphService.GetGraphById(id);
            return Ok(response);
        }
        
        [HttpGet("Generate")]
        public async Task<IActionResult> GenerateRandomGraph([FromQuery] int size, [FromQuery] float density)
        {
            throw new NotImplementedException("Not Implemented");
        }
    }
}
