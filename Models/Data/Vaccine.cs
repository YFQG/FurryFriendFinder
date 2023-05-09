using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Vaccine
{
    public int IdVaccine { get; set; }

    public string? TypeVaccine { get; set; }

    public DateTime? VaccinationDate { get; set; }

    public int? IdPet { get; set; }

    public virtual Pet? IdPetNavigation { get; set; }
}
