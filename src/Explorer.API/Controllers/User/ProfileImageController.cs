using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.User;

[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
[Route("api/user/profile-images")]
public class ProfileImageController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public ProfileImageController(IWebHostEnvironment env)
    {
        _env = env;
    }

    /// <summary>
    /// Upload profile image. Returns URL that can be used in UserProfile.
    /// </summary>
    [AllowAnonymous]
    [Consumes("multipart/form-data")]
    [HttpPost("upload")]
    [RequestSizeLimit(5_000_000)] // 5MB limit for profile pictures
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
            return BadRequest("Unsupported file type. Allowed: JPG, PNG, GIF, WEBP");
        }

        // web root (wwwroot)
        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var imagesFolder = Path.Combine(webRoot, "profile-images");
        Directory.CreateDirectory(imagesFolder);

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(imagesFolder, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return URL that frontend can use
        var imageUrl = $"{Request.Scheme}://{Request.Host}/profile-images/{fileName}";

        return Ok(new { url = imageUrl });
    }
}
