using CQRSPattern.Application.Repositories.Read;
using MediatR;

namespace CQRSPattern.Application.Features.Employee.GetAll;

public class GetAllQueryHandler : IRequestHandler<GetAllQuery, GetAllQueryResult>
{
    public GetAllQueryHandler(IEmployeeReadRepository employeeRepo)
    {
        _employeeRepo = employeeRepo;
    }

    public async Task<GetAllQueryResult> Handle(
        GetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var result = await _employeeRepo.GetAllAsync(cancellationToken);

        return new GetAllQueryResult { Data = result };
    }

    private readonly IEmployeeReadRepository _employeeRepo;
}
