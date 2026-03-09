using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp;
using System;

namespace QuantityMeasurementTests
{
    [TestClass]
    public class LengthUnitRefactoredTests
    {
        private const double EPSILON = 0.001;

        // ---------- ENUM CONSTANT TESTS ----------

        [TestMethod]
        public void testLengthUnitEnum_FeetConstant()
        {
            Assert.AreEqual(1.0, LengthUnit.FEET.GetConversionFactor(), EPSILON);
        }

        [TestMethod]
        public void testLengthUnitEnum_InchesConstant()
        {
            Assert.AreEqual(1.0 / 12.0, LengthUnit.INCHES.GetConversionFactor(), EPSILON);
        }

        [TestMethod]
        public void testLengthUnitEnum_YardsConstant()
        {
            Assert.AreEqual(3.0, LengthUnit.YARDS.GetConversionFactor(), EPSILON);
        }

        [TestMethod]
        public void testLengthUnitEnum_CentimetersConstant()
        {
            Assert.AreEqual(1.0 / 30.48, LengthUnit.CENTIMETERS.GetConversionFactor(), EPSILON);
        }

        // ---------- CONVERT TO BASE UNIT ----------

        [TestMethod]
        public void testConvertToBaseUnit_FeetToFeet()
        {
            Assert.AreEqual(5.0, LengthUnit.FEET.ConvertToBaseUnit(5.0), EPSILON);
        }

        [TestMethod]
        public void testConvertToBaseUnit_InchesToFeet()
        {
            Assert.AreEqual(1.0, LengthUnit.INCHES.ConvertToBaseUnit(12.0), EPSILON);
        }

        [TestMethod]
        public void testConvertToBaseUnit_YardsToFeet()
        {
            Assert.AreEqual(3.0, LengthUnit.YARDS.ConvertToBaseUnit(1.0), EPSILON);
        }

        [TestMethod]
        public void testConvertToBaseUnit_CentimetersToFeet()
        {
            Assert.AreEqual(1.0, LengthUnit.CENTIMETERS.ConvertToBaseUnit(30.48), EPSILON);
        }

        // ---------- CONVERT FROM BASE UNIT ----------

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToFeet()
        {
            Assert.AreEqual(2.0, LengthUnit.FEET.ConvertFromBaseUnit(2.0), EPSILON);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToInches()
        {
            Assert.AreEqual(12.0, LengthUnit.INCHES.ConvertFromBaseUnit(1.0), EPSILON);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToYards()
        {
            Assert.AreEqual(1.0, LengthUnit.YARDS.ConvertFromBaseUnit(3.0), EPSILON);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToCentimeters()
        {
            Assert.AreEqual(30.48, LengthUnit.CENTIMETERS.ConvertFromBaseUnit(1.0), EPSILON);
        }

        // ---------- QUANTITY LENGTH TESTS ----------

        [TestMethod]
        public void testQuantityLengthRefactored_Equality()
        {
            Length a = new Length(1.0, LengthUnit.FEET);
            Length b = new Length(12.0, LengthUnit.INCHES);

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void testQuantityLengthRefactored_ConvertTo()
        {
            Length a = new Length(1.0, LengthUnit.FEET);

            Length result = a.ConvertTo(LengthUnit.INCHES);

            Assert.AreEqual(12.0, result.GetValue(), EPSILON);
            Assert.AreEqual(LengthUnit.INCHES, result.GetUnit());
        }

        [TestMethod]
        public void testQuantityLengthRefactored_Add()
        {
            Length a = new Length(1.0, LengthUnit.FEET);
            Length b = new Length(12.0, LengthUnit.INCHES);

            Length result = a.Add(b, LengthUnit.FEET);

            Assert.AreEqual(2.0, result.GetValue(), EPSILON);
            Assert.AreEqual(LengthUnit.FEET, result.GetUnit());
        }

        [TestMethod]
        public void testQuantityLengthRefactored_AddWithTargetUnit()
        {
            Length a = new Length(1.0, LengthUnit.FEET);
            Length b = new Length(12.0, LengthUnit.INCHES);

            Length result = a.Add(b, LengthUnit.YARDS);

            Assert.AreEqual(0.6667, result.GetValue(), EPSILON);
            Assert.AreEqual(LengthUnit.YARDS, result.GetUnit());
        }

        // ---------- VALIDATION TESTS ----------

        [TestMethod]
        public void testQuantityLengthRefactored_NullUnit()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Length(1.0, (LengthUnit)(-1));
            });
        }

        [TestMethod]
        public void testQuantityLengthRefactored_InvalidValue()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Length(double.NaN, LengthUnit.FEET);
            });
        }

        // ---------- BACKWARD COMPATIBILITY TESTS ----------

        [TestMethod]
        public void testBackwardCompatibility_UC1EqualityTests()
        {
            Length yard = new Length(1.0, LengthUnit.YARDS);
            Length feet = new Length(3.0, LengthUnit.FEET);

            Assert.IsTrue(yard.Equals(feet));
        }

        [TestMethod]
        public void testBackwardCompatibility_UC5ConversionTests()
        {
            Length a = new Length(1.0, LengthUnit.FEET);

            Length result = a.ConvertTo(LengthUnit.INCHES);

            Assert.AreEqual(12.0, result.GetValue(), EPSILON);
        }

        [TestMethod]
        public void testBackwardCompatibility_UC6AdditionTests()
        {
            Length a = new Length(1.0, LengthUnit.YARDS);
            Length b = new Length(3.0, LengthUnit.FEET);

            Length result = a.Add(b, LengthUnit.FEET);

            Assert.AreEqual(6.0, result.GetValue(), EPSILON);
        }

        [TestMethod]
        public void testBackwardCompatibility_UC7AdditionWithTargetUnitTests()
        {
            Length a = new Length(1.0, LengthUnit.FEET);
            Length b = new Length(12.0, LengthUnit.INCHES);

            Length result = a.Add(b, LengthUnit.INCHES);

            Assert.AreEqual(24.0, result.GetValue(), EPSILON);
        }

        // ---------- ARCHITECTURAL TEST ----------

        [TestMethod]
        public void testArchitecturalScalability_MultipleCategories()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(LengthUnit), LengthUnit.FEET));
            Assert.IsTrue(Enum.IsDefined(typeof(LengthUnit), LengthUnit.INCHES));
        }

        // ---------- ROUND TRIP CONVERSION ----------

        [TestMethod]
        public void testRoundTripConversion_RefactoredDesign()
        {
            Length original = new Length(5.0, LengthUnit.FEET);

            Length inches = original.ConvertTo(LengthUnit.INCHES);
            Length backToFeet = inches.ConvertTo(LengthUnit.FEET);

            Assert.AreEqual(original.GetValue(), backToFeet.GetValue(), EPSILON);
        }

        // ---------- ENUM IMMUTABILITY ----------

        [TestMethod]
        public void testUnitImmutability()
        {
            LengthUnit unit = LengthUnit.FEET;

            Assert.AreEqual(LengthUnit.FEET, unit);
        }
    }
}