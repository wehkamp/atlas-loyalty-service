using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loyalty.Api.Controllers.LoyaltyProgram;

public record MyLoyaltyDiscountDto
{
    public required string[] CouponCodes { get; set; }
}

[ApiController]
[Route("loyalty-program")]
public class CustomerNumberLoyaltyEngagementController : ControllerBase
{
    [HttpGet]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IActionResult> PostLoyaltyCodes([FromQuery] string customerNumber)
    {
        // Does this need to be from our database or the coupon database?
        await Task.Delay(1);

        var loyaltyDiscountDto = new MyLoyaltyDiscountDto
        {
            CouponCodes = ["123", "456"]
        };

        return Ok(loyaltyDiscountDto);
    }
}
