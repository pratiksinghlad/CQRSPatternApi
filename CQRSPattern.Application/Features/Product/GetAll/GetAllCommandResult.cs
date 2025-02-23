namespace CQRSPattern.Application.Features.Product.GetAll;

public class GetAllCommandResult
{
    /// <summary>
    /// Result of queried workitems.
    /// </summary>
    public List<ProductModel> Data { get; set; }
}
