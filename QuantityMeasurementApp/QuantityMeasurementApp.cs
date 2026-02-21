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
            Console.WriteLine("Enter first value in feet:");
            string input1 = Console.ReadLine();
            Console.WriteLine("Enter second value in feet:");
            string input2 = Console.ReadLine();

            if (!double.TryParse(input1, out double value1) || !double.TryParse(input2, out double value2))
            {
                Console.WriteLine("Invalid input. Please enter numeric values only.");
                return;
            }
            Feet feet1 = new Feet(value1);
            Feet feet2 = new Feet(value2);

            bool result = feet1.Equals(feet2);

            Console.WriteLine($"Are the two measurements equal? {result}");
        }
    }
}