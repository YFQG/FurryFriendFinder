using System;
using System.Collections.Generic;

namespace FurryFriendFinder.Models.Data;

public partial class Appointment
{
    public int IdAppointment { get; set; }

    public DateTime? Date { get; set; }

    public virtual ICollection<AppointmentUser> AppointmentUsers { get; set; } = new List<AppointmentUser>();
}
