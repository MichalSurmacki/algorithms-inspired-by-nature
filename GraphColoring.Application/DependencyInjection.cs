using System.Reflection;
using GraphColoring.Application.Interfaces.Services;
using GraphColoring.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace GraphColoring.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGraphService, GraphService>();
            services.AddScoped<IAlgorithmService, AlgorithmService>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}