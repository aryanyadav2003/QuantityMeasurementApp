using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        // 1 Foot = 12 Inches
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
                    // 1 cm = 0.393701 inches
                    // convert inches to feet => divide by 12
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

            // Convert source → base (feet)
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

            // Convert base → target
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


        //Compares two Length objects based on their converted base unit values.
        public bool Compare(Length other)
        {
            if (other == null)
            {
                return false;
            }
            return this.ConvertToBaseUnit() == other.ConvertToBaseUnit();
        }
        //Overriding Equals method to follow equality contract:
        public override bool Equals(object obj)
        {
            // Same reference check (Reflexive property)
            if (ReferenceEquals(this, obj))
                return true;

            // Null check
            if (obj is null)
                return false;

            // Type check
            if (obj.GetType() != typeof(Length))
                return false;

            Length other = (Length)obj;

            return this.Compare(other);
        }
        //Overriding GetHashCode when Equals is overridden.
        public override int GetHashCode()
        {
            return ConvertToBaseUnit().GetHashCode();
        }
    }
}