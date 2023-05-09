using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Movement
{
    public int IdMovement { get; set; }

    public bool? MovementType { get; set; }

    public int? Quantity { get; set; }

    public DateTime? Date { get; set; }

    public int IdProduct { get; set; }

    public int? IdInventary { get; set; }

    public virtual Inventory? IdInventaryNavigation { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;
}
