using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration;

public class AnnualAwardService : IAnnualAwardService
{
    private readonly IAnnualAwardRepository _annualAwardRepository;
    private readonly IMapper _mapper;

    public AnnualAwardService(IAnnualAwardRepository repository, IMapper mapper)
    {
        _annualAwardRepository = repository;
        _mapper = mapper;
    }

    public PagedResult<AnnualAwardDto> GetPaged(int page, int pageSize)
    {
        var result = _annualAwardRepository.GetPaged(page, pageSize);

        // Manual mapping to avoid AutoMapper issues with Entity base class
        var items = result.Results.Select(award => new AnnualAwardDto
        {
            Id = award.Id,
            Name = award.Name,
            Description = award.Description,
            Year = award.Year,
            Status = award.Status.ToString(),
            VotingStartDate = award.VotingStartDate,
            VotingEndDate = award.VotingEndDate
        }).ToList();

        return new PagedResult<AnnualAwardDto>(items, result.TotalCount);
    }

    public AnnualAwardDto Create(CreateAnnualAwardDto dto)
    {
        var award = new AnnualAward(
            dto.Name,
            dto.Description,
            dto.Year,
            dto.VotingStartDate,
            dto.VotingEndDate
        );

        var result = _annualAwardRepository.Create(award);

        // Manual mapping
        return new AnnualAwardDto
        {
            Id = result.Id,
            Name = result.Name,
            Description = result.Description,
            Year = result.Year,
            Status = result.Status.ToString(),
            VotingStartDate = result.VotingStartDate,
            VotingEndDate = result.VotingEndDate
        };
    }

    public AnnualAwardDto Update(UpdateAnnualAwardDto dto)
    {
        var award = _annualAwardRepository.Get(dto.Id);

        award.Update(
            dto.Name,
            dto.Description,
            dto.Year,
            dto.VotingStartDate,
            dto.VotingEndDate
        );

        var result = _annualAwardRepository.Update(award);

        // Manual mapping
        return new AnnualAwardDto
        {
            Id = result.Id,
            Name = result.Name,
            Description = result.Description,
            Year = result.Year,
            Status = result.Status.ToString(),
            VotingStartDate = result.VotingStartDate,
            VotingEndDate = result.VotingEndDate
        };
    }

    public void Delete(long id)
    {
        _annualAwardRepository.Delete(id);
    }
}