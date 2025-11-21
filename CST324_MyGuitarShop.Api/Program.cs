
using System.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.Ado.Factories;
using MyGuitarShop.Data.Ado.Repository;
using MyGuitarShop.Data.EFCore.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
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

                builder.Host.UseDefaultServiceProvider(options =>
                {
                    options.ValidateScopes = true;
                    options.ValidateOnBuild = true;
                });

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

            //Ado.net stuff
            builder.Services.AddSingleton(new SqlConnectionFactory(connectionString));

            builder.Services.AddScoped<IRepository<ProductEntity>, ProductRepo>();
            builder.Services.AddScoped<OrderRepo>();

            //EF Core stuff
            builder.Services.AddDbContextFactory<MyGuitarShopContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddScoped<ProductRepository>();
            builder.Services.AddScoped<CategoryRepository>();
            builder.Services.AddScoped<AddressRepository>();
            builder.Services.AddScoped<CustomerRepository>();
            builder.Services.AddScoped<OrderRepository>();
            builder.Services.AddScoped<OrderItemRepository>();
            builder.Services.AddScoped<AdministratorRepository>();

            //MongoDb stuff 
            var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb")
                ?? throw new InvalidOperationException("MongoDb connection string not found.");

            builder.Services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(mongoConnectionString));

            builder.Services.AddSingleton<IMongoDatabase>(sp =>
            {
                var mongoClient = sp.GetRequiredService<IMongoClient>();
                return mongoClient.GetDatabase("MyGuitarShopCluster");
            });

            // Add services to the container.
            builder.Services.AddControllers().AddControllersAsServices();
        }
    }
}
