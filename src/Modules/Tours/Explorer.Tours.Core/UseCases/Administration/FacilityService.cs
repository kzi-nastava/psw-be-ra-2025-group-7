using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public class FacilityService : IFacilityService
    {
        private readonly IFacilityRepository _facilityRepository;
        private readonly IMapper _mapper;

        public FacilityService(IFacilityRepository facilityRepository, IMapper mapper)
        {
            _facilityRepository = facilityRepository;
            _mapper = mapper;
        }

        public PagedResult<FacilityDto> GetPaged(int page, int pageSize)
        {
            var result = _facilityRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<FacilityDto>).ToList();
            return new PagedResult<FacilityDto>(items, result.TotalCount);
        }

        public FacilityDto Create(FacilityDto facility)
        {
            var entity = _mapper.Map<Facility>(facility);
            var result = _facilityRepository.Create(entity);
            return _mapper.Map<FacilityDto>(result);
        }

        public FacilityDto Update(FacilityDto facility)
        {
            var entity = _mapper.Map<Facility>(facility);
            var result = _facilityRepository.Update(entity);
            return _mapper.Map<FacilityDto>(result);
        }

        public void Delete(long id)
        {
            _facilityRepository.Delete(id);
        }
    }
}
