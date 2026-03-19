using Microsoft.AspNetCore.Http;

namespace BookLending.Application.Abstractions
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(IFormFile file, string folderName);
        Task DeleteImageAsync(string imageUrl);
    }
}