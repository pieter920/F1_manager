using System;
using System.Collections.Generic;

namespace F1_managerApi.Models;

public partial class Teamhasseizoen
{
    public int IdteamHasSeizoen { get; set; }

    public int Fkseizoen { get; set; }

    public int Fkteam { get; set; }

    public virtual Seizoen FkseizoenNavigation { get; set; } = null!;

    public virtual Team FkteamNavigation { get; set; } = null!;
}
