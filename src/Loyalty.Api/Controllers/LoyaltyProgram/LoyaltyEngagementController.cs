using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loyalty.Api.Controllers.LoyaltyProgram;

public record LoyaltyDiscountDto
{
    public required string[] CustomerNumbers { get; set; }
    public required string[] ArticleNumbers { get; set; }
    public required string RfmLevel { get; set; }
    public required DateTime StartDateTime { get; set; }
    public required DateTime EndDateTime { get; set; }
    public double? MinimumOrderAmount { get; set; } // Do we want to set a fixed minimum order amount? we assume fixed amount
}

[ApiController]
[Route("loyalty-program")]
public class LoyaltyEngagementController : ControllerBase
{
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IActionResult> PostLoyaltyCodes([FromBody] LoyaltyDiscountDto loyaltyDiscountDto)
    {
        // Save record to the database and see progress
        // Publish command to kafka for puma
        //   Puma: Campagnetag Loyalty-RfmLevel-StartDateTime-EndDateTime or something with weeks?
        //   Puma: List article numbers
        //   Puma command success => result of puma group id => event with customernumbers
        //   Puma command success => save to the database

        //   Puma event => coupon command
        //   create coupon command 
        //   create coupon command => coupon created event
        //   save the coupon created event next to the puma event in the database

        await Task.Delay(1);

        return Ok("");
    }
}
