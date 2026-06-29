using Tijori.Application.Common;
using Tijori.Domain.Entities;

namespace Tijori.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByPhoneAsync(string countryCode, string phoneNumber, CancellationToken cancellationToken = default);
    Task<User?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    Task<User?> GetBySocialLoginAsync(Domain.Enums.SocialLoginProvider provider, string providerKey, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> PhoneExistsAsync(string countryCode, string phoneNumber, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
}

public interface IOtpVerificationRepository
{
    Task<OtpVerification?> GetActiveByUserChannelAndPurposeAsync(
        Guid userId,
        Domain.Enums.OtpChannel channel,
        Domain.Enums.OtpPurpose purpose,
        CancellationToken cancellationToken = default);

    Task<OtpVerification?> GetActiveByUserAndPurposeAsync(
        Guid userId,
        Domain.Enums.OtpPurpose purpose,
        CancellationToken cancellationToken = default);

    Task AddAsync(OtpVerification otpVerification, CancellationToken cancellationToken = default);
    void Update(OtpVerification otpVerification);
    void InvalidateActiveOtps(Guid userId, Domain.Enums.OtpPurpose purpose);
}

public interface IPasswordResetSessionRepository
{
    Task<PasswordResetSession?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(PasswordResetSession session, CancellationToken cancellationToken = default);
    void Update(PasswordResetSession session);
}

public interface IUserLoginRepository
{
    Task AddAsync(UserLogin userLogin, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
