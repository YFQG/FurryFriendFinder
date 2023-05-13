using FurryFriendFinder.Models.Data;

namespace FurryFriendFinder.Models.ViewModels
{
    public class AppointDate
    {
        public Appointment Appointment { get; set; }
        public AppointmentUser appointmentUser { get; set; }
        public User user { get; set; } = new();
        public Pet pet { get; set; } = new();
        public AppointDate(Appointment appointment, AppointmentUser appointmentUser)
        {
            this.Appointment = appointment;
            this.appointmentUser = appointmentUser;
        }
        public AppointDate(Appointment appointment, AppointmentUser appointmentUser, Pet pet, User user)
        {
            this.Appointment = appointment;
            this.appointmentUser = appointmentUser;
            this.pet = pet;
            this.user = user;
        }
    }
}
