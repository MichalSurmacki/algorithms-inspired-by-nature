using GraphColoring.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphColoring.Application.Interfaces
{
    public interface IGraphColoringContext
    {
        public DbSet<Graph> Graphs { get; set; }
        public DbSet<AlgorithmResult> AlgorithmResults { get; set; }
        public int SaveChanges();
    }
}