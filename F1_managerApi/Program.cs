using F1_managerApi.Models;
using Microsoft.AspNetCore.Mvc;
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
var random = new Random();
app.UseHttpsRedirection();
//basic get request
#region simple get request
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
        Username = user.NameUser,
        TeamId = user.Fkteam
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

    return Results.Created($"/user/register", new { UserId = User.Iduser, username = User.NameUser });
});

#endregion
//filter on user ID
#region get stuf based on user
//get all teams from user - WHY MULTIPLE TEAMS?
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
app.MapPost("/Create/Team", async (string NaamTeam, string NationaliteitTeam, int UserID, F1_ManagerDbContext db) =>
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

    return Results.Created("/Create/Team", new
    {
        TeamId = Team.Idteam,
        TeamName = Team.NaamTeam,
        Nationality = Team.NationaliteitTeam
    });
});

#endregion
//Raceweekends API
#region RaceWeekens
//get raceweekends for user
app.MapGet("get/completed/raceweekends/from/user", async (int IDUser, F1_ManagerDbContext db) =>
{
    int CompletedRaceWeekends = await db.Raceweekends
    .Where(rw => rw.Fkuser == IDUser && rw.Completed == 1).CountAsync();
    return CompletedRaceWeekends;
});
//app.MapGet("get/completed/raceweekend/ids/from/user", async (int IDUser, F1_ManagerDbContext db) =>
//{
//    List<int> CompletedRaceWeekendIds = await db.Raceweekends
//    .Where(rw => rw.Fkuser == IDUser && rw.Completed == 1).Select(rw => rw.IdraceWeekend).ToListAsync();
//    return CompletedRaceWeekendIds;
//});
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
app.MapGet("get/raceweekends/by/User/ID/and/season", async (int IDUser, string SeasonName, F1_ManagerDbContext db) =>
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
#region create driver
//Create driver for user
app.MapPost("Create/Driver", async (string VoorNaamDriver, string AchterNaamDriver, string NationaliteitDriver, int Leeftijd, int TeamID, int ratingDriver, F1_ManagerDbContext db) =>
{
    var exists = await db.Drivers
    .Where(ID => ID.Iddriver <= 20)
    .AnyAsync(pbl => pbl.VoornaamDriver == VoorNaamDriver);
    if (exists)
        return Results.Conflict("Driver already exists in real Life");
    //zet in databank
    var Driver = new Driver
    {
        VoornaamDriver = VoorNaamDriver,
        AchternaamDriver = AchterNaamDriver,
        NationaliteitDriver = NationaliteitDriver,
        LeeftijdDriver = Leeftijd,
        Rating = ratingDriver,
        Fkteam = TeamID,
        Confidence = 80
    };
    db.Drivers.Add(Driver);
    await db.SaveChangesAsync();
    return Results.Created($"/Create/Driver", Driver);
});
//get Team by user ID
app.MapGet("get/Team/from/userID", async (int IDUser, F1_ManagerDbContext db) =>
{
    var team = await GetTeamFromUser(IDUser, db);
    if (team == null)
        return Results.NotFound("Team not found for the user");
    return Results.Ok(new { teamID = team.Idteam, teamName = team.NaamTeam, Nationality = team.NationaliteitTeam });
});
//create Auto for user
app.MapPost("/Create/Auto", async (int IDTeam, F1_ManagerDbContext db) =>
{
    var AutoNames = new string[]
    {
        "DS2025",
        "BS2005",
        "MX5000",
        "VEE950",
        "GTS258",
        "CPBR20",
        "LB7FA2",
        "PIG821",
        "AEZ889",
        "FNP771"
    };
    var Auto = new Auto
    {
        NaamAuto = AutoNames[random.Next(AutoNames.Length)],
        PresatieAuto = random.Next(75, 81),
        Fkteam = IDTeam
    };
    db.Autos.Add(Auto);
    await db.SaveChangesAsync();
    return Auto;
});
#endregion
#region simulate raceweekend
//Simulate
app.MapGet("simulate/raceweekend", async (int IDUser, F1_ManagerDbContext db) =>
{
    var raceweekend = await db.Raceweekends
        .Where(rw => rw.Fkuser == IDUser && rw.Completed == 0)
        .FirstOrDefaultAsync();

    var teamUser = await GetTeamFromUser(IDUser, db);
    //get auto prestatie
    var IdAuto = await GetAutoFromUserTeam(teamUser.Idteam, db);
    var PrestatieAuto = IdAuto.PresatieAuto;



    if (raceweekend == null || teamUser == null)
        return Results.NotFound("No raceweekend found");

    var userDrivers = await GetDriversFromUserTeam(teamUser.Idteam, db);
    var DefaultDrivers = await GetAllDefaultDrivers(db);

    var rankedDrivers = DefaultDrivers
        .Concat(userDrivers)
        .OrderByDescending(d => d.Rating + d.Confidence + PrestatieAuto + random.Next(50, 80))
        .ToList();

    var PuntenVerdeling = new[] { 25, 18, 15, 12, 10, 8, 6, 4, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    var results = rankedDrivers.Select((d, index) => new RaceResult(
        Position: index + 1,
        DriverId: d.Iddriver,
        Name: $"{d.VoornaamDriver} {d.AchternaamDriver}",
        Team: d.FkteamNavigation.NaamTeam,
        Punten: PuntenVerdeling[index]
    )).ToList();

    await SaveRaceResults(results, raceweekend.IdraceWeekend, db);
    await CompleteRaceWeekend(raceweekend, db);
    await UpdateDriverRatings(userDrivers, db);
    await UpdateDriverConfidence(userDrivers, db);
    await UpdateAutoRatings(IdAuto, db);
    return Results.Ok(results);
});

app.MapGet("get/track/by/user", async (int IDUser, F1_ManagerDbContext db) =>
{
    var raceWeekend = await GetNextRaceWeekendForUser(IDUser, db);
    if (raceWeekend == null) return Results.NotFound();

    var track = await db.Tracks.FindAsync(raceWeekend.Fktrack);
    if (track == null) return Results.NotFound();

    return Results.Ok(new { track.NaamTrack, track.LandTrack, track.LapsTrack, raceWeekend.BeginDatum, raceWeekend.EindDatum });
});
app.MapGet("get/Previous/track/by/user", async (int IDUser, F1_ManagerDbContext db) =>
{
    var raceWeekend = await GetPreviousRaceWeekendForUser(IDUser, db);
    if (raceWeekend == null) return Results.NotFound();

    var track = await db.Tracks.FindAsync(raceWeekend.Fktrack);
    if (track == null) return Results.NotFound();

    return Results.Ok(new { track.NaamTrack, track.LandTrack, track.LapsTrack, raceWeekend.BeginDatum, raceWeekend.EindDatum });
});
app.MapGet("get/driver/standings", async (int IDUser, F1_ManagerDbContext db) =>
{
    var standings = await GetDriverStandingsForSeason(IDUser, db);
    return Results.Ok(standings);
});
app.MapGet("get/constructor/standings", async (int IDUser, F1_ManagerDbContext db) =>
{
    var standings = await GetConstructorStandingsForSeason(IDUser, db);
    return Results.Ok(standings);
});

app.MapGet("Drivers/{TeamId}", async (int TeamId, F1_ManagerDbContext db) =>
{
    var drivers = await GetDriversFromUserTeam(TeamId, db);
    return Results.Ok(drivers);
});
app.MapGet("Auto/{TeamId}", async (int TeamId, F1_ManagerDbContext db) =>
{
    var drivers = await GetAutoFromUserTeam(TeamId, db);
    return Results.Ok(drivers);
});
app.MapGet("get/raceweekend/result/by/user", async (int IDUser, F1_ManagerDbContext db) =>
{
    var raceWeekend = await GetLatestRaceWeekendHasDriverResult(IDUser, db);
    if (raceWeekend == null) return Results.NotFound();

    var results = raceWeekend.Select((d, index) => new RaceResult(
    Position: d.Positie,
    DriverId: d.Fkdriver,
    Name: $"{d.FkdriverNavigation.VoornaamDriver} {d.FkdriverNavigation.AchternaamDriver}",
    Team: d.FkdriverNavigation.FkteamNavigation.NaamTeam,
    Punten: d.Punten
)).ToList();

    return Results.Ok(results);
});
#endregion
#region static functions
static async Task<List<Raceweekendhasdriver>> GetLatestRaceWeekendHasDriverResult(int IDUser, F1_ManagerDbContext db)
{
    int raceWeekendId = await db.Raceweekends
        .Where(rw => rw.Fkuser == IDUser && rw.Completed == 1)
        .OrderBy(rw => rw.IdraceWeekend)
        .Select(rw => rw.IdraceWeekend)
        .LastOrDefaultAsync();

    return await db.Raceweekendhasdrivers
        .Where(x => x.FkraceWeekend == raceWeekendId)
        .Include(d => d.FkdriverNavigation).ThenInclude(t => t.FkteamNavigation)
        .ToListAsync();
}
static async Task<Raceweekend?> GetPreviousRaceWeekendForUser(int IDUser, F1_ManagerDbContext db)
{
    return await db.Raceweekends
        .Where(rw => rw.Fkuser == IDUser && rw.Completed == 1)
        .OrderBy(rw => rw.IdraceWeekend)
        .LastOrDefaultAsync();
}
static async Task<Raceweekend?> GetNextRaceWeekendForUser(int IDUser, F1_ManagerDbContext db)
{
    return await db.Raceweekends
        .Where(rw => rw.Fkuser == IDUser && rw.Completed == 0)
        .OrderBy(rw => rw.BeginDatum)
        .FirstOrDefaultAsync();
}
static async Task<Team?> GetTeamFromUser(int IDUser, F1_ManagerDbContext db)
{
    var user = await db.Users
        .Where(u => u.Iduser == IDUser)
        .FirstOrDefaultAsync();

    if (user == null || user.Fkteam == null)
        return null;

    return await db.Teams
        .Where(t => t.Idteam == user.Fkteam)
        .FirstOrDefaultAsync();
}
static async Task<List<Driver>> GetDriversFromUserTeam(int IDTeam, F1_ManagerDbContext db)
{
    return await db.Drivers
        .Where(d => d.Fkteam == IDTeam)
        .ToListAsync();
}

static async Task<Auto> GetAutoFromUserTeam(int IDTeam, F1_ManagerDbContext db)
{
    return await db.Autos
        .Where(d => d.Fkteam == IDTeam)
        .FirstOrDefaultAsync();
}

static async Task<List<Driver>> GetAllDefaultDrivers(F1_ManagerDbContext db)
{
    return await db.Drivers
        .Where(d => d.Fkteam <= 10)
        .Include(d => d.FkteamNavigation)
        .ToListAsync();
}
static async Task SaveRaceResults(List<RaceResult> results, int raceweekendID, F1_ManagerDbContext db)
{
    foreach (var result in results)
    {
        var raceWeekendHasDriver = new Raceweekendhasdriver
        {
            Fkdriver = result.DriverId,
            FkraceWeekend = raceweekendID,
            Positie = result.Position,
            Punten = result.Punten
        };
        db.Raceweekendhasdrivers.Add(raceWeekendHasDriver);
    }
    await db.SaveChangesAsync();
}
static async Task CompleteRaceWeekend(Raceweekend raceweekend, F1_ManagerDbContext db)
{
    raceweekend.Completed = 1;
    await db.SaveChangesAsync();
}

static async Task<List<DriverStanding>> GetDriverStandingsForSeason(int IDUser, F1_ManagerDbContext db)
{
    List<int> CompletedRaceWeekendIds = await db.Raceweekends
    .Where(rw => rw.Fkuser == IDUser && rw.Completed == 1).Select(rw => rw.IdraceWeekend).ToListAsync();

    var rwhd = await db.Raceweekendhasdrivers
        .Include(rwhd => rwhd.FkraceWeekendNavigation)
        .Include(rwhd => rwhd.FkdriverNavigation)
            .ThenInclude(d => d.FkteamNavigation)
        .Where(rwhd => CompletedRaceWeekendIds.Contains(rwhd.FkraceWeekend))
        .ToListAsync();

    var orderedDrivers = rwhd
    .GroupBy(rwhd => rwhd.FkdriverNavigation)
    .Select(ds => new
    {
        DriverId = ds.Key.Iddriver,
        Naam = ds.Key.VoornaamDriver + " " + ds.Key.AchternaamDriver,
        Team = ds.Key.FkteamNavigation.NaamTeam,
        Punten = ds.Sum(x => x.Punten)
    })
    .OrderByDescending(d => d.Punten)
    .ToList();

    var standings = new List<DriverStanding>();

    int currentPosition = 0;
    int previousPoints = -1;

    for (int i = 0; i < orderedDrivers.Count; i++)
    {
        var driver = orderedDrivers[i];

        // Only update position when points change
        if (driver.Punten != previousPoints)
        {
            currentPosition = i + 1;
            previousPoints = driver.Punten;
        }

        standings.Add(new DriverStanding(
            currentPosition,
            driver.DriverId,
            driver.Naam,
            driver.Team,
            driver.Punten
        ));
    }

    return standings;
}

static async Task<List<ConstructorStanding>> GetConstructorStandingsForSeason(int IDUser, F1_ManagerDbContext db)
{
    List<int> completedRaceWeekendIds = await db.Raceweekends
        .Where(rw => rw.Fkuser == IDUser && rw.Completed == 1)
        .Select(rw => rw.IdraceWeekend)
        .ToListAsync();

    var rwhd = await db.Raceweekendhasdrivers
        .Include(rwhd => rwhd.FkdriverNavigation)
            .ThenInclude(d => d.FkteamNavigation)
        .Where(rwhd => completedRaceWeekendIds.Contains(rwhd.FkraceWeekend))
        .ToListAsync();

    var orderedConstructors = rwhd
        .GroupBy(rwhd => rwhd.FkdriverNavigation.FkteamNavigation)
        .Select(gs => new
        {
            TeamId = gs.Key.Idteam,
            TeamNaam = gs.Key.NaamTeam,
            Punten = gs.Sum(x => x.Punten),
            Nationality = gs.Key.NationaliteitTeam
        })
        .OrderByDescending(t => t.Punten)
        .ToList();

    var standings = new List<ConstructorStanding>();

    int currentPosition = 0;
    int previousPoints = -1;

    for (int i = 0; i < orderedConstructors.Count; i++)
    {
        var team = orderedConstructors[i];

        // Shared position for equal points
        if (team.Punten != previousPoints)
        {
            currentPosition = i + 1;
            previousPoints = team.Punten;
        }

        standings.Add(new ConstructorStanding(
            currentPosition,
            team.TeamId,
            team.TeamNaam,
            team.Punten,
            team.Nationality
        ));
    }

    return standings;
}

#region update ratings
static async Task UpdateDriverRatings(List<Driver> drivers, F1_ManagerDbContext db)
{
    var random = new Random();
    int randomValue = random.Next(100);
    foreach (var driver in drivers)
    {
        if (driver.LeeftijdDriver < 30)
        {
            if (randomValue < 45)
                driver.Rating = Math.Min(99, driver.Rating + 1);
        }
        else if (driver.LeeftijdDriver > 35)
        {
            if (randomValue < 45)
                driver.Rating = Math.Max(1, driver.Rating - 1);
        }
    }
    await db.SaveChangesAsync();
}

static async Task UpdateAutoRatings(Auto auto, F1_ManagerDbContext db)
{
    var random = new Random();
    int randomValue = random.Next(100);
    if (randomValue < 50)
        auto.PresatieAuto = Math.Min(99, auto.PresatieAuto + 1);
    await db.SaveChangesAsync();
}

static async Task UpdateDriverConfidence(List<Driver> drivers, F1_ManagerDbContext db)
{
    var random = new Random();
    int randomValue = random.Next(100);
    foreach (var driver in drivers)
    {
        if (randomValue < 55)
            driver.Confidence = Math.Min(99, driver.Confidence + 1);
        else
            driver.Confidence = Math.Max(1, driver.Confidence - 1);
    }
    await db.SaveChangesAsync();
}
#endregion
#endregion


app.Run();
record RaceResult(int Position, int DriverId, string Name, string Team, int Punten);
record DriverStanding(int position, int DriverId, string Naam, string Team, int Punten);
record ConstructorStanding(int position, int Team, string Naam, int Punten, string Nationaliteit);