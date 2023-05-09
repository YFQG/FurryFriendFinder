using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Inventory
{
    public int IdInventory { get; set; }

    public int? Quantity { get; set; }

    public int IdProduct { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;

    public virtual ICollection<Movement> Movements { get; set; } = new List<Movement>();
}
