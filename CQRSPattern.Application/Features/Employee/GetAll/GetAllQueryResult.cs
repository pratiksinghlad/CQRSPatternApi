namespace CQRSPattern.Application.Features.Employee.GetAll;

public class GetAllQueryResult
{
    /// <summary>
    /// Result of queried workitems.
    /// </summary>
    public IEnumerable<EmployeeModel> Data { get; set; }
}
