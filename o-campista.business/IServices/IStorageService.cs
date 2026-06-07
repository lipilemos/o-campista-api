using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.business.IServices
{
    public interface IStorageService
    {
        Task<string> UploadAsync(IFormFile arquivo);
    }
}
