using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;
        private readonly INotificationRepository _notificationRepository;

        public ClubService(
            IClubRepository clubRepository,
            IMapper mapper,
            INotificationRepository notificationRepository)
        {
            _clubRepository = clubRepository;
            _mapper = mapper;
            _notificationRepository = notificationRepository;
        }

        public ClubDto Create(ClubDto dto)
        {
            var club = new Club(dto.Name, dto.Description, dto.CreatedBy, dto.ImageUrls);
            var created = _clubRepository.Create(club);
            return MapClubToDto(created);
        }

        public ClubDto Update(ClubDto dto)
        {
            var club = _clubRepository.Get(dto.Id);
            club.Update(dto.Name, dto.Description, dto.ImageUrls);
            var updated = _clubRepository.Update(club);
            return MapClubToDto(updated);
        }

        public void Delete(long id)
        {
            _clubRepository.Delete(id);
        }

        public List<ClubDto> GetAll()
        {
            return _clubRepository.GetAll()
                .Select(c => MapClubToDto(c))
                .ToList();
        }

        public ClubDto Get(long id)
        {
            var club = _clubRepository.Get(id);
            return MapClubToDto(club);
        }

        public void Close(long clubId, long ownerId)
        {
            var club = _clubRepository.Get(clubId);
            EnsureOwner(club, ownerId);

            club.Close();
            _clubRepository.Update(club);
        }

        public void Open(long clubId, long ownerId)
        {
            var club = _clubRepository.Get(clubId);
            EnsureOwner(club, ownerId);

            club.Open();
            _clubRepository.Update(club);
        }

        public void RequestMembership(long clubId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            club.RequestMembership(touristId);
            _clubRepository.Update(club);
        }

        public void WithdrawRequest(long clubId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            club.WithdrawRequest(touristId);
            _clubRepository.Update(club);
        }

        public void AcceptRequest(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            EnsureOwner(club, ownerId);

            club.AcceptRequest(touristId);
            _clubRepository.Update(club);

            var notification = new Notification(touristId, clubId, NotificationType.JoinRequestAccepted);
            _notificationRepository.Create(notification);
        }

        public void RejectRequest(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            EnsureOwner(club, ownerId);

            club.RejectRequest(touristId);
            _clubRepository.Update(club);

            var notification = new Notification(touristId, clubId, NotificationType.JoinRequestRejected);
            _notificationRepository.Create(notification);
        }

        public void InviteTourist(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            EnsureOwner(club, ownerId);

            club.InviteTourist(touristId);
            _clubRepository.Update(club);
        }

        public void AcceptInvitation(long clubId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            club.AcceptInvitation(touristId);
            _clubRepository.Update(club);
        }

        public void RejectInvitation(long clubId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            club.RejectInvitation(touristId);
            _clubRepository.Update(club);
        }

        public void RemoveMember(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            EnsureOwner(club, ownerId);

            if (touristId == ownerId)
                throw new InvalidOperationException("Owner cannot remove themself.");

            club.RemoveMember(touristId);
            _clubRepository.Update(club);
        }

        private void EnsureOwner(Club club, long ownerId)
        {
            if (club.CreatedBy != ownerId)
                throw new ForbiddenException("Only the owner can perform this action.");
        }

        private ClubDto MapClubToDto(Club club)
        {
            if (club == null) return null;
            var dto = new ClubDto
            {
                Id = club.Id,
                Name = club.Name,
                Description = club.Description,
                ImageUrls = club.ImageUrls?.ToList() ?? new List<string>(),
                CreatedBy = club.CreatedBy,
                CreatedAt = club.CreatedAt,
                UpdatedAt = club.UpdatedAt,
                Status = (ClubStatusDto)club.Status
            };

            dto.Members = new List<ClubMemberDto>();
            dto.JoinRequests = new List<ClubJoinRequestDto>();
            dto.Invitations = new List<ClubInvitationDto>();

            // Members
            if (club.Members != null)
            {
                dto.Members = club.Members
                    .Select(m => new ClubMemberDto
                    {
                        TouristId = m.TouristId,
                        JoinedAt = m.JoinedAt
                    })
                    .ToList();
            }

            // JoinRequests
            if (club.JoinRequests != null)
            {
                dto.JoinRequests = club.JoinRequests
                    .Select(r => new ClubJoinRequestDto
                    {
                        TouristId = r.TouristId,
                        RequestedAt = r.RequestedAt
                    })
                    .ToList();
            }

            // Invitations
            if (club.Invitations != null)
            {
                dto.Invitations = club.Invitations
                    .Select(i => new ClubInvitationDto
                    {
                        TouristId = i.TouristId,
                        SentAt = i.SentAt
                    })
                    .ToList();
            }

            return dto;
        }

    }
}
