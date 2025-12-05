using BarberShopManagementSystem.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarberShopManagementSystem.Data.Entities
{
    public class Service: ShopBaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DurationInMinutes { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
