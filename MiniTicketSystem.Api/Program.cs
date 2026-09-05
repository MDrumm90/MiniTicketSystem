using Microsoft.EntityFrameworkCore;
using MiniTicketSystem.Application.Interfaces;
using MiniTicketSystem.Infrastructure.Data;
using MiniTicketSystem.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

var app = builder.Build();
builder.Services.AddDbContext<TicketDbContext>(options =>
    options.UseInMemoryDatabase("InMem"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
