using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessClub_Test.Dtos
{
    public class UploadFileDTO
    {
        [FromForm(Name = "UserId")]
        public int UserId { get; set; }

        [FromForm(Name = "file")]
        public IFormFile file { get; set; }
       
    }
}
