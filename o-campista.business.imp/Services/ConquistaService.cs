using Microsoft.Extensions.Logging;
using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.business.imp.Services
{
    public class ConquistaService : IConquistaService
    {
        private readonly ICheckinRepository _checkinRepository;
        private readonly ITrilhaRepository _trilhaRepository;
        private readonly IPresenteRepository _presenteRepository;
        private readonly IUsuarioPresenteRepository _usuarioPresenteRepository;
        private readonly IUsuarioConquistaRepository _usuarioConquistaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICampingAvaliacaoRepository _avaliacaoRepository;
        private readonly ILogger<ConquistaService> _logger;

        public ConquistaService(
            ICheckinRepository checkinRepository,
            ITrilhaRepository trilhaRepository,
            IPresenteRepository presenteRepository,
            IUsuarioPresenteRepository usuarioPresenteRepository,
            IUsuarioConquistaRepository usuarioConquistaRepository,
            IUsuarioRepository usuarioRepository,
            ICampingAvaliacaoRepository avaliacaoRepository,
            ILogger<ConquistaService> logger)
        {
            _checkinRepository = checkinRepository;
            _trilhaRepository = trilhaRepository;
            _presenteRepository = presenteRepository;
            _usuarioPresenteRepository = usuarioPresenteRepository;
            _usuarioConquistaRepository = usuarioConquistaRepository;
            _usuarioRepository = usuarioRepository;
            _avaliacaoRepository = avaliacaoRepository;
            _logger = logger;
        }

        public async Task VerificarConquistasAsync(Guid usuarioId)
        {
            var totalCheckins = await _checkinRepository.ContarPorUsuarioAsync(usuarioId);
            var totalCheckinsTrilha = await _checkinRepository.ContarCheckinsTrilhaPorUsuarioAsync(usuarioId);
            var totalTrilhas = await _trilhaRepository.ContarConcluidasPorUsuarioAsync(usuarioId);
            var totalTrilhasCriadas = await _trilhaRepository.ContarCriadasPorUsuarioAsync(usuarioId);
            var totalPresentesCriados = await _presenteRepository.ContarCriadosPorUsuarioAsync(usuarioId);
            var totalPresentesEncontrados = await _usuarioPresenteRepository.ContarResgatadosPorUsuarioAsync(usuarioId);
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
            var totalAvaliacoes = await _avaliacaoRepository.ContarPorUsuarioAsync(usuarioId);

            await VerificarConquistasCheckinAsync(usuarioId, totalCheckins);
            await VerificarConquistasTrilhasAsync(usuarioId, totalTrilhas);
            await VerificarConquistasTrilhaCheckinAsync(usuarioId, totalCheckinsTrilha);
            await VerificarConquistasTrilhaCriacaoAsync(usuarioId, totalTrilhasCriadas);
            await VerificarConquistasPresentesAsync(usuarioId, totalPresentesCriados);
            await VerificarConquistasDescobertaAsync(usuarioId, totalPresentesEncontrados);
            await VerificarConquistasNivelAsync(usuarioId, usuario?.Nivel ?? 1);
            await VerificarConquistasAvaliacaoAsync(usuarioId, totalAvaliacoes);
        }

        private async Task VerificarConquistasCheckinAsync(Guid usuarioId,int totalCheckins)
        {
            if (totalCheckins >= 1)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.PrimeiraBarraca);

            if (totalCheckins >= 5)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.CampistaIniciante);

            if (totalCheckins >= 20)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.CampistaExperiente);

            if (totalCheckins >= 50)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.Explorador);

            if (totalCheckins >= 100)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.LendaDoCamping);
        }
        private async Task VerificarConquistasTrilhasAsync(Guid usuarioId, int totalTrilhas)
        {
            if (totalTrilhas >= 1)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.PrimeiroPasso);

            if (totalTrilhas >= 5)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.Trilheiro);
        }

        private async Task VerificarConquistasTrilhaCheckinAsync(Guid usuarioId, int totalCheckinsTrilha)
        {
            if (totalCheckinsTrilha >= 1)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.PrimeiroPassoNaTrilha);

            if (totalCheckinsTrilha >= 5)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.TrilheiroAssiduo);

            if (totalCheckinsTrilha >= 20)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.ExploradorDeTrilhas);

            if (totalCheckinsTrilha >= 50)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.MestreDasTrilhas);
        }

        private async Task VerificarConquistasTrilhaCriacaoAsync(Guid usuarioId, int totalTrilhasCriadas)
        {
            if (totalTrilhasCriadas >= 1)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.CriadorDeTrilhas);

            if (totalTrilhasCriadas >= 5)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.ArquitetoDaTrilha);

            if (totalTrilhasCriadas >= 20)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.LendaDasTrilhas);
        }
        private async Task VerificarConquistasPresentesAsync(Guid usuarioId,int totalPresentes)
        {
            if (totalPresentes >= 1)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.Generoso);

            if (totalPresentes >= 10)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.GuardiaoDaTrilha);

            if (totalPresentes >= 50)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.MestreDosPresentes);
        }
        private async Task VerificarConquistasDescobertaAsync(Guid usuarioId,int totalEncontrados)
        {
            if (totalEncontrados >= 1)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.CacadorDeTesouros);

            if (totalEncontrados >= 10)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.Colecionador);
        }
        private async Task VerificarConquistasNivelAsync(Guid usuarioId,int nivel)
        {
            if (nivel >= 5)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.Nivel5);

            if (nivel >= 10)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.Nivel10);

            if (nivel >= 20)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.Nivel20);
        }
        private async Task VerificarConquistasAvaliacaoAsync(Guid usuarioId, int totalAvaliacoes)
        {
            if (totalAvaliacoes >= 1)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.PrimeiraAvaliacao);

            if (totalAvaliacoes >= 5)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.CriticoIniciante);

            if (totalAvaliacoes >= 10)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.CriticoExperiente);

            if (totalAvaliacoes >= 25)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.Avaliador);

            if (totalAvaliacoes >= 50)
                await ConcederConquistaAsync(usuarioId, (long)ConquistaEnum.MestreDasCriticas);
        }

        private async Task ConcederConquistaAsync(Guid usuarioId,long conquistaId)
        {
            var jaPossui =
                await _usuarioConquistaRepository
                    .ExisteAsync(
                        usuarioId,
                        conquistaId);

            if (jaPossui)
            {
                return;
            }

            var conquista = new UsuarioConquista
            {
                UsuarioId = usuarioId,
                ConquistaId = conquistaId,
                CriadoEm = DateTime.UtcNow
            };

            await _usuarioConquistaRepository
                .CriarAsync(conquista);

            _logger.LogInformation(
                "Conquista liberada. Usuario={UsuarioId} Conquista={ConquistaId}",
                usuarioId,
                conquistaId);
        }
    }
}
