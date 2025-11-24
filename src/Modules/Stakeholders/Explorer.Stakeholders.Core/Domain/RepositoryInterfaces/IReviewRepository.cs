using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IReviewRepository
    {
        Review Get(int id);
        Review GetByPersonId(long personId);
        void Add(Review review);
        void Update(Review review);
        void Delete(int id);
        List<Review> GetAll();
    }

}
