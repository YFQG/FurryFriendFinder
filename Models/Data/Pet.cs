using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Pet
{
    public int IdPet { get; set; }

    public byte[]? PetImage { get; set; }

    public string? PetName { get; set; }

    public string? Gender { get; set; }

    public int? BirthYear { get; set; }

    public int? IdAnimalType { get; set; }

    public int? IdStateHealth { get; set; }

    public int? IdBreed { get; set; }

    public virtual ICollection<Adoption>? Adoptions { get; set; } = new List<Adoption>();

    public virtual ICollection<AppointmentUser>? AppointmentUsers { get; set; } = new List<AppointmentUser>();

    public virtual AnimalType? IdAnimalTypeNavigation { get; set; }

    public virtual Breed? IdBreedNavigation { get; set; }

    public virtual StateHealth? IdStateHealthNavigation { get; set; }

    public virtual ICollection<Vaccine>? Vaccines { get; set; } = new List<Vaccine>();
}
