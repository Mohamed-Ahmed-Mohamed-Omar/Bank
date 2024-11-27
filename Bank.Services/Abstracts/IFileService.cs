using Microsoft.AspNetCore.Http;

namespace Bank.Services.Abstracts
{
    public interface IFileService
    {
        public Task<string> UploadImage(string Location, IFormFile file);
    }
}
