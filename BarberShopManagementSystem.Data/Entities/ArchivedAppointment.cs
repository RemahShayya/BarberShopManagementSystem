using BarberShopManagementSystem.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarberShopManagementSystem.Data.Entities
{
    public class ArchivedAppointment : ShopBaseEntity
    {
        public User Customer { get; set; }
        public string CustomerId { get; set; }
        public User Employee { get; set; }
        public string EmployeeId { get; set; }
        public Guid ServiceId { get; set; }
        public Service Service { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public Review? Review { get; set; }
        public DateTime ArchivedDate { get; set; }
        public string ReviewToken { get; set; }
        public bool ReviewEmailSent { get; set; }
    }
}
