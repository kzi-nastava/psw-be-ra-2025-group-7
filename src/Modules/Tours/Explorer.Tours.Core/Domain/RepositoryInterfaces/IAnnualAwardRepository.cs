using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IAnnualAwardRepository
{
    PagedResult<AnnualAward> GetPaged(int page, int pageSize);
    AnnualAward Get(long id);
    AnnualAward Create(AnnualAward entity);
    AnnualAward Update(AnnualAward entity);
    void Delete(long id);
}
