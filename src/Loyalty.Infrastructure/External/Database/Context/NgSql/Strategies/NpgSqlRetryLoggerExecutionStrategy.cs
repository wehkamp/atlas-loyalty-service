using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using PostgresErrorCodes = Loyalty.Infrastructure.External.Database.Context.NgSql.ErrorCodes.PostgresErrorCodes;

namespace Loyalty.Infrastructure.External.Database.Context.NgSql.Strategies;

public class NpgSqlRetryLoggerExecutionStrategy : NpgsqlRetryingExecutionStrategy
{
    private readonly ILogger<NpgSqlRetryLoggerExecutionStrategy> _logger;

    public NpgSqlRetryLoggerExecutionStrategy(
        ExecutionStrategyDependencies dependencies,
        ILogger<NpgSqlRetryLoggerExecutionStrategy> logger
    ) : base(dependencies)
    {
        _logger = logger;
    }

    protected override bool ShouldRetryOn(Exception? exception)
    {
        var type = exception?.GetType();

        if (exception is PostgresException postgresException)
        {
            _logger.LogWarning(postgresException, "NpgSqlRetryLoggerExecutionStrategy - Retrying due to exception: {ExceptionMessage}, PostgreSQL ErrorCode: {ErrorCode}, Description: {ErrorCodeDescription}", postgresException.Message, postgresException.SqlState, PostgresErrorCodes.GetErrorDescription(postgresException));
        }
        else if (exception is NpgsqlException npgsqlException)
        {
            _logger.LogWarning(npgsqlException, "NpgSqlRetryLoggerExecutionStrategy - Retrying due to exception: {ExceptionMessage}, Ngsql ErrorCode: {ErrorCode}, Description: {ErrorCodeDescription}", npgsqlException.Message, npgsqlException.SqlState, PostgresErrorCodes.GetErrorDescription(npgsqlException));
        }
        else
        {
            _logger.LogWarning(exception, "NpgSqlRetryLoggerExecutionStrategy - Retrying due to exception: {ExceptionMessage}", exception?.Message);
        }

        return base.ShouldRetryOn(exception);
    }

    protected override TimeSpan? GetNextDelay(Exception lastException)
    {
        var delay = base.GetNextDelay(lastException);

        if (lastException is PostgresException postgresException)
        {
            _logger.LogInformation("NpgSqlRetryLoggerExecutionStrategy - Next retry delay: {Delay}, PostgreSQL ErrorCode: {ErrorCode}, Description: {ErrorCodeDescription}", delay, postgresException.SqlState, PostgresErrorCodes.GetErrorDescription(postgresException));
        }
        else
        {
            _logger.LogInformation("NpgSqlRetryLoggerExecutionStrategy - Next retry delay: {Delay}", delay);
        }

        return delay;
    }
}
