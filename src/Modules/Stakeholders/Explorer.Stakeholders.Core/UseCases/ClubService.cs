using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly INotificationRepository _notificationRepository;

        public ClubService(IClubRepository clubRepository, INotificationRepository notificationRepository)
        {
            _clubRepository = clubRepository;
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

            var name = string.IsNullOrWhiteSpace(dto.Name) ? club.Name : dto.Name;
            var description = string.IsNullOrWhiteSpace(dto.Description) ? club.Description : dto.Description;

            // Update: dodaj nove slike (ako postoje), ali ne briši stare ovde
            var imageUrlsToPersist = club.ImageUrls?.ToList() ?? new List<string>();

            if (dto.ImageUrls != null && dto.ImageUrls.Count > 0)
            {
                imageUrlsToPersist = imageUrlsToPersist
                    .Concat(dto.ImageUrls)
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .Select(u => u.Trim())
                    .Distinct()
                    .ToList();
            }

            club.Update(name, description, imageUrlsToPersist);

            var updated = _clubRepository.Update(club);
            return MapClubToDto(updated);
        }

        public void DeleteImage(long clubId, long ownerId, string fileName)
        {
            var club = _clubRepository.Get(clubId);
            EnsureOwner(club, ownerId);

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("fileName is required.");

            var safeFileName = System.IO.Path.GetFileName(fileName).Trim();
            var current = club.ImageUrls?.ToList() ?? new List<string>();

            var newList = current
                .Where(u => !EndsWithFileName(u, safeFileName))
                .Distinct()
                .ToList();

            // idempotent: ok i ako nije obrisalo ništa
            club.Update(club.Name, club.Description, newList);
            _clubRepository.Update(club);
        }

        private static bool EndsWithFileName(string url, string fileName)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(fileName)) return false;

            var u = url.Trim();
            var f = fileName.Trim();

            return u.EndsWith("/" + f, StringComparison.OrdinalIgnoreCase)
                   || u.EndsWith("\\" + f, StringComparison.OrdinalIgnoreCase)
                   || u.Equals(f, StringComparison.OrdinalIgnoreCase);
        }

        public void Delete(long id) => _clubRepository.Delete(id);

        public List<ClubDto> GetAll() =>
            _clubRepository.GetAll().Select(MapClubToDto).ToList();

        public ClubDto Get(long id) =>
            MapClubToDto(_clubRepository.Get(id));

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

            var notification = new Notification(
                userId: touristId,
                clubId: clubId,
                type: NotificationType.JoinRequestAccepted,
                content: $"Your join request to club '{club.Name}' has been accepted"
            );
            _notificationRepository.Create(notification);
        }

        public void RejectRequest(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            EnsureOwner(club, ownerId);

            club.RejectRequest(touristId);
            _clubRepository.Update(club);

            var notification = new Notification(
                userId: touristId,
                clubId: clubId,
                type: NotificationType.JoinRequestRejected,
                content: $"Your join request to club '{club.Name}' has been rejected"
            );
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

        private static void EnsureOwner(Club club, long ownerId)
        {
            if (club.CreatedBy != ownerId)
                throw new ForbiddenException("Only the owner can perform this action.");
        }

        private static ClubDto MapClubToDto(Club club)
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
                Status = (ClubStatusDto)club.Status,
                Members = new List<ClubMemberDto>(),
                JoinRequests = new List<ClubJoinRequestDto>(),
                Invitations = new List<ClubInvitationDto>()
            };

            if (club.Members != null)
            {
                dto.Members = club.Members.Select(m => new ClubMemberDto
                {
                    TouristId = m.TouristId,
                    JoinedAt = m.JoinedAt
                }).ToList();
            }

            if (club.JoinRequests != null)
            {
                dto.JoinRequests = club.JoinRequests.Select(r => new ClubJoinRequestDto
                {
                    TouristId = r.TouristId,
                    RequestedAt = r.RequestedAt
                }).ToList();
            }

            if (club.Invitations != null)
            {
                dto.Invitations = club.Invitations.Select(i => new ClubInvitationDto
                {
                    TouristId = i.TouristId,
                    SentAt = i.SentAt
                }).ToList();
            }

            return dto;
        }
    }
}
