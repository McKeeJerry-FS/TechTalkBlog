using TechTalkBlog.Enums;

namespace TechTalkBlog.Services.Interfaces
{
    public interface IImageService
    {
        public Task<byte[]> ConvertFileToByteArrayAsynC(IFormFile? file);
        public string? ConvertByteArrayToFile(byte[]? FileData, string? extension, DefaultImage defaultImage);
    }
}
