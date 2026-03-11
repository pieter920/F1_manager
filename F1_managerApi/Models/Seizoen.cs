using System;
using System.Collections.Generic;

namespace F1_managerApi.Models;

public partial class Seizoen
{
    public int Idseizoen { get; set; }

    public string NaamSeizoen { get; set; } = null!;

    public DateOnly BeginDatum { get; set; }

    public DateOnly EindDatum { get; set; }

    public virtual ICollection<Raceweekend> Raceweekends { get; set; } = new List<Raceweekend>();

    public virtual ICollection<Teamhasseizoen> Teamhasseizoens { get; set; } = new List<Teamhasseizoen>();
}
