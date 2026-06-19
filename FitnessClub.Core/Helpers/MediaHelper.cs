using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Helpers
{
    public static class MediaHelper
    {
        public static string GetFileContentType(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return "application/octet-stream";
            }

            extension = NormalizeExtension(extension);

            return extension switch
            {
                "pdf" => "application/pdf",
                "doc" => "application/msword",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "xls" => "application/vnd.ms-excel",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "png" => "image/png",
                "jpg" => "image/jpg",
                "jpeg" => "image/jpeg",
                "gif" => "image/gif",
                "svg" => "image/svg+xml",
                "avi" => "video/x-msvideo",
                "ts" => "video/MP2T",
                "mpeg" => "video/mpeg",
                "mp4" => "video/mp4",
                "m3u8" => "application/vnd.apple.mpegurl",
                "mp3" => "audio/mp3",
                "wav" => "audio/wav",
                "apk" => "application/vnd.android.package-archive",
                _ => "application/octet-stream"
            };
        }

        public static string NormalizeExtension(string extension)
           => extension.StartsWith('.') ? extension[1..] : extension;
    }
}
