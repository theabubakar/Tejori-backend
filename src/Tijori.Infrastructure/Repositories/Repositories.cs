using Microsoft.EntityFrameworkCore;
using Tijori.Application.Interfaces.Repositories;
using Tijori.Domain.Entities;
using Tijori.Domain.Enums;
using Tijori.Infrastructure.Data;

namespace Tijori.Infrastructure.Repositories;

public class OtpVerificationRepository : IOtpVerificationRepository
{
    private readonly ApplicationDbContext _context;

    public OtpVerificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<OtpVerification?> GetActiveByUserChannelAndPurposeAsync(
        Guid userId,
        OtpChannel channel,
        OtpPurpose purpose,
        CancellationToken cancellationToken = default) =>
        _context.OtpVerifications
            .Where(x =>
                x.UserId == userId &&
                x.Channel == channel &&
                x.Purpose == purpose &&
                !x.IsUsed &&
                x.ExpiresAt >= DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<OtpVerification?> GetActiveByUserAndPurposeAsync(
        Guid userId,
        OtpPurpose purpose,
        CancellationToken cancellationToken = default) =>
        _context.OtpVerifications
            .Where(x =>
                x.UserId == userId &&
                x.Purpose == purpose &&
                !x.IsUsed &&
                x.ExpiresAt >= DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddAsync(OtpVerification otpVerification, CancellationToken cancellationToken = default) =>
        await _context.OtpVerifications.AddAsync(otpVerification, cancellationToken);

    public void Update(OtpVerification otpVerification) =>
        _context.OtpVerifications.Update(otpVerification);

    public void InvalidateActiveOtps(Guid userId, OtpPurpose purpose)
    {
        var activeOtps = _context.OtpVerifications
            .Where(x => x.UserId == userId && x.Purpose == purpose && !x.IsUsed)
            .ToList();

        foreach (var otp in activeOtps)
        {
            otp.IsUsed = true;
            otp.UpdatedAt = DateTime.UtcNow;
        }
    }
}

public class PasswordResetSessionRepository : IPasswordResetSessionRepository
{
    private readonly ApplicationDbContext _context;

    public PasswordResetSessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<PasswordResetSession?> GetByTokenAsync(string token, CancellationToken cancellationToken = default) =>
        _context.PasswordResetSessions.FirstOrDefaultAsync(x => x.Token == token, cancellationToken);

    public async Task AddAsync(PasswordResetSession session, CancellationToken cancellationToken = default) =>
        await _context.PasswordResetSessions.AddAsync(session, cancellationToken);

    public void Update(PasswordResetSession session) =>
        _context.PasswordResetSessions.Update(session);
}

public class UserLoginRepository : IUserLoginRepository
{
    private readonly ApplicationDbContext _context;

    public UserLoginRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UserLogin userLogin, CancellationToken cancellationToken = default) =>
        await _context.UserLogins.AddAsync(userLogin, cancellationToken);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
