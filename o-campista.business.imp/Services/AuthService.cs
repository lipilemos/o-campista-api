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

        public AuthService(
            IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
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
    }
}
