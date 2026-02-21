using System;

namespace QuantityMeasurementApp
{
    // Represents a measurement in Feet.
    // This class is immutable and supports value-based equality.
    public sealed class Feet
    {
        // Read-only property(immutable)
        public double Value { get; }

        // Constructor to initialize value
        public Feet(double value)
        {
            Value = value;
        }

        // Override Equals for value-based comparison
        public override bool Equals(object obj)
        {
            // Check if same reference
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            // Check for null or different type
            if (obj == null || obj.GetType() != typeof(Feet))
            {
                return false;
            }

            // Cast safely
            Feet other = (Feet)obj;

            // Compare values
            return Value == other.Value;
        }

        // Override GetHashCode to maintain equality contract
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}