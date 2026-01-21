using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly PaymentsContext _context;

        public SaleRepository(PaymentsContext context)
        {
            _context = context;
        }

        public List<Sale> GetAll()
        {
            return _context.Sales
                .AsNoTracking()
                .Include(s => s.SaleTours)
                .ToList();
        }

        public Sale Get(long id)
        {
            return _context.Sales
                .AsNoTracking()
                .Include(s => s.SaleTours)
                .FirstOrDefault(s => s.Id == id);
        }


        public Sale Create(Sale sale)
        {
            _context.Sales.Add(sale);
            _context.SaveChanges();
            return sale;
        }

        public void Update(Sale sale)
        {
            _context.Sales.Update(sale);
            _context.SaveChanges();
        }

        public void Delete(long id)
        {
            var sale = Get(id);
            if (sale == null) return;

            _context.Sales.Remove(sale);
            _context.SaveChanges();
        }

        public Sale? GetActiveSaleForTour(long tourId, DateTime now)
        {
            return _context.Sales
                .Include(s => s.SaleTours)
                .FirstOrDefault(s =>
                    s.Status == SaleStatus.Active &&
                    s.Start <= now &&
                    s.End >= now &&
                    s.SaleTours.Any(st => st.TourId == tourId));
        }



        public List<Sale> GetAllActive(DateTime now)
        {
            return _context.Sales
                .AsNoTracking()
                .Include(s => s.SaleTours)
                .Where(s => s.Status == SaleStatus.Active &&
                            s.Start <= now &&
                            s.End >= now)
                .ToList();
        }

    }
}
