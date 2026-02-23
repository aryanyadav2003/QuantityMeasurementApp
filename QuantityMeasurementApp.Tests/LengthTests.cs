using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp;
using System;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class LengthTests
    {
        [TestMethod]
        public void testConversion_FeetToInches()
        {
            // Arrange
            double value = 1.0;

            // Act
            double result = Length.Convert(value, LengthUnit.FEET, LengthUnit.INCH);

            // Assert
            Assert.AreEqual(12.0, result);
        }

        [TestMethod]
        public void testConversion_InchesToFeet()
        {
            // Arrange
            double value = 24.0;

            // Act
            double result = Length.Convert(value, LengthUnit.INCH, LengthUnit.FEET);

            // Assert
            Assert.AreEqual(2.0, result);
        }

        [TestMethod]
        public void testConversion_YardsToInches()
        {
            // Arrange
            double value = 1.0;

            // Act
            double result = Length.Convert(value, LengthUnit.YARD, LengthUnit.INCH);

            // Assert
            Assert.AreEqual(36.0, result);
        }

        [TestMethod]
        public void testConversion_InchesToYards()
        {
            // Arrange
            double value = 72.0;

            // Act
            double result = Length.Convert(value, LengthUnit.INCH, LengthUnit.YARD);

            // Assert
            Assert.AreEqual(2.0, result);
        }

        [TestMethod]
        public void testConversion_CentimetersToInches()
        {
            // Arrange
            double value = 2.54;

            // Act
            double result = Length.Convert(value, LengthUnit.CENTIMETER, LengthUnit.INCH);

            // Assert (with tolerance)
            Assert.AreEqual(1.0, result, 0.000001);
        }

        [TestMethod]
        public void testConversion_FeetToYard()
        {
            // Arrange
            double value = 6.0;

            // Act
            double result = Length.Convert(value, LengthUnit.FEET, LengthUnit.YARD);

            // Assert
            Assert.AreEqual(2.0, result);
        }

        [TestMethod]
        public void testConversion_RoundTrip_PreservesValue()
        {
            // Arrange
            double value = 5.0;

            // Act
            double converted = Length.Convert(value, LengthUnit.FEET, LengthUnit.INCH);
            double result = Length.Convert(converted, LengthUnit.INCH, LengthUnit.FEET);

            // Assert (within tolerance)
            Assert.AreEqual(value, result, 0.000001);
        }

        [TestMethod]
        public void testConversion_ZeroValue()
        {
            // Arrange
            double value = 0.0;

            // Act
            double result = Length.Convert(value, LengthUnit.FEET, LengthUnit.INCH);

            // Assert
            Assert.AreEqual(0.0, result);
        }

        [TestMethod]
        public void testConversion_NegativeValue()
        {
            // Arrange
            double value = -1.0;

            // Act
            double result = Length.Convert(value, LengthUnit.FEET, LengthUnit.INCH);

            // Assert
            Assert.AreEqual(-12.0, result);
        }

        [TestMethod]
        public void testConversion_InvalidUnit_Throws()
        {
            // Arrange
            double value = 1.0;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                Length.Convert(value, (LengthUnit)999, LengthUnit.FEET);
            });
        }

        [TestMethod]
        public void testConversion_NaNOrInfinite_Throws()
        {
            // Arrange
            double value = double.NaN;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                Length.Convert(value, LengthUnit.FEET, LengthUnit.INCH);
            });
        }

        [TestMethod]
        public void testConversion_PrecisionTolerance()
        {
            // Arrange
            double value = 1.0;

            // Act
            double result = Length.Convert(value, LengthUnit.CENTIMETER, LengthUnit.INCH);

            // Assert (precision check)
            Assert.AreEqual(0.393701, result, 0.000001);
        }
    }
}