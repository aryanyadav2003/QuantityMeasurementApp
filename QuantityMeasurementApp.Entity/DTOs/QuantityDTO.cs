using System;
using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.Entity.DTOs
{
    public class QuantityDTO
    {
        private double _value;
        private string _unit;
        private string _measurementType;

        public QuantityDTO(double value, string unit, string measurementType)
        {
            _value           = value;
            _unit            = unit.ToUpper();
            _measurementType = measurementType.ToUpper();
        }

        [Required(ErrorMessage = "Value is required.")]
        [Range(double.MinValue, double.MaxValue,
            ErrorMessage = "Value must be a finite number.")]
        public double Value
        {
            get { return _value; }
        }

        [Required(ErrorMessage = "Unit is required.")]
        [StringLength(50, ErrorMessage = "Unit must not exceed 50 characters.")]
        public string Unit
        {
            get { return _unit; }
        }

        [Required(ErrorMessage = "MeasurementType is required.")]
        [StringLength(50, ErrorMessage = "MeasurementType must not exceed 50 characters.")]
        public string MeasurementType
        {
            get { return _measurementType; }
        }

        public override string ToString()
        {
            return _value + " " + _unit;
        }
    }
}