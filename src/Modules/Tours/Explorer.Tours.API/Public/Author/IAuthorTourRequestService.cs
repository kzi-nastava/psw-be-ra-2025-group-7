using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Author
{
    public interface IAuthorTourRequestService
    {
        PagedResult<AuthorTourRequestListItemDto> GetOpen(
            int page,
            int pageSize,
            long authorId,
            decimal? minBudget,
            decimal? maxBudget,
            int? difficulty
        );

        AuthorTourRequestDetailsDto GetDetails(long requestId, long authorId);

        TourRequestResponseDto CreateResponse(CreateTourRequestResponseDto dto, long authorId);

        List<AuthorResponseItemDto> GetMyResponses(long authorId);

        TourRequestResponseDto UpdateMyResponse(long responseId, long authorId, UpdateMyResponseDto dto);
        void DeleteMyResponse(long responseId, long authorId);


    }
}