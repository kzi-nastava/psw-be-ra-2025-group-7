using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Infrastructure.Authentication;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/clubs")]
[ApiController]
public class ClubsController : ControllerBase
{
    private readonly IClubService _clubService;

    public ClubsController(IClubService clubService)
    {
        _clubService = clubService;
    }

    [HttpGet]
    public ActionResult<List<ClubDto>> GetAll()
    {
        var result = _clubService.GetAll();
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<ClubDto> GetById(long id)
    {
        var club = _clubService.Get(id);
        return Ok(club);
    }

    [HttpPost]
    public ActionResult<ClubDto> Create([FromBody] ClubDto dto)
    {
        var userId = User.PersonId();

        dto.CreatedBy = userId;
        dto.CreatedAt = DateTime.UtcNow;

        var created = _clubService.Create(dto);
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
        return Ok("Club closed.");
    }

    [HttpPost("{clubId:long}/open")]
    public IActionResult Open(long clubId)
    {
        var ownerId = User.PersonId();

        _clubService.Open(clubId, ownerId);
        return Ok("Club opened.");
    }

    [HttpPost("{clubId:long}/join-requests")]
    public IActionResult RequestMembership(long clubId)
    {
        var touristId = User.PersonId();

        _clubService.RequestMembership(clubId, touristId);
        return Ok("Membership request sent.");
    }

    [HttpDelete("{clubId:long}/join-requests")]
    public IActionResult WithdrawMembershipRequest(long clubId)
    {
        var touristId = User.PersonId();

        _clubService.WithdrawRequest(clubId, touristId);
        return Ok("Membership request withdrawn.");
    }

    [HttpPost("{clubId:long}/join-requests/{touristId:long}/accept")]
    public IActionResult AcceptMembershipRequest(long clubId, long touristId)
    {
        var ownerId = User.PersonId();

        _clubService.AcceptRequest(clubId, ownerId, touristId);
        return Ok("Membership request accepted.");
    }

    [HttpPost("{clubId:long}/join-requests/{touristId:long}/reject")]
    public IActionResult RejectMembershipRequest(long clubId, long touristId)
    {
        var ownerId = User.PersonId();

        _clubService.RejectRequest(clubId, ownerId, touristId);
        return Ok("Membership request rejected.");
    }

    // Owner poziva turistu
    [HttpPost("{clubId:long}/invite/{touristId:long}")]
    public IActionResult InviteTourist(long clubId, long touristId)
    {
        var ownerId = User.PersonId();

        _clubService.InviteTourist(clubId, ownerId, touristId);
        return Ok("Invitation sent.");
    }

    // Turista prihvata svoju pozivnicu
    [HttpPost("{clubId:long}/invitation/accept")]
    public IActionResult AcceptInvitation(long clubId)
    {
        var touristId = User.PersonId();

        _clubService.AcceptInvitation(clubId, touristId);
        return Ok("Invitation accepted.");
    }

    // Turista odbija svoju pozivnicu
    [HttpPost("{clubId:long}/invitation/reject")]
    public IActionResult RejectInvitation(long clubId)
    {
        var touristId = User.PersonId();

        _clubService.RejectInvitation(clubId, touristId);
        return Ok("Invitation rejected.");
    }

    [HttpDelete("{clubId:long}/members/{touristId:long}")]
    public IActionResult RemoveMember(long clubId, long touristId)
    {
        var ownerId = User.PersonId();

        _clubService.RemoveMember(clubId, ownerId, touristId);
        return Ok("Member removed.");
    }
}
