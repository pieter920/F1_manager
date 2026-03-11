using System;
using System.Collections.Generic;

namespace F1_managerApi.Models;

public partial class Raceweekendhasdriver
{
    public int IdraceWeekendHasDriver { get; set; }

    public int Fkdriver { get; set; }

    public int FkraceWeekend { get; set; }

    public int Positie { get; set; }

    public int Punten { get; set; }

    public virtual Driver FkdriverNavigation { get; set; } = null!;

    public virtual Raceweekend FkraceWeekendNavigation { get; set; } = null!;
}
