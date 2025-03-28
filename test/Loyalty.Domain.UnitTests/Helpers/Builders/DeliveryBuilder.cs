using Loyalty.Application.Models;

namespace Loyalty.Domain.UnitTests.Helpers.Builders;
public class DeliveryBuilder
{
    public static DeliveryBuilder Valid => new DeliveryBuilder()
        .Address(new Address
        {
            Addressee = "Jan de Vries",
            Street = "Kerstraat",
            StreetNumber = "1",
            StreetNumberAddition = "a",
            PostalCode = new PostalCode
            {
                Value = "8041AA"
            },
            City = new City
            {
                Value = "Zwolle"
            },
            Country = new Country
            {
                Name = "Netherlands",
                TwoLetterCode = "NL",
                ThreeLetterCode = "NLD",
                Value = "NL"
            }
        });

    public static DeliveryBuilder ValidPickupPoint => Valid
        .Address()
        .PickupPointCode("NL-1234");

    private Address? _address;
    private string? _pickupPointCode;

    public DeliveryBuilder Address(Address? address = null)
    {
        _address = address;
        return this;
    }

    public DeliveryBuilder PickupPointCode(string? pickupPointCode = null)
    {
        _pickupPointCode = pickupPointCode;
        return this;
    }

    public DeliveryBuilder Country(Country country)
    {
        _address!.Country = country;
        return this;
    }

    public DeliveryBuilder Netherlands
        => Country(new Country
        {
            Name = "Netherlands",
            TwoLetterCode = "NL",
            ThreeLetterCode = "NLD",
            Value = "NL"
        });

    public DeliveryBuilder Belgium
        => Country(new Country
        {
            Name = "Belgium",
            TwoLetterCode = "BE",
            ThreeLetterCode = "BEL",
            Value = "BE"
        });

    public Delivery Build()
    {
        return new Delivery
        {
            ShippingAddress = _address,
            PickupPointCode = _pickupPointCode
        };
    }
}
