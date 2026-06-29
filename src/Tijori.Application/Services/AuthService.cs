using Tijori.Application.Common;
using Tijori.Application.DTOs.Auth;
using Tijori.Application.Interfaces.Repositories;
using Tijori.Application.Interfaces.Services;
using Tijori.Application.Validators;
using Tijori.Domain.Entities;
using Tijori.Domain.Enums;

namespace Tijori.Application.Services;

public class AuthService : IAuthService
{
    private const int OtpExpiryMinutes = 1;
    private const int ResetTokenExpiryMinutes = 15;
    private const int MaxOtpAttempts = 5;

    private readonly IUserRepository _userRepository;
    private readonly IOtpVerificationRepository _otpVerificationRepository;
    private readonly IPasswordResetSessionRepository _passwordResetSessionRepository;
    private readonly IUserLoginRepository _userLoginRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpHasher _otpHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IOtpService _otpService;
    private readonly IOtpDeliveryService _otpDeliveryService;
    private readonly ISocialTokenValidator _socialTokenValidator;
    private readonly IMaskingService _maskingService;

    public AuthService(
        IUserRepository userRepository,
        IOtpVerificationRepository otpVerificationRepository,
        IPasswordResetSessionRepository passwordResetSessionRepository,
        IUserLoginRepository userLoginRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IOtpHasher otpHasher,
        IJwtTokenService jwtTokenService,
        IOtpService otpService,
        IOtpDeliveryService otpDeliveryService,
        ISocialTokenValidator socialTokenValidator,
        IMaskingService maskingService)
    {
        _userRepository = userRepository;
        _otpVerificationRepository = otpVerificationRepository;
        _passwordResetSessionRepository = passwordResetSessionRepository;
        _userLoginRepository = userLoginRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _otpHasher = otpHasher;
        _jwtTokenService = jwtTokenService;
        _otpService = otpService;
        _otpDeliveryService = otpDeliveryService;
        _socialTokenValidator = socialTokenValidator;
        _maskingService = maskingService;
    }

    public async Task<RegisterPendingDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = IdentifierHelper.NormalizeEmail(request.Email);
        var normalizedPhone = request.PhoneNumber.Replace(" ", string.Empty);
        var countryCode = IdentifierHelper.NormalizeCountryCode(request.CountryCode);

        if (await _userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new ConflictAppException("Email already exists.");
        }

        if (await _userRepository.PhoneExistsAsync(countryCode, normalizedPhone, cancellationToken))
        {
            throw new ConflictAppException("Phone number already exists.");
        }

        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            CountryCode = countryCode,
            PhoneNumber = normalizedPhone,
            PasswordHash = _passwordHasher.Hash(request.Password),
            TermsAccepted = request.AcceptTerms,
            IsGuest = false,
            IsEmailVerified = false,
            PasswordUpdatedAt = now,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await CreateAndSendEmailOtpAsync(user, OtpPurpose.Registration, cancellationToken);

        return new RegisterPendingDto
        {
            UserId = user.Id,
            MaskedEmail = _maskingService.MaskEmail(user.Email!),
            IsEmailVerified = false
        };
    }

    public async Task<RegistrationOtpVerifiedDto> VerifyRegistrationOtpAsync(
        VerifyRegistrationOtpRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = IdentifierHelper.NormalizeEmail(request.Email);
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || user.IsGuest)
        {
            throw new NotFoundAppException("No account found with this email.");
        }

        if (user.IsEmailVerified)
        {
            return new RegistrationOtpVerifiedDto
            {
                IsEmailVerified = true,
                AuthToken = _jwtTokenService.GenerateToken(user)
            };
        }

        var otp = await _otpVerificationRepository.GetActiveByUserChannelAndPurposeAsync(
            user.Id,
            OtpChannel.Email,
            OtpPurpose.Registration,
            cancellationToken);

        if (otp is null)
        {
            throw new UnauthorizedAppException("OTP has expired or is invalid. Please request a new one.");
        }

        await ValidateOtpAttemptAsync(otp, request.OtpCode, cancellationToken);

        otp.IsUsed = true;
        otp.UpdatedAt = DateTime.UtcNow;
        _otpVerificationRepository.Update(otp);

        user.IsEmailVerified = true;
        user.UpdatedAt = DateTime.UtcNow;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegistrationOtpVerifiedDto
        {
            IsEmailVerified = true,
            AuthToken = _jwtTokenService.GenerateToken(user)
        };
    }

    public async Task<OtpSentDto> ResendRegistrationOtpByEmailAsync(
        ResendRegistrationOtpByEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await GetRegistrationUserAsync(request.Email, cancellationToken);

        if (user.IsEmailVerified)
        {
            throw new ConflictAppException("Email is already verified.");
        }

        await CreateAndSendEmailOtpAsync(user, OtpPurpose.Registration, cancellationToken);

        return new OtpSentDto
        {
            MaskedRecipient = _maskingService.MaskEmail(user.Email!)
        };
    }

    public async Task<AuthTokenDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdentifierAsync(request.Identifier.Trim(), cancellationToken);

        if (user is null || user.IsGuest || string.IsNullOrEmpty(user.PasswordHash))
        {
            throw new UnauthorizedAppException("Invalid credentials.");
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAppException("Invalid credentials.");
        }

        if (!user.IsEmailVerified)
        {
            throw new UnauthorizedAppException("Please complete email verification before signing in.");
        }

        return _jwtTokenService.GenerateToken(user);
    }

    public async Task<AuthTokenDto> GuestAccessAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            IsGuest = true,
            TermsAccepted = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _jwtTokenService.GenerateToken(user);
    }

    public async Task<OtpSentDto> SendForgotPasswordOtpByEmailAsync(
        SendForgotPasswordOtpByEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = IdentifierHelper.NormalizeEmail(request.Email);
        var user = await GetForgotPasswordUserByEmailAsync(normalizedEmail, cancellationToken);

        await CreateAndSendEmailOtpAsync(user, OtpPurpose.ForgotPassword, cancellationToken);

        return new OtpSentDto
        {
            MaskedRecipient = _maskingService.MaskEmail(user.Email!)
        };
    }

    public async Task<OtpVerifiedDto> VerifyForgotPasswordOtpAsync(
        VerifyOtpRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = IdentifierHelper.NormalizeEmail(request.Email);
        var user = await GetForgotPasswordUserByEmailAsync(normalizedEmail, cancellationToken);

        var otp = await _otpVerificationRepository.GetActiveByUserChannelAndPurposeAsync(
            user.Id,
            OtpChannel.Email,
            OtpPurpose.ForgotPassword,
            cancellationToken);

        if (otp is null)
        {
            throw new UnauthorizedAppException("OTP has expired or is invalid. Please request a new one.");
        }

        await ValidateOtpAttemptAsync(otp, request.OtpCode, cancellationToken);

        otp.IsUsed = true;
        otp.UpdatedAt = DateTime.UtcNow;
        _otpVerificationRepository.Update(otp);

        var resetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("+", string.Empty)
            .Replace("/", string.Empty)
            .Replace("=", string.Empty);

        var session = new PasswordResetSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = resetToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(ResetTokenExpiryMinutes),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _passwordResetSessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new OtpVerifiedDto
        {
            ResetToken = resetToken,
            ExpiresAt = session.ExpiresAt
        };
    }

    public Task<OtpSentDto> ResendForgotPasswordOtpByEmailAsync(
        ResendOtpByEmailRequest request,
        CancellationToken cancellationToken = default) =>
        SendForgotPasswordOtpByEmailAsync(
            new SendForgotPasswordOtpByEmailRequest { Email = request.Email },
            cancellationToken);

    public async Task<PasswordChangedDto> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var session = await _passwordResetSessionRepository.GetByTokenAsync(request.ResetToken, cancellationToken);

        if (session is null || session.IsUsed || session.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAppException("Invalid or expired reset token.");
        }

        var user = await _userRepository.GetByIdAsync(session.UserId, cancellationToken);

        if (user is null || user.IsGuest)
        {
            throw new NotFoundAppException("User not found.");
        }

        var now = DateTime.UtcNow;
        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.PasswordUpdatedAt = now;
        user.UpdatedAt = now;
        session.IsUsed = true;
        session.UpdatedAt = now;

        _userRepository.Update(user);
        _passwordResetSessionRepository.Update(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PasswordChangedDto
        {
            Message = "Your password was successfully updated."
        };
    }

    public async Task<AuthTokenDto> AppleSignInAsync(
        SocialLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var socialUser = await _socialTokenValidator.ValidateAppleTokenAsync(request.IdToken, cancellationToken);
        return await AuthenticateSocialUserAsync(SocialLoginProvider.Apple, socialUser, cancellationToken);
    }

    public async Task<AuthTokenDto> GoogleSignInAsync(
        SocialLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var socialUser = await _socialTokenValidator.ValidateGoogleTokenAsync(request.IdToken, cancellationToken);
        return await AuthenticateSocialUserAsync(SocialLoginProvider.Google, socialUser, cancellationToken);
    }

    private async Task<User> GetRegistrationUserAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = IdentifierHelper.NormalizeEmail(email);
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || user.IsGuest)
        {
            throw new NotFoundAppException("No account found with this email.");
        }

        if (user.IsEmailVerified)
        {
            throw new ConflictAppException("Account is already verified.");
        }

        return user;
    }

    private async Task<User> GetForgotPasswordUserByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || user.IsGuest)
        {
            throw new NotFoundAppException("No account found with this email.");
        }

        return user;
    }

    private async Task ValidateOtpAttemptAsync(
        OtpVerification otp,
        string submittedCode,
        CancellationToken cancellationToken)
    {
        if (otp.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAppException("OTP has expired.");
        }

        if (otp.FailedAttempts >= MaxOtpAttempts)
        {
            throw new UnauthorizedAppException("Maximum OTP attempts exceeded.");
        }

        if (!_otpHasher.Verify(submittedCode, otp.CodeHash))
        {
            otp.FailedAttempts++;
            otp.UpdatedAt = DateTime.UtcNow;
            _otpVerificationRepository.Update(otp);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedAppException("Invalid OTP.");
        }
    }

    private async Task<AuthTokenDto> AuthenticateSocialUserAsync(
        SocialLoginProvider provider,
        SocialUserInfo socialUser,
        CancellationToken cancellationToken)
    {
        var existingLogin = await _userRepository.GetBySocialLoginAsync(provider, socialUser.ProviderKey, cancellationToken);

        if (existingLogin is not null)
        {
            return _jwtTokenService.GenerateToken(existingLogin);
        }

        User? user = null;

        if (!string.IsNullOrWhiteSpace(socialUser.Email))
        {
            user = await _userRepository.GetByEmailAsync(
                IdentifierHelper.NormalizeEmail(socialUser.Email),
                cancellationToken);
        }

        var now = DateTime.UtcNow;

        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                FullName = socialUser.FullName,
                Email = string.IsNullOrWhiteSpace(socialUser.Email)
                    ? null
                    : IdentifierHelper.NormalizeEmail(socialUser.Email),
                TermsAccepted = true,
                IsGuest = false,
                IsEmailVerified = !string.IsNullOrWhiteSpace(socialUser.Email),
                CreatedAt = now,
                UpdatedAt = now
            };

            await _userRepository.AddAsync(user, cancellationToken);
        }

        var userLogin = new UserLogin
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Provider = provider,
            ProviderKey = socialUser.ProviderKey,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _userLoginRepository.AddAsync(userLogin, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _jwtTokenService.GenerateToken(user);
    }

    private async Task CreateAndSendEmailOtpAsync(
        User user,
        OtpPurpose purpose,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(user.Email))
        {
            throw new AppException("Email address is not available for this account.");
        }

        _otpVerificationRepository.InvalidateActiveOtps(user.Id, purpose);

        var code = _otpService.GenerateCode();
        var now = DateTime.UtcNow;

        var otp = new OtpVerification
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CodeHash = _otpHasher.Hash(code),
            Channel = OtpChannel.Email,
            Purpose = purpose,
            ExpiresAt = now.AddMinutes(OtpExpiryMinutes),
            IsUsed = false,
            FailedAttempts = 0,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _otpVerificationRepository.AddAsync(otp, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _otpDeliveryService.SendEmailOtpAsync(user.Email, code, cancellationToken);
    }
}
