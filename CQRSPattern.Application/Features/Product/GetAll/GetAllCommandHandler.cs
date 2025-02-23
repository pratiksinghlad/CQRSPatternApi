using MediatR;

namespace CQRSPattern.Application.Features.Product.GetAll;

public class GetAllCommandHandler : IRequestHandler<GetAllCommand, GetAllCommandResult>
{
    public GetAllCommandHandler()
    {
    }

    public async Task<GetAllCommandResult> Handle(GetAllCommand request, CancellationToken token)
    {
        var result = new List<ProductModel>();
        result.Add(new ProductModel { Id = 1 });
        result.Add(new ProductModel { Id = 2 });


        return new GetAllCommandResult { Data = result };
    }
}