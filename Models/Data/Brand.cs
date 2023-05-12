using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Brand
{
    public int IdBrand { get; set; }

    public string? NameBrand { get; set; }

    public virtual ICollection<Product>? Products { get; set; } = new List<Product>();
}
