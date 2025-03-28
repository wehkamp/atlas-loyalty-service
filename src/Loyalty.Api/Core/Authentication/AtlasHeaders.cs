using Microsoft.AspNetCore.Http;
using System;

namespace Loyalty.Api.Core.Authentication;

public record AtlasHeaders(
    string? Shopper,
    string? Level,
    string? TokenId,
    DateTime? IssuedAt,
    string? Issuer,
    string? PreAuthenticationShopper,
    string? LabelId)
{
    public const string AtlasLabel = "Atlas-Label";

    static string? NullWhenEmpty(string value) => string.IsNullOrEmpty(value) ? null : value;
    public static AtlasHeaders FromHeaders(IHeaderDictionary headers)
    {
        var authShopper = headers["Atlas-Auth-Shopper"].ToString();
        var authLevel = headers["Atlas-Auth-Level"].ToString();
        var authTokenId = headers["Atlas-Auth-TokenId"].ToString();
        var issuedAt = headers["Atlas-Auth-IssuedAt"].ToString();
        var authIssuer = headers["Atlas-Auth-Issuer"].ToString();
        var preAuthenticationShopper = headers["Atlas-Auth-Pre-Authentication-Shopper"].ToString();
        var label = headers[AtlasLabel].ToString();
        
        return new AtlasHeaders(
            NullWhenEmpty(authShopper),
            NullWhenEmpty(authLevel),
            NullWhenEmpty(authTokenId),
            string.IsNullOrEmpty(issuedAt) ? null : DateTime.Parse(issuedAt),
            NullWhenEmpty(authIssuer),
            NullWhenEmpty(preAuthenticationShopper),
            NullWhenEmpty(label)
        );
    }
}
