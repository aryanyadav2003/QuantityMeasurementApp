using System;
using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.Entity.DTOs
{
    public class QuantityDTO
    {
        private double _value;
        private string _unit;
        private string _measurementType;

        // Parameterless constructor for deserialization
        public QuantityDTO() { }

        public QuantityDTO(double value, string unit, string measurementType)
        {
            _value           = value;
            _unit            = unit?.ToUpper();
            _measurementType = measurementType?.ToUpper();
        }

        [Required(ErrorMessage = "Value is required.")]
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        [Required(ErrorMessage = "Unit is required.")]
        public string Unit
        {
            get { return _unit; }
            set { _unit = value?.ToUpper(); }
        }

        [Required(ErrorMessage = "MeasurementType is required.")]
        public string MeasurementType
        {
            get { return _measurementType; }
            set { _measurementType = value?.ToUpper(); }
        }

        public override string ToString()
        {
            return _value + " " + _unit;
        }
    }
}