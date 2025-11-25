using System.Collections.Generic;
using System.Linq;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;

namespace Explorer.Tours.Infrastructure.Repositories
{
    public class TouristEquipmentRepository : ITouristEquipmentRepository
    {
        private readonly ToursContext _dbContext;

        public TouristEquipmentRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Equipment> GetAll()
        {
            return _dbContext.Equipment.ToList();
        }

        public List<TouristEquipment> GetByTourist(long touristId)
        {
            return _dbContext.TouristEquipment
                .Where(x => x.TouristId == touristId)
                .ToList();
        }

        public TouristEquipment Create(TouristEquipment entity)
        {
            _dbContext.TouristEquipment.Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public void Delete(long id)
        {
            var entity = _dbContext.TouristEquipment.Find(id);
            if (entity != null)
            {
                _dbContext.TouristEquipment.Remove(entity);
                _dbContext.SaveChanges();
            }
        }
    }
}
