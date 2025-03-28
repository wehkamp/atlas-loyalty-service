using Npgsql;

namespace Loyalty.Infrastructure.External.Database.Context.NgSql.ErrorCodes;

/// <summary>
/// Docs for the error codes: 
/// - https://www.postgresql.org/docs/current/errcodes-appendix.html
/// - https://www.postgresql.org/docs/current/plpgsql-errors-and-messages.html
/// </summary>
public static class PostgresErrorCodes
{
    private const string DefaultFallbackErrorMessage = "Unknown postgresql error code";

    public static readonly Dictionary<string, string> SqlStatesDictionary = new()
    {
        // Class 00 — Successful Completion
        { Npgsql.PostgresErrorCodes.SuccessfulCompletion, "Successful completion" },
        
        // Class 01 — Warning
        { Npgsql.PostgresErrorCodes.Warning, "Warning" },
        { Npgsql.PostgresErrorCodes.DynamicResultSetsReturnedWarning, "Dynamic result sets returned" },
        { Npgsql.PostgresErrorCodes.ImplicitZeroBitPaddingWarning, "Implicit zero-bit padding" },
        { Npgsql.PostgresErrorCodes.NullValueEliminatedInSetFunctionWarning, "Null value eliminated in set function" },
        { Npgsql.PostgresErrorCodes.PrivilegeNotGrantedWarning, "Privilege not granted" },
        { Npgsql.PostgresErrorCodes.PrivilegeNotRevokedWarning, "Privilege not revoked" },
        { Npgsql.PostgresErrorCodes.StringDataRightTruncationWarning, "String data right truncation" },
        { Npgsql.PostgresErrorCodes.DeprecatedFeatureWarning, "Deprecated feature" },

        // Class 02 — No Data
        { Npgsql.PostgresErrorCodes.NoData, "No data" },
        { Npgsql.PostgresErrorCodes.NoAdditionalDynamicResultSetsReturned, "No additional dynamic result sets returned" },
        
        // Class 03 — SQL Statement Not Yet Complete
        { Npgsql.PostgresErrorCodes.SqlStatementNotYetComplete, "SQL statement not yet complete" },
        
        // Class 08 — Connection Exception
        { Npgsql.PostgresErrorCodes.ConnectionException, "Connection exception" },
        { Npgsql.PostgresErrorCodes.ConnectionDoesNotExist, "Connection does not exist" },
        { Npgsql.PostgresErrorCodes.ConnectionFailure, "Connection failure" },
        { Npgsql.PostgresErrorCodes.SqlClientUnableToEstablishSqlConnection, "SQL client unable to establish SQL connection" },
        { Npgsql.PostgresErrorCodes.SqlServerRejectedEstablishmentOfSqlConnection, "SQL server rejected establishment of SQL connection" },
        { Npgsql.PostgresErrorCodes.TransactionResolutionUnknown, "Transaction resolution unknown" },
        { Npgsql.PostgresErrorCodes.ProtocolViolation, "Protocol violation" },
        
        // Class 09 — Triggered Action Exception
        { Npgsql.PostgresErrorCodes.TriggeredActionException, "Triggered action exception" },
        
        // Class 0A — Feature Not Supported
        { Npgsql.PostgresErrorCodes.FeatureNotSupported, "Feature not supported" },
        
        // Class 0B — Invalid Transaction Initiation
        { Npgsql.PostgresErrorCodes.InvalidTransactionInitiation, "Invalid transaction initiation" },
        
        // Class 0F — Locator Exception
        { Npgsql.PostgresErrorCodes.LocatorException, "Locator exception" },
        { Npgsql.PostgresErrorCodes.InvalidLocatorSpecification, "Invalid locator specification" },
        
        // Class 0L — Invalid Grantor
        { Npgsql.PostgresErrorCodes.InvalidGrantor, "Invalid grantor" },
        { Npgsql.PostgresErrorCodes.InvalidGrantOperation, "Invalid grant operation" },
        
        // Class 0P — Invalid Role Specification
        { Npgsql.PostgresErrorCodes.InvalidRoleSpecification, "Invalid role specification" },
        
        // Class 0Z — Diagnostics Exception
        { Npgsql.PostgresErrorCodes.DiagnosticsException, "Diagnostics exception" },
        { Npgsql.PostgresErrorCodes.StackedDiagnosticsAccessedWithoutActiveHandler, "Stacked diagnostics accessed without active handler" },
        
        // Class 20 — Case Not Found
        { Npgsql.PostgresErrorCodes.CaseNotFound, "Case not found" },
        
        // Class 21 — Cardinality Violation
        { Npgsql.PostgresErrorCodes.CardinalityViolation, "Cardinality violation" },
        
        // Class 22 — Data Exception
        { Npgsql.PostgresErrorCodes.DataException, "Data exception" },
        { Npgsql.PostgresErrorCodes.ArraySubscriptError, "Array subscript error" },
        { Npgsql.PostgresErrorCodes.CharacterNotInRepertoire, "Character not in repertoire" },
        { Npgsql.PostgresErrorCodes.DatetimeFieldOverflow, "Datetime field overflow" },
        { Npgsql.PostgresErrorCodes.DivisionByZero, "Division by zero" },
        { Npgsql.PostgresErrorCodes.ErrorInAssignment, "Error in assignment" },
        { Npgsql.PostgresErrorCodes.EscapeCharacterConflict, "Escape character conflict" },
        { Npgsql.PostgresErrorCodes.IndicatorOverflow, "Indicator overflow" },
        { Npgsql.PostgresErrorCodes.IntervalFieldOverflow, "Interval field overflow" },
        { Npgsql.PostgresErrorCodes.InvalidArgumentForLogarithm, "Invalid argument for logarithm" },
        { Npgsql.PostgresErrorCodes.InvalidArgumentForNtileFunction, "Invalid argument for ntile function" },
        { Npgsql.PostgresErrorCodes.InvalidArgumentForNthValueFunction, "Invalid argument for nth_value function" },
        { Npgsql.PostgresErrorCodes.InvalidArgumentForPowerFunction, "Invalid argument for power function" },
        { Npgsql.PostgresErrorCodes.InvalidArgumentForWidthBucketFunction, "Invalid argument for width_bucket function" },
        { Npgsql.PostgresErrorCodes.InvalidCharacterValueForCast, "Invalid character value for cast" },
        { Npgsql.PostgresErrorCodes.InvalidDatetimeFormat, "Invalid datetime format" },
        { Npgsql.PostgresErrorCodes.InvalidEscapeCharacter, "Invalid escape character" },
        { Npgsql.PostgresErrorCodes.InvalidEscapeOctet, "Invalid escape octet" },
        { Npgsql.PostgresErrorCodes.InvalidEscapeSequence, "Invalid escape sequence" },
        { Npgsql.PostgresErrorCodes.NonstandardUseOfEscapeCharacter, "Nonstandard use of escape character" },
        { Npgsql.PostgresErrorCodes.InvalidIndicatorParameterValue, "Invalid indicator parameter value" },
        { Npgsql.PostgresErrorCodes.InvalidParameterValue, "Invalid parameter value" },
        { Npgsql.PostgresErrorCodes.InvalidRegularExpression, "Invalid regular expression" },
        { Npgsql.PostgresErrorCodes.InvalidRowCountInLimitClause, "Invalid row count in limit clause" },
        { Npgsql.PostgresErrorCodes.InvalidRowCountInResultOffsetClause, "Invalid row count in result offset clause" },
        { Npgsql.PostgresErrorCodes.InvalidTablesampleArgument, "Invalid tablesample argument" },
        { Npgsql.PostgresErrorCodes.InvalidTablesampleRepeat, "Invalid tablesample repeat" },
        { Npgsql.PostgresErrorCodes.InvalidTimeZoneDisplacementValue, "Invalid time zone displacement value" },
        { Npgsql.PostgresErrorCodes.InvalidUseOfEscapeCharacter, "Invalid use of escape character" },
        { Npgsql.PostgresErrorCodes.MostSpecificTypeMismatch, "Most specific type mismatch" },
        { Npgsql.PostgresErrorCodes.NullValueNotAllowed, "Null value not allowed" },
        { Npgsql.PostgresErrorCodes.NullValueNoIndicatorParameter, "Null value no indicator parameter" },
        { Npgsql.PostgresErrorCodes.NumericValueOutOfRange, "Numeric value out of range" },
        { Npgsql.PostgresErrorCodes.StringDataLengthMismatch, "String data length mismatch" },
        { Npgsql.PostgresErrorCodes.StringDataRightTruncation, "String data right truncation" },
        { Npgsql.PostgresErrorCodes.SubstringError, "Substring error" },
        { Npgsql.PostgresErrorCodes.TrimError, "Trim error" },
        { Npgsql.PostgresErrorCodes.UnterminatedCString, "Unterminated C string" },
        { Npgsql.PostgresErrorCodes.ZeroLengthCharacterString, "Zero-length character string" },
        { Npgsql.PostgresErrorCodes.FloatingPointException, "Floating point exception" },
        { Npgsql.PostgresErrorCodes.InvalidTextRepresentation, "Invalid text representation" },
        { Npgsql.PostgresErrorCodes.InvalidBinaryRepresentation, "Invalid binary representation" },
        { Npgsql.PostgresErrorCodes.BadCopyFileFormat, "Bad copy file format" },
        { Npgsql.PostgresErrorCodes.UntranslatableCharacter, "Untranslatable character" },
        { Npgsql.PostgresErrorCodes.NotAnXmlDocument, "Not an XML document" },
        { Npgsql.PostgresErrorCodes.InvalidXmlDocument, "Invalid XML document" },
        { Npgsql.PostgresErrorCodes.InvalidXmlContent, "Invalid XML content" },
        { Npgsql.PostgresErrorCodes.InvalidXmlComment, "Invalid XML comment" },
        { Npgsql.PostgresErrorCodes.InvalidXmlProcessingInstruction, "Invalid XML processing instruction" },
        
        // Class 23 — Integrity Constraint Violation
        { Npgsql.PostgresErrorCodes.IntegrityConstraintViolation, "Integrity constraint violation" },
        { Npgsql.PostgresErrorCodes.RestrictViolation, "Restrict violation" },
        { Npgsql.PostgresErrorCodes.NotNullViolation, "Not null violation" },
        { Npgsql.PostgresErrorCodes.ForeignKeyViolation, "Foreign key violation" },
        { Npgsql.PostgresErrorCodes.UniqueViolation, "Unique violation" },
        { Npgsql.PostgresErrorCodes.CheckViolation, "Check violation" },
        { Npgsql.PostgresErrorCodes.ExclusionViolation, "Exclusion violation" },
        
        // Class 24 — Invalid Cursor State
        { Npgsql.PostgresErrorCodes.InvalidCursorState, "Invalid cursor state" },
        
        // Class 25 — Invalid Transaction State
        { Npgsql.PostgresErrorCodes.InvalidTransactionState, "Invalid transaction state" },
        { Npgsql.PostgresErrorCodes.ActiveSqlTransaction, "Active SQL transaction" },
        { Npgsql.PostgresErrorCodes.BranchTransactionAlreadyActive, "Branch transaction already active" },
        { Npgsql.PostgresErrorCodes.HeldCursorRequiresSameIsolationLevel, "Held cursor requires same isolation level" },
        { Npgsql.PostgresErrorCodes.InappropriateAccessModeForBranchTransaction, "Inappropriate access mode for branch transaction" },
        { Npgsql.PostgresErrorCodes.InappropriateIsolationLevelForBranchTransaction, "Inappropriate isolation level for branch transaction" },
        { Npgsql.PostgresErrorCodes.NoActiveSqlTransactionForBranchTransaction, "No active SQL transaction for branch transaction" },
        { Npgsql.PostgresErrorCodes.ReadOnlySqlTransaction, "Read-only SQL transaction" },
        { Npgsql.PostgresErrorCodes.SchemaAndDataStatementMixingNotSupported, "Schema and data statement mixing not supported" },
        { Npgsql.PostgresErrorCodes.NoActiveSqlTransaction, "No active SQL transaction" },
        { Npgsql.PostgresErrorCodes.InFailedSqlTransaction, "In failed SQL transaction" },
        
        // Class 26 — Invalid SQL Statement Name
        { Npgsql.PostgresErrorCodes.InvalidSqlStatementName, "Invalid SQL statement name" },
        
        // Class 27 — Triggered Data Change Violation
        { Npgsql.PostgresErrorCodes.TriggeredDataChangeViolation, "Triggered data change violation" },
        
        // Class 28 — Invalid Authorization Specification
        { Npgsql.PostgresErrorCodes.InvalidAuthorizationSpecification, "Invalid authorization specification" },
        { Npgsql.PostgresErrorCodes.InvalidPassword, "Invalid password" },
        
        // Class 2B — Dependent Privilege Descriptors Still Exist
        { Npgsql.PostgresErrorCodes.DependentPrivilegeDescriptorsStillExist, "Dependent privilege descriptors still exist" },
        { Npgsql.PostgresErrorCodes.DependentObjectsStillExist, "Dependent objects still exist" },
        
        // Class 2D — Invalid Transaction Termination
        { Npgsql.PostgresErrorCodes.InvalidTransactionTermination, "Invalid transaction termination" },
        
        // Class 2F — SQL Routine Exception
        { Npgsql.PostgresErrorCodes.SqlRoutineException, "SQL routine exception" },
        { Npgsql.PostgresErrorCodes.FunctionExecutedNoReturnStatementSqlRoutineException, "Function executed no return statement" },
        { Npgsql.PostgresErrorCodes.ModifyingSqlDataNotPermittedSqlRoutineException, "Modifying SQL data not permitted" },
        { Npgsql.PostgresErrorCodes.ProhibitedSqlStatementAttemptedSqlRoutineException, "Prohibited SQL statement attempted" },
        { Npgsql.PostgresErrorCodes.ReadingSqlDataNotPermittedSqlRoutineException, "Reading SQL data not permitted" },
        
        // Class 34 — Invalid Cursor Name
        { Npgsql.PostgresErrorCodes.InvalidCursorName, "Invalid cursor name" },
        
        // Class 38 — External Routine Exception
        { Npgsql.PostgresErrorCodes.ExternalRoutineException, "External routine exception" },
        { Npgsql.PostgresErrorCodes.ContainingSqlNotPermittedExternalRoutineException, "Containing SQL not permitted" },
        { Npgsql.PostgresErrorCodes.ModifyingSqlDataNotPermittedExternalRoutineException, "Modifying SQL data not permitted" },
        { Npgsql.PostgresErrorCodes.ProhibitedSqlStatementAttemptedExternalRoutineException, "Prohibited SQL statement attempted" },
        { Npgsql.PostgresErrorCodes.ReadingSqlDataNotPermittedExternalRoutineException, "Reading SQL data not permitted" },
        
        // Class 39 — External Routine Invocation Exception
        { Npgsql.PostgresErrorCodes.ExternalRoutineInvocationException, "External routine invocation exception" },
        { Npgsql.PostgresErrorCodes.InvalidSqlstateReturnedExternalRoutineInvocationException, "Invalid SQLSTATE returned" },
        { Npgsql.PostgresErrorCodes.NullValueNotAllowedExternalRoutineInvocationException, "Null value not allowed" },
        { Npgsql.PostgresErrorCodes.TriggerProtocolViolatedExternalRoutineInvocationException, "Trigger protocol violated" },
        { Npgsql.PostgresErrorCodes.SrfProtocolViolatedExternalRoutineInvocationException, "SRF protocol violated" },
        { Npgsql.PostgresErrorCodes.EventTriggerProtocolViolatedExternalRoutineInvocationException, "Event trigger protocol violated" },
        
        // Class 3B — Savepoint Exception
        { Npgsql.PostgresErrorCodes.SavepointException, "Savepoint exception" },
        { Npgsql.PostgresErrorCodes.InvalidSavepointSpecification, "Invalid savepoint specification" },
        
        // Class 3D — Invalid Catalog Name
        { Npgsql.PostgresErrorCodes.InvalidCatalogName, "Invalid catalog name" },
        
        // Class 3F — Invalid Schema Name
        { Npgsql.PostgresErrorCodes.InvalidSchemaName, "Invalid schema name" },
        
        // Class 40 — Transaction Rollback
        { Npgsql.PostgresErrorCodes.TransactionRollback, "Transaction rollback" },
        { Npgsql.PostgresErrorCodes.TransactionIntegrityConstraintViolation, "Transaction integrity constraint violation" },
        { Npgsql.PostgresErrorCodes.SerializationFailure, "Serialization failure" },
        { Npgsql.PostgresErrorCodes.StatementCompletionUnknown, "Statement completion unknown" },
        { Npgsql.PostgresErrorCodes.DeadlockDetected, "Deadlock detected" },
        
        // Class 42 — Syntax Error or Access Rule Violation
        { Npgsql.PostgresErrorCodes.SyntaxErrorOrAccessRuleViolation, "Syntax error or access rule violation" },
        { Npgsql.PostgresErrorCodes.SyntaxError, "Syntax error" },
        { Npgsql.PostgresErrorCodes.InsufficientPrivilege, "Insufficient privilege" },
        { Npgsql.PostgresErrorCodes.CannotCoerce, "Cannot coerce" },
        { Npgsql.PostgresErrorCodes.GroupingError, "Grouping error" },
        { Npgsql.PostgresErrorCodes.WindowingError, "Windowing error" },
        { Npgsql.PostgresErrorCodes.InvalidRecursion, "Invalid recursion" },
        { Npgsql.PostgresErrorCodes.InvalidForeignKey, "Invalid foreign key" },
        { Npgsql.PostgresErrorCodes.InvalidName, "Invalid name" },
        { Npgsql.PostgresErrorCodes.NameTooLong, "Name too long" },
        { Npgsql.PostgresErrorCodes.ReservedName, "Reserved name" },
        { Npgsql.PostgresErrorCodes.DatatypeMismatch, "Datatype mismatch" },
        { Npgsql.PostgresErrorCodes.IndeterminateDatatype, "Indeterminate datatype" },
        { Npgsql.PostgresErrorCodes.CollationMismatch, "Collation mismatch" },
        { Npgsql.PostgresErrorCodes.IndeterminateCollation, "Indeterminate collation" },
        { Npgsql.PostgresErrorCodes.WrongObjectType, "Wrong object type" },
        { Npgsql.PostgresErrorCodes.UndefinedColumn, "Undefined column" },
        { Npgsql.PostgresErrorCodes.UndefinedFunction, "Undefined function" },
        { Npgsql.PostgresErrorCodes.UndefinedTable, "Undefined table" },
        { Npgsql.PostgresErrorCodes.UndefinedParameter, "Undefined parameter" },
        { Npgsql.PostgresErrorCodes.UndefinedObject, "Undefined object" },
        { Npgsql.PostgresErrorCodes.DuplicateColumn, "Duplicate column" },
        { Npgsql.PostgresErrorCodes.DuplicateCursor, "Duplicate cursor" },
        { Npgsql.PostgresErrorCodes.DuplicateDatabase, "Duplicate database" },
        { Npgsql.PostgresErrorCodes.DuplicateFunction, "Duplicate function" },
        { Npgsql.PostgresErrorCodes.DuplicatePreparedStatement, "Duplicate prepared statement" },
        { Npgsql.PostgresErrorCodes.DuplicateSchema, "Duplicate schema" },
        { Npgsql.PostgresErrorCodes.DuplicateTable, "Duplicate table" },
        { Npgsql.PostgresErrorCodes.DuplicateAlias, "Duplicate alias" },
        { Npgsql.PostgresErrorCodes.DuplicateObject, "Duplicate object" },
        { Npgsql.PostgresErrorCodes.AmbiguousColumn, "Ambiguous column" },
        { Npgsql.PostgresErrorCodes.AmbiguousFunction, "Ambiguous function" },
        { Npgsql.PostgresErrorCodes.AmbiguousParameter, "Ambiguous parameter" },
        { Npgsql.PostgresErrorCodes.AmbiguousAlias, "Ambiguous alias" },
        { Npgsql.PostgresErrorCodes.InvalidColumnReference, "Invalid column reference" },
        { Npgsql.PostgresErrorCodes.InvalidColumnDefinition, "Invalid column definition" },
        { Npgsql.PostgresErrorCodes.InvalidCursorDefinition, "Invalid cursor definition" },
        { Npgsql.PostgresErrorCodes.InvalidDatabaseDefinition, "Invalid database definition" },
        { Npgsql.PostgresErrorCodes.InvalidFunctionDefinition, "Invalid function definition" },
        { Npgsql.PostgresErrorCodes.InvalidPreparedStatementDefinition, "Invalid prepared statement definition" },
        { Npgsql.PostgresErrorCodes.InvalidSchemaDefinition, "Invalid schema definition" },
        { Npgsql.PostgresErrorCodes.InvalidTableDefinition, "Invalid table definition" },
        { Npgsql.PostgresErrorCodes.InvalidObjectDefinition, "Invalid object definition" },
        
        // Class 44 — WITH CHECK OPTION Violation
        { Npgsql.PostgresErrorCodes.WithCheckOptionViolation, "WITH CHECK OPTION violation" },
        
        // Class 53 — Insufficient Resources
        { Npgsql.PostgresErrorCodes.InsufficientResources, "Insufficient resources" },
        { Npgsql.PostgresErrorCodes.DiskFull, "Disk full" },
        { Npgsql.PostgresErrorCodes.OutOfMemory, "Out of memory" },
        { Npgsql.PostgresErrorCodes.TooManyConnections, "Too many connections" },
        { Npgsql.PostgresErrorCodes.ConfigurationLimitExceeded, "Configuration limit exceeded" },
        
        // Class 54 — Program Limit Exceeded
        { Npgsql.PostgresErrorCodes.ProgramLimitExceeded, "Program limit exceeded" },
        { Npgsql.PostgresErrorCodes.StatementTooComplex, "Statement too complex" },
        { Npgsql.PostgresErrorCodes.TooManyColumns, "Too many columns" },
        { Npgsql.PostgresErrorCodes.TooManyArguments, "Too many arguments" },
        
        // Class 55 — Object Not In Prerequisite State
        { Npgsql.PostgresErrorCodes.ObjectNotInPrerequisiteState, "Object not in prerequisite state" },
        { Npgsql.PostgresErrorCodes.ObjectInUse, "Object in use" },
        { Npgsql.PostgresErrorCodes.CantChangeRuntimeParam, "Can't change runtime param" },
        { Npgsql.PostgresErrorCodes.LockNotAvailable, "Lock not available" },
        
        // Class 57 — Operator Intervention
        { Npgsql.PostgresErrorCodes.OperatorIntervention, "Operator intervention" },
        { Npgsql.PostgresErrorCodes.QueryCanceled, "Query canceled" },
        { Npgsql.PostgresErrorCodes.AdminShutdown, "Admin shutdown" },
        { Npgsql.PostgresErrorCodes.CrashShutdown, "Crash shutdown" },
        { Npgsql.PostgresErrorCodes.CannotConnectNow, "Cannot connect now" },
        { Npgsql.PostgresErrorCodes.DatabaseDropped, "Database dropped" },
        { Npgsql.PostgresErrorCodes.IdleSessionTimeout, "Idle session timeout" },
        
        // Class 58 — System Error (errors external to PostgreSQL itself)
        { Npgsql.PostgresErrorCodes.SystemError, "System error" },
        { Npgsql.PostgresErrorCodes.IoError, "I/O error" },
        { Npgsql.PostgresErrorCodes.UndefinedFile, "Undefined file" },
        { Npgsql.PostgresErrorCodes.DuplicateFile, "Duplicate file" },
        
        // Class 72 — Snapshot Failure
        { Npgsql.PostgresErrorCodes.SnapshotFailure, "Snapshot failure" },
        
        // Class F0 — Configuration File Error
        { Npgsql.PostgresErrorCodes.ConfigFileError, "Configuration file error" },
        { Npgsql.PostgresErrorCodes.LockFileExists, "Lock file exists" },
        
        // Class HV — Foreign Data Wrapper Error (SQL/MED)
        { Npgsql.PostgresErrorCodes.FdwError, "FDW error" },
        { Npgsql.PostgresErrorCodes.FdwColumnNameNotFound, "FDW column name not found" },
        { Npgsql.PostgresErrorCodes.FdwDynamicParameterValueNeeded, "FDW dynamic parameter value needed" },
        { Npgsql.PostgresErrorCodes.FdwFunctionSequenceError, "FDW function sequence error" },
        { Npgsql.PostgresErrorCodes.FdwInconsistentDescriptorInformation, "FDW inconsistent descriptor information" },
        { Npgsql.PostgresErrorCodes.FdwInvalidAttributeValue, "FDW invalid attribute value" },
        { Npgsql.PostgresErrorCodes.FdwInvalidColumnName, "FDW invalid column name" },
        { Npgsql.PostgresErrorCodes.FdwInvalidColumnNumber, "FDW invalid column number" },
        { Npgsql.PostgresErrorCodes.FdwInvalidDataType, "FDW invalid data type" },
        { Npgsql.PostgresErrorCodes.FdwInvalidDataTypeDescriptors, "FDW invalid data type descriptors" },
        { Npgsql.PostgresErrorCodes.FdwInvalidDescriptorFieldIdentifier, "FDW invalid descriptor field identifier" },
        { Npgsql.PostgresErrorCodes.FdwInvalidHandle, "FDW invalid handle" },
        { Npgsql.PostgresErrorCodes.FdwInvalidOptionIndex, "FDW invalid option index" },
        { Npgsql.PostgresErrorCodes.FdwInvalidOptionName, "FDW invalid option name" },
        { Npgsql.PostgresErrorCodes.FdwInvalidStringLengthOrBufferLength, "FDW invalid string length or buffer length" },
        { Npgsql.PostgresErrorCodes.FdwInvalidStringFormat, "FDW invalid string format" },
        { Npgsql.PostgresErrorCodes.FdwInvalidUseOfNullPointer, "FDW invalid use of null pointer" },
        { Npgsql.PostgresErrorCodes.FdwTooManyHandles, "FDW too many handles" },
        { Npgsql.PostgresErrorCodes.FdwOutOfMemory, "FDW out of memory" },
        { Npgsql.PostgresErrorCodes.FdwNoSchemas, "FDW no schemas" },
        { Npgsql.PostgresErrorCodes.FdwOptionNameNotFound, "FDW option name not found" },
        { Npgsql.PostgresErrorCodes.FdwReplyHandle, "FDW reply handle" },
        { Npgsql.PostgresErrorCodes.FdwSchemaNotFound, "FDW schema not found" },
        { Npgsql.PostgresErrorCodes.FdwTableNotFound, "FDW table not found" },
        { Npgsql.PostgresErrorCodes.FdwUnableToCreateExecution, "FDW unable to create execution" },
        { Npgsql.PostgresErrorCodes.FdwUnableToCreateReply, "FDW unable to create reply" },
        { Npgsql.PostgresErrorCodes.FdwUnableToEstablishConnection, "FDW unable to establish connection" },
        
        // Class P0 — PL/pgSQL Error
        { Npgsql.PostgresErrorCodes.PlpgsqlError, "PL/pgSQL error" },
        { Npgsql.PostgresErrorCodes.RaiseException, "Raise exception" },
        { Npgsql.PostgresErrorCodes.NoDataFound, "No data found" },
        { Npgsql.PostgresErrorCodes.TooManyRows, "Too many rows" },
        { Npgsql.PostgresErrorCodes.AssertFailure, "Assert failure" },
        
        // Class XX — Internal Error
        { Npgsql.PostgresErrorCodes.InternalError, "Internal error" },
        { Npgsql.PostgresErrorCodes.DataCorrupted, "Data corrupted" },
        { Npgsql.PostgresErrorCodes.IndexCorrupted, "Index corrupted" }
    };

    public static string GetErrorDescription(NpgsqlException npgsqlException)
    {
        if (string.IsNullOrWhiteSpace(npgsqlException.SqlState))
        {
            return DefaultFallbackErrorMessage;
        }

        return GetErrorDescription(npgsqlException.SqlState);
    }

    public static string GetErrorDescription(PostgresException postgresException)
        => GetErrorDescription(postgresException.SqlState);

    public static string GetErrorDescription(string sqlState)
        => SqlStatesDictionary.TryGetValue(sqlState, out var description) ? description : DefaultFallbackErrorMessage;
}
