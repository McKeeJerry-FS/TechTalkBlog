﻿using TechTalkBlog.Services.Interfaces;

namespace TechTalkBlog.Services
{
    public class ImageService : IImageService
    {
        private readonly string _defaultImage = "/img/silo_img.jpg";

        public string? ConvertByteArrayToFile(byte[]? fileData, string? extension)
        {
            try
            {
                if (fileData == null)
                {
                    // show default
                    return _defaultImage;
                }
                string? imageBase64Data = Convert.ToBase64String(fileData);
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
