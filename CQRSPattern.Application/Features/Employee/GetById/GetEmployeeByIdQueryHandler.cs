using CQRSPattern.Application.Repositories.Read;
using MediatR;

namespace CQRSPattern.Application.Features.Employee.GetById;

/// <summary>
/// Handles retrieval of a single employee by identifier.
/// </summary>
public sealed class GetEmployeeByIdQueryHandler
    : IRequestHandler<GetEmployeeByIdQuery, GetEmployeeByIdQueryResult>
{
    private readonly IEmployeeReadRepository _employeeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmployeeByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="employeeRepository">The employee read repository.</param>
    public GetEmployeeByIdQueryHandler(IEmployeeReadRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    /// <inheritdoc />
    public async Task<GetEmployeeByIdQueryResult> Handle(
        GetEmployeeByIdQuery request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id, cancellationToken);

        return new GetEmployeeByIdQueryResult { Data = employee };
    }
}