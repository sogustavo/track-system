using Amazon.SQS;
using StorageService.Models;
using StorageService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSService<IAmazonSQS>();

builder.Services.Configure<LogOptions>(builder.Configuration.GetSection(LogOptions.Log));
builder.Services.Configure<SqsOptions>(builder.Configuration.GetSection(SqsOptions.Sqs));

builder.Services.AddTransient<ILogService, LogService>();

builder.Services.AddHostedService<ConsumerService>();

var app = builder.Build();

app.Run();
