
using System.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.Ado.Factories;
using MyGuitarShop.Data.Ado.Repository;
using MyGuitarShop.Data.EFCore.Data;
using Microsoft.EntityFrameworkCore;
using MyGuitarShop.Data.EFCore.Repositories;

namespace CST324_MyGuitarShop.Api
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                AddLogging(builder);

                AddServices(builder);

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                if (builder.Environment.IsDevelopment())
                {
                    builder.Services.AddEndpointsApiExplorer();
                    builder.Services.AddSwaggerGen();
                }

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                ConfigureApplication(app);

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                if(Debugger.IsAttached) Debugger.Break();

                Console.WriteLine(ex.Message);
            }
        }

        private static void AddLogging(WebApplicationBuilder builder)
        {
            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddFilter("Microsoft", LogLevel.Information)
                    .AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information)
                    .AddConsole();
            });

            builder.Services.AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.RequestPath
                                        | HttpLoggingFields.RequestMethod
                                        | HttpLoggingFields.ResponseStatusCode;
            });

        }

        private static void ConfigureApplication(WebApplication app)
        {
            app.UseHttpLogging();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("MyGuitarShop")
                ?? throw new InvalidOperationException("MyGuitarShop connection string not found.");

            builder.Services.AddSingleton(new SqlConnectionFactory(connectionString));

            builder.Services.AddScoped<IRepository<ProductDto>, ProductRepo>();

            builder.Services.AddDbContextFactory<MyGuitarShopContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddScoped<ProductRepository>();
            builder.Services.AddScoped<CategoryRepository>();
            builder.Services.AddScoped<AddressRepository>();
            builder.Services.AddScoped<CustomerRepository>();
            builder.Services.AddScoped<OrderRepository>();
            builder.Services.AddScoped<OrderItemRepository>();
            builder.Services.AddScoped<AdministratorRepository>();
            
            // Add services to the container.
            builder.Services.AddControllers();
        }
    }
}
