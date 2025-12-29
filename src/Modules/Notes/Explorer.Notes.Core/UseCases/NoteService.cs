using AutoMapper;
using Explorer.Notes.API.Dtos;
using Explorer.Notes.API.Public;
using Explorer.Notes.Core.Domain;
using Explorer.Notes.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Notes.Core.UseCases
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;
        private readonly IMapper _mapper;

        public NoteService(INoteRepository noteRepository, IMapper mapper)
        {
            _noteRepository = noteRepository;
            _mapper = mapper;
        }

        public List<NoteDto> GetByUserId(long userId)
        {
            var notes = _noteRepository.GetByUserId(userId);
            var sorted = notes.OrderByDescending(n => n.IsPinned)
                              .ThenByDescending(n => n.UpdatedAt)
                              .ToList();
            return _mapper.Map<List<NoteDto>>(sorted);
        }

        public NoteDto Get(long id, long userId)
        {
            var note = _noteRepository.Get(id);
            if (note == null)
                throw new NotFoundException($"Note with ID {id} not found.");

            if (note.UserId != userId)
                throw new ForbiddenException("You cannot access this note.");

            return _mapper.Map<NoteDto>(note);
        }

        public NoteDto Create(long userId, CreateNoteDto dto)
        {
            var note = new Note(userId, dto.Title, dto.Content, (NoteType)dto.Type, dto.Tags);
            var created = _noteRepository.Create(note);
            return _mapper.Map<NoteDto>(created);
        }

        public NoteDto Update(long userId, UpdateNoteDto dto)
        {
            var existing = _noteRepository.Get(dto.Id);
            if (existing == null)
                throw new NotFoundException($"Note with ID {dto.Id} not found.");

            if (existing.UserId != userId)
                throw new ForbiddenException("You cannot edit this note.");

            existing.Update(dto.Title, dto.Content, (NoteType)dto.Type, dto.Tags);
            var updated = _noteRepository.Update(existing);
            return _mapper.Map<NoteDto>(updated);
        }

        public void Delete(long id, long userId)
        {
            var note = _noteRepository.Get(id);
            if (note == null)
                throw new NotFoundException($"Note with ID {id} not found.");

            if (note.UserId != userId)
                throw new ForbiddenException("You cannot delete this note.");

            _noteRepository.Delete(note);
        }
    }
}