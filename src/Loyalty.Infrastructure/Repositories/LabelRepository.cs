using Loyalty.Application.Repositories;
using Loyalty.Domain.Core;
using Microsoft.Extensions.Configuration;

namespace Loyalty.Infrastructure.Repositories;

public class LabelRepository : ILabelRepository
{
    private readonly IList<string> _knownLabelsList;
    public const string KnownLabelsKey = "Atlas-Labels";

    public LabelRepository(IConfiguration configuration)
    {
        _knownLabelsList = configuration.GetSection(KnownLabelsKey).Get<List<string>>() ?? new List<string>();
    }

    public AtlasLabel? GetLabel(string label)
    {
        if (!_knownLabelsList.Contains(label))
        {
            return null;
        }

        return new AtlasLabel(label);
    }
}
