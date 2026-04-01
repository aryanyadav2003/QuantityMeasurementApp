using System;

namespace QuantityMeasurementApp.Business

{
    public interface ISupportsArithmetic
    {
        bool IsSupported();
    }

    public interface IMeasurable
    {
        double GetConversionFactor();
        double ConvertToBaseUnit(double value);
        double ConvertFromBaseUnit(double baseValue);
        string GetUnitName();
        bool SupportsArithmetic();
        void ValidateOperationSupport(string operation);
    }
}