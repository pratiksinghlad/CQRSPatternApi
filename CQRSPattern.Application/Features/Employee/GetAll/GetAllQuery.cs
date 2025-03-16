using MediatR;

namespace CQRSPattern.Application.Features.Employee.GetAll;

public class GetAllQuery : IRequest<GetAllQueryResult>
{
    /// <summary>
    /// Default CTor
    /// </summary>
    public GetAllQuery()
    {
    }

    /// <summary>
    /// Create new instance of the query.
    /// </summary>
    /// <returns></returns>
    public static GetAllQuery Create()
    {
        return new GetAllQuery()
        {
            
        };
    }
}