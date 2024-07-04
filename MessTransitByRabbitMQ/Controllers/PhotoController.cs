using AddPhotoService.Infrastructure;
using AddPhotoService.Models;
using MassTransit;
using MessagesTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Image>>> GetAll()
        {
            var images = await _context.Images.ToListAsync();
            return Ok(images);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var photo = new Image
            {
                //Id = 1,
                Name = file.Name,
                Size = "20 x 30",
                ImageUrl = Path.Combine("uploads", file.FileName),
                Proccesung = false
            };

            using (var stream = new FileStream(photo.ImageUrl, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _context.Images.Add(photo);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new ImageAdded { ImageId = photo.Id, Name = photo.Name, Size = photo.Size, ImageUrl = photo.ImageUrl });

            return Ok(new { photo.Id });
        }
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var photo = await _context.Images.FindAsync(id);
            if (photo == null)
            {
               var t= BadRequest("Not photo");
                return NotFound(t);
            }
            _context.Images.Remove(photo);
            await _context.SaveChangesAsync();
            Console.WriteLine("Deleted");
            return NoContent();
        }
    }
}
