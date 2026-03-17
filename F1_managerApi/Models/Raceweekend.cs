using System;
using System.Collections.Generic;

namespace F1_managerApi.Models;

public partial class Raceweekend
{
    public int IdraceWeekend { get; set; }

    public DateOnly BeginDatum { get; set; }

    public DateOnly EindDatum { get; set; }

    public int Fkseizoen { get; set; }

    public int Fktrack { get; set; }

    public int? Fkuser { get; set; }

    public virtual Seizoen FkseizoenNavigation { get; set; } = null!;

    public virtual Track FktrackNavigation { get; set; } = null!;

    public virtual User? FkuserNavigation { get; set; }

    public virtual ICollection<Raceweekendhasdriver> Raceweekendhasdrivers { get; set; } = new List<Raceweekendhasdriver>();
}
