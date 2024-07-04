using System.Collections.Generic;
using AddPhotoService.Models;
using Microsoft.EntityFrameworkCore;
namespace AddPhotoService.Infrastructure
{
    public class PhotoContext: DbContext
    {
        public PhotoContext(DbContextOptions<PhotoContext> options) : base(options) { }
        public DbSet<Image> Images { get; set; }

    }
}
