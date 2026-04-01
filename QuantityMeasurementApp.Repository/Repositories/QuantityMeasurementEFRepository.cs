using System;
using System.Collections.Generic;
using System.Linq;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Repository.Repositories
{
    // EF Core based repository
    // Replaces QuantityMeasurementCacheRepository for UC17 API
    // Uses DbContext to persist data to InMemory database
    public class QuantityMeasurementEFRepository : IQuantityMeasurementRepository
    {
        private readonly QuantityMeasurementDbContext _context;

        public QuantityMeasurementEFRepository(QuantityMeasurementDbContext context)
        {
            _context = context;
            Console.WriteLine("[EFRepository] Initialised with DbContext.");
        }

        // Save entity — equivalent of JPA repository.save()
        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Measurements.Add(entity);
            _context.SaveChanges();

            Console.WriteLine("[EFRepository] Saved: " + entity.Operation);
        }

        // Get all — equivalent of JPA repository.findAll()
        public IList<QuantityMeasurementEntity> GetAll()
        {
            return _context.Measurements.ToList();
        }

        public IList<QuantityMeasurementEntity> GetByOperation(string operation)
     {
    if (string.IsNullOrWhiteSpace(operation))
        throw new ArgumentException("Operation cannot be empty.");

    return _context.Measurements
        .Where(e => e.Operation == operation.ToUpper())
        .ToList();
}

public IList<QuantityMeasurementEntity> GetByMeasurementType(string measurementType)
{
    if (string.IsNullOrWhiteSpace(measurementType))
        throw new ArgumentException("Measurement type cannot be empty.");

    return _context.Measurements
        .Where(e => e.Operand1MeasurementType == measurementType.ToUpper())
        .ToList();
}
        // Count — equivalent of JPA repository.count()
        public int GetTotalCount()
        {
            return _context.Measurements.Count();
        }

        // Delete all — equivalent of JPA repository.deleteAll()
        public void Clear()
        {
            _context.Measurements.RemoveRange(_context.Measurements);
            _context.SaveChanges();
            Console.WriteLine("[EFRepository] All records deleted.");
        }

        public string GetPoolStatistics()
        {
            return "[EFRepository] InMemory DB — Total records: "
                + _context.Measurements.Count();
        }

        public void ReleaseResources()
        {
            _context.Dispose();
            Console.WriteLine("[EFRepository] DbContext disposed.");
        }
    }
}