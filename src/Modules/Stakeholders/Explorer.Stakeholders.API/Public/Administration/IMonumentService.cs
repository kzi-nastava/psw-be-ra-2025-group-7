using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public.Administration;

public interface IMonumentService
{
    PagedResult<MonumentDto> GetPaged(int page, int pageSize);
    MonumentDto Create(MonumentDto monument);
    MonumentDto Update(MonumentDto monument);
    void Delete(long id);
}
