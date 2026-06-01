using BarberShopManagementSystem.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarberShopManagementSystem.Data.Entities
{
    public class EmployeeProfession : ShopBaseEntity
    {
        public Profession Profession { get; set; }
        public Guid ProfessionId { get; set; }
        public User Employee { get; set; }
        public string EmployeeId { get; set; }
    }
}
