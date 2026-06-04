using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.api.Models.Requests;
using o_campista.api.Models.Responses;
using o_campista.api.Services;
using o_campista.entities.Entities;

namespace o_campista.api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
    [FromBody] LoginRequest request,
    [FromServices] CampistaDbContext context
)
    {
        var usuario =
            await context.Usuarios
                .FirstOrDefaultAsync(
                    x => x.Email == request.Email
                );

        if (usuario is null)
        {
            return Unauthorized(
                new
                {
                    message = "Usuário ou senha inválidos"
                });
        }

        var senhaValida =
            BCrypt.Net.BCrypt.Verify(
                request.Senha,
                usuario.SenhaHash
            );

        if (!senhaValida)
        {
            return Unauthorized(
                new
                {
                    message = "Usuário ou senha inválidos"
                });
        }

        var tokenService =
            new TokenService();

        var token =
            tokenService.GenerateToken(
                usuario.Email
            );

        return Ok(
            new LoginResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Token = token
            });
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(
    [FromBody] RegisterRequest request,
    [FromServices] CampistaDbContext context
)
    {
        var usuarioExistente =
            await context.Usuarios
                .AnyAsync(x =>
                    x.Email == request.Email);

        if (usuarioExistente)
        {
            return BadRequest(
                new
                {
                    message = "Email já cadastrado"
                });
        }

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            Ativo = true,
            DataCriacao = DateTime.UtcNow,
            XP= 0,
            Nivel = 1,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(
                request.Senha
            )
        };

        context.Usuarios.Add(usuario);

        await context.SaveChangesAsync();

        return Ok();
    }
}