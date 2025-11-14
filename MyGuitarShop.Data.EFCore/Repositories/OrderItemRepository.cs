using MyGuitarShop.Data.EFCore.Abstract;
using MyGuitarShop.Data.EFCore.Data;
using MyGuitarShop.Data.EFCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGuitarShop.Data.EFCore.Repositories
{
    public class OrderItemRepository(MyGuitarShopContext dbContext) : RepositoryBase<OrderItem>(dbContext) { }
}
