using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IFacilityRepository
    {
        PagedResult<Facility> GetPaged(int page, int pageSize);
        Facility Create(Facility facility);
        Facility Update(Facility facility);
        void Delete(long id);
        Facility? GetById(long id);
    }
}
