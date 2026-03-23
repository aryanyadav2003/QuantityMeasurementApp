using System;
using System.IO;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Repository;
using QuantityMeasurementApp.Controller;
using QuantityMeasurementApp.Business;
using QuantityMeasurementApp.Repository.Repositories;
using QuantityMeasurementApp.Business.Exceptions;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class QuantityTests
    {
        private IQuantityMeasurementRepository _repository;
        private IQuantityMeasurementService    _service;
        private QuantityMeasurementController  _controller;

        // Captured once before any test runs — always points to the real stdout
        private static readonly TextWriter _standardOut = Console.Out;

        [TestInitialize]
        public void SetUp()
        {
            // Suppress singleton startup messages
            Console.SetOut(TextWriter.Null);

            _repository = QuantityMeasurementCacheRepository.GetInstance();
            _repository.Clear();
            _service    = new QuantityMeasurementServiceImpl(_repository);
            _controller = new QuantityMeasurementController(_service);

            // Restore before each test body runs
            Console.SetOut(_standardOut);
        }

        // ── ENTITY TESTS ──────────────────────────────────────

        [TestMethod]
        public void testQuantityEntity_SingleOperandConstruction()
        {
            QuantityDTO operand1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            QuantityDTO result   = new QuantityDTO(12.0, "INCHES", "LENGTH");

            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                operand1, OperationType.CONVERT, result);

            Assert.AreEqual(operand1,              entity.Operand1);
            Assert.AreEqual(OperationType.CONVERT, entity.Operation);
            Assert.AreEqual(result,                entity.Result);
            Assert.IsFalse(entity.HasError);
        }

        [TestMethod]
        public void testQuantityEntity_BinaryOperandConstruction()
        {
            QuantityDTO operand1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            QuantityDTO operand2 = new QuantityDTO(12.0, "INCHES", "LENGTH");
            QuantityDTO result   = new QuantityDTO(2.0,  "FEET",   "LENGTH");

            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                operand1, operand2, OperationType.ADD, result);

            Assert.AreEqual(operand1,          entity.Operand1);
            Assert.AreEqual(operand2,          entity.Operand2);
            Assert.AreEqual(OperationType.ADD, entity.Operation);
            Assert.AreEqual(result,            entity.Result);
            Assert.IsFalse(entity.HasError);
        }

        [TestMethod]
        public void testQuantityEntity_ErrorConstruction()
        {
            QuantityDTO operand1 = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            QuantityDTO operand2 = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");

            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                operand1, operand2, OperationType.ADD, "Temperature does not support ADD");

            Assert.IsTrue(entity.HasError);
            Assert.AreEqual("Temperature does not support ADD", entity.ErrorMessage);
        }

        [TestMethod]
        public void testQuantityEntity_ToString_Success()
        {
            QuantityDTO operand1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            QuantityDTO result   = new QuantityDTO(12.0, "INCHES", "LENGTH");

            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                operand1, OperationType.CONVERT, result);

            string text = entity.ToString();

            Assert.IsTrue(text.Contains("CONVERT"));
            Assert.IsFalse(text.Contains("ERROR"));
        }

        [TestMethod]
        public void testQuantityEntity_ToString_Error()
        {
            QuantityDTO operand1 = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            QuantityDTO operand2 = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");

            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                operand1, operand2, OperationType.ADD, "Temperature does not support ADD");

            string text = entity.ToString();

            Assert.IsTrue(text.Contains("ERROR"));
        }

        // ── SERVICE COMPARE TESTS ─────────────────────────────

        [TestMethod]
        public void testService_CompareEquality_SameUnit_Success()
        {
            QuantityDTO q1 = new QuantityDTO(1.0, "FEET", "LENGTH");
            QuantityDTO q2 = new QuantityDTO(1.0, "FEET", "LENGTH");

            QuantityDTO result = _service.Compare(q1, q2);

            Assert.AreEqual(1,       result.Value);
            Assert.AreEqual("EQUAL", result.Unit);
        }

        [TestMethod]
        public void testService_CompareEquality_DifferentUnit_Success()
        {
            QuantityDTO q1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            QuantityDTO q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");

            QuantityDTO result = _service.Compare(q1, q2);

            Assert.AreEqual(1,       result.Value);
            Assert.AreEqual("EQUAL", result.Unit);
        }

        [TestMethod]
        public void testService_CompareEquality_CrossCategory_Error()
        {
            QuantityDTO q1 = new QuantityDTO(1.0, "FEET",     "LENGTH");
            QuantityDTO q2 = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");

            bool exceptionThrown = false;
            try { _service.Compare(q1, q2); }
            catch (QuantityMeasurementException) { exceptionThrown = true; }

            Assert.IsTrue(exceptionThrown);
        }

        // ── SERVICE CONVERT TESTS ─────────────────────────────

        [TestMethod]
        public void testService_Convert_Success()
        {
            QuantityDTO q      = new QuantityDTO(1.0, "FEET", "LENGTH");
            QuantityDTO result = _service.Convert(q, "INCHES");

            Assert.AreEqual(12.0,     result.Value, 0.001);
            Assert.AreEqual("INCHES", result.Unit);
        }

        // ── SERVICE ADD TESTS ─────────────────────────────────

        [TestMethod]
        public void testService_Add_Success()
        {
            QuantityDTO q1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            QuantityDTO q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");

            QuantityDTO result = _service.Add(q1, q2, "FEET");

            Assert.AreEqual(2.0,    result.Value, 0.001);
            Assert.AreEqual("FEET", result.Unit);
        }

        [TestMethod]
        public void testService_Add_UnsupportedOperation_Error()
        {
            QuantityDTO q1 = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            QuantityDTO q2 = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");

            bool exceptionThrown = false;
            try { _service.Add(q1, q2, "CELSIUS"); }
            catch (QuantityMeasurementException) { exceptionThrown = true; }

            Assert.IsTrue(exceptionThrown);
        }

        // ── SERVICE SUBTRACT TESTS ────────────────────────────

        [TestMethod]
        public void testService_Subtract_Success()
        {
            QuantityDTO q1 = new QuantityDTO(2.0, "FEET",   "LENGTH");
            QuantityDTO q2 = new QuantityDTO(6.0, "INCHES", "LENGTH");

            QuantityDTO result = _service.Subtract(q1, q2, "FEET");

            Assert.AreEqual(1.5,    result.Value, 0.001);
            Assert.AreEqual("FEET", result.Unit);
        }

        // ── SERVICE DIVIDE TESTS ──────────────────────────────

        [TestMethod]
        public void testService_Divide_Success()
        {
            QuantityDTO q1 = new QuantityDTO(2.0, "FEET", "LENGTH");
            QuantityDTO q2 = new QuantityDTO(1.0, "FEET", "LENGTH");

            QuantityDTO result = _service.Divide(q1, q2);

            Assert.AreEqual(2.0,      result.Value, 0.001);
            Assert.AreEqual("SCALAR", result.Unit);
        }

        [TestMethod]
        public void testService_Divide_ByZero_Error()
        {
            QuantityDTO q1 = new QuantityDTO(1.0, "FEET", "LENGTH");
            QuantityDTO q2 = new QuantityDTO(0.0, "FEET", "LENGTH");

            bool exceptionThrown = false;
            try { _service.Divide(q1, q2); }
            catch (QuantityMeasurementException) { exceptionThrown = true; }

            Assert.IsTrue(exceptionThrown);
        }

        // ── CONTROLLER TESTS ──────────────────────────────────

        [TestMethod]
        public void testController_DemonstrateEquality_Success()
        {
            QuantityDTO q1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            QuantityDTO q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");
            _controller.PerformCompare(q1, q2);
        }

        [TestMethod]
        public void testController_DemonstrateConversion_Success()
        {
            QuantityDTO q = new QuantityDTO(1.0, "FEET", "LENGTH");
            _controller.PerformConvert(q, "INCHES");
        }

        [TestMethod]
        public void testController_DemonstrateAddition_Success()
        {
            QuantityDTO q1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            QuantityDTO q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");
            _controller.PerformAdd(q1, q2, "FEET");
        }

        [TestMethod]
        public void testController_DemonstrateAddition_Error()
        {
            QuantityDTO q1 = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            QuantityDTO q2 = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");
            _controller.PerformAdd(q1, q2, "CELSIUS");
        }

        [TestMethod]
        public void testController_DisplayResult_Success()
        {
            // Redirect BEFORE calling any method that writes to console
            StringWriter sw = new StringWriter();
            Console.SetOut(sw);

            QuantityDTO q = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");
            _controller.PerformConvert(q, "GRAM");

            // Capture output THEN restore
            string output = sw.ToString();
            Console.SetOut(_standardOut);

            Assert.IsTrue(output.Contains("Result"),
                "Expected output to contain 'Result' but got: " + output);
        }

        [TestMethod]
        public void testController_DisplayResult_Error()
        {
            StringWriter sw = new StringWriter();
            Console.SetOut(sw);

            QuantityDTO q1 = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            QuantityDTO q2 = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");
            _controller.PerformAdd(q1, q2, "CELSIUS");

            string output = sw.ToString();
            Console.SetOut(_standardOut);

            Assert.IsTrue(output.Contains("ERROR"),
                "Expected output to contain 'ERROR' but got: " + output);
        }

        [TestMethod]
        public void testController_ConsoleOutput_Format()
        {
            // Redirect BEFORE calling any method that writes to console
            StringWriter sw = new StringWriter();
            Console.SetOut(sw);

            QuantityDTO q1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            QuantityDTO q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");
            _controller.PerformCompare(q1, q2);

            // Capture output THEN restore
            string output = sw.ToString();
            Console.SetOut(_standardOut);

            Assert.IsTrue(output.Contains("Result"),
                "Expected output to contain 'Result' but got: " + output);
        }

        [TestMethod]
        public void testController_AllOperations()
        {
            QuantityDTO q1 = new QuantityDTO(2.0, "FEET", "LENGTH");
            QuantityDTO q2 = new QuantityDTO(1.0, "FEET", "LENGTH");

            _controller.PerformCompare(q1, q2);
            _controller.PerformConvert(q1, "INCHES");
            _controller.PerformAdd(q1, q2, "FEET");
            _controller.PerformSubtract(q1, q2, "FEET");
            _controller.PerformDivide(q1, q2);
        }

        [TestMethod]
        public void testController_NullService_Prevention()
        {
            bool exceptionThrown = false;
            try
            {
                QuantityMeasurementController c = new QuantityMeasurementController(null);
            }
            catch (ArgumentNullException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        // ── LAYER SEPARATION TESTS ────────────────────────────

        [TestMethod]
        public void testLayerSeparation_ServiceIndependence()
        {
            QuantityDTO q      = new QuantityDTO(1.0, "FEET", "LENGTH");
            QuantityDTO result = _service.Convert(q, "INCHES");

            Assert.IsNotNull(result);
            Assert.AreEqual(12.0, result.Value, 0.001);
        }

        [TestMethod]
        public void testLayerSeparation_ControllerIndependence()
        {
            IQuantityMeasurementService anotherService =
                new QuantityMeasurementServiceImpl(_repository);

            QuantityMeasurementController controller =
                new QuantityMeasurementController(anotherService);

            Assert.IsNotNull(controller);
        }

        [TestMethod]
        public void testLayerDecoupling_ServiceChange()
        {
            IQuantityMeasurementService newService =
                new QuantityMeasurementServiceImpl(_repository);

            QuantityDTO result = newService.Convert(
                new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"), "GRAM");

            Assert.AreEqual(1000.0, result.Value, 0.001);
        }

        [TestMethod]
        public void testLayerDecoupling_EntityChange()
        {
            QuantityDTO dto = new QuantityDTO(1.0, "FEET", "LENGTH");

            Assert.AreEqual(1.0,      dto.Value);
            Assert.AreEqual("FEET",   dto.Unit);
            Assert.AreEqual("LENGTH", dto.MeasurementType);
        }

        // ── DATA FLOW TESTS ───────────────────────────────────

        [TestMethod]
        public void testDataFlow_ControllerToService()
        {
            QuantityDTO q1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            QuantityDTO q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");

            QuantityDTO result = _service.Add(q1, q2, "FEET");

            Assert.IsNotNull(result);
            Assert.AreEqual("FEET", result.Unit);
        }

        [TestMethod]
        public void testDataFlow_ServiceToController()
        {
            QuantityDTO q      = new QuantityDTO(1.0, "FEET", "LENGTH");
            QuantityDTO result = _service.Convert(q, "INCHES");

            Assert.IsNotNull(result);
            Assert.AreEqual("INCHES", result.Unit);
            Assert.AreEqual("LENGTH", result.MeasurementType);
        }

        // ── ENTITY IMMUTABILITY AND OPERATION TYPE TESTS ──────

        [TestMethod]
        public void testEntity_Immutability()
        {
            QuantityDTO operand1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            QuantityDTO result   = new QuantityDTO(12.0, "INCHES", "LENGTH");

            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                operand1, OperationType.CONVERT, result);

            Assert.AreEqual(operand1,              entity.Operand1);
            Assert.AreEqual(OperationType.CONVERT, entity.Operation);
            Assert.AreEqual(result,                entity.Result);
        }

        [TestMethod]
        public void testEntity_OperationType_Tracking()
        {
            QuantityDTO q1 = new QuantityDTO(1.0, "FEET",   "LENGTH");
            QuantityDTO q2 = new QuantityDTO(1.0, "INCHES", "LENGTH");
            QuantityDTO r  = new QuantityDTO(2.0, "FEET",   "LENGTH");

            QuantityMeasurementEntity addEntity = new QuantityMeasurementEntity(
                q1, q2, OperationType.ADD, r);
            QuantityMeasurementEntity subEntity = new QuantityMeasurementEntity(
                q1, q2, OperationType.SUBTRACT, r);
            QuantityMeasurementEntity divEntity = new QuantityMeasurementEntity(
                q1, q2, OperationType.DIVIDE, 1.0);

            Assert.AreEqual(OperationType.ADD,      addEntity.Operation);
            Assert.AreEqual(OperationType.SUBTRACT, subEntity.Operation);
            Assert.AreEqual(OperationType.DIVIDE,   divEntity.Operation);
        }

        // ── SERVICE VALIDATION TESTS ──────────────────────────

        [TestMethod]
        public void testService_NullEntity_Rejection()
        {
            bool exceptionThrown = false;
            try { _service.Compare(null, null); }
            catch (QuantityMeasurementException) { exceptionThrown = true; }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void testService_ValidationConsistency()
        {
            QuantityDTO q1 = new QuantityDTO(1.0, "FEET",     "LENGTH");
            QuantityDTO q2 = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");

            bool addThrew = false;
            try { _service.Add(q1, q2, "FEET"); }
            catch (QuantityMeasurementException) { addThrew = true; }

            bool subThrew = false;
            try { _service.Subtract(q1, q2, "FEET"); }
            catch (QuantityMeasurementException) { subThrew = true; }

            bool divThrew = false;
            try { _service.Divide(q1, q2); }
            catch (QuantityMeasurementException) { divThrew = true; }

            Assert.IsTrue(addThrew);
            Assert.IsTrue(subThrew);
            Assert.IsTrue(divThrew);
        }

        [TestMethod]
        public void testService_ExceptionHandling_AllOperations()
        {
            QuantityDTO t1 = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            QuantityDTO t2 = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");

            bool addThrew = false;
            try { _service.Add(t1, t2, "CELSIUS"); }
            catch (QuantityMeasurementException) { addThrew = true; }

            bool subThrew = false;
            try { _service.Subtract(t1, t2, "CELSIUS"); }
            catch (QuantityMeasurementException) { subThrew = true; }

            bool divThrew = false;
            try { _service.Divide(t1, t2); }
            catch (QuantityMeasurementException) { divThrew = true; }

            Assert.IsTrue(addThrew);
            Assert.IsTrue(subThrew);
            Assert.IsTrue(divThrew);
        }

        // ── SERVICE ALL CATEGORIES AND UNITS TESTS ────────────

        [TestMethod]
        public void testService_AllMeasurementCategories()
        {
            QuantityDTO r1 = _service.Convert(
                new QuantityDTO(1.0, "FEET", "LENGTH"), "INCHES");
            Assert.AreEqual(12.0, r1.Value, 0.001);

            QuantityDTO r2 = _service.Convert(
                new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"), "GRAM");
            Assert.AreEqual(1000.0, r2.Value, 0.001);

            QuantityDTO r3 = _service.Convert(
                new QuantityDTO(1.0, "LITRE", "VOLUME"), "MILLILITRE");
            Assert.AreEqual(1000.0, r3.Value, 0.001);

            QuantityDTO r4 = _service.Convert(
                new QuantityDTO(0.0, "CELSIUS", "TEMPERATURE"), "FAHRENHEIT");
            Assert.AreEqual(32.0, r4.Value, 0.001);
        }

        [TestMethod]
        public void testService_AllUnitImplementations()
        {
            QuantityDTO r1 = _service.Convert(
                new QuantityDTO(1.0, "YARDS", "LENGTH"), "FEET");
            Assert.AreEqual(3.0, r1.Value, 0.001);

            QuantityDTO r2 = _service.Convert(
                new QuantityDTO(30.48, "CENTIMETERS", "LENGTH"), "FEET");
            Assert.AreEqual(1.0, r2.Value, 0.001);

            QuantityDTO r3 = _service.Convert(
                new QuantityDTO(1.0, "POUND", "WEIGHT"), "GRAM");
            Assert.AreEqual(453.592, r3.Value, 0.001);

            QuantityDTO r4 = _service.Convert(
                new QuantityDTO(1.0, "GALLON", "VOLUME"), "LITRE");
            Assert.AreEqual(3.78541, r4.Value, 0.001);

            QuantityDTO r5 = _service.Convert(
                new QuantityDTO(32.0, "FAHRENHEIT", "TEMPERATURE"), "CELSIUS");
            Assert.AreEqual(0.0, r5.Value, 0.001);
        }

        // ── INTEGRATION TESTS ─────────────────────────────────

        [TestMethod]
        public void testIntegration_EndToEnd_LengthAddition()
        {
            QuantityDTO q1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            QuantityDTO q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");

            QuantityDTO result = _service.Add(q1, q2, "FEET");

            Assert.AreEqual(2.0,    result.Value, 0.001);
            Assert.AreEqual("FEET", result.Unit);
        }

        [TestMethod]
        public void testIntegration_EndToEnd_TemperatureUnsupported()
        {
            QuantityDTO t1 = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            QuantityDTO t2 = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");

            bool exceptionThrown = false;
            try { _service.Add(t1, t2, "CELSIUS"); }
            catch (QuantityMeasurementException) { exceptionThrown = true; }

            Assert.IsTrue(exceptionThrown);
        }

        // ── BACKWARD COMPATIBILITY UC1–UC14 ───────────────────

        [TestMethod]
        public void testBackwardCompatibility_AllUC1_UC14_Tests()
        {
            QuantityDTO r1 = _service.Compare(
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"));
            Assert.AreEqual(1, r1.Value);

            QuantityDTO r2 = _service.Compare(
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(2.0, "FEET", "LENGTH"));
            Assert.AreEqual(0, r2.Value);

            QuantityDTO r3 = _service.Compare(
                new QuantityDTO(1.0,  "FEET",   "LENGTH"),
                new QuantityDTO(12.0, "INCHES", "LENGTH"));
            Assert.AreEqual(1, r3.Value);

            QuantityDTO r4 = _service.Compare(
                new QuantityDTO(1.0, "YARDS", "LENGTH"),
                new QuantityDTO(3.0, "FEET",  "LENGTH"));
            Assert.AreEqual(1, r4.Value);

            QuantityDTO r5 = _service.Convert(
                new QuantityDTO(1.0, "GALLON", "VOLUME"), "LITRE");
            Assert.AreEqual(3.78541, r5.Value, 0.001);

            QuantityDTO r6 = _service.Compare(
                new QuantityDTO(1.0,    "KILOGRAM", "WEIGHT"),
                new QuantityDTO(1000.0, "GRAM",     "WEIGHT"));
            Assert.AreEqual(1, r6.Value);

            QuantityDTO r7 = _service.Compare(
                new QuantityDTO(1.0,    "LITRE",      "VOLUME"),
                new QuantityDTO(1000.0, "MILLILITRE", "VOLUME"));
            Assert.AreEqual(1, r7.Value);

            QuantityDTO r8 = _service.Add(
                new QuantityDTO(1.0,  "FEET",   "LENGTH"),
                new QuantityDTO(12.0, "INCHES", "LENGTH"), "FEET");
            Assert.AreEqual(2.0, r8.Value, 0.001);

            QuantityDTO r9 = _service.Add(
                new QuantityDTO(1.0,    "KILOGRAM", "WEIGHT"),
                new QuantityDTO(1000.0, "GRAM",     "WEIGHT"), "KILOGRAM");
            Assert.AreEqual(2.0, r9.Value, 0.001);

            QuantityDTO r10 = _service.Add(
                new QuantityDTO(1.0,    "LITRE",      "VOLUME"),
                new QuantityDTO(1000.0, "MILLILITRE", "VOLUME"), "LITRE");
            Assert.AreEqual(2.0, r10.Value, 0.001);

            QuantityDTO r11 = _service.Compare(
                new QuantityDTO(0.0,  "CELSIUS",    "TEMPERATURE"),
                new QuantityDTO(32.0, "FAHRENHEIT", "TEMPERATURE"));
            Assert.AreEqual(1, r11.Value);

            QuantityDTO r12 = _service.Compare(
                new QuantityDTO(100.0, "CELSIUS",    "TEMPERATURE"),
                new QuantityDTO(212.0, "FAHRENHEIT", "TEMPERATURE"));
            Assert.AreEqual(1, r12.Value);

            QuantityDTO r13 = _service.Compare(
                new QuantityDTO(-40.0, "CELSIUS",    "TEMPERATURE"),
                new QuantityDTO(-40.0, "FAHRENHEIT", "TEMPERATURE"));
            Assert.AreEqual(1, r13.Value);
        }

        // ── SCALABILITY TESTS ─────────────────────────────────

        [TestMethod]
        public void testScalability_NewOperation_Addition()
        {
            QuantityDTO q1 = new QuantityDTO(1.0,    "LITRE",      "VOLUME");
            QuantityDTO q2 = new QuantityDTO(1000.0, "MILLILITRE", "VOLUME");

            QuantityDTO result = _service.Add(q1, q2, "LITRE");

            Assert.AreEqual(2.0,     result.Value, 0.001);
            Assert.AreEqual("LITRE", result.Unit);
        }
    }
}