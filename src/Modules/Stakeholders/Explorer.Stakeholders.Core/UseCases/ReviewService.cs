using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository repo, IMapper mapper)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ReviewDto CreateReview(CreateReviewDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var review = new Review(
                rating: dto.Rating,
                comment: dto.Comment,
                personId: dto.PersonId
            );

            _repo.Add(review);
            return _mapper.Map<ReviewDto>(review);
        }

        public ReviewDto GetMyReview(long personId)
        {
            var review = _repo.GetByPersonId(personId);
            if (review == null) return null;

            return _mapper.Map<ReviewDto>(review);
        }

        public ReviewDto UpdateReview(int id, UpdateReviewDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var review = _repo.Get(id);
            if (review == null)
                throw new KeyNotFoundException($"Review with id {id} not found.");

            review.Update(dto.Rating, dto.Comment);
            _repo.Update(review);

            return _mapper.Map<ReviewDto>(review);
        }

        public void DeleteReview(int id)
        {
            var review = _repo.Get(id);
            if (review == null)
                throw new KeyNotFoundException($"Review with id {id} not found.");

            _repo.Delete(id);
        }

        public List<ReviewDto> GetAll()
        {
            var reviews = _repo.GetAll();
            return _mapper.Map<List<ReviewDto>>(reviews);
        }

    }
}
