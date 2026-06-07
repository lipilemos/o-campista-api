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

namespace o_campista.business.imp.Services
{
    public class StorageService : IStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public StorageService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async Task<string> UploadAsync(IFormFile arquivo)
        {
            try
            {
                var nomeArquivo =$"{Guid.NewGuid()}{Path.GetExtension(arquivo.FileName)}";

                var bucketUrl = _configuration["Supabase:Url"];
                var bucket = _configuration["Supabase:Bucket"];
                var apiKey = _configuration["Supabase:ApiKey"];
                var uploadUrl = $"{bucketUrl}/storage/v1/object/{bucket}/{nomeArquivo}";

                if (string.IsNullOrWhiteSpace(bucketUrl))
                {
                    throw new InvalidOperationException("Supabase bucket URL not configured (Supabase:BucketUrl).");
                }
                if (!string.IsNullOrEmpty(apiKey))
                {
                    _httpClient.DefaultRequestHeaders.Remove("apikey");
                    _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);

                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                }
                // Read uploaded file into ImageSharp image
                using var inStream = arquivo.OpenReadStream();
                using var image = await Image.LoadAsync(inStream);

                // Resize to limit dimensions while preserving aspect ratio (e.g., max 1920x1080)
                var maxWidth = 1920;
                var maxHeight = 1080;

                var ratio = Math.Min((double)maxWidth / image.Width, (double)maxHeight / image.Height);
                if (ratio < 1)
                {
                    var newWidth = (int)Math.Round(image.Width * ratio);
                    var newHeight = (int)Math.Round(image.Height * ratio);
                    image.Mutate(x => x.Resize(newWidth, newHeight));
                }

                // Reduce quality to prioritize smaller size
                var encoder = new JpegEncoder
                {
                    Quality = 70 // lower quality for smaller size; tune as needed
                };

                await using var outStream = new MemoryStream();
                await image.SaveAsJpegAsync(outStream, encoder);
                outStream.Position = 0;
                
                using var content = new StreamContent(outStream);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");


                var putResponse = await _httpClient.PutAsync(uploadUrl, content);
                var body = await putResponse.Content.ReadAsStringAsync();

                Console.WriteLine($"Status: {putResponse.StatusCode}");
                Console.WriteLine($"Resposta: {body}");
                putResponse.EnsureSuccessStatusCode();

                return uploadUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception();
            }
        }
    }
}
