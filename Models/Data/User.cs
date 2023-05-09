using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class User
{
    public int IdUser { get; set; }

    public string? Name { get; set; }

    public bool? State { get; set; }

    public DateTime? BirthDate { get; set; }

    public int? IdRh { get; set; }

    public virtual ICollection<Access> Accesses { get; set; } = new List<Access>();

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<Adoption> Adoptions { get; set; } = new List<Adoption>();

    public virtual ICollection<AppointmentUser> AppointmentUsers { get; set; } = new List<AppointmentUser>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Rh? IdRhNavigation { get; set; }

    public virtual ICollection<Phone> Phones { get; set; } = new List<Phone>();

    public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();
}
