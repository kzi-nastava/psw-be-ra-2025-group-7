using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public ReviewDto CreateReview(CreateReviewDto dto)
        {
            var review = new Review(
                reviewId: 0,        // EF će generisati
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
            return _mapper.Map<ReviewDto>(review);
        }

        public ReviewDto UpdateReview(int id, UpdateReviewDto dto)
        {
            var review = _repo.Get(id);
            review.Update(dto.Rating, dto.Comment);
            _repo.Update(review);

            return _mapper.Map<ReviewDto>(review);
        }

        public void DeleteReview(int id)
        {
            _repo.Delete(id);
        }

        public List<ReviewDto> GetAll()
        {
            return _mapper.Map<List<ReviewDto>>(_repo.GetAll());
        }
    }

}
