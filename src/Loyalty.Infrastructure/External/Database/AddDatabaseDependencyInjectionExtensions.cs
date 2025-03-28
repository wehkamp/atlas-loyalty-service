using Loyalty.Infrastructure.External.Database.Context;
using Loyalty.Infrastructure.External.Database.Context.Interceptors;
using Loyalty.Infrastructure.External.Database.Context.NgSql.ErrorCodes;
using Loyalty.Infrastructure.External.Database.Context.NgSql.Strategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Loyalty.Infrastructure.External.Database;

internal static class AddDatabaseDependencyInjectionExtensions
{
    const string ConnectionStringPostgresWriterSection = "PostgresWriter";
    const string ConnectionStringPostgresReaderSection = "PostgresReader";

    private static int GetDbCommandTimeout()
    {
        const int defaultDbCommandTimeout = 45;
        const int kafkaDbCommandTimeout = 120;
        var entryAssembly = Assembly.GetEntryAssembly();

        // Set command timeout to the default if we can't determine the assembly
        if (entryAssembly == null)
        {
            return defaultDbCommandTimeout;
        }

        // Set command timeout to the default if we can't determine the name
        var assemblyName = entryAssembly.GetName().Name;
        if (string.IsNullOrWhiteSpace(assemblyName))
        {
            return defaultDbCommandTimeout;
        }

        // Set command timeout to 45 if the project is a WebApi due to HTTP has a timeout of 60 seconds
        if (assemblyName.EndsWith(".Api", StringComparison.InvariantCultureIgnoreCase))
        {
            return defaultDbCommandTimeout;
        }

        // Set command timeout to 120 seconds if we are not an api
        return kafkaDbCommandTimeout;
    }

    private static IList<string> GetCommonErrorCodesToIgnore()
    {
        return new List<string>
        {
            // We don't want to retry if it was a success
            // Class 00 — Successful Completion
            Npgsql.PostgresErrorCodes.SuccessfulCompletion, // "00000" - successful_completion

            // Retrying violations won't help to retry
            // Class 23 — Integrity Constraint Violation
            Npgsql.PostgresErrorCodes.IntegrityConstraintViolation, // "23000" - integrity_constraint_violation
            Npgsql.PostgresErrorCodes.RestrictViolation, // "23001" - restrict_violation
            Npgsql.PostgresErrorCodes.NotNullViolation, // "23502" - not_null_violation
            Npgsql.PostgresErrorCodes.ForeignKeyViolation, // "23503" - foreign_key_violation
            Npgsql.PostgresErrorCodes.UniqueViolation, // "23505" - unique_violation
            Npgsql.PostgresErrorCodes.CheckViolation, // "23514" - check_violation
            Npgsql.PostgresErrorCodes.ExclusionViolation, // "23P01" - exclusion_violation

            // Class 22 — Data Exception
            Npgsql.PostgresErrorCodes.DataException, // "22000" - data_exception
            Npgsql.PostgresErrorCodes.StringDataRightTruncation, // "22001" - string_data_right_truncation
            Npgsql.PostgresErrorCodes.NumericValueOutOfRange, // "22003" - numeric_value_out_of_range
            Npgsql.PostgresErrorCodes.NullValueNotAllowed, // "22004" - null_value_not_allowed
            Npgsql.PostgresErrorCodes.InvalidDatetimeFormat, // "22007" - invalid_datetime_format

            // Class 42 — Syntax Error or Access Rule Violation
            Npgsql.PostgresErrorCodes.SyntaxErrorOrAccessRuleViolation, // "42000" - syntax_error_or_access_rule_violation
            Npgsql.PostgresErrorCodes.SyntaxError, // "42601" - syntax_error
            Npgsql.PostgresErrorCodes.InsufficientPrivilege, // "42501" - insufficient_privilege
            Npgsql.PostgresErrorCodes.CannotCoerce, // "42846" - cannot_coerce
            Npgsql.PostgresErrorCodes.GroupingError, // "42803" - grouping_error
            Npgsql.PostgresErrorCodes.WindowingError, // "42P20" - windowing_error
            Npgsql.PostgresErrorCodes.InvalidRecursion, // "42P19" - invalid_recursion
            Npgsql.PostgresErrorCodes.InvalidForeignKey, // "42830" - invalid_foreign_key
            Npgsql.PostgresErrorCodes.InvalidName, // "42602" - invalid_name
            Npgsql.PostgresErrorCodes.NameTooLong, // "42622" - name_too_long
            Npgsql.PostgresErrorCodes.ReservedName, // "42939" - reserved_name
            Npgsql.PostgresErrorCodes.DatatypeMismatch, // "42804" - datatype_mismatch
            Npgsql.PostgresErrorCodes.IndeterminateDatatype, // "42P18" - indeterminate_datatype
            Npgsql.PostgresErrorCodes.CollationMismatch, // "42P21" - collation_mismatch
            Npgsql.PostgresErrorCodes.IndeterminateCollation, // "42P22" - indeterminate_collation
            Npgsql.PostgresErrorCodes.WrongObjectType, // "42809" - wrong_object_type
            Npgsql.PostgresErrorCodes.UndefinedColumn, // "42703" - undefined_column
            Npgsql.PostgresErrorCodes.UndefinedFunction, // "42883" - undefined_function
            Npgsql.PostgresErrorCodes.UndefinedTable, // "42P01" - undefined_table
            Npgsql.PostgresErrorCodes.UndefinedParameter, // "42P02" - undefined_parameter
            Npgsql.PostgresErrorCodes.UndefinedObject, // "42704" - undefined_object
            Npgsql.PostgresErrorCodes.DuplicateColumn, // "42701" - duplicate_column
            Npgsql.PostgresErrorCodes.DuplicateCursor, // "42P03" - duplicate_cursor
            Npgsql.PostgresErrorCodes.DuplicateDatabase, // "42P04" - duplicate_database
            Npgsql.PostgresErrorCodes.DuplicateFunction, // "42723" - duplicate_function
            Npgsql.PostgresErrorCodes.DuplicatePreparedStatement, // "42P05" - duplicate_prepared_statement
            Npgsql.PostgresErrorCodes.DuplicateSchema, // "42P06" - duplicate_schema
            Npgsql.PostgresErrorCodes.DuplicateTable, // "42P07" - duplicate_table
            Npgsql.PostgresErrorCodes.DuplicateAlias, // "42712" - duplicate_alias
            Npgsql.PostgresErrorCodes.DuplicateObject, // "42710" - duplicate_object
            Npgsql.PostgresErrorCodes.AmbiguousColumn, // "42702" - ambiguous_column
            Npgsql.PostgresErrorCodes.AmbiguousFunction, // "42725" - ambiguous_function
            Npgsql.PostgresErrorCodes.AmbiguousParameter, // "42P08" - ambiguous_parameter
            Npgsql.PostgresErrorCodes.AmbiguousAlias, // "42P09" - ambiguous_alias
            Npgsql.PostgresErrorCodes.InvalidColumnReference, // "42P10" - invalid_column_reference
            Npgsql.PostgresErrorCodes.InvalidColumnDefinition, // "42611" - invalid_column_definition
            Npgsql.PostgresErrorCodes.InvalidCursorDefinition, // "42P11" - invalid_cursor_definition
            Npgsql.PostgresErrorCodes.InvalidDatabaseDefinition, // "42P12" - invalid_database_definition
            Npgsql.PostgresErrorCodes.InvalidFunctionDefinition, // "42P13" - invalid_function_definition
            Npgsql.PostgresErrorCodes.InvalidPreparedStatementDefinition, // "42P14" - invalid_prepared_statement_definition
            Npgsql.PostgresErrorCodes.InvalidSchemaDefinition, // "42P15" - invalid_schema_definition
            Npgsql.PostgresErrorCodes.InvalidTableDefinition, // "42P16" - invalid_table_definition
            Npgsql.PostgresErrorCodes.InvalidObjectDefinition, // "42P17" - invalid_object_definition
        };
    }

    private static IList<string> GetWriterDbRetryErrorCodesList()
    {
        var allNgSqlErrorCodes = PostgresErrorCodes.SqlStatesDictionary.Select(dictionary => dictionary.Key).ToList();

        // For the writer instance we don't have any special error codes to ignore so we can just use the common ones
        var errorCodesToIgnore = GetCommonErrorCodesToIgnore();

        // Remove the error codes we want to skip
        return allNgSqlErrorCodes.Except(errorCodesToIgnore).ToList();
    }

    private static IList<string> GetReaderDbRetryErrorCodesList()
    {
        var allNgSqlErrorCodes = PostgresErrorCodes.SqlStatesDictionary.Select(dictionary => dictionary.Key).ToList();

        // For the reader instance we have some additional error codes to ignore
        var errorCodesToIgnore = GetCommonErrorCodesToIgnore().Concat(new List<string>
        {
            // Class 0A — Feature Not Supported
            Npgsql.PostgresErrorCodes.FeatureNotSupported, // "0A000" - feature_not_supported

            // Class 21 — Cardinality Violation
            Npgsql.PostgresErrorCodes.CardinalityViolation, // "21000" - cardinality_violation

            // Class 25 — Invalid Transaction State
            Npgsql.PostgresErrorCodes.InvalidTransactionState, // "25000" - invalid_transaction_state
            Npgsql.PostgresErrorCodes.ActiveSqlTransaction, // "25001" - active_sql_transaction
            Npgsql.PostgresErrorCodes.NoActiveSqlTransaction, // "25P01" - no_active_sql_transaction

            // Class 28 — Invalid Authorization Specification
            Npgsql.PostgresErrorCodes.InvalidAuthorizationSpecification, // "28000" - invalid_authorization_specification
            Npgsql.PostgresErrorCodes.InvalidPassword, // "28P01" - invalid_password

            // Class 40 — Transaction Rollback
            Npgsql.PostgresErrorCodes.TransactionRollback, // "40000" - transaction_rollback
            Npgsql.PostgresErrorCodes.SerializationFailure, // "40001" - serialization_failure
            Npgsql.PostgresErrorCodes.DeadlockDetected, // "40P01" - deadlock_detected

            // Additional error codes to ignore for read queries
            // Class 53 — Insufficient Resources
            Npgsql.PostgresErrorCodes.InsufficientResources, // "53000" - insufficient_resources
            Npgsql.PostgresErrorCodes.DiskFull, // "53100" - disk_full
            Npgsql.PostgresErrorCodes.OutOfMemory, // "53200" - out_of_memory
            Npgsql.PostgresErrorCodes.TooManyConnections, // "53300" - too_many_connections
            Npgsql.PostgresErrorCodes.ConfigurationLimitExceeded, // "53400" - configuration_limit_exceeded

            // Class 54 — Program Limit Exceeded
            Npgsql.PostgresErrorCodes.ProgramLimitExceeded, // "54000" - program_limit_exceeded
            Npgsql.PostgresErrorCodes.StatementTooComplex, // "54001" - statement_too_complex
            Npgsql.PostgresErrorCodes.TooManyColumns, // "54011" - too_many_columns
            Npgsql.PostgresErrorCodes.TooManyArguments, // "54023" - too_many_arguments

            // Class 55 — Object Not In Prerequisite State
            Npgsql.PostgresErrorCodes.ObjectNotInPrerequisiteState, // "55000" - object_not_in_prerequisite_state
            Npgsql.PostgresErrorCodes.ObjectInUse, // "55006" - object_in_use
            Npgsql.PostgresErrorCodes.CantChangeRuntimeParam, // "55P02" - cant_change_runtime_param
            Npgsql.PostgresErrorCodes.LockNotAvailable, // "55P03" - lock_not_available

            // Class 57 — Operator Intervention
            Npgsql.PostgresErrorCodes.OperatorIntervention, // "57000" - operator_intervention
            Npgsql.PostgresErrorCodes.QueryCanceled, // "57014" - query_canceled
            Npgsql.PostgresErrorCodes.AdminShutdown, // "57P01" - admin_shutdown
            Npgsql.PostgresErrorCodes.CrashShutdown, // "57P02" - crash_shutdown
            Npgsql.PostgresErrorCodes.CannotConnectNow, // "57P03" - cannot_connect_now
            Npgsql.PostgresErrorCodes.DatabaseDropped, // "57P04" - database_dropped
            Npgsql.PostgresErrorCodes.IdleSessionTimeout, // "57P05" - idle_session_timeout

            // Class 58 — System Error (errors external to PostgreSQL itself)
            Npgsql.PostgresErrorCodes.SystemError, // "58000" - system_error
            Npgsql.PostgresErrorCodes.IoError, // "58030" - io_error
            Npgsql.PostgresErrorCodes.UndefinedFile, // "58P01" - undefined_file
            Npgsql.PostgresErrorCodes.DuplicateFile, // "58P02" - duplicate_file

            // Class 72 — Snapshot Failure
            Npgsql.PostgresErrorCodes.SnapshotFailure, // "72000" - snapshot_failure

            // Class F0 — Configuration File Error
            Npgsql.PostgresErrorCodes.ConfigFileError, // "F0000" - config_file_error
            Npgsql.PostgresErrorCodes.LockFileExists, // "F0001" - lock_file_exists
        }).ToList();

        // Remove the error codes we want to skip
        return allNgSqlErrorCodes.Except(errorCodesToIgnore).ToList();
    }

    private static IServiceCollection AddWriterConnection(this IServiceCollection services, IConfiguration configuration, int dbCommandTimeout, IList<string> errorCodesToRetryList)
    {
        // Configuration for writer DbContext
        services.AddDbContextFactory<OmsCoreAuditTrailWriterDbContext>((serviceProvider, options) =>
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            options.UseNpgsql(configuration.GetConnectionString(ConnectionStringPostgresWriterSection), ngSqlDabContextOptionsBuilder =>
            {
                // Use query splitting for performance improvements for read queries
                ngSqlDabContextOptionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

                // Set the retry strategy
                ngSqlDabContextOptionsBuilder.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(1),
                    errorCodesToAdd: errorCodesToRetryList
                );

                // Set the command timeout
                ngSqlDabContextOptionsBuilder.CommandTimeout(dbCommandTimeout);

                // Add logging for why we retry the queries when we do it
                ngSqlDabContextOptionsBuilder.ExecutionStrategy(dependencies =>
                    new NpgSqlRetryLoggerExecutionStrategy(dependencies, loggerFactory.CreateLogger<NpgSqlRetryLoggerExecutionStrategy>())
                );
            });

            // Because we want to see what we modified, we want to set the default tracking behaviour to track all
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            options.AddInterceptors(serviceProvider.GetServices<IInterceptor>());
        }, lifetime: ServiceLifetime.Scoped);

        return services;
    }

    private static IServiceCollection AddReaderConnection(this IServiceCollection services, IConfiguration configuration, int dbCommandTimeout, IList<string> errorCodesToRetryList)
    {
        // Configuration for reader DbContext
        services.AddDbContextFactory<OmsCoreAuditTrailReaderDbContext>((serviceProvider, options) =>
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            options.UseNpgsql(configuration.GetConnectionString(ConnectionStringPostgresReaderSection), ngSqlDabContextOptionsBuilder =>
            {
                // Use query splitting for performance improvements for read queries
                ngSqlDabContextOptionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

                // Set the retry strategy
                ngSqlDabContextOptionsBuilder.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(1),
                    errorCodesToAdd: errorCodesToRetryList
                );

                // Set the command timeout
                ngSqlDabContextOptionsBuilder.CommandTimeout(dbCommandTimeout);

                // Add logging for why we retry the queries when we do it
                ngSqlDabContextOptionsBuilder.ExecutionStrategy(dependencies =>
                    new NpgSqlRetryLoggerExecutionStrategy(dependencies, loggerFactory.CreateLogger<NpgSqlRetryLoggerExecutionStrategy>())
                );
            });

            // Never track the queries for the reader
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.AddInterceptors(serviceProvider.GetServices<IInterceptor>());
        }, lifetime: ServiceLifetime.Scoped);

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Do this out of the context factory so we only get these values once
        var dbCommandTimeout = GetDbCommandTimeout();

        // Add db context
        services.AddScoped<IInterceptor, DomainStateInterceptor>();
        // services.AddScoped<IInterceptor, LabelInterceptor>(); // TODO: Fix before enabling again
        //services.AddScoped<IInterceptor, SlowQueryExecutionInterceptor>();

        // Use a separate reader and writer connection
        services.AddWriterConnection(configuration: configuration, dbCommandTimeout: dbCommandTimeout, errorCodesToRetryList: GetWriterDbRetryErrorCodesList());
        services.AddReaderConnection(configuration: configuration, dbCommandTimeout: dbCommandTimeout, errorCodesToRetryList: GetReaderDbRetryErrorCodesList());

        return services;
    }
}
