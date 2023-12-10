using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using NSubstitute;
using PixelService.Endpoints;
using PixelService.Models;
using PixelService.Services;
using Shared.Models;

namespace Tests.Unit.Endpoints;

public class TrackingEndpointsTests
{
    [Fact]
    public async Task Track_Endpoint_Returns_Image()
    {
        // Arrange
        var publisher = Substitute.For<IPublisherService>();
        var options = Substitute.For<IOptions<TrackOptions>>();
        var context = Substitute.For<HttpContext>();

        options.Value.Returns(new TrackOptions
        {
            ImagePath = "../../../../src/PixelService/Resources/return-image.gif",
            ImageType = "image/gif"
        });

        // Act
        var result = await TrackingEndpoints.Track(publisher, options, context);

        // Assert
        await publisher.Received(1).Publish(Arg.Any<Visit>());

        Assert.IsAssignableFrom<FileContentHttpResult>(result);
        
        var fileResult = result as FileContentHttpResult;

        Assert.NotNull(fileResult);
        Assert.Equal("image/gif", fileResult.ContentType);
    }
}
