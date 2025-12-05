using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarberShopManagementSystem.Data.Entities
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }

        public ICollection<Appointment> CustomerAppointments { get; set; } = new List<Appointment>();
        public ICollection<Appointment> BarberAppointments { get; set; } = new List<Appointment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<BarberSchedule> BarberSchedules { get; set; } = new List<BarberSchedule>();
    }
}
