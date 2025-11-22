using CST324_MyGuitarShop.Api.Abstract;
using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Data.EFCore.Entities;
using MyGuitarShop.Data.EFCore.Repositories;

namespace CST324_MyGuitarShop.Api.Controllers.EFCoreControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsEFCoreController(
        ILogger<ProductsEFCoreController> logger,
        ProductRepository repository) 
        : BaseController<ProductDto, Product>(logger, repository)
    {    }
}
