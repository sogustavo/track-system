namespace PixelService.Models;

public class SnsOptions
{
    public const string Sns = "Sns";

    public string Topic { get; init; } = default!;
}
