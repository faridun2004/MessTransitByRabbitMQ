using ImageProccessor.Infrastructure;
using ImageProccessor.Models;
using MassTransit;
using MessagesTransit;
using System.Drawing;
using System.Drawing.Imaging;

public class ImageAddedConsumers : IConsumer<ImageAdded>
{
    private readonly PhotoContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ImageAddedConsumers> _logger;

    public ImageAddedConsumers(PhotoContext context, IPublishEndpoint publishEndpoint, ILogger<ImageAddedConsumers> logger)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ImageAdded> context)
    {
        var message = context.Message;
        var photo = await _context.Images.FindAsync(message.ImageId);

        if (photo == null)
        {
            photo = new ImageDto { Id = message.ImageId, ImageUrl = message.ImageUrl, Proccesung = false };
            _context.Images.Add(photo);
            await _context.SaveChangesAsync();
        }

        // Process original image
        _logger.LogInformation($"Processing photo: {photo.ImageUrl}");
        await Task.Delay(1000); // Simulating image processing time
        photo.Proccesung = true;

        // Save changes
        await _context.SaveChangesAsync();

        // Publish processed message
        await _publishEndpoint.Publish(new Imageprocessing { ImageId = photo.Id, Name = photo.Name, Size = photo.Size, ImageUrl = photo.ImageUrl });
        _logger.LogInformation($"Photo processed: {photo.ImageUrl}");

        // Resize and save different sizes
        var sizes = new List<Size> { new Size(100, 100), new Size(300, 300), new Size(600, 600) }; // Example sizes in pixels

        foreach (var size in sizes)
        {
            var resizedFilePath = $"path_to_resized_{size.Width}x{size.Height}_{photo.ImageUrl}";
            ResizeAndSave(photo.ImageUrl, resizedFilePath, size.Width, size.Height);
            _logger.LogInformation($"Resized photo saved: {resizedFilePath}");
        }
    }

    private void ResizeAndSave(string originalFilePath, string resizedFilePath, int width, int height)
    {
        using (var image = Image.FromFile(originalFilePath))
        {
            using (var resizedImage = new Bitmap(width, height))
            {
                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    graphics.DrawImage(image, 0, 0, width, height);
                }
                resizedImage.Save(resizedFilePath, ImageFormat.Jpeg);
            }
        }
    }
}
