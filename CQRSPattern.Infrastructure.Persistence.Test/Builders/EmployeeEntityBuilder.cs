using CQRSPattern.Infrastructure.Persistence.Entities;
using CQRSPattern.Shared.Test;

namespace OMP.Devops.Infrastructure.Persistence.Test.Builders;

public class EmployeeEntityBuilder : GenericBuilder<EmployeeEntity>
{
    public EmployeeEntityBuilder()
    {
        SetDefaults(() => new EmployeeEntity
        {
            Id = 1,
            FirstName = "Pratik",
            LastName = "Lad",
            BirthDate = new DateTime(2000, 1, 1),
            HireDate = new DateTime(2020, 1, 1),
            Gender = "M"
        });
    }
}

public class EmployeeEntityListBuilder : GenericBuilder<List<EmployeeEntity>>
{
    public EmployeeEntityListBuilder()
    {
        SetDefaults(() => new List<EmployeeEntity>
        {
            new EmployeeEntityBuilder().Build(),
            new EmployeeEntityBuilder().With(x =>
            {
                x.Id = 2;
                x.FirstName = "John";
                x.LastName = "Doe";
                x.BirthDate = new DateTime(2001, 1, 1);
                x.HireDate  = new DateTime(2021, 1, 1);
            }).Build(),
        });
    }
}