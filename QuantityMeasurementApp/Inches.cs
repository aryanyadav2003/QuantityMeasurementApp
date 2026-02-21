using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuantityMeasurementApp
{
    // This class is immutable and supports value-based equality.
    public class Inches
    {
        // Read-only property (immutable)
        public double Value { get; }

        // Constructor to initialize value
        public Inches(double value)
        {
            Value = value;
        }
        public override bool Equals(object obj)
        {
            // Check if same reference
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            // Check for null or different type
            if (obj == null || obj.GetType() != typeof(Inches))
            {
                return false;
            }
            // Cast safely
            Inches other = (Inches)obj;

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