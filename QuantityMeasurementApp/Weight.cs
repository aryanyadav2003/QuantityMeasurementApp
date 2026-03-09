using System;

namespace QuantityMeasurementApp
{
    public class Weight
    {
        private readonly double value;
        private readonly WeightUnit unit;

        public Weight(double value, WeightUnit unit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException("Invalid numeric value");
            }

            if (!Enum.IsDefined(typeof(WeightUnit), unit))
            {
                throw new ArgumentException("Invalid unit");
            }

            this.value = value;
            this.unit = unit;
        }

        private double ToBase()
        {
            return WeightUnitExtensions.ConvertToBaseUnit(unit, value);
        }

        public Weight ConvertTo(WeightUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit))
            {
                throw new ArgumentException("Invalid target unit");
            }

            double baseValue = ToBase();
            double converted = WeightUnitExtensions.ConvertFromBaseUnit(targetUnit, baseValue);

            return new Weight(converted, targetUnit);
        }

        public Weight Add(Weight other)
        {
            if (other == null)
            {
                throw new ArgumentException("Other weight cannot be null");
            }

            double sumBase = this.ToBase() + other.ToBase();
            double result = WeightUnitExtensions.ConvertFromBaseUnit(this.unit, sumBase);

            return new Weight(result, this.unit);
        }

        public Weight Add(Weight other, WeightUnit targetUnit)
        {
            if (other == null)
            {
                throw new ArgumentException("Other weight cannot be null");
            }

            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit))
            {
                throw new ArgumentException("Invalid target unit");
            }

            double sumBase = this.ToBase() + other.ToBase();
            double result = WeightUnitExtensions.ConvertFromBaseUnit(targetUnit, sumBase);

            return new Weight(result, targetUnit);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(Weight))
            {
                return false;
            }

            Weight other = (Weight)obj;

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