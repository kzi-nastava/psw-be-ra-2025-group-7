using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly StakeholdersContext _context;

        public ReviewRepository(StakeholdersContext context)
        {
            _context = context;
        }

        //DODATO ZA ISPIS INFO OD OSOBE ***********
        public Review Get(int id)
        {
            return _context.Reviews
                .Include(r => r.Person)
                .FirstOrDefault(r => r.Id == id);
        }

        public Review GetByPersonId(long personId)
        {
            return _context.Reviews
                .Include(r => r.Person)
                .FirstOrDefault(r => r.PersonId == personId);
        }

        public List<Review> GetAll()
        {
            return _context.Reviews
                .Include(r => r.Person)
                .ToList();
        }

        //*******************

        /*public Review Get(int id)
        {
            return _context.Reviews.FirstOrDefault(r => r.Id == id);
        }

        public Review GetByPersonId(long personId)
        {
            return _context.Reviews.FirstOrDefault(r => r.PersonId == personId);
        }

        public List<Review> GetAll()
        {
            return _context.Reviews.ToList();
        }*/

        public void Add(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public void Update(Review review)
        {
            _context.Reviews.Update(review);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _context.Reviews.FirstOrDefault(r => r.Id == id);
            if (entity == null) return;

            _context.Reviews.Remove(entity);
            _context.SaveChanges();
        }
    }
}
