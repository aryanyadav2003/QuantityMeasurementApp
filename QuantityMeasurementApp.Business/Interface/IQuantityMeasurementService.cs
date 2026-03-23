using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Business
{
    public interface IQuantityMeasurementService
    {
        // Returns QuantityDTO with Unit = "EQUAL" or "NOT_EQUAL"
        QuantityDTO Compare(QuantityDTO quantity1, QuantityDTO quantity2);

        // targetUnit is the unit name to convert to e.g. "INCHES", "GRAM"
        QuantityDTO Convert(QuantityDTO quantity, string targetUnit);

        // targetUnit is the unit name for the result e.g. "FEET"
        QuantityDTO Add(QuantityDTO quantity1, QuantityDTO quantity2, string targetUnit);

        QuantityDTO Subtract(QuantityDTO quantity1, QuantityDTO quantity2, string targetUnit);

        // Returns scalar result wrapped in QuantityDTO with Unit = "SCALAR"
        QuantityDTO Divide(QuantityDTO quantity1, QuantityDTO quantity2);
    }
}