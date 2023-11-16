using TechTalkBlog.Services.Interfaces;
using TechTalkBlog.Enums;

namespace TechTalkBlog.Services
{
    public class ImageService : IImageService
    {
        private readonly string? _defaultBlogPostImage = "/img/img_placeholder.jpg";
        private readonly string? _defaultBlogUserImage = "/img/silo_img.jpg";
        private readonly string? _defaultCategoryImage = "/img/category_default.png";
        private readonly string? _defaultAuthorImage = "/img/headshot.png";

        public string? ConvertByteArrayToFile(byte[]? fileData, string? extension, DefaultImage defaultImage)
        {
            try
            {
                if (fileData == null || fileData.Length == 0)
                {
                    // show default
                    switch (defaultImage)
                    {
                        case DefaultImage.AuthorImage: 
                            return _defaultAuthorImage;
                        case DefaultImage.BlogPostImage: 
                            return _defaultBlogPostImage;
                        case DefaultImage.BlogUserImage: 
                            return _defaultBlogUserImage;
                        case DefaultImage.CategoryImage: 
                            return _defaultCategoryImage;
                    }
                }
                string? imageBase64Data = Convert.ToBase64String(fileData!);
                imageBase64Data = string.Format($"data:{extension};base64, {imageBase64Data}");
                return imageBase64Data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsynC(IFormFile? file)
        {
            try
            {
                if (file != null)
                {
                    using MemoryStream memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    byte[] byteFile = memoryStream.ToArray();
                    memoryStream.Close();

                    return byteFile;
                }
                return null!;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
