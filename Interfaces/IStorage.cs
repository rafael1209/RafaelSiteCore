namespace RafaelSiteCore.Interfaces;

public interface IStorage
{
        Task<string> UploadFile(IFormFile file);
}