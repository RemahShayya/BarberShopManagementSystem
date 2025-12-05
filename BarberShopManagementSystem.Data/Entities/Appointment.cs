using BarberShopManagementSystem.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarberShopManagementSystem.Data.Entities
{
    public class Appointment : ShopBaseEntity
    {
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentStart { get; set; }
        public DateTime AppointmentDuration { get; set; }
        public DateTime AppointmentEndTime { get; set; }
        public User Customer { get; set; }
        public string CustomerId { get; set; }
        public User Barber { get; set; }
        public string BarberId { get; set; }
        public Service Service { get; set; }
        public Guid ServiceId { get; set; }
        public Review Review { get; set; }
    }
}
