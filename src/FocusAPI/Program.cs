using FluentValidation;
using MediatR;
using System.Reflection;
using System.Runtime.CompilerServices;
using FocusAPI.Configuration.PipelineBehaviors;
using FocusAPI.Data;
using FocusCore.Validators.Users;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MediatR
Assembly[] assemblies = [Assembly.GetExecutingAssembly()];
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

// Configure FluentValidation and ValidationPipeline
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
// Register FocusCore assembly containing validators with FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(CreateUserValidator)));

// Register DbContext
builder.Services.AddDbContext<FocusContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

var app = builder.Build();

// Instantiate the DbContext so that the database is created if it doesn't exist
_ = app.Services.GetService<FocusContext>();

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
