using System;
using System.Collections.Generic;
using System.Text;
using GraphColoring.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GraphColoring.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //dependency injection for dbcontextinterface
            services.AddScoped<IGraphColoringContext>(provider => provider.GetService<GraphColoringContext>());

            var builder = new SqlConnectionStringBuilder();
            builder.ConnectionString = configuration.GetConnectionString("GraphColoringConnection");
            builder.UserID = configuration["UserID"];
            builder.Password = configuration["Password"];
            builder.DataSource = configuration["Datasource"] ?? "localhost";
            builder.InitialCatalog = configuration["InitialCatalog"] ?? "GraphColoringDb";
            services.AddDbContext<GraphColoringContext>(opt => opt.UseSqlServer(builder.ConnectionString));

            return services;
        }
    }
}
