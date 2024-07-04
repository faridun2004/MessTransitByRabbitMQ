using System.Drawing;

namespace AddPhotoService.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Size { get; set; }
        public string? ImageUrl { get; set; }
        public bool Proccesung { get; set; }
    }
}
