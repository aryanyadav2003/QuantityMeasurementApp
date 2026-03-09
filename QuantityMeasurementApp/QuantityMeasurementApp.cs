using System;

namespace QuantityMeasurementApp
{
    public class QuantityMeasurementApp
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== UC7 Addition with Target Unit ===");

            Length a = new Length(1.0, LengthUnit.FEET);
            Length b = new Length(12.0, LengthUnit.INCH);

            Length resultFeet = a.Add(b, LengthUnit.FEET);
            Console.WriteLine("1 Feet + 12 Inch = " + resultFeet.GetValue() + " Feet");

            Length resultInch = a.Add(b, LengthUnit.INCH);
            Console.WriteLine("1 Feet + 12 Inch = " + resultInch.GetValue() + " Inch");

            Length resultYard = a.Add(b, LengthUnit.YARD);
            Console.WriteLine("1 Feet + 12 Inch = " + resultYard.GetValue() + " Yard");

            Length c = new Length(1.0, LengthUnit.INCH);
            Length d = new Length(1.0, LengthUnit.INCH);

            Length resultCm = c.Add(d, LengthUnit.CENTIMETER);
            Console.WriteLine("1 Inch + 1 Inch = " + resultCm.GetValue() + " CM");
        }
    }
}