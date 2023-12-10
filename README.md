# Tracking System

<div align="center">
    <img src="./docs/tracking.gif">
</div>

Simple tracking system to capture requests and stores visitors asynchronously in the plain text log for further analysis.

The solution consists of two services (projects): one collects the data, other handles writing to the file.

## Pixel Service

This API has only one endpoint `GET /track` that returns a transparent 1-pixel image in GIF format and collects the following information:

- `Referrer` header
- `User-Agent` header
- Visitor IP address

After the information has been collected, it sends an event to a `SNS topic`.

## Storage Service

Consume events from a `SQS Queue` and stores it in the `append-only` file. The path to the file is defined via `appsettings.json`.

The format of the file is `date-time of the visit in ISO 8601 format in UTC | referrer | user-agent | ip`.

The IP address is the only mandatory value. The rest can be substituted with null when empty.

Example

```
2023-12-10T22:22:43.7936205+00:00|https://localhost:7058/swagger/index.html|null|192.168.1.1
2023-12-11T10:06:40.5064909+00:00|https://localhost:7058/swagger/index.html|Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0|10.0.0.1
```

## Approach

To address the presented challenge, several approaches could be considered, such as utilizing `gRPC`. However, to sidestep the complexity associated with implementing protocol buffers and similar mechanisms, I have chosen a more straightforward solution in this case. I opted for a simplified `Pub/Sub` approach employing `LocalStack`, along with the integration of `SNS` and `SQS`.