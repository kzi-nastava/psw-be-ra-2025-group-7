using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IClubMessageRepository
{
    ClubMessage Create(ClubMessage message);
    ClubMessage Get(long id);
    ClubMessage Update(ClubMessage message);
    void Delete(long id);
    PagedResult<ClubMessage> GetByClub(long clubId, int page, int pageSize);
    Club GetClub(long clubId);  // Helper za proveru da li klub postoji i vlasništva
}
