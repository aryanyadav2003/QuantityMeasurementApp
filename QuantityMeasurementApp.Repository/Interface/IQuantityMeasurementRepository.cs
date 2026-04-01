using System.Collections.Generic;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Repository
{
    public interface IQuantityMeasurementRepository
    {
        // Already existing
        void Save(QuantityMeasurementEntity entity);
        IList<QuantityMeasurementEntity> GetAll();
        void Clear();

        // New methods for UC16
        IList<QuantityMeasurementEntity> GetByOperation(string operation);
        IList<QuantityMeasurementEntity> GetByMeasurementType(string measurementType);
        int GetTotalCount();
        string GetPoolStatistics();
        void ReleaseResources();
    }
}