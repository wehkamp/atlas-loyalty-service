namespace Loyalty.Infrastructure.External.Kafka.Exceptions;

public class LabelNotFoundException : Exception
{
    public LabelNotFoundException(string label) : base($"AtlasLabel '{label}' not configured for this service.")
    { }
}
