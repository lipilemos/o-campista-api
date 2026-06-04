using o_campista.api.Models.Responses;

namespace o_campista.api.Services
{
    public class MapaService : IMapaService
    {
        public async Task<IEnumerable<CampingMapaResponse>> ObterCampingsMapaAsync()
        {
            var campings = new List<CampingMapaResponse>
        {
            new()
            {
                Id = 1,
                Nome = "Camping Recanto Verde",
                Descricao = "Camping familiar próximo ao rio.",
                Latitude = -22.0174m,
                Longitude = -47.8903m,
                Cidade = "São Carlos",
                Estado = "SP",
                Tipo = "camping",
                Endereco = "Estrada Municipal km 15",
                Telefone = "(16) 99999-9999",
                Avaliacao = 4.8,
                FotoPrincipal = "https://cdn.ocampista.com/campings/1.jpg",
                Recursos =
                [
                    new() { Nome = "Banheiro", Disponivel = true },
                    new() { Nome = "Energia", Disponivel = true },
                    new() { Nome = "WiFi", Disponivel = false },
                    new() { Nome = "Pet Friendly", Disponivel = true }
                ]
            },
            new()
            {
                Id = 2,
                Nome = "Camping Cachoeira Azul",
                Descricao = "Área cercada por mata nativa.",
                Latitude = -21.9987m,
                Longitude = -47.8745m,
                Cidade = "São Carlos",
                Estado = "SP",
                Endereco = "Rodovia SP-215 km 120",
                Telefone = "(16) 98888-8888",
                Tipo = "cachoeira",
                Avaliacao = 4.6,
                FotoPrincipal = "https://cdn.ocampista.com/campings/2.jpg",
                Recursos =
                [
                    new() { Nome = "Banheiro", Disponivel = true },
                    new() { Nome = "Energia", Disponivel = false },
                    new() { Nome = "WiFi", Disponivel = false }
                ]
            }
        };

            return await Task.FromResult(campings);
        }
    }
}
