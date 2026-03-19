using BookLending.Application.Abstractions;
using BookLending.Application.Setting;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace BookLending.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<FileService> _logger;
        private readonly CloudinarySetting _settings;

        public FileService(Cloudinary cloudinary,
                           ILogger<FileService> logger,
                           IOptions<CloudinarySetting> settings)
        {
            _cloudinary = cloudinary;
            _logger = logger;
            _settings = settings.Value;
        }
        public async Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Upload attempt with null or empty file.");
                throw new ArgumentException("No file selected.");
            }

            if (file.Length > _settings.MaxFileSizeInMB * 1024 * 1024)
            {
                _logger.LogWarning("File size exceeds limit: {FileSize} bytes", file.Length);
                throw new ArgumentException("File size must not exceed 10 MB.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_settings.AllowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Invalid file extension: {Extension}", extension);
                throw new ArgumentException($"Only {string.Join(", ", _settings.AllowedExtensions)} files are allowed.");
            }
            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folderName,
                    UseFilename = true,
                    Overwrite = false,
                    UniqueFilename = true
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary upload failed: {ErrorMessage}", result.Error.Message);
                    throw new Exception($"Cloudinary upload failed: {result.Error.Message}");
                }

                _logger.LogInformation("Image uploaded successfully to folder: {Folder}", folderName);
                return result.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during image upload to folder: {Folder}", folderName);
                throw;
            }
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            try
            {
                var match = Regex.Match(imageUrl, @"/image/upload/(?:v\d+/)?(.+?)\.(?:jpg|jpeg|png|webp)$", RegexOptions.IgnoreCase);

                if (!match.Success)
                {
                    _logger.LogWarning("Could not extract publicId from Cloudinary URL: {ImageUrl}", imageUrl);
                    return;
                }

                var publicId = match.Groups[1].Value;

                var deletionParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deletionParams);

                if (result.Result != "ok")
                {
                    _logger.LogWarning("Failed to delete image from Cloudinary. Public ID: {PublicId}, Result: {Result}", publicId, result.Result);
                }
                else
                {
                    _logger.LogInformation("Successfully deleted image from Cloudinary. Public ID: {PublicId}", publicId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during image deletion from Cloudinary with URL: {ImageUrl}", imageUrl);
            }
        }
    }
}