using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Bussiness.Interface.Core
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
