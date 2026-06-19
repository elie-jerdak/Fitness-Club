using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;

namespace FitnessClub_Test.Core.Helpers
{
    public static class ImageProcessingHelper
    {
        // 1. Size check
        public static bool IsWithinSizeLimit(byte[] imageBytes, int maxBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return false;
            return imageBytes.Length <= maxBytes;
        }
        
        // 2. Extension check
        public static bool IsAllowedExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(fileName)
                ?.ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }
        
        // 3️. Format validation (real image check)
        public static bool IsValidImageFormat(byte[] imageBytes)
        {
            try
            {
                using var image = Image.Load(imageBytes);
                return image.Metadata.DecodedImageFormat?.Name switch
                {
                    "JPEG" => true,
                    "PNG" => true,
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }
        
        // 4. Resize & compress
        public static byte[] ResizeAndCompress(byte[] imageBytes, int maxWidth, int maxHeight, string extension)
        {
            using var image = Image.Load(imageBytes);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxWidth, maxHeight)
            }));
            using var output = new MemoryStream();
            if (extension == ".png")
            {
                image.Save(output, new PngEncoder());
            }
            else
            {
                image.Save(output, new JpegEncoder
                {
                    Quality = 85
                });
            }
            return output.ToArray();
        }
    }
}
