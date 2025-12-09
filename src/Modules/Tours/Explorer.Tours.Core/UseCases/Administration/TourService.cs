using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IMapper _mapper;

        public TourService(ITourRepository repository, IEquipmentRepository equipmentRepository, IMapper mapper)
        {
            _tourRepository = repository;
            _equipmentRepository = equipmentRepository;
            _mapper = mapper;
        }

        public PagedResult<TourDto> GetPagedByAuthor(int page, int pageSize, long authorId)
        {
            var result = _tourRepository.GetPagedByAuthor(page, pageSize, authorId);

            var items = result.Results.Select(_mapper.Map<TourDto>).ToList();
            return new PagedResult<TourDto>(items, result.TotalCount);
        }

        public TourDto Create(TourDto entity)
        {
            entity.PublishedAt = null;
            entity.ArchivedAt = null;
            entity.Status = (int)TourStatus.Draft;

            var result = _tourRepository.Create(_mapper.Map<Tour>(entity));
            return _mapper.Map<TourDto>(result);
        }

        public TourDto Update(TourDto entity)
        {
            var existingTour = _tourRepository.Get(entity.Id);

            if (existingTour.AuthorId != entity.AuthorId)
                throw new ForbiddenException("You can only update your own tours.");

            // Mapiramo DTO → postojeći domen objekat
            _mapper.Map(entity, existingTour);

            existingTour.Validate();

            var result = _tourRepository.Update(existingTour);
            return _mapper.Map<TourDto>(result);
        }

        public void Delete(long id, long authorId)
        {
            var tour = _tourRepository.Get(id);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("You can only delete your own tours.");

            if (tour.Status != TourStatus.Draft)
                throw new InvalidOperationException("Only Draft tours can be deleted.");

            _tourRepository.Delete(id);
        }

        // ================= Kartica 3 – ključne tačke (tvoj deo) =================

        public TourDto AddKeyPoint(long tourId, long authorId, KeyPointDto keyPointDto)
        {
            var tour = GetAuthorDraftTourOrThrow(tourId, authorId);

            var keyPoint = _mapper.Map<KeyPoint>(keyPointDto);
            tour.AddKeyPoint(keyPoint); // domen čuva pravilo "samo Draft"

            var updated = _tourRepository.Update(tour);
            return _mapper.Map<TourDto>(updated);
        }

        public TourDto UpdateKeyPoint(long tourId, long authorId, int index, KeyPointDto keyPointDto)
        {
            var tour = GetAuthorDraftTourOrThrow(tourId, authorId);

            var keyPoint = _mapper.Map<KeyPoint>(keyPointDto);
            tour.UpdateKeyPoint(index, keyPoint);

            var updated = _tourRepository.Update(tour);
            return _mapper.Map<TourDto>(updated);
        }

        public TourDto RemoveKeyPoint(long tourId, long authorId, int index)
        {
            var tour = GetAuthorDraftTourOrThrow(tourId, authorId);

            tour.RemoveKeyPoint(index);

            var updated = _tourRepository.Update(tour);
            return _mapper.Map<TourDto>(updated);
        }

        /// <summary>
        /// Pomoćna metoda: osigurava da je tura autora i da je u Draft statusu.
        /// </summary>
        private Tour GetAuthorDraftTourOrThrow(long tourId, long authorId)
        {
            var tour = _tourRepository.Get(tourId);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("You can modify only your own tours.");

            if (tour.Status != TourStatus.Draft)
                throw new InvalidOperationException("Changes can only be made while tour is in Draft status.");

            return tour;
        }

        // ================= Životni ciklus ture (član 1) =================

        public TourDto Publish(long id, long authorId)
        {
            var tour = _tourRepository.Get(id);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("You can only publish your own tours.");

            tour.Publish();
            var result = _tourRepository.Update(tour);

            return _mapper.Map<TourDto>(result);
        }

        public TourDto Archive(long id, long authorId)
        {
            var tour = _tourRepository.Get(id);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("You can only archive your own tours.");

            tour.Archive();
            var result = _tourRepository.Update(tour);

            return _mapper.Map<TourDto>(result);
        }

        public TourDto Reactivate(long id, long authorId, int newStatus)
        {
            var tour = _tourRepository.Get(id);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("You can only reactivate your own tours.");

            tour.Reactivate((TourStatus)newStatus);
            var result = _tourRepository.Update(tour);

            return _mapper.Map<TourDto>(result);
        }
        //trajanje ture
        public TourDto AddTourDuration(long tourId, long authorId, TourDurationDto durationDto)
        {
            var tour = GetAuthorDraftTourOrThrow(tourId, authorId);

            tour.AddDuration(_mapper.Map<TourDuration>(durationDto));

            var updated = _tourRepository.Update(tour);
            return _mapper.Map<TourDto>(updated);
        }

        public TourDto UpdateTourDuration(long tourId, long authorId, int index, TourDurationDto durationDto)
        {
            var tour = GetAuthorDraftTourOrThrow(tourId, authorId);

            tour.UpdateDuration(index, _mapper.Map<TourDuration>(durationDto));

            var updated = _tourRepository.Update(tour);
            return _mapper.Map<TourDto>(updated);
        }

        public TourDto RemoveTourDuration(long tourId, long authorId, int index)
        {
            var tour = GetAuthorDraftTourOrThrow(tourId, authorId);

            tour.RemoveDuration(index);

            var updated = _tourRepository.Update(tour);
            return _mapper.Map<TourDto>(updated);
        }

        public TourDto AddEquipment(long tourId, long authorId, long equipmentId)
        {
            var tour = _tourRepository.Get(tourId);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("You can only modify your own tours.");

            var equipment = _equipmentRepository.Get(equipmentId);

            tour.AddEquipment(equipment);

            var updated = _tourRepository.Update(tour);
            return _mapper.Map<TourDto>(updated);
        }

        public TourDto RemoveEquipment(long tourId, long authorId, long equipmentId)
        {
            var tour = _tourRepository.Get(tourId);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("You can only modify your own tours.");

            tour.RemoveEquipment(equipmentId);

            var updated = _tourRepository.Update(tour);
            return _mapper.Map<TourDto>(updated);
        }
    }
}
