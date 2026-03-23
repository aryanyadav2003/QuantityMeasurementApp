using System;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Controller
{
    public class Menu
    {
        private QuantityMeasurementController _controller;

        public Menu(QuantityMeasurementController controller)
        {
            _controller = controller;
        }

        public void Start()
        {
            bool running = true;
            while (running)
            {
                running = ShowMainMenu();
            }
            Console.WriteLine("\nGoodbye!");
        }

        // ── MAIN MENU ─────────────────────────────────────────

        private bool ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("    Quantity Measurement Menu            ");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("1.  Length Conversion");
            Console.WriteLine("2.  Weight Conversion");
            Console.WriteLine("3.  Volume Conversion");
            Console.WriteLine("4.  Temperature Conversion");
            Console.WriteLine("5.  Length Addition");
            Console.WriteLine("6.  Weight Addition");
            Console.WriteLine("7.  Volume Addition");
            Console.WriteLine("8.  Length Subtraction");
            Console.WriteLine("9.  Weight Subtraction");
            Console.WriteLine("10. Volume Subtraction");
            Console.WriteLine("11. Length Division");
            Console.WriteLine("12. Weight Division");
            Console.WriteLine("13. Volume Division");
            Console.WriteLine("14. Compare Quantities");
            Console.WriteLine("0.  Exit");
            Console.WriteLine("-----------------------------------------");
            Console.Write("Select option: ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":  DoConvert("LENGTH",      LengthUnits());      break;
                case "2":  DoConvert("WEIGHT",      WeightUnits());      break;
                case "3":  DoConvert("VOLUME",      VolumeUnits());      break;
                case "4":  DoConvert("TEMPERATURE", TemperatureUnits()); break;
                case "5":  DoAdd("LENGTH",           LengthUnits());     break;
                case "6":  DoAdd("WEIGHT",           WeightUnits());     break;
                case "7":  DoAdd("VOLUME",           VolumeUnits());     break;
                case "8":  DoSubtract("LENGTH",      LengthUnits());     break;
                case "9":  DoSubtract("WEIGHT",      WeightUnits());     break;
                case "10": DoSubtract("VOLUME",      VolumeUnits());     break;
                case "11": DoDivide("LENGTH",        LengthUnits());     break;
                case "12": DoDivide("WEIGHT",        WeightUnits());     break;
                case "13": DoDivide("VOLUME",        VolumeUnits());     break;
                case "14": DoCompare();                                   break;
                case "0":  return false;
                default:
                    Console.WriteLine("  Invalid choice. Press any key...");
                    Console.ReadKey();
                    break;
            }

            return true;
        }

        // ── OPERATIONS ────────────────────────────────────────

        private void DoConvert(string measurementType, string[] units)
        {
            Console.Clear();
            Console.WriteLine("----------- " + measurementType + " CONVERSION -----------");

            double val      = ReadDouble("Enter value       : ");
            string fromUnit = PickUnit("Select source unit:", units);
            string toUnit   = PickUnit("Select target unit:", units);

            QuantityDTO q = new QuantityDTO(val, fromUnit, measurementType);
            _controller.PerformConvert(q, toUnit);
            Pause();
        }

        private void DoAdd(string measurementType, string[] units)
        {
            Console.Clear();
            Console.WriteLine("----------- " + measurementType + " ADDITION -----------");

            double val1   = ReadDouble("Enter first value : ");
            string unit1  = PickUnit("Select first unit :", units);
            double val2   = ReadDouble("Enter second value: ");
            string unit2  = PickUnit("Select second unit:", units);
            string target = PickUnit("Select result unit:", units);

            QuantityDTO q1 = new QuantityDTO(val1, unit1, measurementType);
            QuantityDTO q2 = new QuantityDTO(val2, unit2, measurementType);
            _controller.PerformAdd(q1, q2, target);
            Pause();
        }

        private void DoSubtract(string measurementType, string[] units)
        {
            Console.Clear();
            Console.WriteLine("----------- " + measurementType + " SUBTRACTION -----------");

            double val1   = ReadDouble("Enter first value : ");
            string unit1  = PickUnit("Select first unit :", units);
            double val2   = ReadDouble("Enter second value: ");
            string unit2  = PickUnit("Select second unit:", units);
            string target = PickUnit("Select result unit:", units);

            QuantityDTO q1 = new QuantityDTO(val1, unit1, measurementType);
            QuantityDTO q2 = new QuantityDTO(val2, unit2, measurementType);
            _controller.PerformSubtract(q1, q2, target);
            Pause();
        }

        private void DoDivide(string measurementType, string[] units)
        {
            Console.Clear();
            Console.WriteLine("----------- " + measurementType + " DIVISION -----------");

            double val1  = ReadDouble("Enter first value : ");
            string unit1 = PickUnit("Select first unit :", units);
            double val2  = ReadDouble("Enter second value: ");
            string unit2 = PickUnit("Select second unit:", units);

            QuantityDTO q1 = new QuantityDTO(val1, unit1, measurementType);
            QuantityDTO q2 = new QuantityDTO(val2, unit2, measurementType);
            _controller.PerformDivide(q1, q2);
            Pause();
        }

        private void DoCompare()
        {
            Console.Clear();
            Console.WriteLine("----------- COMPARE QUANTITIES -----------");
            Console.WriteLine("Select measurement type:");
            Console.WriteLine("  1. Length");
            Console.WriteLine("  2. Weight");
            Console.WriteLine("  3. Volume");
            Console.WriteLine("  4. Temperature");
            Console.Write("Choice: ");

            string input = Console.ReadLine();
            string measurementType;
            string[] units;

            if (input == "1")      { measurementType = "LENGTH";      units = LengthUnits(); }
            else if (input == "2") { measurementType = "WEIGHT";      units = WeightUnits(); }
            else if (input == "3") { measurementType = "VOLUME";      units = VolumeUnits(); }
            else if (input == "4") { measurementType = "TEMPERATURE"; units = TemperatureUnits(); }
            else
            {
                Console.WriteLine("  Invalid choice. Press any key...");
                Console.ReadKey();
                return;
            }

            double val1  = ReadDouble("Enter first value : ");
            string unit1 = PickUnit("Select first unit :", units);
            double val2  = ReadDouble("Enter second value: ");
            string unit2 = PickUnit("Select second unit:", units);

            QuantityDTO q1 = new QuantityDTO(val1, unit1, measurementType);
            QuantityDTO q2 = new QuantityDTO(val2, unit2, measurementType);
            _controller.PerformCompare(q1, q2);
            Pause();
        }

        // ── UNIT LISTS ────────────────────────────────────────

        private string[] LengthUnits()
        {
            return new string[] { "FEET", "INCHES", "YARDS", "CENTIMETERS" };
        }

        private string[] WeightUnits()
        {
            return new string[] { "KILOGRAM", "GRAM", "POUND" };
        }

        private string[] VolumeUnits()
        {
            return new string[] { "LITRE", "MILLILITRE", "GALLON" };
        }

        private string[] TemperatureUnits()
        {
            return new string[] { "CELSIUS", "FAHRENHEIT" };
        }

        // ── INPUT HELPERS ─────────────────────────────────────

        private string PickUnit(string prompt, string[] units)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                for (int i = 0; i < units.Length; i++)
                {
                    Console.WriteLine("  " + (i + 1) + ". " + units[i]);
                }
                Console.Write("Choice: ");

                string input = Console.ReadLine();
                int choice;

                if (int.TryParse(input, out choice) && choice >= 1 && choice <= units.Length)
                {
                    return units[choice - 1];
                }

                Console.WriteLine("  Invalid. Enter a number between 1 and " + units.Length);
            }
        }

        private double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                double value;

                if (double.TryParse(input, out value))
                {
                    return value;
                }

                Console.WriteLine("  Invalid number. Try again.");
            }
        }

        private void Pause()
        {
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }
    }
}