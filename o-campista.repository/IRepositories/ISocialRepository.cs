using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories;

public interface ISocialRepository
{
    Task<bool> EstaSeguidoAsync(Guid seguidorId, Guid seguidoId);
    Task<bool> SegueMutuamenteAsync(Guid usuarioAId, Guid usuarioBId);
    Task SegueAsync(Seguidor seguidor);
    Task PararDeSegueAsync(Guid seguidorId, Guid seguidoId);
    Task<List<Usuario>> ObterSeguidoresAsync(Guid usuarioId);
    Task<List<Usuario>> ObterSeguindoAsync(Guid usuarioId);
    Task<int> ContarSeguidoresAsync(Guid usuarioId);
    Task<int> ContarSeguindoAsync(Guid usuarioId);
    Task<List<Usuario>> BuscarPorNomeAsync(string nome);
    Task<ConfiguracaoPrivacidade?> ObterPrivacidadeAsync(Guid usuarioId);
    Task SalvarPrivacidadeAsync(ConfiguracaoPrivacidade config);
    Task<List<Usuario>> ObterSugestoesAsync(Guid usuarioId, int limite = 10);
}
