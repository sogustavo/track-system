using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using PixelService.Models;
using PixelService.Services;
using Shared.Models;
using System.Net;
using System.Text.Json;

namespace Tests.Unit.Services;

public class PublisherServiceTests
{
    [Fact]
    public async Task Publish_ThrowsIfMessageIsNull()
    {
        // Arrange
        var sns = Substitute.For<IAmazonSimpleNotificationService>();
        var options = Substitute.For<IOptions<SnsOptions>>();
        var logger = Substitute.For<ILogger<PublisherService>>();

        var publisher = new PublisherService(sns, options, logger);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => publisher.Publish(null!));
    }

    [Fact]
    public async Task Publish_SuccessfullyPublishesMessage()
    {
        // Arrange
        var sns = Substitute.For<IAmazonSimpleNotificationService>();
        var options = Substitute.For<IOptions<SnsOptions>>();
        var logger = Substitute.For<ILogger<PublisherService>>();

        sns.PublishAsync(Arg.Any<PublishRequest>(), Arg.Any<CancellationToken>()).Returns(new PublishResponse { HttpStatusCode = HttpStatusCode.OK });

        options.Value.Returns(new SnsOptions
        {
            Topic = "arn:aws:sns:topic"
        });

        var publisher = new PublisherService(sns, options, logger);

        var message = new Visit
        {
            Referer = "some-referer",
            UserAgent = "some-user-agent",
            IpAddress = "some-ip"
        };

        var serializedMessage = JsonSerializer.Serialize(message);

        // Act
        await publisher.Publish(message);

        // Assert
        await sns.Received(1).PublishAsync(Arg.Is<PublishRequest>(request => request.TopicArn == "arn:aws:sns:topic" && request.Message == serializedMessage && request.MessageAttributes.ContainsKey("Type")), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Publish_LogsErrorOnUnsuccessfulPublish()
    {
        // Arrange
        var sns = Substitute.For<IAmazonSimpleNotificationService>();
        var options = Substitute.For<IOptions<SnsOptions>>();
        var logger = Substitute.For<MockLogger<PublisherService>>();

        sns.PublishAsync(Arg.Any<PublishRequest>(), Arg.Any<CancellationToken>()).Returns(new PublishResponse { HttpStatusCode = HttpStatusCode.InternalServerError });

        options.Value.Returns(new SnsOptions { Topic = "arn:aws:sns:topic" });

        var publisher = new PublisherService(sns, options, logger);

        // Act
        await publisher.Publish(new Visit());

        // Assert
        logger.Received(1).Log(LogLevel.Error, "Not able to publish message. SNS returned status code InternalServerError", null);
    }
}
