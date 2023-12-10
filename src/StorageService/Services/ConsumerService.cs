using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using Shared.Models;
using StorageService.Models;
using System.Net;
using System.Text.Json;

namespace StorageService.Services;

public class ConsumerService(ILogService logService, IAmazonSQS sqs, IOptions<SqsOptions> options, ILogger<ConsumerService> logger) : BackgroundService
{
    private readonly ILogService _logService = logService;
    private readonly IAmazonSQS _sqs = sqs;
    private readonly SqsOptions _options = options.Value;
    private readonly ILogger<ConsumerService> _logger = logger;
    private readonly List<string> _attributes = ["*"];

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var queue = await _sqs.GetQueueUrlAsync(_options.Queue, cancellationToken);

        var request = new ReceiveMessageRequest()
        {
            QueueUrl = queue.QueueUrl,
            MaxNumberOfMessages = 10,
            AttributeNames = _attributes,
            MessageAttributeNames = _attributes
        };

        var multplier = 1;

        while (!cancellationToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(request, cancellationToken);

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                _logger.LogWarning("Not able to received messages. SQS returned status code {StatusCode}", response.HttpStatusCode);

                continue;
            }

            if (response.Messages.Count == 0)
            {
                var seconds = TimeSpan.FromSeconds(1 * multplier);

                _logger.LogDebug("Nothing in the queue. Retrying in {Seconds} seconds", seconds);

                await Task.Delay(seconds, cancellationToken);

                multplier += 2;

                continue;
            }

            multplier = 1;

            foreach (var message in response.Messages)
            {
                _logger.LogDebug("Processing message {Id}. Receipt Handle {ReceiptHandle}, Body {Body}", message.MessageId, message.ReceiptHandle, message.Body);

                var visit = JsonSerializer.Deserialize<Visit>(message.Body);

                if (visit is null)
                {
                    _logger.LogWarning("Not able to deserialize message. Id {Id}. Receipt Handle {ReceiptHandle}, Body {Body}", message.MessageId, message.ReceiptHandle, message.Body);

                    continue;
                }

                if (await _logService.Write(visit))
                {
                    await _sqs.DeleteMessageAsync(queue.QueueUrl, message.ReceiptHandle, cancellationToken);
                }
            }
        }
    }
}
