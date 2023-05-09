using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class AnimalType
{
    public int IdAnimalType { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
