using Microsoft.EntityFrameworkCore;
using MiniTicketSystem.Application.Interfaces;
using MiniTicketSystem.Application.Services;
using MiniTicketSystem.Infrastructure.Data;
using MiniTicketSystem.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketQueryService, TicketQueryService>();
builder.Services.AddDbContext<TicketDbContext>(options =>
options.UseInMemoryDatabase("InMem"));

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<TicketDbContext>();

    await TicketDbSeeder.SeedDataAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
