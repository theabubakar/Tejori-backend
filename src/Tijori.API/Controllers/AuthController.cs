using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tijori.Application.Common;
using Tijori.Application.DTOs.Auth;
using Tijori.Application.Interfaces.Services;

namespace Tijori.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<RegisterPendingDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<RegisterPendingDto>.Ok(result, "Account created. Please verify the OTP sent to your email."));
    }

    [HttpPost("register/verify-otp")]
    [ProducesResponseType(typeof(ApiResponse<RegistrationOtpVerifiedDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyRegistrationOtp(
        [FromBody] VerifyRegistrationOtpRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.VerifyRegistrationOtpAsync(request, cancellationToken);
        return Ok(ApiResponse<RegistrationOtpVerifiedDto>.Ok(result, "Your account was successfully verified."));
    }

    [HttpPost("register/resend-otp/email")]
    [ProducesResponseType(typeof(ApiResponse<OtpSentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResendRegistrationOtpByEmail(
        [FromBody] ResendRegistrationOtpByEmailRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.ResendRegistrationOtpByEmailAsync(request, cancellationToken);
        return Ok(ApiResponse<OtpSentDto>.Ok(result, "OTP sent successfully."));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        return Ok(ApiResponse<AuthTokenDto>.Ok(result, "Login successfully."));
    }

    [HttpPost("guest")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GuestAccess(CancellationToken cancellationToken)
    {
        var result = await _authService.GuestAccessAsync(cancellationToken);
        return Ok(ApiResponse<AuthTokenDto>.Ok(result, "Guest access granted."));
    }

    [HttpPost("forgot-password/otp/email")]
    [ProducesResponseType(typeof(ApiResponse<OtpSentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendForgotPasswordOtpByEmail(
        [FromBody] SendForgotPasswordOtpByEmailRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.SendForgotPasswordOtpByEmailAsync(request, cancellationToken);
        return Ok(ApiResponse<OtpSentDto>.Ok(result, "OTP sent successfully."));
    }

    [HttpPost("forgot-password/verify-otp")]
    [ProducesResponseType(typeof(ApiResponse<OtpVerifiedDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyForgotPasswordOtp(
        [FromBody] VerifyOtpRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.VerifyForgotPasswordOtpAsync(request, cancellationToken);
        return Ok(ApiResponse<OtpVerifiedDto>.Ok(result, "OTP verified successfully."));
    }

    [HttpPost("forgot-password/resend-otp/email")]
    [ProducesResponseType(typeof(ApiResponse<OtpSentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResendForgotPasswordOtpByEmail(
        [FromBody] ResendOtpByEmailRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.ResendForgotPasswordOtpByEmailAsync(request, cancellationToken);
        return Ok(ApiResponse<OtpSentDto>.Ok(result, "OTP sent successfully."));
    }

    [HttpPost("forgot-password/reset-password")]
    [ProducesResponseType(typeof(ApiResponse<PasswordChangedDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.ResetPasswordAsync(request, cancellationToken);
        return Ok(ApiResponse<PasswordChangedDto>.Ok(result, result.Message));
    }

    [HttpPost("social/apple")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AppleSignIn(
        [FromBody] SocialLoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.AppleSignInAsync(request, cancellationToken);
        return Ok(ApiResponse<AuthTokenDto>.Ok(result, "Signed in with Apple successfully."));
    }

    [HttpPost("social/google")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GoogleSignIn(
        [FromBody] SocialLoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.GoogleSignInAsync(request, cancellationToken);
        return Ok(ApiResponse<AuthTokenDto>.Ok(result, "Signed in with Google successfully."));
    }
}
