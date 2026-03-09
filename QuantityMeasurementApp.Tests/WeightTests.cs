using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp;
using System;

namespace QuantityMeasurementTests
{
    [TestClass]
    public class WeightTests
    {

        [TestMethod]
        public void testEquality_KilogramToKilogram_SameValue()
        {
            Weight a = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void testEquality_KilogramToKilogram_DifferentValue()
        {
            Weight a = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(2.0, WeightUnit.KILOGRAM);

            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        public void testEquality_KilogramToGram_EquivalentValue()
        {
            Weight a = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(1000.0, WeightUnit.GRAM);

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void testEquality_GramToKilogram_EquivalentValue()
        {
            Weight a = new Weight(1000.0, WeightUnit.GRAM);
            Weight b = new Weight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void testEquality_WeightVsLength_Incompatible()
        {
            Weight weight = new Weight(1.0, WeightUnit.KILOGRAM);
            Length length = new Length(1.0, LengthUnit.FEET);

            Assert.IsFalse(weight.Equals(length));
        }

        [TestMethod]
        public void testEquality_NullComparison()
        {
            Weight a = new Weight(1.0, WeightUnit.KILOGRAM);

            Assert.IsFalse(a.Equals(null));
        }

        [TestMethod]
        public void testEquality_SameReference()
        {
            Weight a = new Weight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(a));
        }

        [TestMethod]
        public void testEquality_NullUnit()
        {
            Assert.Throws<ArgumentException>(delegate ()
            {
                Weight a = new Weight(1.0, (WeightUnit)(-1));
            });
        }

        [TestMethod]
        public void testEquality_TransitiveProperty()
        {
            Weight a = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(1000.0, WeightUnit.GRAM);
            Weight c = new Weight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(c));
            Assert.IsTrue(a.Equals(c));
        }

        [TestMethod]
        public void testEquality_ZeroValue()
        {
            Weight a = new Weight(0.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(0.0, WeightUnit.GRAM);

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void testEquality_NegativeWeight()
        {
            Weight a = new Weight(-1.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(-1000.0, WeightUnit.GRAM);

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void testEquality_LargeWeightValue()
        {
            Weight a = new Weight(1000000.0, WeightUnit.GRAM);
            Weight b = new Weight(1000.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void testEquality_SmallWeightValue()
        {
            Weight a = new Weight(0.001, WeightUnit.KILOGRAM);
            Weight b = new Weight(1.0, WeightUnit.GRAM);

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void testConversion_PoundToKilogram()
        {
            Weight pound = new Weight(2.20462, WeightUnit.POUND);

            Weight result = pound.ConvertTo(WeightUnit.KILOGRAM);

            Weight expected = new Weight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(result.Equals(expected));
        }

        [TestMethod]
        public void testConversion_KilogramToPound()
        {
            Weight kg = new Weight(1.0, WeightUnit.KILOGRAM);

            Weight result = kg.ConvertTo(WeightUnit.POUND);

            Weight expected = new Weight(2.20462, WeightUnit.POUND);

            Assert.IsTrue(result.Equals(expected));
        }

        [TestMethod]
        public void testConversion_SameUnit()
        {
            Weight a = new Weight(5.0, WeightUnit.KILOGRAM);

            Weight result = a.ConvertTo(WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(result));
        }

        [TestMethod]
        public void testConversion_ZeroValue()
        {
            Weight a = new Weight(0.0, WeightUnit.KILOGRAM);

            Weight result = a.ConvertTo(WeightUnit.GRAM);

            Weight expected = new Weight(0.0, WeightUnit.GRAM);

            Assert.IsTrue(result.Equals(expected));
        }

        [TestMethod]
        public void testConversion_NegativeValue()
        {
            Weight a = new Weight(-1.0, WeightUnit.KILOGRAM);

            Weight result = a.ConvertTo(WeightUnit.GRAM);

            Weight expected = new Weight(-1000.0, WeightUnit.GRAM);

            Assert.IsTrue(result.Equals(expected));
        }

        [TestMethod]
        public void testConversion_RoundTrip()
        {
            Weight a = new Weight(1.5, WeightUnit.KILOGRAM);

            Weight grams = a.ConvertTo(WeightUnit.GRAM);
            Weight back = grams.ConvertTo(WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(back));
        }

        [TestMethod]
        public void testAddition_SameUnit_KilogramPlusKilogram()
        {
            Weight a = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(2.0, WeightUnit.KILOGRAM);

            Weight result = a.Add(b);

            Weight expected = new Weight(3.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(result.Equals(expected));
        }

        [TestMethod]
        public void testAddition_CrossUnit_KilogramPlusGram()
        {
            Weight a = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(1000.0, WeightUnit.GRAM);

            Weight result = a.Add(b);

            Weight expected = new Weight(2.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(result.Equals(expected));
        }

        [TestMethod]
        public void testAddition_CrossUnit_PoundPlusKilogram()
        {
            Weight pound = new Weight(2.20462, WeightUnit.POUND);
            Weight kg = new Weight(1.0, WeightUnit.KILOGRAM);

            Weight result = pound.Add(kg);

            Weight expected = new Weight(4.40924, WeightUnit.POUND);

            Assert.IsTrue(result.Equals(expected));
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Kilogram()
        {
            Weight a = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(1000.0, WeightUnit.GRAM);

            Weight result = a.Add(b, WeightUnit.GRAM);

            Weight expected = new Weight(2000.0, WeightUnit.GRAM);

            Assert.IsTrue(result.Equals(expected));
        }

        [TestMethod]
        public void testAddition_Commutativity()
        {
            Weight a = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(1000.0, WeightUnit.GRAM);

            Weight result1 = a.Add(b);
            Weight result2 = b.Add(a);

            Assert.IsTrue(result1.Equals(result2));
        }

        [TestMethod]
        public void testAddition_WithZero()
        {
            Weight a = new Weight(5.0, WeightUnit.KILOGRAM);
            Weight zero = new Weight(0.0, WeightUnit.GRAM);

            Weight result = a.Add(zero);

            Assert.IsTrue(a.Equals(result));
        }

        [TestMethod]
        public void testAddition_NegativeValues()
        {
            Weight a = new Weight(5.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(-2000.0, WeightUnit.GRAM);

            Weight result = a.Add(b);

            Weight expected = new Weight(3.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(result.Equals(expected));
        }

        [TestMethod]
        public void testAddition_LargeValues()
        {
            Weight a = new Weight(1000000.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(1000000.0, WeightUnit.KILOGRAM);

            Weight result = a.Add(b);

            Weight expected = new Weight(2000000.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(result.Equals(expected));
        }
    }
}