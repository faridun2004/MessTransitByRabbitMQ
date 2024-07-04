using AddPhotoService.Infrastructure;
using AddPhotoService.Models;
using MassTransit;
using MessagesTransit;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace AddPhotoService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PhotoController : ControllerBase
    {
        private readonly PhotoContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public PhotoController(PhotoContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var photo = new Image
            {
                //Id = 1,
                Size=  "20 x 30",
                ImageUrl = Path.Combine("uploads", file.FileName),
                Proccesung = false
            };

            using (var stream = new FileStream(photo.ImageUrl, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _context.Images.Add(photo);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new ImageAdded { ImageId = photo.Id, Name=photo.Name, Size=photo.Size, ImageUrl = photo.ImageUrl });

            return Ok(new { photo.Id });
        }
    }
}
