using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Administration
{
    public interface IFacilityService
    {
        PagedResult<FacilityDto> GetPaged(int page, int pageSize);
        FacilityDto Create(FacilityDto facility);
        FacilityDto Update(FacilityDto facility);
        void Delete(int id);
    }
}
