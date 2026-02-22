using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class LengthTests
    {
        [TestMethod]
        public void TestEquality_FeetToFeet_SameValue()
        {
            Length l1 = new Length(1.0, LengthUnit.FEET);
            Length l2 = new Length(1.0, LengthUnit.FEET);
            Assert.IsTrue(l1.Equals(l2));
        }
        [TestMethod]
        public void TestEquality_InchToInch_SameValue()
        {
            Length l1 = new Length(1.0, LengthUnit.INCH);
            Length l2 = new Length(1.0, LengthUnit.INCH);
            Assert.IsTrue(l1.Equals(l2));
        }

        [TestMethod]
        public void TestEquality_FeetToFeet_DifferentValue()
        {
            Length l1 = new Length(1.0, LengthUnit.FEET);
            Length l2 = new Length(2.0, LengthUnit.FEET);
            Assert.IsFalse(l1.Equals(l2));
        }

        [TestMethod]
        public void TestEquality_InchToInch_DifferentValue()
        {
            Length l1 = new Length(1.0, LengthUnit.INCH);
            Length l2 = new Length(2.0, LengthUnit.INCH);

            Assert.IsFalse(l1.Equals(l2));
        }
        [TestMethod]
        public void TestEquality_FeetToInch_EquivalentValue()
        {
            Length l1 = new Length(1.0, LengthUnit.FEET);
            Length l2 = new Length(12.0, LengthUnit.INCH);
            Assert.IsTrue(l1.Equals(l2));
        }

        [TestMethod]
        public void TestEquality_InchToFeet_EquivalentValue()
        {
            Length l1 = new Length(12.0, LengthUnit.INCH);
            Length l2 = new Length(1.0, LengthUnit.FEET);
            Assert.IsTrue(l1.Equals(l2));
        }

        [TestMethod]
        public void TestEquality_SameReference()
        {
            Length l1 = new Length(1.0, LengthUnit.FEET);

            Assert.IsTrue(l1.Equals(l1));
        }

        [TestMethod]
        public void TestEquality_NullComparison()
        {
            Length l1 = new Length(1.0, LengthUnit.FEET);

            Assert.IsFalse(l1.Equals(null));
        }

        [TestMethod]
        public void TestEquality_NonLengthObject()
        {
            Length l1 = new Length(1.0, LengthUnit.FEET);
            object obj = new object();

            Assert.IsFalse(l1.Equals(obj));
        }
    }
}