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
            {
                throw new ArgumentException("Invalid numeric value");
            }

            if (!Enum.IsDefined(typeof(LengthUnit), unit))
            {
                throw new ArgumentException("Invalid unit");
            }

            this.value = value;
            this.unit = unit;
        }

        private double ToBase()
        {
            return LengthUnitExtensions.ConvertToBaseUnit(unit, value);
        }

        public Length ConvertTo(LengthUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
            {
                throw new ArgumentException("Invalid target unit");
            }

            double baseValue = ToBase();
            double converted = LengthUnitExtensions.ConvertFromBaseUnit(targetUnit, baseValue);

            return new Length(converted, targetUnit);
        }

        public Length Add(Length other)
        {
            if (other == null)
            {
                throw new ArgumentException("Other length cannot be null");
            }

            double sumBase = this.ToBase() + other.ToBase();
            double result = LengthUnitExtensions.ConvertFromBaseUnit(this.unit, sumBase);

            return new Length(result, this.unit);
        }

        public Length Add(Length other, LengthUnit targetUnit)
        {
            if (other == null)
            {
                throw new ArgumentException("Other length cannot be null");
            }

            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
            {
                throw new ArgumentException("Invalid target unit");
            }

            double sumBase = this.ToBase() + other.ToBase();
            double result = LengthUnitExtensions.ConvertFromBaseUnit(targetUnit, sumBase);

            return new Length(result, targetUnit);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(Length))
            {
                return false;
            }

            Length other = (Length)obj;

            double difference = Math.Abs(this.ToBase() - other.ToBase());

            if (difference < 0.0001)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToBase().GetHashCode();
        }

        public override string ToString()
        {
            return value + " " + unit;
        }
    }
}