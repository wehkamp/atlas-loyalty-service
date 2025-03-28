using Loyalty.Domain.Core;

namespace Loyalty.Application.Repositories;

public interface ILabelRepository : IRepository
{
    AtlasLabel? GetLabel(string label);
}
