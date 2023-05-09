using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Phone
{
    public int IdPhone { get; set; }

    public long? Phone1 { get; set; }

    public int? IdUser { get; set; }

    public virtual User? IdUserNavigation { get; set; }
}
