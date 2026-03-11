using System;
using System.Collections.Generic;

namespace F1_managerApi.Models;

public partial class Teamhasdriver
{
    public int IdteamHasDriver { get; set; }

    public int Fkteam { get; set; }

    public int Fkdriver { get; set; }

    public DateOnly? BeginDatum { get; set; }

    public DateOnly EindDatum { get; set; }

    public virtual Driver FkdriverNavigation { get; set; } = null!;

    public virtual Team FkteamNavigation { get; set; } = null!;
}
