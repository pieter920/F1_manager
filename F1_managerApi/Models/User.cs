using System;
using System.Collections.Generic;

namespace F1_managerApi.Models;

public partial class User
{
    public int Iduser { get; set; }

    public string NameUser { get; set; } = null!;

    public string PassWordUser { get; set; } = null!;

    public int? Fkteam { get; set; }

    public virtual Team? FkteamNavigation { get; set; }

    public virtual ICollection<Raceweekend> Raceweekends { get; set; } = new List<Raceweekend>();

    public virtual ICollection<Seizoen> Seizoens { get; set; } = new List<Seizoen>();
}
