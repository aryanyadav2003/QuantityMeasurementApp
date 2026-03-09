using System;

namespace QuantityMeasurementApp
{
    public enum WeightUnit
    {
        KILOGRAM,
        GRAM,
        POUND
    }

    public static class WeightUnitExtensions
    {
        public static double GetConversionFactor(WeightUnit unit)
        {
            switch (unit)
            {
                case WeightUnit.KILOGRAM:
                    return 1.0;

                case WeightUnit.GRAM:
                    return 0.001;

                case WeightUnit.POUND:
                    return 0.453592;

                default:
                    throw new ArgumentException("Invalid unit");
            }
        }

        public static double ConvertToBaseUnit(WeightUnit unit, double value)
        {
            return value * GetConversionFactor(unit);
        }

        public static double ConvertFromBaseUnit(WeightUnit unit, double baseValue)
        {
            return baseValue / GetConversionFactor(unit);
        }
    }
}