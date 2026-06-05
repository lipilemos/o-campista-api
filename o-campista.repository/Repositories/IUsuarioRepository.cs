using o_campista.entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<bool> EmailExisteAsync(string email);
        Task AdicionarAsync(Usuario usuario);
        Task SalvarAlteracoesAsync();
    }
}
