using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using o_campista.api.Services;
using o_campista.business.imp.Dictionaries;
using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<LoginResponse> LoginAsync(
            LoginRequest request)
        {
            var usuario =
                await _usuarioRepository
                    .ObterPorEmailAsync(
                        request.Email);

            if (usuario is null)
            {
                throw new UnauthorizedAccessException(
                    "Usuário ou senha inválidos");
            }

            var senhaValida =
                BCrypt.Net.BCrypt.Verify(
                    request.Senha,
                    usuario.SenhaHash);

            if (!senhaValida)
            {
                throw new UnauthorizedAccessException(
                    "Usuário ou senha inválidos");
            }

            var token =
                new TokenService()
                    .GenerateToken(
                        usuario.Email);

            return new LoginResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Token = token,
                Nivel = usuario.Nivel,
                FotoPerfil = usuario.FotoPerfil,
                Xp = usuario.XP,
                XpProximoNivel = NivelXpDictionary.ObterXpProximoNivel(usuario.Nivel),
                TotalCheckins = usuario.Checkins.Count,
                TotalCampingsVisitados = usuario.Checkins.Select(x => x.CampingId).Distinct().Count(),
                TotalTrilhasConcluidas = usuario.UsuarioTrilhas.Count(x => x.Concluida),
                Conquistas =
                    usuario.UsuarioConquistas
                        .Select(c => new ConquistaResponse
                        {
                            Id = c.Conquista.Id,
                            Nome = c.Conquista.Nome,
                            Descricao = c.Conquista.Descricao,
                            Icone = c.Conquista.Icone,
                            DataConquista = c.CriadoEm
                        }),
                Presentes =
                    usuario.UsuarioPresentes
                        .Select(p => new PresenteResponse
                        {
                            Id = p.Presente.Id,
                            Nome = p.Presente.Nome,
                            Descricao = p.Presente.Descricao,
                            CodigoResgate = p.Presente.CodigoResgate,
                            Utilizado = p.Utilizado
                        })
            };
        }

        public async Task RegistrarAsync(
            RegisterRequest request)
        {
            var emailExiste =
                await _usuarioRepository
                    .EmailExisteAsync(
                        request.Email);

            if (emailExiste)
            {
                throw new ApplicationException(
                    "Email já cadastrado");
            }

            var usuario = new Usuario
            {
                Nome = request.Nome,

                Email = request.Email,

                Ativo = true,

                Nivel = 1,

                XP = 0,

                DataCriacao = DateTime.UtcNow,

                SenhaHash =
                    BCrypt.Net.BCrypt
                        .HashPassword(
                            request.Senha)
            };

            await _usuarioRepository
                .AdicionarAsync(usuario);

            await _usuarioRepository
                .SalvarAlteracoesAsync();
        }

        public async Task ForgotPasswordAsync(
            ForgotPasswordRequest request)
        {
            var usuario =
                await _usuarioRepository
                    .ObterPorEmailAsync(request.Email);

            if (usuario is null)
                return;

            var token =
                new TokenService()
                    .GenerateResetToken(usuario.Email);

            await _emailService.SendPasswordResetEmailAsync(usuario.Email, token);
        }

        public async Task ResetPasswordAsync(
            ResetPasswordRequest request)
        {
            var email =
                new TokenService()
                    .ValidateResetToken(request.Token);

            if (email is null)
                throw new UnauthorizedAccessException(
                    "Token inválido ou expirado");

            var usuario =
                await _usuarioRepository
                    .ObterPorEmailAsync(email);

            if (usuario is null)
                throw new UnauthorizedAccessException(
                    "Usuário não encontrado");

            usuario.SenhaHash =
                BCrypt.Net.BCrypt
                    .HashPassword(request.NovaSenha);

            await _usuarioRepository
                .AtualizarAsync(usuario);
        }

        public async Task<LoginResponse> RefreshTokenAsync(
            string email)
        {
            var usuario =
                await _usuarioRepository
                    .ObterPorEmailAsync(email);

            if (usuario is null)
                throw new UnauthorizedAccessException(
                    "Usuário não encontrado");

            var token =
                new TokenService()
                    .GenerateToken(usuario.Email);

            return new LoginResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                FotoPerfil = usuario.FotoPerfil ?? string.Empty,
                Token = token,
                Nivel = usuario.Nivel,
                Xp = usuario.XP,
                XpProximoNivel = NivelXpDictionary.ObterXpProximoNivel(usuario.Nivel),
                TotalCheckins = usuario.Checkins.Count,
                TotalCampingsVisitados = usuario.Checkins.Select(x => x.CampingId).Distinct().Count(),
                TotalTrilhasConcluidas = usuario.UsuarioTrilhas.Count(x => x.Concluida),
                Conquistas =
                    usuario.UsuarioConquistas
                        .Select(c => new ConquistaResponse
                        {
                            Id = c.Conquista.Id,
                            Nome = c.Conquista.Nome,
                            Descricao = c.Conquista.Descricao,
                            Icone = c.Conquista.Icone,
                            DataConquista = c.CriadoEm
                        }),
                Presentes =
                    usuario.UsuarioPresentes
                        .Select(p => new PresenteResponse
                        {
                            Id = p.Presente.Id,
                            Nome = p.Presente.Nome,
                            Descricao = p.Presente.Descricao,
                            CodigoResgate = p.Presente.CodigoResgate,
                            Utilizado = p.Utilizado
                        })
            };
        }

        public async Task<LoginResponse> GoogleAuthAsync(
            GoogleAuthRequest request)
        {
            GoogleJsonWebSignature.Payload payload;

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [_configuration["Google:ClientId"]!]
                };

                payload = await GoogleJsonWebSignature
                    .ValidateAsync(request.IdToken, settings);
            }
            catch (InvalidJwtException)
            {
                throw new UnauthorizedAccessException(
                    "Token do Google inválido");
            }

            var usuario =
                await _usuarioRepository
                    .ObterPorEmailAsync(payload.Email);

            if (usuario is null)
            {
                var novoUsuario = new Usuario
                {
                    Nome = payload.Name,
                    Email = payload.Email,
                    SenhaHash = string.Empty,
                    FotoPerfil = payload.Picture,
                    Ativo = true,
                    Nivel = 1,
                    XP = 0,
                    DataCriacao = DateTime.UtcNow
                };

                await _usuarioRepository
                    .AdicionarAsync(novoUsuario);

                await _usuarioRepository
                    .SalvarAlteracoesAsync();

                usuario = await _usuarioRepository
                    .ObterPorEmailAsync(payload.Email);
            }

            var token =
                new TokenService()
                    .GenerateToken(usuario!.Email);

            return new LoginResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                FotoPerfil = payload.Picture ?? string.Empty,
                Token = token,
                Nivel = usuario.Nivel,
                Xp = usuario.XP,
                XpProximoNivel = NivelXpDictionary.ObterXpProximoNivel(usuario.Nivel),
                TotalCheckins = usuario.Checkins.Count,
                TotalCampingsVisitados = usuario.Checkins.Select(x => x.CampingId).Distinct().Count(),
                TotalTrilhasConcluidas = usuario.UsuarioTrilhas.Count(x => x.Concluida),
                Conquistas =
                    usuario.UsuarioConquistas
                        .Select(c => new ConquistaResponse
                        {
                            Id = c.Conquista.Id,
                            Nome = c.Conquista.Nome,
                            Descricao = c.Conquista.Descricao,
                            Icone = c.Conquista.Icone,
                            DataConquista = c.CriadoEm
                        }),
                Presentes =
                    usuario.UsuarioPresentes
                        .Select(p => new PresenteResponse
                        {
                            Id = p.Presente.Id,
                            Nome = p.Presente.Nome,
                            Descricao = p.Presente.Descricao,
                            CodigoResgate = p.Presente.CodigoResgate,
                            Utilizado = p.Utilizado
                        })
            };
        }
    }
}
