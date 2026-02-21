using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class InchesTests
    {
        [TestMethod]
        public void TestEquality_SameValue()
        {
            // Arrange
            Inches i1 = new Inches(1.0);
            Inches i2 = new Inches(1.0);

            // Act
            bool result = i1.Equals(i2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestEquality_DifferentValue()
        {
            // Arrange
            Inches i1 = new Inches(1.0);
            Inches i2 = new Inches(2.0);

            // Act
            bool result = i1.Equals(i2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEquality_NullComparison()
        {
            // Arrange
            Inches i1 = new Inches(1.0);

            // Act
            bool result = i1.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEquality_SameReference()
        {
            // Arrange
            Inches i1 = new Inches(1.0);

            // Act
            bool result = i1.Equals(i1);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestEquality_NonNumericInput()
        {
            // Arrange
            Inches i1 = new Inches(double.NaN);
            Inches i2 = new Inches(double.NaN);

            // Act
            bool result = i1.Equals(i2);

            // Assert
            Assert.IsFalse(result);
            // Because NaN == NaN is false in double comparison
        }
    }
}