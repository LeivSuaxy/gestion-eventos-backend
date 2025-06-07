using Org.BouncyCastle.Tls;

namespace event_horizon_backend.Core.Services;

public class ImageHandler
{
    public async Task<string> SaveImageAsync(IFormFile image)
    {
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);

        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        return $"/images/{uniqueFileName}";
    }
}