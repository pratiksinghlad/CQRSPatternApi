using CQRSPattern.Application.Features.Employee.Patch;
using CQRSPattern.Shared.Test;

namespace CQRSPattern.Application.Test.Builders.Employee.Patch;

/// <summary>
/// Builder for creating PatchEmployeeCommand instances for testing.
/// </summary>
public class PatchEmployeeCommandBuilder : GenericBuilder<PatchEmployeeCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PatchEmployeeCommandBuilder"/> class.
    /// </summary>
    public PatchEmployeeCommandBuilder()
    {
        SetDefaults(() => new PatchEmployeeCommand
        {
            Id = 1,
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            Gender = "Female",
            BirthDate = new DateTime(1990, 5, 15),
            HireDate = new DateTime(2023, 3, 1)
        });
    }

    /// <summary>
    /// Creates a command with only FirstName set for testing partial updates.
    /// </summary>
    /// <param name="id">The employee ID</param>
    /// <param name="firstName">The first name to update</param>
    /// <returns>A PatchEmployeeCommand instance</returns>
    public PatchEmployeeCommand WithOnlyFirstName(int id, string firstName)
    {
        return With(x => x.Id, id)
               .With(x => x.FirstName, firstName)
               .With(x => x.LastName, null)
               .With(x => x.Gender, null)
               .With(x => x.BirthDate, null)
               .With(x => x.HireDate, null)
               .Build();
    }

    /// <summary>
    /// Creates a command with no updates for testing validation.
    /// </summary>
    /// <param name="id">The employee ID</param>
    /// <returns>A PatchEmployeeCommand instance</returns>
    public PatchEmployeeCommand WithNoUpdates(int id)
    {
        return With(x => x.Id, id)
               .With(x => x.FirstName, null)
               .With(x => x.LastName, null)
               .With(x => x.Gender, null)
               .With(x => x.BirthDate, null)
               .With(x => x.HireDate, null)
               .Build();
    }
}
