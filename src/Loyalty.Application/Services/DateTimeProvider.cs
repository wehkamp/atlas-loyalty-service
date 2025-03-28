namespace Loyalty.Application.Services;
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetNow() => DateTime.Now;
}

public interface IDateTimeProvider
{
    DateTime GetNow();
}
