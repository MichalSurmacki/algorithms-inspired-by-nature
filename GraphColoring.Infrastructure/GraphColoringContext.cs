using GraphColoring.Application.Interfaces;
using GraphColoring.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace GraphColoring.Infrastructure
{
    public class GraphColoringContext : DbContext, IGraphColoringContext
    {
        public DbSet<Graph> Graphs { get; set; }
        public DbSet<AlgorithmResult> AlgorithmResults { get; set; }

        public GraphColoringContext(DbContextOptions<GraphColoringContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}