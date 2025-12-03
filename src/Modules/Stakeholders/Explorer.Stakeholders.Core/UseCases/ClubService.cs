using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;


namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;

        public ClubService(IClubRepository clubRepository, IMapper mapper)
        {
            _clubRepository = clubRepository;
            _mapper = mapper;
        }
        public ClubDto Create(ClubDto clubDto)
        {
            var entity = new Club(
                clubDto.Name,
                clubDto.Description,
                clubDto.CreatedBy,
                clubDto.ImageUrls
            );
            var created = _clubRepository.Create(entity);
            return _mapper.Map<ClubDto>(created);
        }

        public ClubDto Update(ClubDto clubDto)
        {
            var existing = _clubRepository.GetAll().FirstOrDefault(c => c.Id == clubDto.Id);
            if (existing == null)
            {
               
                throw new NotFoundException("Club not found.");
            }

            existing.Update(
                clubDto.Name,
                clubDto.Description,
                clubDto.ImageUrls
            );

            var updated = _clubRepository.Update(existing);

            return _mapper.Map<ClubDto>(updated);
        }

        public void Delete(long id)
        {
            var club = _clubRepository.GetAll().FirstOrDefault(c => c.Id == id);
            if (club == null)
            {
                throw new NotFoundException("Club not found.");
            }

            _clubRepository.Delete(id);
        }

        public List<ClubDto> GetAll()
        {
            var clubs = _clubRepository.GetAll();
            return clubs.Select(_mapper.Map<ClubDto>).ToList();
        }
    
    }
}
