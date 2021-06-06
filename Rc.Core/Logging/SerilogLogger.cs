using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Rc.Core.Logging
{
    public class SerilogLogger<T> : ILogger<T>
    {
        private ILogger Logger 
        {
            get => AppLogging.Logger;
            init => AppLogging.Logger = value;
        }

        public SerilogLogger(IConfiguration configuration)
        {
            if (Logger != null)
                return;

            Serilog.Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            using SerilogLoggerFactory factory = new SerilogLoggerFactory(Serilog.Log.Logger);
            Logger = factory.CreateLogger("mainLogger");
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Logger.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel) => Logger.IsEnabled(logLevel);

        public IDisposable BeginScope<TState>(TState state) => Logger.BeginScope(state);
    }

    internal static class AppLogging
    {

        internal static ILogger Logger;
    }
}
