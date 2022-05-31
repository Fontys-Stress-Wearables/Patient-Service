namespace Patient_Service.Interfaces;

public interface IBlobStorageService
{
    public Task<string> UploadProfileImage_GetImageUrl(IFormFile imageFile, string fileName);
    public void DeleteProfileImage(string imageName);
}