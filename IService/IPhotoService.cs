using CloudinaryDotNet.Actions;

namespace Personal_Collection_Manager.IService
{
    public interface IPhotoService
    {
        public Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        public Task<DeletionResult> DeletePhotoAsync(string publicId);
        public ImageUploadResult AddPhoto(IFormFile file);
        public DeletionResult DeletePhoto(string publicId);
    }
}
