using System;

namespace QuantityMeasurementApp
{
    public class QuantityMeasurementApp
    {
        public static void Main(string[] args)
        {
            Length l1 = new Length(1.0, LengthUnit.YARD);
            Length l2 = new Length(3.0, LengthUnit.FEET);
            Length l3 = new Length(36.0, LengthUnit.INCH);

            Console.WriteLine("Yard equals Feet: " + l1.Equals(l2));   // True
            Console.WriteLine("Yard equals Inch: " + l1.Equals(l3));  // True

            Length cm = new Length(1.0, LengthUnit.CENTIMETER);
            Length inch = new Length(0.393701, LengthUnit.INCH);

            Console.WriteLine("CM equals Inch: " + cm.Equals(inch));  // True

            Console.WriteLine("--- Static Conversion ---");

            double feetToInch = Length.Convert(1.0, LengthUnit.FEET, LengthUnit.INCH);
            Console.WriteLine("1 Foot = " + feetToInch + " Inches");

            double yardToFeet = Length.Convert(3.0, LengthUnit.YARD, LengthUnit.FEET);
            Console.WriteLine("3 Yard = " + yardToFeet + " Feet");

            double cmToInch = Length.Convert(2.54, LengthUnit.CENTIMETER, LengthUnit.INCH);
            Console.WriteLine("2.54 CM = " + cmToInch + " Inch");

            Console.WriteLine("--- Instance Conversion ---");

            Length original = new Length(5.0, LengthUnit.FEET);
            Length converted = original.ConvertTo(LengthUnit.INCH);

            Console.WriteLine("Original: 5 FEET");
            Console.WriteLine("Converted: " + Length.Convert(5.0, LengthUnit.FEET, LengthUnit.INCH) + " INCH");
        }
    }
}