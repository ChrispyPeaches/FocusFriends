using FluentValidation;
using MediatR;
using System.Reflection;
using FocusAPI.Configuration.PipelineBehaviors;
using FocusCore.Validators.Users;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
