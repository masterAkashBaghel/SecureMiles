using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;


namespace SecureMiles.Services.Cloudinary
{
    public class CloudinaryService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["CloudinarySettings:CloudName"],
                configuration["CloudinarySettings:ApiKey"],
                configuration["CloudinarySettings:ApiSecret"]
            );

            _cloudinary = new CloudinaryDotNet.Cloudinary(account);
        }

        public async Task<ImageUploadResult> UploadFileAsync(IFormFile file, string folder)
        {
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult;
            }
        }

        public async Task DeleteFileAsync(string publicId, string folder = null)
        {
            var fullPublicId = string.IsNullOrEmpty(folder) ? publicId : $"{folder}/{publicId}";
            Console.WriteLine($"Attempting to delete file with publicId: {fullPublicId}");

            var deletionParams = new DeletionParams(fullPublicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.Result != "ok")
            {
                // Log the result for debugging
                Console.WriteLine($"Failed to delete the document from Cloudinary. Result: {result.Result}, Error: {result.Error?.Message}");
                throw new InvalidOperationException("Failed to delete the document from Cloudinary.");
            }
        }

    }


}