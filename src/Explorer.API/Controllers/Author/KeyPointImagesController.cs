using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("api/author/keypoint-images")]
    public class KeyPointImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public KeyPointImagesController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [Authorize(Policy = "authorPolicy")]
        [Consumes("multipart/form-data")]
        [HttpPost("upload")]
        [RequestSizeLimit(10_000_000)]
        public async Task<ActionResult<object>> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty.");
            }

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || Array.IndexOf(allowed, ext) < 0)
            {
                return BadRequest("Unsupported file type. Only .jpg, .jpeg, .png, .gif, .webp are allowed.");
            }

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var imagesFolder = Path.Combine(webRoot, "keypoint-images");
            Directory.CreateDirectory(imagesFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(imagesFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"/keypoint-images/{fileName}";

            return Ok(new { url = imageUrl });
        }
    }
}
