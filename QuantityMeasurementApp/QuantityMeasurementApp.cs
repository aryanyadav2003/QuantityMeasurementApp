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
            Length l1 = new Length(1.0, LengthUnit.YARD);
            Length l2 = new Length(3.0, LengthUnit.FEET);
            Length l3 = new Length(36.0, LengthUnit.INCH);
            Console.WriteLine(l1.Equals(l2)); // True
            Console.WriteLine(l1.Equals(l3)); // True

            Length cm = new Length(1.0, LengthUnit.CENTIMETER);
            Length inch = new Length(0.393701, LengthUnit.INCH);
            Console.WriteLine(cm.Equals(inch)); // True
        }
    }
}