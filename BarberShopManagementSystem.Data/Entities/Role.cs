using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarberShopManagementSystem.Data.Entities
{
    public class Role: IdentityRole<string>
    {
        public string Description { get; set; }
    }
}
