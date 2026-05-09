using BarberShopManagementSystem.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShopManagementSystem.Data.Entities
{
    public class BarberSchedule : ShopBaseEntity
    {
        public User Barber { get; set; }
        public string BarberId { get; set; }
        public DateTime Day { get; set; }
        public TimeSpan? StartHour { get; set; }
        public TimeSpan? EndHour { get; set; }
        public bool IsDayOff { get; set; } = false;
        [NotMapped]
        public DayOfWeek DayOfWeek => Day.DayOfWeek;
    }
}
