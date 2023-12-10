using System.Net.Mime;

namespace PixelService.Models;

public class TrackOptions
{
    public const string Track = "Track";

    public string ImagePath { get; init; } = default!;

    public string ImageType { get; init; } = MediaTypeNames.Image.Gif;
}
