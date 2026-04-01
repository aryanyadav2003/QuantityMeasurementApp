using System;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Business;
using QuantityMeasurementApp.Business.Exceptions;

namespace QuantityMeasurementApp.Controller
{
    public class QuantityMeasurementController
    {
        private IQuantityMeasurementService _service;

        public QuantityMeasurementController(IQuantityMeasurementService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            _service = service;
        }

        public void PerformCompare(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                QuantityDTO result = _service.Compare(q1, q2);
                bool isEqual = result.Value == 1;
                Console.WriteLine("  Result : " + q1 + " == " + q2 + " => " + isEqual);
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine("  [Compare ERROR] " + ex.Message);
            }
        }

        public void PerformConvert(QuantityDTO q, string targetUnit)
        {
            try
            {
                QuantityDTO result = _service.Convert(q, targetUnit);
                Console.WriteLine("  Result : " + q + " => " + result);
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine("  [Convert ERROR] " + ex.Message);
            }
        }

        public void PerformAdd(QuantityDTO q1, QuantityDTO q2, string targetUnit)
        {
            try
            {
                QuantityDTO result = _service.Add(q1, q2, targetUnit);
                Console.WriteLine("  Result : " + q1 + " + " + q2 + " = " + result);
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine("  [Add ERROR] " + ex.Message);
            }
        }

        public void PerformSubtract(QuantityDTO q1, QuantityDTO q2, string targetUnit)
        {
            try
            {
                QuantityDTO result = _service.Subtract(q1, q2, targetUnit);
                Console.WriteLine("  Result : " + q1 + " - " + q2 + " = " + result);
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine("  [Subtract ERROR] " + ex.Message);
            }
        }

        public void PerformDivide(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                QuantityDTO result = _service.Divide(q1, q2);
                Console.WriteLine("  Result : " + q1 + " / " + q2 + " = " + result.Value);
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine("  [Divide ERROR] " + ex.Message);
            }
        }
    }
}