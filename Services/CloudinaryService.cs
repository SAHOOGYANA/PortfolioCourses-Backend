using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioCourses.Api.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var acc = new Account(
                config["CloudinarySettings:CloudName"],
                config["CloudinarySettings:ApiKey"],
                config["CloudinarySettings:ApiSecret"]
            );
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0) return null;

            using var stream = file.OpenReadStream();

            // Safe parsing of MIME type and extension
            var contentType = file.ContentType?.ToLower() ?? "";
            var extension = Path.GetExtension(file.FileName)?.ToLower() ?? "";
            string uploadedUrl = null;

// 1. VIDEO / AUDIO BUCKET
if (contentType.StartsWith("video/") || contentType.StartsWith("audio/"))
{
    var uploadParams = new VideoUploadParams
    {
        File = new FileDescription(file.FileName, stream),
        Folder = folderName,
        UseFilename = true,        // 🔥 NAYA ADD KIYA
        UniqueFilename = false     // 🔥 NAYA ADD KIYA
    };
    var result = await _cloudinary.UploadAsync(uploadParams);
    uploadedUrl = result.SecureUrl?.ToString();
}
// 2. IMAGE / PDF BUCKET
else if (contentType.StartsWith("image/"))
{
    var uploadParams = new ImageUploadParams
    {
        File = new FileDescription(file.FileName, stream),
        Folder = folderName,
        UseFilename = true,        // 🔥 NAYA ADD KIYA
        UniqueFilename = false     // 🔥 NAYA ADD KIYA
    };
    var result = await _cloudinary.UploadAsync(uploadParams);
    uploadedUrl = result.SecureUrl?.ToString();
}
// 3. RAW BUCKET (Zip, Python files, etc.)
else
{
    var uploadParams = new RawUploadParams
    {
        File = new FileDescription(file.FileName, stream),
        Folder = folderName,
        UseFilename = true,        // 🔥 NAYA ADD KIYA
        UniqueFilename = false     // 🔥 NAYA ADD KIYA
    };
    var result = await _cloudinary.UploadAsync(uploadParams);
    uploadedUrl = result.SecureUrl?.ToString();
}

            return uploadedUrl;
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return false;

            try
            {
                var parts = fileUrl.Split("/upload/");
                if (parts.Length < 2) return false;

                // Smartly detect which bucket this file belongs to based on the URL
                var resourceType = ResourceType.Image;
                if (fileUrl.Contains("/video/upload/")) resourceType = ResourceType.Video;
                else if (fileUrl.Contains("/raw/upload/")) resourceType = ResourceType.Raw;

                var pathSegments = parts[1].Split('/');
                // Remove version tag if it exists (e.g., v1776807433)
                var skipCount = pathSegments[0].StartsWith("v") && pathSegments[0].Length > 2 && char.IsDigit(pathSegments[0][1]) ? 1 : 0;

                var publicIdWithExtension = string.Join("/", pathSegments.Skip(skipCount));
                var publicId = publicIdWithExtension;

                // 🔥 BULLETPROOF DELETE LOGIC
                // Cloudinary 'raw' files keep their extension in the Public ID.
                // Images and Videos drop their extension in the Public ID.
                if (resourceType != ResourceType.Raw)
                {
                    int lastDotIndex = publicIdWithExtension.LastIndexOf('.');
                    // Agar dot milta hai, tabhi hatao (handles files with no extensions safely)
                    if (lastDotIndex > 0)
                    {
                        publicId = publicIdWithExtension.Substring(0, lastDotIndex);
                    }
                }

                var deletionParams = new DeletionParams(publicId) { ResourceType = resourceType };
                var result = await _cloudinary.DestroyAsync(deletionParams);

                return result.Result == "ok";
            }
            catch
            {
                return false;
            }
        }
    }
}