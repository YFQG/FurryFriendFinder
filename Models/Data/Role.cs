using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Role
{
    public int IdRole { get; set; }

    public string RoleType { get; set; } = null!;

    public virtual ICollection<Access> Accesses { get; set; } = new List<Access>();
}
