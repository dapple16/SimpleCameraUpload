using Microsoft.AspNetCore.Mvc;

namespace SimpleCameraUpload.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CameraController : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile image)
    {
        if (image == null || image.Length == 0)
            return BadRequest("No image uploaded.");

        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        if (!Directory.Exists(uploads))
            Directory.CreateDirectory(uploads);

        var filePath = Path.Combine(uploads, image.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        return Ok(new { filePath });
    }
}
