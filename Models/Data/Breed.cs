using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Breed
{
    public int IdBreed { get; set; }

    public string? Breed1 { get; set; }

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}
