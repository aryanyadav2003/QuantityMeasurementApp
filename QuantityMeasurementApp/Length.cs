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

            this.value = value;
            this.unit = unit;
        }

        // Convert any unit → FEET (base unit)
        private double ConvertToFeet()
        {
            switch (unit)
            {
                case LengthUnit.FEET:
                    return value;

                case LengthUnit.INCH:
                    return value / 12.0;

                case LengthUnit.YARD:
                    return value * 3.0;

                case LengthUnit.CENTIMETER:
                    return (value * 0.393701) / 12.0;

                default:
                    throw new ArgumentException("Unsupported Unit");
            }
        }

        // Convert FEET → target unit
        private static double ConvertFromFeet(double feet, LengthUnit targetUnit)
        {
            switch (targetUnit)
            {
                case LengthUnit.FEET:
                    return feet;

                case LengthUnit.INCH:
                    return feet * 12.0;

                case LengthUnit.YARD:
                    return feet / 3.0;

                case LengthUnit.CENTIMETER:
                    return feet * 12.0 / 0.393701;

                default:
                    throw new ArgumentException("Unsupported Unit");
            }
        }

        // UC7 — Add with explicit target unit
        public Length Add(Length other, LengthUnit targetUnit)
        {
            if (other == null)
                throw new ArgumentException("Second operand cannot be null");

            double totalFeet = this.ConvertToFeet() + other.ConvertToFeet();

            double result = ConvertFromFeet(totalFeet, targetUnit);

            return new Length(result, targetUnit);
        }

        // Compare equality
        public bool Compare(Length other)
        {
            if (other == null)
                return false;

            double a = this.ConvertToFeet();
            double b = other.ConvertToFeet();

            return Math.Abs(a - b) < 0.0001;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Length other)
                return false;

            return Compare(other);
        }

        public override int GetHashCode()
        {
            return ConvertToFeet().GetHashCode();
        }

        // Helper methods for tests
        public double GetValue()
        {
            return value;
        }

        public LengthUnit GetUnit()
        {
            return unit;
        }

        public double GetValueInFeet()
        {
            return ConvertToFeet();
        }
    }
}