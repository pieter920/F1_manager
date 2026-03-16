using System;
using System.Collections.Generic;

namespace F1_managerApi.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string NameUser { get; set; } = null!;

    public string PassWordUser { get; set; } = null!;
}
