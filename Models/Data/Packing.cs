using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Packing
{
    public int IdPacking { get; set; }

    public string? TypePacking { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
