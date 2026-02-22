using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class LengthTests
    {
        [TestMethod]
        public void testEquality_YardToYard_SameValue()
        {
            Length l1 = new Length(1.0, LengthUnit.YARD);
            Length l2 = new Length(1.0, LengthUnit.YARD);
            Assert.IsTrue(l1.Equals(l2));
        }

        [TestMethod]
        public void testEquality_YardToYard_DifferentValue()
        {
            Length l1 = new Length(1.0, LengthUnit.YARD);
            Length l2 = new Length(2.0, LengthUnit.YARD);
            Assert.IsFalse(l1.Equals(l2));
        }
        [TestMethod]
        public void testEquality_YardToFeet_EquivalentValue()
        {
            Length l1 = new Length(1.0, LengthUnit.YARD);
            Length l2 = new Length(3.0, LengthUnit.FEET);
            Assert.IsTrue(l1.Equals(l2));
        }

        [TestMethod]
        public void testEquality_FeetToYard_EquivalentValue()
        {
            Length l1 = new Length(3.0, LengthUnit.FEET);
            Length l2 = new Length(1.0, LengthUnit.YARD);
            Assert.IsTrue(l1.Equals(l2));
        }

        [TestMethod]
        public void testEquality_YardToInches_EquivalentValue()
        {
            Length l1 = new Length(1.0, LengthUnit.YARD);
            Length l2 = new Length(36.0, LengthUnit.INCH);
            Assert.IsTrue(l1.Equals(l2));
        }

        [TestMethod]
        public void testEquality_InchesToYard_EquivalentValue()
        {
            Length l1 = new Length(36.0, LengthUnit.INCH);
            Length l2 = new Length(1.0, LengthUnit.YARD);
            Assert.IsTrue(l1.Equals(l2));
        }

        [TestMethod]
        public void testEquality_YardToFeet_NonEquivalentValue()
        {
            Length l1 = new Length(1.0, LengthUnit.YARD);
            Length l2 = new Length(2.0, LengthUnit.FEET);
            Assert.IsFalse(l1.Equals(l2));
        }

        [TestMethod]
        public void testEquality_centimetersToInches_EquivalentValue()
        {
            Length l1 = new Length(1.0, LengthUnit.CENTIMETER);
            Length l2 = new Length(0.393701, LengthUnit.INCH);
            Assert.IsTrue(l1.Equals(l2));
        }

        [TestMethod]
        public void testEquality_centimetersToFeet_NonEquivalentValue()
        {
            Length l1 = new Length(1.0, LengthUnit.CENTIMETER);
            Length l2 = new Length(1.0, LengthUnit.FEET);
            Assert.IsFalse(l1.Equals(l2));
        }

        [TestMethod]
        public void testEquality_MultiUnit_TransitiveProperty()
        {
            Length yard = new Length(1.0, LengthUnit.YARD);
            Length feet = new Length(3.0, LengthUnit.FEET);
            Length inches = new Length(36.0, LengthUnit.INCH);

            Assert.IsTrue(yard.Equals(feet));
            Assert.IsTrue(feet.Equals(inches));
            Assert.IsTrue(yard.Equals(inches));
        }

        [TestMethod]
        public void testEquality_YardSameReference()
        {
            Length yard = new Length(2.0, LengthUnit.YARD);
            Assert.IsTrue(yard.Equals(yard));
        }

        [TestMethod]
        public void testEquality_YardNullComparison()
        {
            Length yard = new Length(2.0, LengthUnit.YARD);
            Assert.IsFalse(yard.Equals(null));
        }

        [TestMethod]
        public void testEquality_CentimetersSameReference()
        {
            Length cm = new Length(5.0, LengthUnit.CENTIMETER);
            Assert.IsTrue(cm.Equals(cm));
        }

        [TestMethod]
        public void testEquality_CentimetersNullComparison()
        {
            Length cm = new Length(5.0, LengthUnit.CENTIMETER);
            Assert.IsFalse(cm.Equals(null));
        }

        [TestMethod]
        public void testEquality_AllUnits_ComplexScenario()
        {
            Length yard = new Length(2.0, LengthUnit.YARD);
            Length feet = new Length(6.0, LengthUnit.FEET);
            Length inches = new Length(72.0, LengthUnit.INCH);

            Assert.IsTrue(yard.Equals(feet));
            Assert.IsTrue(feet.Equals(inches));
            Assert.IsTrue(yard.Equals(inches));
        }
    }
}