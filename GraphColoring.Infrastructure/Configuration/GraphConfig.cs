using System;
using System.Collections.Generic;
using System.Text.Json;
using GraphColoring.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphColoring.Infrastructure.Configuration
{
    public class GraphConfig : IEntityTypeConfiguration<Graph>
    {
        public void Configure(EntityTypeBuilder<Graph> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(g => g.AdjacencyMatrix)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, default),
                    v => JsonSerializer.Deserialize<List<List<int>>>(v, default)
                );

            builder.HasMany(s => s.Results)
            .WithOne(g => g.Graph)
            /*.HasForeignKey(g => g.Id)*/
            .IsRequired();

            builder.Property(g => g.Name)
                .HasMaxLength(50);
        }
    }
}