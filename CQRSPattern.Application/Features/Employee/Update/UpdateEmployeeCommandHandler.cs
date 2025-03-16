using CQRSPattern.Application.Repositories.Write;
using MediatR;

namespace CQRSPattern.Application.Features.Employee.Update;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Unit>
{
    public UpdateEmployeeCommandHandler(IEmployeeWriteRepository employeeRepo)
    {
        _employeeRepo = employeeRepo;
    }

    public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        EmployeeModel employee = new()
        {
            Id = request.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            BirthDate = request.BirthDate,
            HireDate = request.HireDate,
        };

        await _employeeRepo.UpdateAsync(employee, cancellationToken);

        return Unit.Value;
    }

    private readonly IEmployeeWriteRepository _employeeRepo;
}