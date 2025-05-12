using CQRSPattern.Application.Features.Employee;
using CQRSPattern.Shared.Test;

namespace CQRSPattern.Application.Test.Builders.Employee.GetAll;

public class EmployeeModelBuilder : GenericBuilder<EmployeeModel>
{
    public EmployeeModelBuilder()
    {
        SetDefaults(() =>
            new EmployeeModel
            {
                Id = 1,
                FirstName = "Pratik",
                LastName = "Lad",
                BirthDate = new DateTime(2000, 1, 1),
                Gender = "M",
                HireDate = new DateTime(2020, 1, 1),
            }
        );
    }
}

public class EmployeeModelListBuilder : GenericBuilder<List<EmployeeModel>>
{
    public EmployeeModelListBuilder()
    {
        SetDefaults(() =>
            new List<EmployeeModel>
            {
                new EmployeeModelBuilder().Build(),
                new EmployeeModelBuilder()
                    .With(x =>
                    {
                        x.Id = 2;
                        x.FirstName = "John";
                        x.LastName = "Doe";
                        x.BirthDate = new DateTime(2001, 1, 1);
                        x.HireDate = new DateTime(2021, 1, 1);
                    })
                    .Build(),
            }
        );
    }
}
