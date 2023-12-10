using System.Globalization;
using System.Text.Json.Serialization;

namespace Shared.Models;

public class Visit
{
    public Visit() { }

    public Visit(string? referer, string? userAgent, string? ipAddress, DateTimeOffset? visitedAt = null) : base()
    {
        Referer = referer;
        UserAgent = userAgent;
        IpAddress = ipAddress;
        VisitedAt = visitedAt ?? DateTimeOffset.UtcNow;
    }

    public string? Referer { get; init; }

    public string? UserAgent { get; init; }

    public string? IpAddress { get; init; }

    public DateTimeOffset VisitedAt { get; init; }

    [JsonIgnore]
    public string Type => nameof(Visit);

    public override string ToString()
    {
        return string.Join('|', VisitedAt.ToString("o", CultureInfo.InvariantCulture), Referer ?? "null", UserAgent ?? "null", IpAddress ?? "null");
    }
}
