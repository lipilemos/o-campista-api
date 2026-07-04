namespace o_campista.repository.IRepositories;

public interface IRankingRepository
{
    /// <summary>Usuários ordenados por XP decrescente com contagem de check-ins.</summary>
    Task<List<(Guid Id, string Nome, string? FotoPerfil, int Nivel, int Xp, int TotalCheckins)>>
        ObterRankingGlobalAsync(int skip, int take);

    /// <summary>Ranking restrito aos IDs informados (seguidos), sem paginação.</summary>
    Task<List<(Guid Id, string Nome, string? FotoPerfil, int Nivel, int Xp, int TotalCheckins)>>
        ObterRankingPorIdsAsync(IEnumerable<Guid> ids);

    /// <summary>Campings ordenados por total de check-ins decrescente.</summary>
    Task<List<(long Id, string Nome, string? Cidade, string? Estado, string? FotoPrincipal, decimal AvaliacaoMedia, int TotalCheckins)>>
        ObterRankingCampingsAsync(int skip, int take);
}
