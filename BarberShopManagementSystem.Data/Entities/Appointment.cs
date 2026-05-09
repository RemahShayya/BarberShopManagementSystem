using BarberShopManagementSystem.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarberShopManagementSystem.Data.Entities
{
    public class Appointment : ShopBaseEntity
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CustomerId { get; set; }
        public User Customer { get; set; }
        public string BarberId { get; set; }
        public User Barber { get; set; }

        public Guid ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
