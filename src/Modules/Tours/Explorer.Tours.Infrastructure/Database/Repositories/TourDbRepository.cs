using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourDbRepository : ITourRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<Tour> _dbSet;

    public TourDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Tour>();
    }

    public PagedResult<Tour> GetPagedByAuthor(int page, int pageSize, long authorId)
    {
        var query = _dbSet
            .Include(t => t.KeyPoints)
            .Include(t => t.TourDurations)
            .Include(t => t.RequiredEquipment)
            .Include(t => t.Images)
            .Where(t => t.AuthorId == authorId);

        var totalCount = query.Count();

        var items = query
            .OrderBy(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<Tour>(items, totalCount);
    }

    public PagedResult<Tour> GetPublishedTours(int page, int pageSize)
    {
        var task = _dbSet
            .Where(t => t.Status == TourStatus.Published)
            .GetPagedById(page, pageSize);

        task.Wait();
        return task.Result;
    }

    public Tour Get(long id)
    {
        var entity = _dbSet
            .Include(t => t.KeyPoints)
            .Include(t => t.TourDurations)
            .Include(t => t.RequiredEquipment)
            .Include(t => t.Images)
            .FirstOrDefault(t => t.Id == id);
        if (entity == null) throw new NotFoundException("Not found: " + id);
        return entity;
    }

    public Tour Create(Tour entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public Tour Update(Tour entity)
    {
        try
        {
            // Učitaj postojeći entitet iz baze SA tracking-om
            var existingTour = _dbSet
                .Include(t => t.RequiredEquipment)
                .Include(t => t.KeyPoints)
                .Include(t => t.TourDurations)
                .Include(t => t.Images)
                .FirstOrDefault(t => t.Id == entity.Id);

            if (existingTour == null)
                throw new NotFoundException($"Tour with id {entity.Id} not found.");

            // Ažuriraj sva svojstva osim kolekcija
            DbContext.Entry(existingTour).CurrentValues.SetValues(entity);

            // Sinhronizuj RequiredEquipment kolekciju
            var existingEquipmentIds = existingTour.RequiredEquipment.Select(e => e.Id).ToHashSet();
            var newEquipmentIds = entity.RequiredEquipment.Select(e => e.Id).ToHashSet();

            // Ukloni opremu koja više ne treba
            var toRemove = existingTour.RequiredEquipment
                .Where(e => !newEquipmentIds.Contains(e.Id))
                .ToList();
            foreach (var equipment in toRemove)
            {
                existingTour.RequiredEquipment.Remove(equipment);
            }

            // Dodaj novu opremu
            var toAdd = newEquipmentIds.Except(existingEquipmentIds).ToList();
            foreach (var equipmentId in toAdd)
            {
                var equipment = DbContext.Equipment.Find(equipmentId);
                if (equipment != null)
                {
                    existingTour.RequiredEquipment.Add(equipment);
                }
            }

            DbContext.SaveChanges();

            // Ponovo učitaj da bi bili sigurni da imamo sve Include-ove
            DbContext.Entry(existingTour).State = EntityState.Detached;
            return Get(entity.Id);
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException($"An error occurred while updating: {e.InnerException?.Message ?? e.Message}");
        }
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }

    public List<Tour> GetAll()
    {
        return DbContext.Tours
            .Include(t => t.KeyPoints)
            .Include(t => t.Images)
            .ToList();
    }
    public IEnumerable<Tour> GetPublishedWithKeyPoints()
    {
        return _dbSet
            .Include(t => t.KeyPoints)
            .Include(t => t.TourDurations)
            .Include(t => t.Images)
            .Where(t => t.Status == TourStatus.Published)
            .ToList();
    }

    public Tour GetForPreview(long id)
    {
        var tour = _dbSet
            .Include(t => t.Images)
            .Include(t => t.KeyPoints)
            .FirstOrDefault(t => t.Id == id);

        if (tour == null)
            throw new NotFoundException($"Tour with id {id} not found.");

        return tour;
    }

}