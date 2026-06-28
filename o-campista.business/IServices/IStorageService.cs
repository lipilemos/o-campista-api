using Microsoft.AspNetCore.Http;
using o_campista.shared.Enums;

namespace o_campista.business.IServices
{
    public interface IStorageService
    {
        Task<string> UploadAsync(IFormFile arquivo, BucketTypeEnum type);
    }
}
