using Microsoft.Extensions.Logging;

namespace Tests.Unit
{
    public abstract class MockLogger<T> : ILogger<T>
    {
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var unboxed = (IReadOnlyList<KeyValuePair<string, object>>)state!;
            string message = formatter(state, exception);

            this.Log();
            this.Log(logLevel, message, exception);
            this.Log(logLevel, unboxed.ToDictionary(k => k.Key, v => v.Value), exception);
        }

        public abstract void Log();

        public abstract void Log(LogLevel logLevel, string message, Exception? exception = null);

        public abstract void Log(LogLevel logLevel, IDictionary<string, object> state, Exception? exception = null);

        public virtual bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public abstract IDisposable BeginScope<TState>(TState state);
    }
}
