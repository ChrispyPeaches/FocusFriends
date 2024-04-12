using System.Reflection;
using FluentValidation;
using FocusAPI.Configuration.PipelineBehaviors;
using FocusAPI.Data;
using FocusAPI.Helpers;
using FocusAPI.Models;
using FocusCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MediatR.Registration;
using static FocusAPI.Methods.Sync.SyncItems;

namespace FocusAPI;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configure MediatR
        RegisterMediatR(builder);

        // Register DbContext
        builder.Services.AddDbContext<FocusContext>(options =>
        {
            options.UseSqlServer(builder.Configuration["DefaultConnectionString"]);
        });

        builder.Services.AddScoped<ISyncService, SyncService>();
        builder.Services.AddTransient<Seeder>();

        var app = builder.Build();

        Task.Run(() => RunSeeder(app.Services));

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    public static void RegisterMediatR(WebApplicationBuilder builder)
    {

        builder.Services.AddMediatR(cfg => cfg
            .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        builder.Services.AddTransient(
            typeof(IRequestHandler<Query<MindfulnessTip>, Response<MindfulnessTip>>),
            typeof(Handler<MindfulnessTip>));

    }

    /// <summary>
    /// Instantiate and seed the DbContext so that the database is created and filled with necessary data
    /// if it hasn't previously been created
    /// </summary>
    private static async Task RunSeeder(IServiceProvider services)
    {
        ILogger<Program>? logger = null;
        try
        {
            var scopedServiceProvider = services
                .CreateScope()
                .ServiceProvider;

            logger = scopedServiceProvider
                .GetService<ILogger<Program>>();
            Seeder seeder = scopedServiceProvider.GetRequiredService<Seeder>();

            await seeder.SeedData();
        }
        catch (Exception ex)
        {
            if (logger != null)
            {
                logger.LogError(ex, "Error when instantiating and seeding database");
            }
        }

    }
}

/// <summary>Registers all the handlers, validators, and behaviors.</summary>

