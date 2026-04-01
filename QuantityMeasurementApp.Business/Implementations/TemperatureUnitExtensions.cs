using System;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Entity.Enums;

namespace QuantityMeasurementApp.Business.Implementations
{
    public static class TemperatureUnitExtensions
    {
        private static ISupportsArithmetic supportsArithmetic = new SupportsArithmeticImpl(() => false);

        public static double GetConversionFactor(this TemperatureUnit unit)
        {
            // Temperature uses non-linear conversion — factor not applicable
            return 1.0;
        }

        public static double ConvertToBaseUnit(this TemperatureUnit unit, double value)
        {
            // Base unit is CELSIUS
            switch (unit)
            {
                case TemperatureUnit.CELSIUS:
                    return value;
                case TemperatureUnit.FAHRENHEIT:
                    return (value - 32.0) * 5.0 / 9.0;
                default:
                    throw new ArgumentException("Invalid temperature unit");
            }
        }

        public static double ConvertFromBaseUnit(this TemperatureUnit unit, double baseValue)
        {
            // baseValue is in CELSIUS
            switch (unit)
            {
                case TemperatureUnit.CELSIUS:
                    return baseValue;
                case TemperatureUnit.FAHRENHEIT:
                    return (baseValue * 9.0 / 5.0) + 32.0;
                default:
                    throw new ArgumentException("Invalid temperature unit");
            }
        }

        public static string GetUnitName(this TemperatureUnit unit)
        {
            return unit.ToString();
        }

        public static bool SupportsArithmetic(this TemperatureUnit unit)
        {
            return supportsArithmetic.IsSupported();
        }

        public static void ValidateOperationSupport(this TemperatureUnit unit, string operation)
        {
            if (supportsArithmetic.IsSupported() == false)
            {
                throw new NotSupportedException("Temperature does not support " + operation + ". Temperature arithmetic is not meaningful.");
            }
        }
    }
}