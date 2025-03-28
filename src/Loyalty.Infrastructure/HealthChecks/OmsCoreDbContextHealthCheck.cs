using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using PostgresErrorCodes = Loyalty.Infrastructure.External.Database.Context.NgSql.ErrorCodes.PostgresErrorCodes;

namespace Loyalty.Infrastructure.HealthChecks;

public class OmsCoreDbContextHealthCheck<TContext> : IHealthCheck
    where TContext : DbContext
{
    private readonly TContext _dbContext;

    public OmsCoreDbContextHealthCheck(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        // Create a linked cancellation token to ensure it has its own timeouts and can be cancelled independently.
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        try
        {
            // Attempt to execute a simple query to verify database connectivity.
            await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cancellationTokenSource.Token);
            return HealthCheckResult.Healthy("Database is reachable.");
        }
        catch (NpgsqlException npgsqlException)
        {
            // Catch SQL exceptions that indicate database-related errors.
            // This could be due to issues like incorrect SQL syntax, database server being down, etc.
            return HealthCheckResult.Unhealthy($"Database connection failed. ErrorCode: {npgsqlException.SqlState}, Description: {PostgresErrorCodes.GetErrorDescription(npgsqlException)}", npgsqlException);
        }
        catch (TimeoutException timeoutException)
        {
            // Catch timeout exceptions that indicate the query execution exceeded the allowed time.
            // This could happen if the database server is under heavy load or there are network latency issues.
            return HealthCheckResult.Unhealthy("Database connection timed out.", timeoutException);
        }
        catch (TaskCanceledException taskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Catch task canceled exceptions that occur if the operation is cancelled by a user or a system timeout.
            // This is useful for identifying if the health check was interrupted before completion.
            return HealthCheckResult.Degraded("Database check task cancelled by the cancellation token.", taskCanceledException);
        }
        catch (TaskCanceledException taskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // Catch task canceled exceptions that occur if the operation is cancelled by a user or a system timeout.
            // This is useful for identifying if the health check was interrupted before completion.
            return HealthCheckResult.Degraded("Database check task cancelled while the cancellation token has not requested the cancellation.", taskCanceledException);
        }
        catch (OperationCanceledException operationCanceledException)
        {
            // Catch operation canceled exceptions that occur if the operation is cancelled by a user or a system timeout.
            // This is useful for identifying if the health check was interrupted before completion.
            return HealthCheckResult.Degraded("Database check operation cancelled.", operationCanceledException);
        }
        catch (Exception exception)
        {
            // Catch any other general exceptions that might occur.
            // This serves as a catch-all for unexpected errors that do not fall under the previous categories.
            return HealthCheckResult.Unhealthy("Database check failed.", exception);
        }
    }
}
