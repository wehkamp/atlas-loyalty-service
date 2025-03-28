namespace Loyalty.Infrastructure.External.Database.Context.Exceptions;

public class DbActionsAreOnlyAllowedOnWriterContextException : Exception
{
    public DbActionsAreOnlyAllowedOnWriterContextException() : base("Database actions are only allowed on writer context.")
    { }
}
