using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;
using PixelService.Models;
using Shared.Models;
using System.Net;
using System.Text.Json;

namespace PixelService.Services;

public interface IPublisherService
{
    Task Publish(Visit message, CancellationToken cancellationToken = default);
}

public class PublisherService(IAmazonSimpleNotificationService sns, IOptions<SnsOptions> options, ILogger<PublisherService> logger) : IPublisherService
{
    private readonly IAmazonSimpleNotificationService _sns = sns;
    private readonly SnsOptions _options = options.Value;
    private readonly ILogger<PublisherService> _logger = logger;

    public async Task Publish(Visit message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var request = new PublishRequest
        {
            TopicArn = _options.Topic,
            Message = JsonSerializer.Serialize(message),
            MessageAttributes = new() { { nameof(Visit.Type), new() { DataType = nameof(String), StringValue = message.Type } } }
        };

        var response = await _sns.PublishAsync(request, cancellationToken);

        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("Not able to publish message. SNS returned status code {StatusCode}", response.HttpStatusCode);
        }
    }
}
