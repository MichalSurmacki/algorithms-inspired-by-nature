using AutoMapper;
using GraphColoring.Application.Dtos.Graphs;
using GraphColoring.Domain.Entities;

namespace GraphColoring.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Graph, GraphReadDto>()
                .AfterMap((s, d) => d.ProcessMatrix());
        }
    }
}