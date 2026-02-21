using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class FeetTests
    {
        [TestMethod]
        public void TestEquality_SameValue_ReturnsTrue()
        {
            //Arrange
            Feet feet1=new Feet(1.0);
            Feet feet2=new Feet(1.0);

             // Act
            bool result = feet1.Equals(feet2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestEquality_DifferentValue_ReturnsFalse()
        {
            // Arrange
            Feet feet1 = new Feet(1.0);
            Feet feet2 = new Feet(2.0);

            // Act
            bool result = feet1.Equals(feet2);

            // Assert
            Assert.IsFalse(result);
        }

         [TestMethod]
        public void TestEquality_NullComparison_ReturnsFalse()
        {
            // Arrange
            Feet feet = new Feet(1.0);

            // Act
            bool result = feet.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEquality_SameReference_ReturnsTrue()
        {
            // Arrange
            Feet feet = new Feet(1.0);

            // Act
            bool result = feet.Equals(feet);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestEquality_NonFeetObject_ReturnsFalse()
        {
            // Arrange
            Feet feet = new Feet(1.0);
            object nonFeet = new object();

            // Act
            bool result = feet.Equals(nonFeet);

            // Assert
            Assert.IsFalse(result);
        }
    }

}