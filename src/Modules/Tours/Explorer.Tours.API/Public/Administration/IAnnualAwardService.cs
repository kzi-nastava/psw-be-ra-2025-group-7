using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration;

public interface IAnnualAwardService
{
    PagedResult<AnnualAwardDto> GetPaged(int page, int pageSize);
    AnnualAwardDto Create(CreateAnnualAwardDto dto);
    AnnualAwardDto Update(UpdateAnnualAwardDto dto);
    void Delete(long id);
}
