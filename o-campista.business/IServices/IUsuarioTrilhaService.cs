namespace o_campista.business.IServices
{
    public interface IUsuarioTrilhaService
    {
        Task ConcluirTrilhaAsync(Guid usuarioId, long trilhaId);
    }
}
