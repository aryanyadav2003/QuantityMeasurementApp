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