using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Options;
using Personal_Collection_Manager.Helpers;
using Personal_Collection_Manager.IService;

namespace Personal_Collection_Manager.Services
{
    public class PhotoService : IPhotoService
    {
        private const int _width = 600;
        private const int _height = 400;
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> options)
        {
            var account = new Account(
                options.Value.CloudName,
                options.Value.ApiKey,
                options.Value.ApiSecret
                );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(_height).Width(_width).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public ImageUploadResult AddPhoto(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(_height).Width(_width).Crop("fill").Gravity("face")
                };
                uploadResult = _cloudinary.Upload(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string url)
        {
            var deleteParams = new DeletionParams(GetImageIdFromUrl(url));
            var res = await _cloudinary.DestroyAsync(deleteParams);
            return res;
        }

        public DeletionResult DeletePhoto(string url)
        {
            var deleteParams = new DeletionParams(GetImageIdFromUrl(url));
            var res = _cloudinary.Destroy(deleteParams);
            return res;
        }

        private string GetImageIdFromUrl(string url)
        {
            int startIndex = url.LastIndexOf('/') + 1;
            int length = url.LastIndexOf('.') - startIndex;
            return url.Substring(startIndex >= 0 ? startIndex : 0, length >= 0 ? length : 0);
        }
    }
}
