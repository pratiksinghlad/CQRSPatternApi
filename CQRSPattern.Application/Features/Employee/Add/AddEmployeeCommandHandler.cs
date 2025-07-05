using CQRSPattern.Application.Repositories.Write;
using MediatR;

namespace CQRSPattern.Application.Features.Employee.Add;

public class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, Unit>
{
    public AddEmployeeCommandHandler(IEmployeeWriteRepository employeeRepo)
    {
        _employeeRepo = employeeRepo;
    }

    public async Task<Unit> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
    {
        EmployeeModel employee = new ()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            BirthDate = request.BirthDate,
            HireDate = request.HireDate,
        };

        await _employeeRepo.AddAsync(employee, cancellationToken);

        return Unit.Value;
    }

    private readonly IEmployeeWriteRepository _employeeRepo;
}
