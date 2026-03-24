using System;
using System.Collections.Generic;

namespace F1_managerApi.Models;

public partial class Driver
{
    public int Iddriver { get; set; }

    public string VoornaamDriver { get; set; } = null!;

    public string AchternaamDriver { get; set; } = null!;

    public string NationaliteitDriver { get; set; } = null!;

    public int Confidence { get; set; }

    public int Rating { get; set; }

    public int LeeftijdDriver { get; set; }

    public int Fkteam { get; set; }

    public virtual Team FkteamNavigation { get; set; } = null!;

    public virtual ICollection<Raceweekendhasdriver> Raceweekendhasdrivers { get; set; } = new List<Raceweekendhasdriver>();
}
