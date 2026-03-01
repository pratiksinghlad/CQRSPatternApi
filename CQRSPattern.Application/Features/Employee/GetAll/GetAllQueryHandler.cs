using CQRSPattern.Application.Repositories.Read;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace CQRSPattern.Application.Features.Employee.GetAll;

public class GetAllQueryHandler : IRequestHandler<GetAllQuery, GetAllQueryResult>
{
    public GetAllQueryHandler(IEmployeeReadRepository employeeRepo, IMemoryCache cache)
    {
        _employeeRepo = employeeRepo;
        _cache = cache; 
    }

    public async Task<GetAllQueryResult> Handle(
        GetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        if (_cache.TryGetValue("key", out GetAllQueryResult? cachedValue))
        {
            return cachedValue;
        }

        var result = await _employeeRepo.GetAllAsync(cancellationToken);

        _cache.Set("key", new GetAllQueryResult { Data = result }, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(30)
        });

        return new GetAllQueryResult { Data = result };
    }

    private readonly IEmployeeReadRepository _employeeRepo;
    private readonly IMemoryCache _cache;
}
