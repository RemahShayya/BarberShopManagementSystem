using BarberShopManagementSystem.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarberShopManagementSystem.Data.Entities
{
    public class BarberSchedule : ShopBaseEntity
    {
        public User Barber { get; set; }
        public string BarberId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartHour { get; set; }
        public TimeSpan EndHour { get; set; }
    }
        public enum DayOfWeek
        {
            Sunday,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday
        }
}
