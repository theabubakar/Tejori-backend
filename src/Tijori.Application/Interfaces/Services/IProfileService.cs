using Tijori.Application.Common;

namespace Tijori.Application.Interfaces.Services;

public interface IProfileService
{
    Task<ProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
}
