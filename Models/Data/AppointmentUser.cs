using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class AppointmentUser
{
    public int IdAppointmentUser { get; set; }

    public int? IdPet { get; set; }

    public int IdUser { get; set; }

    public int IdAppointment { get; set; }

    public virtual Appointment? IdAppointmentNavigation { get; set; } = null!;

    public virtual Pet? IdPetNavigation { get; set; }

    public virtual User? IdUserNavigation { get; set; } = null!;
}
