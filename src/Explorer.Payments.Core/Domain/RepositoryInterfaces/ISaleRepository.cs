using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ISaleRepository
    {
        Sale Get(long id);
        List<Sale> GetAll();
        Sale Create(Sale sale);
        void Update(Sale sale);
        void Delete(long id);

        Sale? GetActiveSaleForTour(long tourId, DateTime now);
        List<Sale> GetAllActive(DateTime now);
    }
}
