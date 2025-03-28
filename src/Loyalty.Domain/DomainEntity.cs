using Loyalty.Domain.Exceptions;

namespace Loyalty.Domain;
public abstract class DomainEntity
{
    private DateTime? _creationDateTime;

    /// <summary>
    /// Date and time of the creation of the order
    /// </summary>
    public DateTime? CreationDateTime
    {
        get => _creationDateTime;
        set
        {
            if (_creationDateTime.HasValue && value is null)
            {
                throw new DomainException("Not allowed to null CreationDateTime");
            }

            if (_creationDateTime.HasValue && value is not null && Math.Abs(_creationDateTime.Value.Subtract(value.Value).Ticks) > 10)
            {
                throw new DomainException("Not allowed to change CreationDateTime");
            }

            _creationDateTime = value;
        }
    }

    /// <summary>
    /// Date and time of the last mutation on the order
    /// </summary>
    public DateTime? MutationDateTime { get; set; }
}
