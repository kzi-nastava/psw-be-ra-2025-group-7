using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class EncounterRepository : IEncounterRepository
    {
        private readonly EncountersContext _context;

        public EncounterRepository(EncountersContext context)
        {
            _context = context;
        }

        public Encounter Create(Encounter encounter)
        {
            _context.Encounters.Add(encounter);
            _context.SaveChanges();
            return encounter;
        }

        public Encounter Update(Encounter encounter)
        {
            _context.Encounters.Update(encounter);
            _context.SaveChanges();
            return encounter;
        }

        public void Delete(long id)
        {
            var entity = _context.Encounters.FirstOrDefault(x => x.Id == id);
            if (entity == null) return;

            _context.Encounters.Remove(entity);
            _context.SaveChanges();
        }

        public Encounter? Get(long id)
        {
            return _context.Encounters.FirstOrDefault(x => x.Id == id);
        }

        public PagedResult<Encounter> GetPaged(int page, int pageSize, EncounterStatus? status, EncounterType? type)
        {
            IQueryable<Encounter> query = _context.Encounters;

            if (status.HasValue) query = query.Where(x => x.Status == status.Value);
            if (type.HasValue) query = query.Where(x => x.Type == type.Value);

            var total = query.Count();
            var items = query
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Encounter>(items, total);
        }

        public IEnumerable<Encounter> GetAll()
        {
            return _context.Encounters.ToList();
        }

    }
}