namespace StorageService.Models;

public class SqsOptions
{
    public const string Sqs = "Sqs";

    public string Queue { get; init; } = default!;
}
