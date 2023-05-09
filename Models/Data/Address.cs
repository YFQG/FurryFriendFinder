using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Address
{
    public int IdAddress { get; set; }

    public string? Address1 { get; set; }

    public int IdUser { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
