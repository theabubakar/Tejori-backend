using Tijori.Application.Common;

namespace Tijori.Application.Interfaces.Services;

public interface IHomeService
{
    Task<HomeDto> GetHomeAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface ICurrentUserService
{
    Guid GetRequiredUserId();
}
