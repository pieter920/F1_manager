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
app.MapGet("get Drivers", async (F1_ManagerDbContext db) =>
{
    var items = await db.Drivers.ToListAsync();
    return Results.Ok(items);
});
//get all teams
app.MapGet("get Teams", async (F1_ManagerDbContext db) =>
{
    var items = await db.Teams.ToListAsync();
    return Results.Ok(items);
});
//get all seasons
app.MapGet("get seasons", async (F1_ManagerDbContext db) =>
{
    var items = await db.Seizoens.ToListAsync();
    return Results.Ok(items);
});
//get all raceweekends for season
app.MapGet("get raceweekends per season", async (int seasonId, F1_ManagerDbContext db) =>
{
    var items = await db.Raceweekends.Where(pbl => pbl.Fkseizoen == seasonId).ToListAsync();
    return Results.Ok(items);
});
//get all raceweekends for track
app.MapGet("get raceweekends per track", async (int TrackID, F1_ManagerDbContext db) =>
{
    var items = await db.Raceweekends.Where(pbl => pbl.Fktrack == TrackID).ToListAsync();
    return Results.Ok(items);
});
//get all auto's
app.MapGet("get auto's", async (F1_ManagerDbContext db) =>
{
    var items = await db.Autos.ToListAsync();
    return Results.Ok(items);
});

app.Run();