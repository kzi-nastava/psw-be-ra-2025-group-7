using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class EncounterProgressRepository : IEncounterProgressRepository
    {
        private readonly EncountersContext _context;
        public EncounterProgressRepository(EncountersContext context)
        {
            _context = context;
        }
        public EncounterProgress Create(EncounterProgress encounterProgress)
        {
            _context.EncounterProgresses.Add(encounterProgress);
            _context.SaveChanges();
            return encounterProgress;
        }

        public EncounterProgress Update(EncounterProgress encounterProgress)
        {
            _context.EncounterProgresses.Update(encounterProgress);
            _context.SaveChanges();
            return encounterProgress;
        }

        public void Delete(long id)
        {
            var entity = _context.EncounterProgresses.FirstOrDefault(x => x.Id == id);
            if (entity == null) return;
            _context.EncounterProgresses.Remove(entity);
            _context.SaveChanges();
        }

        public EncounterProgress? Get(long id)
        {
            return _context.EncounterProgresses.FirstOrDefault(x => x.Id == id);
        }

        public List<EncounterProgress> GetAll()
        {
            return _context.EncounterProgresses.ToList();
        }
        public bool isCompleted(long encounterId, int participantId)
        {
            return _context.EncounterProgresses.Any(ep => ep.EncounterId == encounterId && ep.UserId == participantId && ep.Status == EncounterProgress.EncounterProgressStatus.Completed);
        }
    }
}
