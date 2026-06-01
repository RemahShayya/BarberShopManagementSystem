using BarberShopManagementSystem.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarberShopManagementSystem.Data.Entities
{
    public class Profession : ShopBaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<EmployeeProfession> EmployeeProfessions { get; set; }
        public ICollection<Service> Services { get; set; }
    }
}
