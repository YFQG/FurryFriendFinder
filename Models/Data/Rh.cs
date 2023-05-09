using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Rh
{
    public int IdRh { get; set; }

    public string? RhType { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
