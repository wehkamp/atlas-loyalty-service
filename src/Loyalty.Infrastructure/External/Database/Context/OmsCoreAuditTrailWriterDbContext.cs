using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Loyalty.Infrastructure.External.Database.Context;
public class OmsCoreAuditTrailWriterDbContext : OmsCoreAuditTrailDbContext
{
    public OmsCoreAuditTrailWriterDbContext(DbContextOptions<OmsCoreAuditTrailDbContext> options) : base(options)
    { }

    [ActivatorUtilitiesConstructor]
    public OmsCoreAuditTrailWriterDbContext(DbContextOptions<OmsCoreAuditTrailWriterDbContext> options) : base(options)
    { }
}
