using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProccessor.Models
{
    public class ImageDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Size { get; set; }
        public string? ImageUrl { get; set; }
        public bool Proccesung { get; set; }
    }
}
