using System;
using System.Collections.Generic;

namespace F1_managerApi.Models;

public partial class Track
{
    public int Idtrack { get; set; }

    public string NaamTrack { get; set; } = null!;

    public string LandTrack { get; set; } = null!;

    public int LapsTrack { get; set; }

    public virtual ICollection<Raceweekend> Raceweekends { get; set; } = new List<Raceweekend>();
}
