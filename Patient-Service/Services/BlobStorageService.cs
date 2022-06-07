using Azure.Storage.Blobs;
using Patient_Service.Interfaces;

namespace Patient_Service.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly IConfiguration _configuration;
    private BlobContainerClient _blobContainerClient;
    private readonly INatsService _natsService;
    
    public BlobStorageService(IConfiguration configuration , INatsService natsService)
    {
        _configuration = configuration;
        _natsService = natsService;
        try
        {
            _blobContainerClient = new BlobContainerClient(_configuration.GetConnectionString("blogStorageConnectionString"),
                _configuration.GetConnectionString("blobImageContainerName"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _natsService.Publish("th-logs","", e.Message);
        }
    }

    public async Task<string> UploadProfileImage_GetImageUrl(IFormFile imageFile, string fileName)
    {
        using(var memoryStream = new MemoryStream()) {
            imageFile.CopyTo(memoryStream);
        }

        var blob = _blobContainerClient.GetBlobClient(fileName);
        
        var stream = imageFile.OpenReadStream();
        await blob.UploadAsync(stream);
        string imageUrl = blob.Uri.AbsoluteUri;
        Console.WriteLine("--> image uploaded: " + imageUrl);

        return imageUrl;
    }
    
    public void DeleteProfileImage(string imageName)
    {
        var blob = _blobContainerClient.GetBlobClient(imageName);
        blob.Delete();
        Console.WriteLine($"{imageName} is deleted!");
    }
}