using F1_managerApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
#region simple get request
//get all drivers
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
#region user checks and register
//check
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
#region get stuf based on user
//get all teams from user
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
//Raceweekends API
#region RaceWeekens
//get raceweekends for user
app.MapGet("get/raceweekends/from/user", async (int IDUser, F1_ManagerDbContext db) =>
{
    List<int> naamRaceweekends = await db.Raceweekends
    .Where(rw => db.Users
        .Any(u => u.Iduser == IDUser && rw.Fkuser == u.Iduser))
    .Select(rw => rw.IdraceWeekend)
    .ToListAsync();
    return naamRaceweekends;
});
//get raceweekens by ID
app.MapGet("get/raceweekends/by/ID", async (int IdraceWeekend, F1_ManagerDbContext db) =>
{
    var result = await db.Raceweekends
        .Where(rw => rw.IdraceWeekend == IdraceWeekend)
        .Join(db.Tracks,
            rw => rw.Fktrack,
            t => t.Idtrack,
            (rw, t) => new
            {
                NaamTrack = t.NaamTrack,
                LapsTrack = t.LapsTrack,
                NationTrack = t.LandTrack
            })
        .FirstOrDefaultAsync();

    if (result == null)
        return Results.NotFound("Raceweekend not found");

    return Results.Ok(result);
});
//get raceweekens by user ID and season Name
app.MapGet("get/raceweekends/by/User/ID/and/season", async (int IDUser,string SeasonName, F1_ManagerDbContext db) =>
{
    var result = await db.Raceweekends
        .Join(db.Seizoens,
              rw => rw.Fkseizoen,
              s => s.Idseizoen,
              (rw, s) => new { rw, s })
        .Where(x => x.rw.Fkuser == IDUser && x.s.NaamSeizoen == SeasonName)
        .Select(x => x.rw)
        .ToListAsync();

    if (result == null)
        return Results.NotFound("No raceweekends found");

    return Results.Ok(result);
});

#endregion
#region create calendar
//create start calendar for user
app.MapPost("create/Eerste/calendar", async (int IDUser, int seasonID, F1_ManagerDbContext db) =>
{
    var raceweekends = new List<Raceweekend>
    {
        new Raceweekend { BeginDatum = new DateOnly(2025, 3, 14),  EindDatum = new DateOnly(2025, 3, 16), Fkseizoen = seasonID, Fktrack = 1,  Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 3, 21),  EindDatum = new DateOnly(2025, 3, 23), Fkseizoen = seasonID, Fktrack = 2,  Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 4, 4),   EindDatum = new DateOnly(2025, 4, 6), Fkseizoen = seasonID, Fktrack = 3,  Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 4, 11),  EindDatum = new DateOnly(2025, 4, 13), Fkseizoen = seasonID, Fktrack = 4,  Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 4, 18),  EindDatum = new DateOnly(2025, 4, 20), Fkseizoen = seasonID, Fktrack = 5,  Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 5, 2),   EindDatum = new DateOnly(2025, 5, 4), Fkseizoen = seasonID, Fktrack = 6,  Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 5, 16),  EindDatum = new DateOnly(2025, 5, 18), Fkseizoen = seasonID, Fktrack = 7,  Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 5, 23),  EindDatum = new DateOnly(2025, 5, 25), Fkseizoen = seasonID, Fktrack = 8,  Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 5, 30),  EindDatum = new DateOnly(2025, 6, 1), Fkseizoen = seasonID, Fktrack = 9,  Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 6, 13),  EindDatum = new DateOnly(2025, 6, 15), Fkseizoen = seasonID, Fktrack = 10, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 6, 27),  EindDatum = new DateOnly(2025, 6, 29), Fkseizoen = seasonID, Fktrack = 11, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 7, 4),   EindDatum = new DateOnly(2025, 7, 6), Fkseizoen = seasonID, Fktrack = 12, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 7, 25),  EindDatum = new DateOnly(2025, 7, 27), Fkseizoen = seasonID, Fktrack = 13, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 8, 1),   EindDatum = new DateOnly(2025, 8, 3), Fkseizoen = seasonID, Fktrack = 14, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 8, 29),  EindDatum = new DateOnly(2025, 8, 31), Fkseizoen = seasonID, Fktrack = 15, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 9, 5),   EindDatum = new DateOnly(2025, 9, 7), Fkseizoen = seasonID, Fktrack = 16, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 9, 19),  EindDatum = new DateOnly(2025, 9, 21), Fkseizoen = seasonID, Fktrack = 17, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 10, 3),  EindDatum = new DateOnly(2025, 10, 5), Fkseizoen = seasonID, Fktrack = 18, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 10, 17), EindDatum = new DateOnly(2025, 10, 19), Fkseizoen = seasonID, Fktrack = 19, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 10, 24), EindDatum = new DateOnly(2025, 10, 26), Fkseizoen = seasonID, Fktrack = 20, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 11, 7),  EindDatum = new DateOnly(2025, 11, 9), Fkseizoen = seasonID, Fktrack = 21, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 11, 20), EindDatum = new DateOnly(2025, 11, 22), Fkseizoen = seasonID, Fktrack = 22, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 11, 28), EindDatum = new DateOnly(2025, 11, 30), Fkseizoen = seasonID, Fktrack = 23, Fkuser = IDUser },
        new Raceweekend { BeginDatum = new DateOnly(2025, 12, 5),  EindDatum = new DateOnly(2025, 12, 7), Fkseizoen = seasonID, Fktrack = 24, Fkuser = IDUser },
    };
    await db.Raceweekends.AddRangeAsync(raceweekends);
    await db.SaveChangesAsync();
    return Results.Ok();
});
app.MapPost("create/Eerste/seizon", async (int IDUser, F1_ManagerDbContext db) =>
{
    var seizon = new Seizoen
    {
        NaamSeizoen = "Seizoen 2025",
        BeginDatum = new DateOnly(2024, 12, 13),
        EindDatum = new DateOnly(2025, 12, 12),
        Fkuser = IDUser
    };

    db.Seizoens.Add(seizon);
    await db.SaveChangesAsync();
    return Results.Ok();
});
//get ID from seizon
app.MapGet("get/ID/from/SeizoenName", async (string NaamSeizoen, F1_ManagerDbContext db) =>
{
    var Seizoen = await db.Seizoens
        .Where(u => u.NaamSeizoen == NaamSeizoen)
        .FirstOrDefaultAsync();
    if (Seizoen == null)
        return Results.NotFound("User not found");
    return Results.Ok(Seizoen.Idseizoen);
});
#endregion
app.Run();