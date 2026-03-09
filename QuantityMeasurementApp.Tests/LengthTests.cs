using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp;
using System;

namespace QuantityMeasurementTests
{
    [TestClass]
    public class LengthAdditionTests
    {
        private const double EPSILON = 0.001;

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Feet()
        {
            Length a = new Length(1.0, LengthUnit.FEET);
            Length b = new Length(12.0, LengthUnit.INCH);

            Length result = a.Add(b, LengthUnit.FEET);

            Assert.AreEqual(2.0, result.GetValue(), EPSILON);
            Assert.AreEqual(LengthUnit.FEET, result.GetUnit());
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Inches()
        {
            Length a = new Length(1.0, LengthUnit.FEET);
            Length b = new Length(12.0, LengthUnit.INCH);

            Length result = a.Add(b, LengthUnit.INCH);

            Assert.AreEqual(24.0, result.GetValue(), EPSILON);
            Assert.AreEqual(LengthUnit.INCH, result.GetUnit());
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Yards()
        {
            Length a = new Length(1.0, LengthUnit.FEET);
            Length b = new Length(12.0, LengthUnit.INCH);

            Length result = a.Add(b, LengthUnit.YARD);

            Assert.AreEqual(0.6667, result.GetValue(), EPSILON);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Centimeters()
        {
            Length a = new Length(1.0, LengthUnit.INCH);
            Length b = new Length(1.0, LengthUnit.INCH);

            Length result = a.Add(b, LengthUnit.CENTIMETER);

            Assert.AreEqual(5.08, result.GetValue(), EPSILON);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsFirstOperand()
        {
            Length a = new Length(2.0, LengthUnit.YARD);
            Length b = new Length(3.0, LengthUnit.FEET);

            Length result = a.Add(b, LengthUnit.YARD);

            Assert.AreEqual(3.0, result.GetValue(), EPSILON);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsSecondOperand()
        {
            Length a = new Length(2.0, LengthUnit.YARD);
            Length b = new Length(3.0, LengthUnit.FEET);

            Length result = a.Add(b, LengthUnit.FEET);

            Assert.AreEqual(9.0, result.GetValue(), EPSILON);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Commutativity()
        {
            Length a = new Length(1.0, LengthUnit.FEET);
            Length b = new Length(12.0, LengthUnit.INCH);

            Length result1 = a.Add(b, LengthUnit.YARD);
            Length result2 = b.Add(a, LengthUnit.YARD);

            Assert.AreEqual(result1.GetValue(), result2.GetValue(), EPSILON);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_WithZero()
        {
            Length a = new Length(5.0, LengthUnit.FEET);
            Length b = new Length(0.0, LengthUnit.INCH);

            Length result = a.Add(b, LengthUnit.YARD);

            Assert.AreEqual(1.6667, result.GetValue(), EPSILON);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NegativeValues()
        {
            Length a = new Length(5.0, LengthUnit.FEET);
            Length b = new Length(-2.0, LengthUnit.FEET);

            Length result = a.Add(b, LengthUnit.INCH);

            Assert.AreEqual(36.0, result.GetValue(), EPSILON);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NullTargetUnit()
        {
            Length a = new Length(1.0, LengthUnit.FEET);
            Length b = new Length(12.0, LengthUnit.INCH);

            Assert.Throws<ArgumentException>(() =>
            {
                a.Add(b, (LengthUnit)Enum.Parse(typeof(LengthUnit), null));
            });
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_LargeToSmallScale()
        {
            Length a = new Length(1000.0, LengthUnit.FEET);
            Length b = new Length(500.0, LengthUnit.FEET);

            Length result = a.Add(b, LengthUnit.INCH);

            Assert.AreEqual(18000.0, result.GetValue(), EPSILON);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SmallToLargeScale()
        {
            Length a = new Length(12.0, LengthUnit.INCH);
            Length b = new Length(12.0, LengthUnit.INCH);

            Length result = a.Add(b, LengthUnit.YARD);

            Assert.AreEqual(0.6667, result.GetValue(), EPSILON);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_PrecisionTolerance()
        {
            Length a = new Length(2.54, LengthUnit.CENTIMETER);
            Length b = new Length(1.0, LengthUnit.INCH);

            Length result = a.Add(b, LengthUnit.INCH);

            Assert.AreEqual(2.0, result.GetValue(), EPSILON);
        }
    }
}