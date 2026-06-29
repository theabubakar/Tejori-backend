using Microsoft.EntityFrameworkCore;
using Tijori.Application.Interfaces.Repositories;
using Tijori.Application.Validators;
using Tijori.Domain.Entities;
using Tijori.Domain.Enums;
using Tijori.Infrastructure.Data;

namespace Tijori.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        _context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public Task<User?> GetByPhoneAsync(string countryCode, string phoneNumber, CancellationToken cancellationToken = default) =>
        _context.Users.FirstOrDefaultAsync(
            x => x.CountryCode == countryCode && x.PhoneNumber == phoneNumber,
            cancellationToken);

    public async Task<User?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        if (IdentifierHelper.IsEmail(identifier))
        {
            return await GetByEmailAsync(IdentifierHelper.NormalizeEmail(identifier), cancellationToken);
        }

        var (countryCode, phoneNumber) = IdentifierHelper.ParsePhoneIdentifier(identifier);
        return await GetByPhoneAsync(countryCode, phoneNumber, cancellationToken);
    }

    public Task<User?> GetBySocialLoginAsync(
        SocialLoginProvider provider,
        string providerKey,
        CancellationToken cancellationToken = default) =>
        _context.Users
            .Include(x => x.Logins)
            .FirstOrDefaultAsync(
                x => x.Logins.Any(l => l.Provider == provider && l.ProviderKey == providerKey),
                cancellationToken);

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default) =>
        _context.Users.AnyAsync(x => x.Email == email, cancellationToken);

    public Task<bool> PhoneExistsAsync(string countryCode, string phoneNumber, CancellationToken cancellationToken = default) =>
        _context.Users.AnyAsync(
            x => x.CountryCode == countryCode && x.PhoneNumber == phoneNumber,
            cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await _context.Users.AddAsync(user, cancellationToken);

    public void Update(User user) =>
        _context.Users.Update(user);
}
