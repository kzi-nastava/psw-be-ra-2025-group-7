using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("api/encounter-images")]
    public class EncounterImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public EncounterImageController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [AllowAnonymous]
        [Consumes("multipart/form-data")]
        [HttpPost("upload")]
        [RequestSizeLimit(10_000_000)] //10MB
        public async Task<ActionResult<object>> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty.");
            }

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || Array.IndexOf(allowed, ext) < 0)
            {
                return BadRequest("Unsupported file type.");
            }

            // web root (wwwroot)
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var imagesFolder = Path.Combine(webRoot, "encounter-images");
            Directory.CreateDirectory(imagesFolder);

            // unikatno ime fajla
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(imagesFolder, fileName);

            // snimi fajl
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // URL koji frontend može da koristi (npr. za <img src="...">)
            var imageUrl = $"{Request.Scheme}://{Request.Host}/encounter-images/{fileName}";

            return Ok(new { url = imageUrl });
        }
    }
}
