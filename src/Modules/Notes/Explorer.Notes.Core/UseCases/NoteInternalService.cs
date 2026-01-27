using System.Collections.Generic;
using Explorer.Notes.API.Dtos;
using Explorer.Notes.API.Internal;
using Explorer.Notes.API.Public;

namespace Explorer.Notes.Core.UseCases
{
    public class NoteInternalService : INoteInternalService
    {
        private readonly INoteService _noteService;

        public NoteInternalService(INoteService noteService)
        {
            _noteService = noteService;
        }

        public void CreateCouponNote(long touristId, string couponCode, string authorFullName, long? tourId = null)
        {
            _noteService.Create(touristId, new CreateNoteDto
            {
                Title = "Kupon za ture",
                Content = $"Kupon: {couponCode} (10% popusta). Važi samo za najskuplju turu od autora {authorFullName}.",
                Type = NoteTypeDto.Observation,
                Tags = new List<string> { "kupon", "ture" },
                TourId = tourId
            });
        }
    }
}
