using Explorer.Stakeholders.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubService
    {
        ClubDto Create(ClubDto club);
        ClubDto Update(ClubDto club);
        void Delete(long id);
        List<ClubDto> GetAll();
        ClubDto Get(long id);

        void Close(long clubId, long ownerId);
        void Open(long clubId, long ownerId);
        void RequestMembership(long clubId, long touristId);
        void WithdrawRequest(long clubId, long touristId);
        void AcceptRequest(long clubId, long ownerId, long touristId);
        void RejectRequest(long clubId, long ownerId, long touristId);
        void InviteTourist(long clubId, long ownerId, long touristId);
        void AcceptInvitation(long clubId, long touristId);
        void RejectInvitation(long clubId, long touristId);
        void RemoveMember(long clubId, long ownerId, long touristId);
    }
}
