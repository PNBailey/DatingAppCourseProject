using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

// This service is going to handle the upoading and deletion of our users photos. The reason why we crwtae services like this is that our upload photo service  will have one responsibility and that's simply receiving a file as a parameter, then upload the file to cloudinary and then receive the result back and then return the result to wahtever needs it

namespace API.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file); 

        Task<DeletionResult> DeletionPhotoAsync(string publicId);
    }
}