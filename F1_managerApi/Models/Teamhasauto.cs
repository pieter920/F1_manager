using System;
using System.Collections.Generic;

namespace F1_managerApi.Models;

public partial class Teamhasauto
{
    public int IdteamHasAuto { get; set; }

    public int Fkauto { get; set; }

    public int Fkteam { get; set; }

    public virtual Auto FkautoNavigation { get; set; } = null!;

    public virtual Team FkteamNavigation { get; set; } = null!;
}
