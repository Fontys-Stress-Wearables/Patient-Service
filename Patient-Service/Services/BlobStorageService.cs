using Azure.Storage.Blobs;
using Patient_Service.Interfaces;

namespace Patient_Service.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly IConfiguration _configuration;
    private BlobContainerClient _blobContainerClient;
    
    public BlobStorageService(IConfiguration configuration)
    {
        _configuration = configuration;
        _blobContainerClient = new BlobContainerClient(_configuration.GetConnectionString("blogStorageConnectionString"),
            _configuration.GetConnectionString("blobImageContainerName"));

    }

    public async Task<string> UploadProfileImage_GetImageUrl(IFormFile imageFile)
    {
        using(var memoryStream = new MemoryStream()) {
            imageFile.CopyTo(memoryStream);
        }

        var blob = _blobContainerClient.GetBlobClient(imageFile.FileName);
        
        var stream = imageFile.OpenReadStream();
        await blob.UploadAsync(stream);
        string imageUrl = blob.Uri.AbsoluteUri;
        Console.WriteLine("--> image uploaded: " + imageUrl);

        return imageUrl;
    }
}