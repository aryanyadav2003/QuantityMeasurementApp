using System;

namespace QuantityMeasurementApp.Entity
{
    // Data Transfer Object — carries value, unit name, and measurement type
    // between layers. No business logic lives here.
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

        public double Value
        {
            get { return _value; }
        }

        public string Unit
        {
            get { return _unit; }
        }

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