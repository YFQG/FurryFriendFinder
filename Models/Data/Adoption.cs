using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Adoption
{
    public int IdAdoption { get; set; }

    public int? IdPet { get; set; }

    public int? IdUser { get; set; }

    public int? IdAdoptionDate { get; set; }

    public virtual AdoptionDate? IdAdoptionDateNavigation { get; set; }

    public virtual Pet? IdPetNavigation { get; set; }

    public virtual User? IdUserNavigation { get; set; }
}
