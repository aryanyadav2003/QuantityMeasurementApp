using System;

namespace QuantityMeasurementApp
{
    public enum LengthUnit
    {
        FEET,
        INCH,
        YARD,
        CENTIMETER
    }

    public static class LengthUnitExtensions
    {
        public static double GetConversionFactor(LengthUnit unit)
        {
            switch (unit)
            {
                case LengthUnit.FEET:
                    return 1.0;

                case LengthUnit.INCH:
                    return 1.0 / 12.0;

                case LengthUnit.YARD:
                    return 3.0;

                case LengthUnit.CENTIMETER:
                    return 1.0 / 30.48;

                default:
                    throw new ArgumentException("Invalid unit");
            }
        }

        public static double ConvertToBaseUnit(LengthUnit unit, double value)
        {
            return value * GetConversionFactor(unit);
        }

        public static double ConvertFromBaseUnit(LengthUnit unit, double baseValue)
        {
            return baseValue / GetConversionFactor(unit);
        }
    }
}