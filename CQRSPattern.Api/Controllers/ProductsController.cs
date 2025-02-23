using CQRSPattern.Application.Features.Product;
using CQRSPattern.Application.Features.Product.GetAll;
using CQRSPattern.Application.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CQRSPattern.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="factory"></param>
    public ProductsController(IMediatorFactory factory)
    {
        _factory = factory;
    }

    [HttpGet]
    public async Task<IEnumerable<ProductModel>> GetAsync(CancellationToken token)
    {
        var scope = _factory.CreateScope();
        var result = await scope.SendAsync(GetAllCommand.CreateCommand(), token);
        
        return result.Data;
    }

    private readonly IMediatorFactory _factory;
}