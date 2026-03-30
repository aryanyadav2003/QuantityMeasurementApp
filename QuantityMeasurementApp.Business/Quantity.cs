using System;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Business.Implementations;
using QuantityMeasurementApp.Entity.Enums;

namespace QuantityMeasurementApp.Business
{
    public class Quantity<U> where U : struct
    {
        private double value;
        private U unit;

        public Quantity(double value, U unit)
        {
            this.value = value;
            this.unit  = unit;
        }

        public double Value
        {
            get { return value; }
        }

        public U Unit
        {
            get { return unit; }
        }

        private double ToBase()
        {
            if (unit is LengthUnit)
            {
                LengthUnit u = (LengthUnit)(object)unit;
                return u.ConvertToBaseUnit(value);
            }
            if (unit is WeightUnit)
            {
                WeightUnit u = (WeightUnit)(object)unit;
                return u.ConvertToBaseUnit(value);
            }
            if (unit is VolumeUnit)
            {
                VolumeUnit u = (VolumeUnit)(object)unit;
                return u.ConvertToBaseUnit(value);
            }
            if (unit is TemperatureUnit)
            {
                TemperatureUnit u = (TemperatureUnit)(object)unit;
                return u.ConvertToBaseUnit(value);
            }
            throw new ArgumentException("Unsupported unit type: " + typeof(U).Name);
        }

        private double ConvertBaseValueToTarget(double baseValue, U targetUnit)
        {
            if (targetUnit is LengthUnit)
            {
                LengthUnit u = (LengthUnit)(object)targetUnit;
                return u.ConvertFromBaseUnit(baseValue);
            }
            if (targetUnit is WeightUnit)
            {
                WeightUnit u = (WeightUnit)(object)targetUnit;
                return u.ConvertFromBaseUnit(baseValue);
            }
            if (targetUnit is VolumeUnit)
            {
                VolumeUnit u = (VolumeUnit)(object)targetUnit;
                return u.ConvertFromBaseUnit(baseValue);
            }
            if (targetUnit is TemperatureUnit)
            {
                TemperatureUnit u = (TemperatureUnit)(object)targetUnit;
                return u.ConvertFromBaseUnit(baseValue);
            }
            throw new ArgumentException("Unsupported unit type: " + typeof(U).Name);
        }

        private static double RoundToTwoDecimals(double value)
        {
            return Math.Round(value, 2);
        }

        private void ValidateArithmeticOperands(Quantity<U> other)
        {
            if (other == null)
                throw new ArgumentException("Operand cannot be null");

            if (!double.IsFinite(this.value))
                throw new ArgumentException("This quantity value must be finite");

            if (!double.IsFinite(other.value))
                throw new ArgumentException("Operand value must be finite");
        }

        private void ValidateOperationSupport(string operation)
        {
            if (unit is TemperatureUnit)
            {
                TemperatureUnit u = (TemperatureUnit)(object)unit;
                u.ValidateOperationSupport(operation);
            }
        }

        private double PerformBaseArithmetic(Quantity<U> other, ArithmeticOperation operation)
        {
            double base1 = this.ToBase();
            double base2 = other.ToBase();

            switch (operation)
            {
                case ArithmeticOperation.ADD:
                    return base1 + base2;

                case ArithmeticOperation.SUBTRACT:
                    return base1 - base2;

                case ArithmeticOperation.DIVIDE:
                    if (Math.Abs(base2) < 1e-10)
                        throw new ArithmeticException("Division by zero is not allowed");
                    return base1 / base2;

                case ArithmeticOperation.MULTIPLY:
                    return base1 * base2;

                default:
                    throw new ArgumentException("Unsupported arithmetic operation");
            }
        }

        public Quantity<U> ConvertTo(U targetUnit)
        {
            double baseValue = ToBase();
            double result    = ConvertBaseValueToTarget(baseValue, targetUnit);
            return new Quantity<U>(result, targetUnit);
        }

        public Quantity<U> Add(Quantity<U> other, U targetUnit)
        {
            ValidateOperationSupport("ADD");
            ValidateArithmeticOperands(other);
            double baseResult  = PerformBaseArithmetic(other, ArithmeticOperation.ADD);
            double resultValue = ConvertBaseValueToTarget(baseResult, targetUnit);
            return new Quantity<U>(RoundToTwoDecimals(resultValue), targetUnit);
        }

        public Quantity<U> Subtract(Quantity<U> other, U targetUnit)
        {
            ValidateOperationSupport("SUBTRACT");
            ValidateArithmeticOperands(other);
            double baseResult  = PerformBaseArithmetic(other, ArithmeticOperation.SUBTRACT);
            double resultValue = ConvertBaseValueToTarget(baseResult, targetUnit);
            return new Quantity<U>(RoundToTwoDecimals(resultValue), targetUnit);
        }

        public double Divide(Quantity<U> other)
        {
            ValidateOperationSupport("DIVIDE");
            ValidateArithmeticOperands(other);
            return PerformBaseArithmetic(other, ArithmeticOperation.DIVIDE);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Quantity<U> other = obj as Quantity<U>;

            if (other == null)
                return false;

            double base1 = this.ToBase();
            double base2 = other.ToBase();

            return Math.Abs(base1 - base2) < 0.0001;
        }

        public override int GetHashCode()
        {
            return ToBase().GetHashCode();
        }

        public override string ToString()
        {
            return value + " " + unit.ToString();
        }
    }
}