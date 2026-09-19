namespace NerjaLogisticsERP.Web.Services;

/// <summary>
/// Minimal, dependency-free file logger — appends formatted lines to a single file. Chosen
/// over a package like Serilog specifically because NuGet restore against api.nuget.org has
/// proven unreliable in this environment (repeated NU1301 timeouts on unrelated packages).
/// Log rotation is deliberately left to the server's own logrotate rather than reimplemented
/// here — see the deployment notes for the logrotate config this expects.
/// </summary>
public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;
    private readonly object _writeLock = new();

    public FileLoggerProvider(string filePath)
    {
        _filePath = filePath;
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
    }

    public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, _filePath, _writeLock);

    public void Dispose() { }

    private sealed class FileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _filePath;
        private readonly object _writeLock;

        public FileLogger(string categoryName, string filePath, object writeLock)
        {
            _categoryName = categoryName;
            _filePath = filePath;
            _writeLock = writeLock;
        }

        // Always true — actual level filtering already happens one layer up, in the
        // framework's LoggerFilterOptions (driven by the standard Logging:LogLevel config),
        // before this is ever reached. Duplicating that here would just risk drifting out of
        // sync with what the Console provider honors from the same config section.
        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public void Log<TState>(
            LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var line = $"{DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {_categoryName}: {formatter(state, exception)}";
            if (exception is not null)
                line += Environment.NewLine + exception;

            lock (_writeLock)
            {
                File.AppendAllText(_filePath, line + Environment.NewLine);
            }
        }
    }
}
