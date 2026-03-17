using System;
using System.Collections.Generic;

namespace F1_managerApi.Models;

public partial class Team
{
    public int Idteam { get; set; }

    public string NaamTeam { get; set; } = null!;

    public string NationaliteitTeam { get; set; } = null!;

    public virtual ICollection<Auto> Autos { get; set; } = new List<Auto>();

    public virtual ICollection<Teamhasdriver> Teamhasdrivers { get; set; } = new List<Teamhasdriver>();

    public virtual ICollection<Teamhasseizoen> Teamhasseizoens { get; set; } = new List<Teamhasseizoen>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
