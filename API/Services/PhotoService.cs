using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {

        private readonly Cloudinary _cloudinary; // As we need to have access to cloudinary, we creat a private readonly field. We need to give this field the details of our API keys so we get our configuration below in the constructor
        public PhotoService(IOptions<CloudinarySettings> config) // The way we get our configuration when we've setup a class to store our configuration like we have done in our cloudinarysettings class, is we use the IOptions interface and we pass in our configuration class 
        {
            var acc = new Account( // This is a cloudinary account we are going to create here. This takes the configuration options. Be careful of the ordering here
                config.Value.CloudName,
                config.Value.Apikey,
                config.Value.ApiSecret
            ); 

            _cloudinary = new Cloudinary(acc); // now we assign a new cloudinary to our _cloudinary private field and we pass in the configuration settings tjhat we get from our CloudinarySettings class
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
           var uploadResult = new ImageUploadResult();

           if(file.Length > 0) // Length if a property we have available on a file 
           {
                using var stream = file.OpenReadStream();// as we want to dispose of our stream as soon as we are finished with this method, we use the using statement 

                var uploadParams = new ImageUploadParams 
                {
                    File = new FileDescription(file.FileName, stream), // This sets the description of the file
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face") // This defines what we want to transform our image into. Cloudinary will then take care of cropping and resizing the image etc 
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams); // This is the part where we actually upload the file to cloudinary. When the UploadAsync is called, it returns the result of the uploaded which is the url and the photo id. It can also return an error message if there was a problem uploading the image 
           }

            return uploadResult; // This returns the upload Result which we get back from cloudinary which is the url and the public id (which is the photo id) to the client 
        }

        public async Task<DeletionResult> DeletionPhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result  = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}