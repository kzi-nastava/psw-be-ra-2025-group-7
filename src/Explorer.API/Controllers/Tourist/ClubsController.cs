using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/clubs")]
[ApiController]
public class ClubsController : ControllerBase
{
    private readonly IClubService _clubService;
    private readonly StakeholdersContext? _db;
    private readonly IWebHostEnvironment? _env;

    public ClubsController(IClubService clubService, StakeholdersContext db, IWebHostEnvironment env)
    {
        _clubService = clubService;
        _db = db;
        _env = env;
    }

    [HttpGet]
    public ActionResult<List<ClubDto>> GetAll()
    {
        var result = _clubService.GetAll();
        EnrichWithUsernames(result);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<ClubDto> GetById(long id)
    {
        var club = _clubService.Get(id);
        EnrichWithUsernames(club);
        return Ok(club);
    }

    [HttpGet("tourists/lookup")]
    public ActionResult<List<TouristLookupDto>> GetTouristsLookup()
    {
        if (_db == null) return Ok(new List<TouristLookupDto>());

        var tourists = _db.People
            .Join(_db.Users, p => p.UserId, u => u.Id, (p, u) => new { PersonId = p.Id, u.Username, u.Role })
            .Where(x => x.Role == Explorer.Stakeholders.Core.Domain.UserRole.Tourist)
            .OrderBy(x => x.Username)
            .Select(x => new TouristLookupDto
            {
                Id = x.PersonId,
                Username = x.Username
            })
            .ToList();

        return Ok(tourists);
    }

    [HttpPost("{clubId:long}/images")]
    [RequestSizeLimit(25_000_000)]
    public async Task<ActionResult<List<string>>> UploadImages(long clubId, [FromForm] List<IFormFile> files)
    {
        if (_env == null) return BadRequest("File upload is not available in this environment.");

        var ownerId = User.PersonId();

        var club = _clubService.Get(clubId);
        if (club == null) throw new NotFoundException("Club not found.");

        if (club.CreatedBy != ownerId) return Forbid();

        if (files == null || files.Count == 0)
            return BadRequest("No files were sent.");

        var webRoot = _env.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRoot))
        {
            webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        var clubFolder = Path.Combine(webRoot, "uploads", "clubs", clubId.ToString());
        Directory.CreateDirectory(clubFolder);

        var savedUrls = new List<string>();

        foreach (var file in files)
        {
            if (file == null || file.Length == 0) continue;

            if (string.IsNullOrWhiteSpace(file.ContentType) ||
                !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                continue;

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(clubFolder, fileName);

            await using (var stream = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(stream);
            }

            var publicUrl = $"/uploads/clubs/{clubId}/{fileName}";
            savedUrls.Add(publicUrl);
        }

        if (savedUrls.Count == 0)
            return BadRequest("No valid images uploaded.");

        club.ImageUrls ??= new List<string>();
        club.ImageUrls = club.ImageUrls
            .Concat(savedUrls)
            .Distinct()
            .ToList();

        var updated = _clubService.Update(club);
        return Ok(updated.ImageUrls);
    }

    // ✅ DELETE single image
    // FE: DELETE /api/tourist/clubs/{clubId}/images?fileName=xxx.png
    [HttpDelete("{clubId:long}/images")]
    public IActionResult DeleteImage(long clubId, [FromQuery] string fileName)
    {
        var ownerId = User.PersonId();

        if (string.IsNullOrWhiteSpace(fileName))
            return BadRequest("fileName is required.");

        var safeFileName = Path.GetFileName(fileName).Trim();

        _clubService.DeleteImage(clubId, ownerId, safeFileName);

        // pokušaj obrisati i fajl sa diska (ako postoji)
        if (_env != null)
        {
            var webRoot = _env.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var fullPath = Path.Combine(webRoot, "uploads", "clubs", clubId.ToString(), safeFileName);
            try
            {
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }
            catch
            {
                // DB je već ažuriran – ne rušimo request ako file delete failuje
            }
        }

        return NoContent();
    }

    [HttpPost]
    public ActionResult<ClubDto> Create([FromBody] ClubDto dto)
    {
        var userId = User.PersonId();

        dto.CreatedBy = userId;
        dto.CreatedAt = DateTime.UtcNow;

        var created = _clubService.Create(dto);
        EnrichWithUsernames(created);
        return Ok(created);
    }

    [HttpPut("{id:long}")]
    public ActionResult<ClubDto> Update(long id, [FromBody] ClubDto dto)
    {
        var userId = User.PersonId();
        dto.Id = id;

        var existing = _clubService.GetAll().FirstOrDefault(c => c.Id == id);
        if (existing == null)
            throw new NotFoundException("Club not found.");

        if (existing.CreatedBy != userId)
            return Forbid();

        var updated = _clubService.Update(dto);
        EnrichWithUsernames(updated);
        return Ok(updated);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var userId = User.PersonId();

        var existing = _clubService.GetAll().FirstOrDefault(c => c.Id == id);
        if (existing == null)
            throw new NotFoundException("Club not found.");

        if (existing.CreatedBy != userId)
            return Forbid();

        _clubService.Delete(id);
        return Ok();
    }

    [HttpPost("{clubId:long}/close")]
    public IActionResult Close(long clubId)
    {
        var ownerId = User.PersonId();
        _clubService.Close(clubId, ownerId);
        return NoContent();
    }

    [HttpPost("{clubId:long}/open")]
    public IActionResult Open(long clubId)
    {
        var ownerId = User.PersonId();
        _clubService.Open(clubId, ownerId);
        return NoContent();
    }

    [HttpPost("{clubId:long}/join-requests")]
    public IActionResult RequestMembership(long clubId)
    {
        var touristId = User.PersonId();
        _clubService.RequestMembership(clubId, touristId);
        return NoContent();
    }

    [HttpDelete("{clubId:long}/join-requests")]
    public IActionResult WithdrawMembershipRequest(long clubId)
    {
        var touristId = User.PersonId();
        _clubService.WithdrawRequest(clubId, touristId);
        return NoContent();
    }

    [HttpPost("{clubId:long}/join-requests/{touristId:long}/accept")]
    public IActionResult AcceptMembershipRequest(long clubId, long touristId)
    {
        var ownerId = User.PersonId();
        _clubService.AcceptRequest(clubId, ownerId, touristId);
        return NoContent();
    }

    [HttpPost("{clubId:long}/join-requests/{touristId:long}/reject")]
    public IActionResult RejectMembershipRequest(long clubId, long touristId)
    {
        var ownerId = User.PersonId();
        _clubService.RejectRequest(clubId, ownerId, touristId);
        return NoContent();
    }

    [HttpPost("{clubId:long}/invite/{touristId:long}")]
    public IActionResult InviteTourist(long clubId, long touristId)
    {
        var ownerId = User.PersonId();
        _clubService.InviteTourist(clubId, ownerId, touristId);
        return NoContent();
    }

    [HttpPost("{clubId:long}/invitation/accept")]
    public IActionResult AcceptInvitation(long clubId)
    {
        var touristId = User.PersonId();
        _clubService.AcceptInvitation(clubId, touristId);
        return NoContent();
    }

    [HttpPost("{clubId:long}/invitation/reject")]
    public IActionResult RejectInvitation(long clubId)
    {
        var touristId = User.PersonId();
        _clubService.RejectInvitation(clubId, touristId);
        return NoContent();
    }

    [HttpDelete("{clubId:long}/members/{touristId:long}")]
    public IActionResult RemoveMember(long clubId, long touristId)
    {
        var ownerId = User.PersonId();
        _clubService.RemoveMember(clubId, ownerId, touristId);
        return NoContent();
    }

    // -------------------------
    // Helpers: username mapping
    // -------------------------

    private void EnrichWithUsernames(List<ClubDto> clubs)
    {
        if (_db == null) return;
        if (clubs == null || clubs.Count == 0) return;

        var allPersonIds = clubs
            .SelectMany(c =>
                (c.Members?.Select(m => m.TouristId) ?? Enumerable.Empty<long>())
                .Concat(c.JoinRequests?.Select(r => r.TouristId) ?? Enumerable.Empty<long>())
                .Concat(c.Invitations?.Select(i => i.TouristId) ?? Enumerable.Empty<long>())
            )
            .Distinct()
            .ToList();

        if (allPersonIds.Count == 0) return;

        var usernamesByPersonId = _db.People
            .Where(p => allPersonIds.Contains(p.Id))
            .Join(_db.Users, p => p.UserId, u => u.Id, (p, u) => new { PersonId = p.Id, u.Username })
            .ToDictionary(x => x.PersonId, x => x.Username);

        foreach (var club in clubs)
        {
            ApplyUsernames(club, usernamesByPersonId);
        }
    }

    private void EnrichWithUsernames(ClubDto club)
    {
        if (_db == null) return;
        if (club == null) return;

        var personIds = new List<long>();
        if (club.Members != null) personIds.AddRange(club.Members.Select(m => m.TouristId));
        if (club.JoinRequests != null) personIds.AddRange(club.JoinRequests.Select(r => r.TouristId));
        if (club.Invitations != null) personIds.AddRange(club.Invitations.Select(i => i.TouristId));

        personIds = personIds.Distinct().ToList();
        if (personIds.Count == 0) return;

        var usernamesByPersonId = _db.People
            .Where(p => personIds.Contains(p.Id))
            .Join(_db.Users, p => p.UserId, u => u.Id, (p, u) => new { PersonId = p.Id, u.Username })
            .ToDictionary(x => x.PersonId, x => x.Username);

        ApplyUsernames(club, usernamesByPersonId);
    }

    private static void ApplyUsernames(ClubDto club, Dictionary<long, string> usernamesByPersonId)
    {
        if (club.Members != null)
        {
            foreach (var m in club.Members)
                m.TouristName = usernamesByPersonId.TryGetValue(m.TouristId, out var uname) ? uname : null;
        }

        if (club.JoinRequests != null)
        {
            foreach (var r in club.JoinRequests)
                r.TouristName = usernamesByPersonId.TryGetValue(r.TouristId, out var uname) ? uname : null;
        }

        if (club.Invitations != null)
        {
            foreach (var i in club.Invitations)
                i.TouristName = usernamesByPersonId.TryGetValue(i.TouristId, out var uname) ? uname : null;
        }
    }
}
