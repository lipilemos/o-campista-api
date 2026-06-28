namespace o_campista.business.IServices
{
    public interface IConquistaService
    {
        Task VerificarConquistasAsync(Guid usuarioId);
    }
}
