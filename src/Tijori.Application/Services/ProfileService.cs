using Tijori.Application.Common;
using Tijori.Application.Interfaces.Repositories;
using Tijori.Application.Interfaces.Services;

namespace Tijori.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;

    public ProfileService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundAppException("User not found.");

        return new ProfileDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            CountryCode = user.CountryCode,
            PhoneNumber = user.PhoneNumber,
            ProfileImageUrl = user.ProfileImageUrl,
            Language = user.Language,
            NotificationPreference = user.NotificationPreference
        };
    }
}
