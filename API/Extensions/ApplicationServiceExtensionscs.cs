
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensionscs
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITokenService, TokenService>();


            //Injects our DataContext class from API/Data/DataContext.cs to our project. Options is a lambda expression passed into curly braces that sets up how we want our service to be configured.
            services.AddDbContext<DataContext>(options =>
            {
                //.UseSqllite takes 3 parameters. the first of which is connection string to link our DataContext class to our DB.
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}
