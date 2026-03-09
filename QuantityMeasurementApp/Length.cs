using System;

namespace QuantityMeasurementApp
{
    public class Length
    {
        private readonly double value;
        private readonly LengthUnit unit;

        public Length(double value, LengthUnit unit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Invalid numeric value");

            // Validate unit
            if (!Enum.IsDefined(typeof(LengthUnit), unit))
                throw new ArgumentException("Invalid unit");

            this.value = value;
            this.unit = unit;
        }

        public double GetValue()
        {
            return value;
        }

        public LengthUnit GetUnit()
        {
            return unit;
        }

        // Convert to another unit
        public Length ConvertTo(LengthUnit targetUnit)
        {
            double baseValue = unit.ConvertToBaseUnit(value);
            double convertedValue = targetUnit.ConvertFromBaseUnit(baseValue);

            return new Length(convertedValue, targetUnit);
        }

        // UC7 + UC8 Addition
        public Length Add(Length other, LengthUnit targetUnit)
        {
            if (other == null)
                throw new ArgumentException("Other length cannot be null");

            double baseValue1 = unit.ConvertToBaseUnit(value);
            double baseValue2 = other.unit.ConvertToBaseUnit(other.value);

            double sumBase = baseValue1 + baseValue2;

            double result = targetUnit.ConvertFromBaseUnit(sumBase);

            return new Length(result, targetUnit);
        }

        public bool Compare(Length other)
        {
            if (other == null)
                return false;

            double base1 = unit.ConvertToBaseUnit(value);
            double base2 = other.unit.ConvertToBaseUnit(other.value);

            return Math.Abs(base1 - base2) < 0.0001;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Length other)
                return false;

            return Compare(other);
        }

        public override int GetHashCode()
        {
            return unit.ConvertToBaseUnit(value).GetHashCode();
        }

        public override string ToString()
        {
            return $"{value} {unit}";
        }
    }
}