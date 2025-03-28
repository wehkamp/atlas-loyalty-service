using Prometheus;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Loyalty.Infrastructure.Logging;
internal class PrometheusSink : ILogEventSink
{
    private readonly Counter _counter;

    public PrometheusSink()
    {
        _counter = Metrics.CreateCounter("log_message_counter", "Total count of log messages", "level");
    }

    public void Emit(LogEvent logEvent)
    {
        var level = logEvent.Level.ToString().ToLower();

        _counter.WithLabels(level).Inc();
    }
}

public static class SerilogExtensions
{
    public static LoggerConfiguration Prometheus(this LoggerSinkConfiguration sinkConfiguration)
        => sinkConfiguration.Sink<PrometheusSink>(LogEventLevel.Verbose);
}
