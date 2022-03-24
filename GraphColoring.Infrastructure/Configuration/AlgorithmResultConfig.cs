using GraphColoring.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GraphColoring.Infrastructure.Configuration
{
    public class AlgorithmResultConfig : IEntityTypeConfiguration<AlgorithmResult>
    {
        public void Configure(EntityTypeBuilder<AlgorithmResult> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(r => r.ColoredNodes)
                .IsRequired()
                .HasConversion(
                    v => JsonSerializer.Serialize(v, default),
                    v => JsonSerializer.Deserialize<List<int>>(v, default)
                );

            builder.Property(r => r.TimeInMiliseconds)
                .IsRequired();

            builder.Property(r => r.JsonInfo)
                .IsRequired();

            builder.Property(r => r.NumberOfColors)
                .IsRequired();

            builder.HasOne(r => r.Graph)
                .WithMany(g => g.Results)
                .IsRequired();
        }
    }
}
