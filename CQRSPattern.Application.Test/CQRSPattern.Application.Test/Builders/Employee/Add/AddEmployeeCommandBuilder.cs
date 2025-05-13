using CQRSPattern.Application.Features.Employee.Add;
using CQRSPattern.Shared.Test;

namespace CQRSPattern.Application.Test.Builders.Employee.GetAll;

public class AddEmployeeCommandBuilder : GenericBuilder<AddEmployeeCommand>
{
    public AddEmployeeCommandBuilder()
    {
        var employee = new EmployeeModelBuilder().Build();

        SetDefaults(() =>
            new AddEmployeeCommand
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Gender = employee.Gender,
                HireDate = employee.HireDate,
                BirthDate = employee.BirthDate,
            }
        );
    }
}
