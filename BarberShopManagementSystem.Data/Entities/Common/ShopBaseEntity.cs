using System;
using System.Collections.Generic;
using System.Text;

namespace BarberShopManagementSystem.Data.Entities.Common
{
    public abstract class ShopBaseEntity: IShopBaseEntity
    {
        public Guid Id { get; set; }
    }
}
