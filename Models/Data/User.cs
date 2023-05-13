using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class User
{

    public int IdUser { get; set; }

    public string? Name { get; set; }

    public bool? State { get; set; } = true;

    public DateTime? BirthDate { get; set; }

    public int? IdRh { get; set; }

    public virtual ICollection<Access>? Accesses { get; set; }

    public virtual ICollection<Address>? Addresses { get; set; }

    public virtual ICollection<Adoption>? Adoptions { get; set; }

    public virtual ICollection<AppointmentUser>? AppointmentUsers { get; set; }

    public virtual ICollection<Comment>? Comments { get; set; }

    public virtual Rh? IdRhNavigation { get; set; }

    public virtual ICollection<Phone>? Phones { get; set; } = null;

    public virtual ICollection<Publication>? Publications { get; set; }
}
