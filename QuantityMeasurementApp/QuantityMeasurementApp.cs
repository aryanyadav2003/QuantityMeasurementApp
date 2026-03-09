using System;

namespace QuantityMeasurementApp
{
    public class QuantityMeasurementApp
    {
        public static void Main(string[] args)
        {
            Length l1 = new Length(1.0, LengthUnit.FEET);
            Length l2 = new Length(12.0, LengthUnit.INCH);

            Console.WriteLine("Length Equality:");
            Console.WriteLine(l1.Equals(l2));

            Length sumLength = l1.Add(l2);
            Console.WriteLine("Length Addition: " + sumLength);

            Weight w1 = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight w2 = new Weight(1000.0, WeightUnit.GRAM);

            Console.WriteLine("Weight Equality:");
            Console.WriteLine(w1.Equals(w2));

            Weight sumWeight = w1.Add(w2);
            Console.WriteLine("Weight Addition: " + sumWeight);
        }
    }
}