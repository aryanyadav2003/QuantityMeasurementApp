using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantityMeasurementApp;

namespace QuantityMeasurementApp
{
    public class QuantityMeasurementApp
    {
         public static void Main(string[] args)
        {
            Length length1 = new Length(1.0, LengthUnit.FEET);
            Length length2 = new Length(12.0, LengthUnit.INCH);

            Console.WriteLine("Are lengths equal? " + length1.Equals(length2));
        }
    }
}