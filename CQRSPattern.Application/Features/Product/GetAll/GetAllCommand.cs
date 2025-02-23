using MediatR;

namespace CQRSPattern.Application.Features.Product.GetAll;

public class GetAllCommand : IRequest<GetAllCommandResult>
{
    /// <summary>
    /// Default CTor (for builders in testing)
    /// </summary>
    public GetAllCommand()
    {
    }

    /// <summary>
    /// Create new instance of the command.
    /// </summary>
    /// <param name="title">Title of the bug.</param>
    /// <returns></returns>
    public static GetAllCommand CreateCommand()
    {
        return new GetAllCommand()
        {
            
        };
    }
}