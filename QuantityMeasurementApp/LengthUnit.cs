using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuantityMeasurementApp
{
    // Enum representing supported length units.
    // Conversion factors are defined relative to base unit (Feet).
    public enum LengthUnit
    {
        FEET,       // 1 foot = 1 foot
        INCH,       // 1 inch = 1/12 feet
        YARD,        // 1 yard = 3 feet
        CENTIMETER   // 1 cm = 0.393701 inches = 0.393701/12 feet
    }
}