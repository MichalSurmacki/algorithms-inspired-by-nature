using GraphColoring.Application.Dtos.Graphs.Requests;
using GraphColoring.Application.Dtos.Graphs.Responses;
using GraphColoring.Application.Interfaces;
using GraphColoring.Application.Interfaces.Services;
using GraphColoring.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
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
        public async Task<IActionResult> CreateGraph(CreateGraphRequest request)
        {
            var response = await _graphService.LoadGraph(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("File")]
        public async Task<IActionResult> CreateGraphFromDIMACS([Required] IFormFile fileDIMACS)
        {
            CreateGraphResponse response;
            using(var fileSteam = fileDIMACS.OpenReadStream())
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
    }
}
