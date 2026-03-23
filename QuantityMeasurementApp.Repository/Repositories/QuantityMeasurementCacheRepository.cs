using System;
using System.Collections.Generic;
using System.Linq;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Repository.Repositories
{
    public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
    {
        private static QuantityMeasurementCacheRepository? _instance;
        private readonly List<QuantityMeasurementEntity>   _cache;

        private QuantityMeasurementCacheRepository()
        {
            _cache = new List<QuantityMeasurementEntity>();
        }

        public static QuantityMeasurementCacheRepository GetInstance()
        {
            if (_instance == null)
                _instance = new QuantityMeasurementCacheRepository();
            return _instance;
        }

        // Save entity to cache
        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            _cache.Add(entity);
        }

        // Get all entities
        public IList<QuantityMeasurementEntity> GetAll()
        {
            return _cache.ToList();
        }

        // Filter by operation type e.g. "ADD", "COMPARE"
        public IList<QuantityMeasurementEntity> GetByOperation(string operation)
        {
            return _cache
                .Where(e => e.Operation.ToString() == operation.ToUpper())
                .ToList();
        }

        // Filter by measurement type e.g. "LENGTH", "WEIGHT"
        public IList<QuantityMeasurementEntity> GetByMeasurementType(string measurementType)
        {
            return _cache
                .Where(e => e.Operand1.MeasurementType == measurementType.ToUpper())
                .ToList();
        }

        // Total count
        public int GetTotalCount()
        {
            return _cache.Count;
        }

        // Clear all
        public void Clear()
        {
            _cache.Clear();
        }

        // Not applicable for cache
        public string GetPoolStatistics()
        {
            return "[CacheRepository] In-memory cache — no connection pool.";
        }

        // Nothing to release for cache
        public void ReleaseResources()
        {
            Console.WriteLine("[CacheRepository] No resources to release.");
        }
    }
}