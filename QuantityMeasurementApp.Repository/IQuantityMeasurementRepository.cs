using System.Collections.Generic;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Repository
{
    public interface IQuantityMeasurementRepository
    {
        void Save(QuantityMeasurementEntity entity);
        IList<QuantityMeasurementEntity> GetAll();
        void Clear();
    }
}