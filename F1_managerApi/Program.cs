using F1_managerApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "Server=localhost;Database=f1_manager;User=root;Password=1234;";

builder.Services.AddDbContext<F1_ManagerDbContext>(options =>

options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//get all drivers
app.MapGet("/Driver", async (F1_ManagerDbContext db) =>
{
    var items = await db.Drivers.ToListAsync();
    return Results.Ok(items);
});


app.Run();
