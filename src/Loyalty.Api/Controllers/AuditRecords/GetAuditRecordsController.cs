using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Loyalty.Application.Models;
using Loyalty.Application.Models.Responses.AuditRecords;
using Loyalty.Application.Queries;
using Loyalty.Application.Queries.AuditRecords;
using Loyalty.Domain.Core;
using System;
using System.Threading.Tasks;

namespace Loyalty.Api.Controllers.AuditRecords;

[ApiController]
[Route("audit-records")]
public class GetAuditRecordsController : ControllerBase
{
    private readonly IQueryHandler<GetAuditRecordsQuery, PageResponse<AuditRecordResponse>> _queryHandler;
    private readonly IQueryHandler<GetAuditRecordDetailsQuery, AuditDetailsResponse> _queryDetailsHandler;

    public GetAuditRecordsController(IQueryHandler<GetAuditRecordsQuery, PageResponse<AuditRecordResponse>> queryHandler, IQueryHandler<GetAuditRecordDetailsQuery, AuditDetailsResponse> queryDetailsHandler)
    {
        _queryHandler = queryHandler;
        _queryDetailsHandler = queryDetailsHandler;
    }

    /// <summary>
    /// Returns all audit records matching the given query
    /// </summary>
    /// <param name="orderNumber">Only records for the given order number are returned</param>
    /// <param name="skip">Bypasses the specified number of elements in the sequence and then returns the remaining elements</param>
    /// <param name="take">Returns the given number of elements from the sequence</param>
    /// <param name="types">Only records of the given type are returned</param>
    /// <returns></returns>
    [HttpGet]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PageResponse<AuditRecordResponse>))]
    public async Task<IActionResult> GetAuditRecordsAsync(
        [FromQuery] string orderNumber,
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromQuery] string[]? types
    )
    {
        var result = await _queryHandler.ExecuteAsync(new GetAuditRecordsQuery
        {
            OrderNumber = new OrderNumber(orderNumber),
            Skip = skip,
            Take = take,
            AuditRecordTypes = types
        }, HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Returns details about a specific audit record
    /// </summary>
    /// <param name="orderNumber">Only record for the given order number is returned</param>
    /// <param name="documentId">Only record for the given documentId is returned</param>
    /// <param name="type">Only record for the given type is returned</param>
    /// <returns></returns>
    [HttpGet("details")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuditDetailsResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAuditRecordDetailsAsync(
        [FromQuery] string orderNumber,
        [FromQuery] Guid documentId,
        [FromQuery] string type
    )
    {
        var result = await _queryDetailsHandler.ExecuteAsync(new GetAuditRecordDetailsQuery
        {
            OrderNumber = new OrderNumber(orderNumber),
            DocumentId = documentId,
            Type = type,
        }, HttpContext.RequestAborted);

        return Ok(result);
    }
}
