using Microsoft.Extensions.Options;
using PixelService.Models;
using PixelService.Services;
using System.Net.Mime;

namespace PixelService.Endpoints;

public static class TrackingEndpoints
{
    public static WebApplication MapTrackingEndpoints(this WebApplication app)
    {
        app.MapGet("track", Track).Produces(StatusCodes.Status200OK, contentType: MediaTypeNames.Image.Gif);

        return app;
    }

    internal static async Task<IResult> Track(IPublisherService publisher, IOptions<TrackOptions> options, HttpContext context)
    {
        var referer = context.Request.Headers.Referer;
        var userAgent = context.Request.Headers.UserAgent;
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        var publish = publisher.Publish(new(referer, userAgent, ipAddress));

        var _options = options.Value;

        var image = File.ReadAllBytes(_options.ImagePath);

        await publish;

        return TypedResults.File(image, _options.ImageType);
    }
}
