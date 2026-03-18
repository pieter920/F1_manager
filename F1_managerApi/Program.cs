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
#region simple get request

app.MapGet("get/Drivers", async (F1_ManagerDbContext db) =>
{
    var items = await db.Drivers.ToListAsync();
    return Results.Ok(items);
});
//get first 10 teams
app.MapGet("get/10/Teams", async (F1_ManagerDbContext db) =>
{
    var items = await db.Teams
    .Where(pbl => pbl.Idteam <= 10)
    .Select(t => t.NaamTeam)
    .ToListAsync();
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

#endregion
//user checks and register
//check
#region user checks and register

app.MapGet("/user/check", async (string username, string password, F1_ManagerDbContext db) =>
{
    var user = await db.Users
        .FirstOrDefaultAsync(u => u.NameUser == username && u.PassWordUser == password);

    if (user == null)
        return Results.Unauthorized();

    return Results.Ok(new
    {
        UserId = user.Iduser,
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

    return Results.Created($"/user/register", User);
});

#endregion
//filter on user ID
//get all teams from user
#region get stuf based on user

app.MapGet("get/Teams/from/user", async (int IDUser, F1_ManagerDbContext db) =>
{
    List<string> naamTeams = await db.Teams
    .Where(t => db.Users
        .Any(u => u.Iduser == IDUser && u.Fkteam == t.Idteam))
    .Select(t => t.NaamTeam)
    .ToListAsync();

    return naamTeams;
});
//get if FKTeam is empty for user
app.MapGet("get/empty/team/from/user", async (int IDUser, F1_ManagerDbContext db) =>
{
    var user = await db.Users
        .Where(u => u.Iduser == IDUser)
        .FirstOrDefaultAsync();

    if (user == null)
        return Results.NotFound("User not found");

    bool hasNoTeam = user.Fkteam == null;
    return Results.Ok(hasNoTeam);
});
//get ID from Username
app.MapGet("get/ID/from/username", async (string username, F1_ManagerDbContext db) =>
{
    var user = await db.Users
        .Where(u => u.NameUser == username)
        .FirstOrDefaultAsync();
    if (user == null)
        return Results.NotFound("User not found");
    return Results.Ok(user.Iduser);
});
//create team for user
app.MapPost("/Create/Team", async (string NaamTeam, string NationaliteitTeam,int UserID, F1_ManagerDbContext db) =>
{
    var exists = await db.Teams
    .Where(ID => ID.Idteam <= 10)
    .AnyAsync(pbl => pbl.NaamTeam == NaamTeam);
    if (exists)
        return Results.Conflict("Team already exists F1_manager");
    //zet in databank
    var Team = new Team { NaamTeam = NaamTeam, NationaliteitTeam = NationaliteitTeam };

    db.Teams.Add(Team);
    await db.SaveChangesAsync();

    var user = await db.Users.FirstOrDefaultAsync(u => u.Iduser == UserID);
    if (user == null)
        return Results.NotFound("User not found");
    user.Fkteam = Team.Idteam; 
    await db.SaveChangesAsync();

    return Results.Created($"/Create/Team", Team);
});

#endregion

app.Run();