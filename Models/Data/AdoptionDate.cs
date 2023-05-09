using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class AdoptionDate
{
    public int IdAdoptionDate { get; set; }

    public DateTime? RegisterAdoption { get; set; }

    public virtual ICollection<Adoption> Adoptions { get; set; } = new List<Adoption>();
}
