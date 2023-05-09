using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class StateHealth
{
    public int IdStateHealth { get; set; }

    public string? State { get; set; }

    public bool? Castrated { get; set; }

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}
