using System;

namespace QuantityMeasurementApp
{
    // Generic Length class that represents a measurement with value and unit.
    // This class follows DRY principle by eliminating duplicate Feet and Inches classes.
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

            this.value = value;
            this.unit = unit;
        }

        // Converts the current length to base unit (Feet).
        private double ConvertToBaseUnit()
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

        // Static Convert Method (UC5 API)
        public static double Convert(double value, LengthUnit source, LengthUnit target)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Invalid numeric value");

            double valueInFeet;

            switch (source)
            {
                case LengthUnit.FEET:
                    valueInFeet = value;
                    break;

                case LengthUnit.INCH:
                    valueInFeet = value / 12.0;
                    break;

                case LengthUnit.YARD:
                    valueInFeet = value * 3.0;
                    break;

                case LengthUnit.CENTIMETER:
                    valueInFeet = (value * 0.393701) / 12.0;
                    break;

                default:
                    throw new ArgumentException("Unsupported Source Unit");
            }

            switch (target)
            {
                case LengthUnit.FEET:
                    return valueInFeet;

                case LengthUnit.INCH:
                    return valueInFeet * 12.0;

                case LengthUnit.YARD:
                    return valueInFeet / 3.0;

                case LengthUnit.CENTIMETER:
                    return valueInFeet * 12.0 / 0.393701;

                default:
                    throw new ArgumentException("Unsupported Target Unit");
            }
        }

        // Instance conversion (immutability)
        public Length ConvertTo(LengthUnit target)
        {
            double newValue = Convert(this.value, this.unit, target);
            return new Length(newValue, target);
        }
        // Returns result in unit of first operand
        public Length Add(Length other)
        {
            if (other == null)
            {
                throw new ArgumentException("Second operand cannot be null");
            }

            // Convert both to base unit (Feet)
            double firstInFeet = this.ConvertToBaseUnit();
            double secondInFeet = other.ConvertToBaseUnit();

            // Add
            double totalFeet = firstInFeet + secondInFeet;

            // Convert back to unit of first operand
            double resultValue = Convert(totalFeet, LengthUnit.FEET, this.unit);

            return new Length(resultValue, this.unit);
        }

        // Compares two Length objects based on their base unit values.
        public bool Compare(Length other)
        {
            if (other == null)
            {
                return false;
            }

            return this.ConvertToBaseUnit() == other.ConvertToBaseUnit();
        }

        // Overriding Equals method
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is null)
                return false;

            if (obj.GetType() != typeof(Length))
                return false;

            Length other = (Length)obj;

            return this.Compare(other);
        }

        // Overriding GetHashCode
        public override int GetHashCode()
        {
            return ConvertToBaseUnit().GetHashCode();
        }
    }
}