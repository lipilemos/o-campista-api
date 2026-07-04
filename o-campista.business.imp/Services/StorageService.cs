using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using o_campista.business.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Microsoft.Extensions.Logging;
using o_campista.shared.Enums;
using o_campista.shared.Helper;


namespace o_campista.business.imp.Services
{
    public class StorageService : IStorageService
    {
        private readonly ILogger<StorageService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;


        public StorageService(IConfiguration configuration, HttpClient httpClient, ILogger<StorageService> logger)

        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;

        }
        public async Task<string> UploadAsync(IFormFile arquivo, BucketTypeEnum tipoBucket)
        {
            try
            {
                _logger.LogInformation(
                    "Iniciando upload do arquivo {Arquivo}",
                    arquivo.FileName);

                var nomeArquivo =
                    $"{Guid.NewGuid()}{Path.GetExtension(arquivo.FileName)}";

                var bucketUrl = _configuration["Supabase:Url"];
                var bucket = _configuration["Supabase:"+ tipoBucket.GetDescription()];
                var apiKey = _configuration["Supabase:ApiKey"];

                if (string.IsNullOrWhiteSpace(bucketUrl))
                {
                    _logger.LogError(
                        "Configuração Supabase:Url não encontrada.");

                    throw new InvalidOperationException(
                        "Supabase URL não configurada.");
                }

                var uploadUrl = $"{bucketUrl}/storage/v1/object/{bucket}/{nomeArquivo}";
                var publicUrl = $"{bucketUrl}/storage/v1/object/public/{bucket}/{nomeArquivo}";

                _logger.LogInformation(
                    "Realizando upload para {UploadUrl}",
                    uploadUrl);

                if (!string.IsNullOrEmpty(apiKey))
                {
                    _httpClient.DefaultRequestHeaders.Remove("apikey");
                    _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);

                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    _httpClient.DefaultRequestHeaders.Add("Authorization",$"Bearer {apiKey}");
                }

                using var inStream = arquivo.OpenReadStream();
                using var image = await Image.LoadAsync(inStream);

                _logger.LogInformation(
                    "Imagem carregada. Dimensões originais: {Width}x{Height}",
                    image.Width,
                    image.Height);

                var maxWidth = 1920;
                var maxHeight = 1080;

                var ratio = Math.Min(
                    (double)maxWidth / image.Width,
                    (double)maxHeight / image.Height);

                if (ratio < 1)
                {
                    var newWidth =
                        (int)Math.Round(image.Width * ratio);

                    var newHeight =
                        (int)Math.Round(image.Height * ratio);

                    image.Mutate(x =>
                        x.Resize(newWidth, newHeight));

                    _logger.LogInformation(
                        "Imagem redimensionada para {Width}x{Height}",
                        newWidth,
                        newHeight);
                }

                var encoder = new JpegEncoder
                {
                    Quality = 70
                };

                await using var outStream = new MemoryStream();

                await image.SaveAsJpegAsync(
                    outStream,
                    encoder);

                outStream.Position = 0;

                using var content =
                    new StreamContent(outStream);

                content.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(
                        "image/jpeg");

                var putResponse =
                    await _httpClient.PutAsync(
                        uploadUrl,
                        content);

                var responseBody =
                    await putResponse.Content.ReadAsStringAsync();

                _logger.LogInformation(
                    "Resposta do Supabase. Status: {StatusCode}",
                    putResponse.StatusCode);

                if (!putResponse.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "Falha no upload. Status: {StatusCode}. Resposta: {Resposta}",
                        putResponse.StatusCode,
                        responseBody);
                }

                putResponse.EnsureSuccessStatusCode();

                _logger.LogInformation(
                    "Upload concluído com sucesso. Arquivo: {NomeArquivo}",
                    nomeArquivo);

                return publicUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao realizar upload do arquivo {Arquivo}",
                    arquivo?.FileName);

                throw;
            }
        }
    }
}
