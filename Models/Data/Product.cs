using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Product
{
    public int IdProduct { get; set; }

    public string? ProductName { get; set; }

    public int IdAnimalType { get; set; }

    public int IdPacking { get; set; }

    public int IdBrand { get; set; }

    public virtual AnimalType? IdAnimalTypeNavigation { get; set; } = null!;

    public virtual Brand? IdBrandNavigation { get; set; } = null!;

    public virtual Packing? IdPackingNavigation { get; set; } = null!;

    public virtual ICollection<Inventory>? Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Movement>? Movements { get; set; } = new List<Movement>();
}
