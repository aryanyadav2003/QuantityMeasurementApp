using System;

namespace QuantityMeasurementApp
{
    public class QuantityMeasurementApp
    {
        public static void Main(string[] args)
        {
            Length a = new Length(1.0, LengthUnit.FEET);
            Length b = new Length(12.0, LengthUnit.INCHES);

            Length result1 = a.Add(b, LengthUnit.FEET);
            Console.WriteLine(result1);

            Length result2 = a.Add(b, LengthUnit.YARDS);
            Console.WriteLine(result2);

            Length converted = a.ConvertTo(LengthUnit.INCHES);
            Console.WriteLine(converted);

            Length c = new Length(36.0, LengthUnit.INCHES);
            Length d = new Length(1.0, LengthUnit.YARDS);

            Console.WriteLine(c.Equals(d));
        }
    }
}