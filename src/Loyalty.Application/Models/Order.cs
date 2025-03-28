namespace Loyalty.Application.Models;

public class Order
{
    public long OrderId { get; set; }
    public required string OrderNumber { get; set; }
    public required string CustomerNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public required IEnumerable<OrderLine> Lines { get; set; }
    public required Delivery Delivery { get; set; }
}

public class  Delivery
{
    public Address? ShippingAddress { get; set; }
    public string? PickupPointCode { get; set; }
}

public class Address
{
    public required string Addressee { get; set; }
    public required string Street { get; set; }
    public required string StreetNumber { get; set; }
    public string? StreetNumberAddition { get; set; }
    public required PostalCode PostalCode { get; set; }
    public required City City { get; set; }
    public required Country Country { get; set; }
}

public class  PostalCode
{
    public required string Value { get; set; }
}

public class  City
{
    public required string Value { get; set; }
}

public class  Country
{
    public required string Name { get; set; }
    public required string TwoLetterCode { get; set; }
    public required string ThreeLetterCode { get; set; }
    public required string Value { get; set; }
}

public class OrderLine
{
    public long OrderLineId { get; set; }
    public required CurrentPrice CurrentPrice { get; set; }
    public required Article Article { get; set; }
}

public class CurrentPrice
{
    public decimal AmountInc { get; set; }
    public required VATCode VATCode { get; set; }
}

public class VATCode
{
    public required string Value { get; set; }
}

public class Article
{
    public string? ArticleGroup { get; set; }
}
