using System;

namespace QuantityMeasurementApp.Entity
{
    public enum OperationType
    {
        COMPARE,
        CONVERT,
        ADD,
        SUBTRACT,
        DIVIDE
    }

    public class QuantityMeasurementEntity
    {
        private QuantityDTO   _operand1;
        private QuantityDTO   _operand2;
        private OperationType _operation;
        private DateTime      _timestamp;
        private QuantityDTO   _result;
        private bool          _comparisonResult;
        private double        _scalarResult;
        private bool          _hasError;
        private string        _errorMessage;

        // Single operand — convert
        public QuantityMeasurementEntity(QuantityDTO operand1, OperationType operation, QuantityDTO result)
        {
            _operand1  = operand1;
            _operation = operation;
            _result    = result;
            _timestamp = DateTime.Now;
            _hasError  = false;
        }

        // Two operands, quantity result — add / subtract
        public QuantityMeasurementEntity(QuantityDTO operand1, QuantityDTO operand2, OperationType operation, QuantityDTO result)
        {
            _operand1  = operand1;
            _operand2  = operand2;
            _operation = operation;
            _result    = result;
            _timestamp = DateTime.Now;
            _hasError  = false;
        }

        // Two operands, bool result — compare
        public QuantityMeasurementEntity(QuantityDTO operand1, QuantityDTO operand2, OperationType operation, bool comparisonResult)
        {
            _operand1         = operand1;
            _operand2         = operand2;
            _operation        = operation;
            _comparisonResult = comparisonResult;
            _timestamp        = DateTime.Now;
            _hasError         = false;
        }

        // Two operands, scalar result — divide
        public QuantityMeasurementEntity(QuantityDTO operand1, QuantityDTO operand2, OperationType operation, double scalarResult)
        {
            _operand1     = operand1;
            _operand2     = operand2;
            _operation    = operation;
            _scalarResult = scalarResult;
            _timestamp    = DateTime.Now;
            _hasError     = false;
        }

        // Error case
        public QuantityMeasurementEntity(QuantityDTO operand1, QuantityDTO operand2, OperationType operation, string errorMessage)
        {
            _operand1     = operand1;
            _operand2     = operand2;
            _operation    = operation;
            _errorMessage = errorMessage;
            _hasError     = true;
            _timestamp    = DateTime.Now;
        }

        public QuantityDTO   Operand1         { get { return _operand1; } }
        public QuantityDTO   Operand2         { get { return _operand2; } }
        public OperationType Operation        { get { return _operation; } }
        public DateTime      Timestamp        { get { return _timestamp; } }
        public QuantityDTO   Result           { get { return _result; } }
        public bool          ComparisonResult { get { return _comparisonResult; } }
        public double        ScalarResult     { get { return _scalarResult; } }
        public bool          HasError         { get { return _hasError; } }
        public string        ErrorMessage     { get { return _errorMessage; } }

        public override string ToString()
        {
            string time = _timestamp.ToString("HH:mm:ss");

            if (_hasError)
                return "[" + time + "] ERROR in " + _operation + ": " + _errorMessage;

            if (_operation == OperationType.COMPARE)
                return "[" + time + "] " + _operation + ": " + _operand1 + " == " + _operand2 + " => " + _comparisonResult;

            if (_operation == OperationType.DIVIDE)
                return "[" + time + "] " + _operation + ": " + _operand1 + " / " + _operand2 + " = " + _scalarResult;

            if (_operand2 != null)
                return "[" + time + "] " + _operation + ": " + _operand1 + " and " + _operand2 + " = " + _result;

            return "[" + time + "] " + _operation + ": " + _operand1 + " => " + _result;
        }
    }
}