using System.Threading;
using System.Threading.Tasks;
using GraphColoring.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphColoring.Application.Interfaces
{
    public interface IGraphColoringContext
    {
        public DbSet<Graph> Graphs { get; set; }
        public DbSet<AlgorithmResult> AlgorithmResults { get; set; }
        
        public int SaveChanges();
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}