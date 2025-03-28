using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace Loyalty.Api.Controllers.AuditRecords;

[ApiController]
[Route("loyalty-progran")]
public class PostLoyaltyProgramDiscountsController : ControllerBase
{
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IActionResult> PostLoyaltyCodes(
        string[] discountCodes, string[] customerNumbers, string rfmLevel)
    {
        


        return Ok("");
    }
}
