using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using o_campista.business.IServices;
using o_campista.shared.Models.Requests;
using System.Security.Claims;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var response =
            await _authService
                .LoginAsync(request);

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        await _authService
            .RegistrarAsync(request);

        return Ok();
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleAuth(
        GoogleAuthRequest request)
    {
        try
        {
            var response =
                await _authService
                    .GoogleAuthAsync(request);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordRequest request)
    {
        await _authService
            .ForgotPasswordAsync(request);

        return Ok(new { mensagem = "Se o e-mail estiver cadastrado, enviaremos as instruções de recuperação." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordRequest request)
    {
        try
        {
            await _authService
                .ResetPasswordAsync(request);

            return Ok(new { mensagem = "Senha alterada com sucesso." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var email = User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(email))
            return Unauthorized();

        try
        {
            var response =
                await _authService
                    .RefreshTokenAsync(email);

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}