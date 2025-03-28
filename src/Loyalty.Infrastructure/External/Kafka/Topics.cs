using System.Collections.Immutable;

namespace Loyalty.Infrastructure.External.Kafka;

public static class Topics
{
    public const string OmsCoreAuditTrailCommandsPublic = "oms-core-audit-trail-commands";
    public const string OmsCoreAuditTrailPrivateDbCommands = "oms-core-audit-trail-private-commands-db";
    public const string OmsCoreAuditTrailPrivateS3Commands = "oms-core-audit-trail-private-commands-s3";

    public static readonly IImmutableList<string> OmsCoreAuditTrailTopics =
    [
        OmsCoreAuditTrailCommandsPublic,
        OmsCoreAuditTrailPrivateDbCommands,
        OmsCoreAuditTrailPrivateS3Commands,
    ];
}
