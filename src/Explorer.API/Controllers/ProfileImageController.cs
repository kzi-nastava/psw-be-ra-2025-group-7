using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize]
    [Route("api/profile-images")]
    [ApiController]
    public class ProfileImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public ProfileImageController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Invalid file type. Only images are allowed." });

            if (file.Length > 15 * 1024 * 1024)
                return BadRequest(new { message = "File size must be less than 15MB." });

            var fileName = $"{Guid.NewGuid()}{extension}";

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "profiles");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"/images/profiles/{fileName}";
            return Ok(new { url });
        }
    }
}