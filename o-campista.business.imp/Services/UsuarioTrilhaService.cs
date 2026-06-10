using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.entities.Entities.o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.business.imp.Services
{
    public class UsuarioTrilhaService : IUsuarioTrilhaService
    {
        private readonly IUsuarioTrilhaRepository _repository;

        private readonly IConquistaService _conquistaService;

        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioTrilhaService(
            IUsuarioTrilhaRepository repository,
            IConquistaService conquistaService,
            IUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _conquistaService = conquistaService;
            _usuarioRepository = usuarioRepository;
        }

        public async Task ConcluirTrilhaAsync(Guid usuarioId,long trilhaId)
        {
            var registro = await _repository.ObterAsync(usuarioId, trilhaId);

            if (registro is null)
            {
                registro = new UsuarioTrilha
                {
                    UsuarioId = usuarioId,
                    TrilhaId = trilhaId,
                    Concluida = true,
                    CriadoEm = DateTime.UtcNow,
                    ConcluidaEm = DateTime.UtcNow
                };

                await _repository.CriarAsync(registro);
            }
            else if (!registro.Concluida)
            {
                registro.Concluida = true;
                registro.ConcluidaEm = DateTime.UtcNow;

                await _repository.AtualizarAsync(
                    registro);
            }

            var usuario =
                await _usuarioRepository
                    .ObterPorIdAsync(usuarioId);

            if (usuario is not null)
            {
                usuario.XP += 200;

                await _usuarioRepository
                    .AtualizarAsync(usuario);
            }

            await _conquistaService
                .VerificarConquistasAsync(
                    usuarioId);
        }
    }
}
