using Amazon.SimpleNotificationService;
using Microsoft.OpenApi.Models;
using PixelService.Endpoints;
using PixelService.Models;
using PixelService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();

builder.Services.Configure<TrackOptions>(builder.Configuration.GetSection(TrackOptions.Track));
builder.Services.Configure<SnsOptions>(builder.Configuration.GetSection(SnsOptions.Sns));

builder.Services.AddTransient<IPublisherService, PublisherService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Pixel Service"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();

app.MapTrackingEndpoints();

app.Run();
