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
//basic get request
//get all drivers
app.MapGet("get/Drivers", async (F1_ManagerDbContext db) =>
{
    var items = await db.Drivers.ToListAsync();
    return Results.Ok(items);
});
//get all teams
app.MapGet("get/Teams", async (F1_ManagerDbContext db) =>
{
    var items = await db.Teams.ToListAsync();
    return Results.Ok(items);
});
//get all seasons
app.MapGet("get/seasons", async (F1_ManagerDbContext db) =>
{
    var items = await db.Seizoens.ToListAsync();
    return Results.Ok(items);
});
//get all raceweekends for season
app.MapGet("get/raceweekends/per/season", async (int seasonId, F1_ManagerDbContext db) =>
{
    var items = await db.Raceweekends.Where(pbl => pbl.Fkseizoen == seasonId).ToListAsync();
    return Results.Ok(items);
});
//get all raceweekends for track
app.MapGet("get/raceweekends/per/track", async (int TrackID, F1_ManagerDbContext db) =>
{
    var items = await db.Raceweekends.Where(pbl => pbl.Fktrack == TrackID).ToListAsync();
    return Results.Ok(items);
});
//get all auto's
app.MapGet("get/auto's", async (F1_ManagerDbContext db) =>
{
    var items = await db.Autos.ToListAsync();
    return Results.Ok(items);
});
//get all track's
app.MapGet("get/Track's", async (F1_ManagerDbContext db) =>
{
    var items = await db.Tracks.ToListAsync();
    return Results.Ok(items);
});
//get track by id
app.MapGet("get/track/per/ID", async (int TrackID, F1_ManagerDbContext db) =>
{
    var items = await db.Tracks.Where(pbl => pbl.Idtrack == TrackID).ToListAsync();
    return Results.Ok(items);
});

//user checks and register
//check
app.MapGet("/user/check", async (string username, string password, F1_ManagerDbContext db) =>
{
    var user = await db.Users
        .FirstOrDefaultAsync(u => u.NameUser == username && u.PassWordUser == password);

    if (user == null)
        return Results.Unauthorized();

    return Results.Ok(new
    {
        UserId = user.IdUser,
        Username = user.NameUser
    });
});
//register
app.MapPost("/user/register", async (string username, string password, F1_ManagerDbContext db) =>
{
    var exists = await db.Users
    .AnyAsync(pbl => pbl.NameUser == username);
    if (exists)
        return Results.Conflict("Acount already in F1_manager");
    //zet in databank
    var User = new User { NameUser = username, PassWordUser = password };

    db.Users.Add(User);
    await db.SaveChangesAsync();

    return Results.Created($"/user_register", User);
});

//filter on user ID


app.Run();