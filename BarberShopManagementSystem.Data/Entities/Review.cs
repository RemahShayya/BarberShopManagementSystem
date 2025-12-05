using BarberShopManagementSystem.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BarberShopManagementSystem.Data.Entities
{
    public class Review : ShopBaseEntity
    {
        public Appointment Appointment { get; set; }
        public Guid AppointmentId { get; set; }
        public User Barber { get; set; }
        public string BarberId { get; set; }
        public User Customer { get; set; }
        public string CustomerId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
