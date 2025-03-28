using Loyalty.Application.Models;
using System.Collections.Generic;

namespace Loyalty.Domain.UnitTests.Helpers.Builders;
public class OrderBuilder
{
    public static OrderBuilder ValidOrder => new OrderBuilder()
        .OrderId(1)
        .OrderNumber("L-1")
        .CustomerNumber("C-1")
        .TotalAmount(100)
        .Delivery(DeliveryBuilder.Valid.Build())
        .OrderLines(
        [
            new OrderLine
            {
                OrderLineId = 1,
                CurrentPrice = new CurrentPrice
                {
                    AmountInc = 50,
                    VATCode = new VATCode
                    {
                        Value = "V6"
                    }
                },
                Article = new Article
                {
                    ArticleGroup = "123",
                },
            },
        ]);

    private long _orderId;
    private string _orderNumber = "";
    private string _customerNumber = "";
    private decimal _totalAmount;
    private Delivery? _delivery;
    private IEnumerable<OrderLine> _orderLines = [];

    public OrderBuilder OrderId(long orderId)
    {
        _orderId = orderId;
        return this;
    }

    public OrderBuilder OrderNumber(string orderNumber)
    {
        _orderNumber = orderNumber;
        return this;
    }

    public OrderBuilder CustomerNumber(string customerNumber)
    {
        _customerNumber = customerNumber;
        return this;
    }

    public OrderBuilder TotalAmount(decimal totalAmount)
    {
        _totalAmount = totalAmount;
        return this;
    }

    public OrderBuilder Delivery(Delivery delivery)
    {
        _delivery = delivery;
        return this;
    }

    public OrderBuilder OrderLines(IEnumerable<OrderLine> orderLines)
    {
        _orderLines = orderLines;
        return this;
    }

    public Order Build()
    {
        return new Order
        {
            OrderId = _orderId,
            OrderNumber = _orderNumber,
            CustomerNumber = _customerNumber,
            TotalAmount = _totalAmount,
            Lines = _orderLines,
            Delivery = _delivery!,
        };
    }
}
