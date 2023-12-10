using Microsoft.Extensions.Options;
using Shared.Models;
using StorageService.Models;

namespace StorageService.Services;

public interface ILogService
{
    Task<bool> Write(Visit message);
}
public class LogService(IOptions<LogOptions> options, ILogger<LogService> logger) : ILogService
{
    private readonly LogOptions _options = options.Value;
    private readonly ILogger<LogService> _logger = logger;

    public async Task<bool> Write(Visit message)
    {
        ArgumentNullException.ThrowIfNull(message);

        try
        {
            _logger.LogDebug("Writing Log to {Path}", _options.Path);

            await File.AppendAllTextAsync(_options.Path, $"{message}{Environment.NewLine}");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write to the log file. {ExceptionMessage}", ex.Message);

            return false;
        }
    }
}
