namespace Loyalty.Domain.UnitTests.Helpers.Builders;
public class CustomerBuilder
{
    public static Customer.Customer ValidCustomer => new CustomerBuilder().Build();

    private string _customerId = "l-1";
    private string _label = "debug-localhost";
    private long _id = 1;

    public CustomerBuilder WithCustomerId(string customerId)
    {
        _customerId = customerId;
        return this;
    }

    public CustomerBuilder WithLabel(string label)
    {
        _label = label;
        return this;
    }

    public CustomerBuilder WithId(long id)
    {
        _id = id;
        return this;
    }

    public Customer.Customer Build()
    {
        return new Customer.Customer
        {
            Id = _id,
            CustomerId = _customerId,
            Label = _label
        };
    }
}
