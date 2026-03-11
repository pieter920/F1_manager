using System;
using System.Collections.Generic;

namespace F1_managerApi.Models;

public partial class Auto
{
    public int Idauto { get; set; }

    public int PresatieAuto { get; set; }

    public string NaamAuto { get; set; } = null!;

    public virtual ICollection<Teamhasauto> Teamhasautos { get; set; } = new List<Teamhasauto>();
}
