using CQRSPattern.Application.Features.Employee.GetAll;
using CQRSPattern.Shared.Test;

namespace CQRSPattern.Application.Test.Builders.Employee.GetAll;

public class GetAllQueryResultBuilder : GenericBuilder<GetAllQueryResult>
{
    public GetAllQueryResultBuilder()
    {

        SetDefaults(() => new GetAllQueryResult
        {
            Data = new EmployeeModelListBuilder().Build(),
        });
    }
}
