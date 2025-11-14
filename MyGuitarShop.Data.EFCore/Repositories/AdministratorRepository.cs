using MyGuitarShop.Data.EFCore.Abstract;
using MyGuitarShop.Data.EFCore.Data;
using MyGuitarShop.Data.EFCore.Entities;

namespace MyGuitarShop.Data.EFCore.Repositories
{
    public class AdministratorRepository(MyGuitarShopContext dbContext) : RepositoryBase<Administrator>(dbContext) {
    }
}
