using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.Entity.DTOs.Validators
{
    public static class QuantityInputValidator
    {
        // ── Valid measurement types ───────────────────────────

        private static readonly HashSet<string> ValidMeasurementTypes = new HashSet<string>
        {
            "LENGTH", "WEIGHT", "VOLUME", "TEMPERATURE"
        };

        // ── Valid units per measurement type ──────────────────

        private static readonly Dictionary<string, HashSet<string>> ValidUnits =
            new Dictionary<string, HashSet<string>>
            {
                { "LENGTH",      new HashSet<string> { "FEET", "INCHES", "YARDS", "CENTIMETERS" } },
                { "WEIGHT",      new HashSet<string> { "KILOGRAM", "GRAM", "POUND" } },
                { "VOLUME",      new HashSet<string> { "LITRE", "MILLILITRE", "GALLON" } },
                { "TEMPERATURE", new HashSet<string> { "CELSIUS", "FAHRENHEIT" } }
            };

        // ── Operations that do not support arithmetic ─────────

        private static readonly HashSet<string> ArithmeticUnsupported = new HashSet<string>
        {
            "TEMPERATURE"
        };

        // ── Public validation entry points ────────────────────

        public static void ValidateForCompare(QuantityInputDTO input)
        {
            ValidateThisQuantity(input);
            ValidateThatQuantityRequired(input, "COMPARE");
            ValidateSameMeasurementType(input);
        }

        public static void ValidateForConvert(QuantityInputDTO input)
        {
            ValidateThisQuantity(input);
            ValidateTargetUnitRequired(input, "CONVERT");
            ValidateTargetUnitCompatible(input);
        }

        public static void ValidateForAdd(QuantityInputDTO input)
        {
            ValidateThisQuantity(input);
            ValidateThatQuantityRequired(input, "ADD");
            ValidateTargetUnitRequired(input, "ADD");
            ValidateSameMeasurementType(input);
            ValidateArithmeticSupported(input, "ADD");
            ValidateTargetUnitCompatible(input);
        }

        public static void ValidateForSubtract(QuantityInputDTO input)
        {
            ValidateThisQuantity(input);
            ValidateThatQuantityRequired(input, "SUBTRACT");
            ValidateTargetUnitRequired(input, "SUBTRACT");
            ValidateSameMeasurementType(input);
            ValidateArithmeticSupported(input, "SUBTRACT");
            ValidateTargetUnitCompatible(input);
        }

        public static void ValidateForDivide(QuantityInputDTO input)
        {
            ValidateThisQuantity(input);
            ValidateThatQuantityRequired(input, "DIVIDE");
            ValidateSameMeasurementType(input);
            ValidateArithmeticSupported(input, "DIVIDE");
        }

        // ── Private validation helpers ────────────────────────

        private static void ValidateThisQuantity(QuantityInputDTO input)
        {
            if (input == null)
                throw new ValidationException("Request body is required.");

            if (input.ThisQuantity == null)
                throw new ValidationException("ThisQuantity is required.");

            ValidateQuantity(input.ThisQuantity, "ThisQuantity");
        }

        private static void ValidateThatQuantityRequired(
            QuantityInputDTO input, string operation)
        {
            if (input.ThatQuantity == null)
                throw new ValidationException(
                    "ThatQuantity is required for " + operation + " operation.");

            ValidateQuantity(input.ThatQuantity, "ThatQuantity");
        }

        private static void ValidateTargetUnitRequired(
            QuantityInputDTO input, string operation)
        {
            if (string.IsNullOrWhiteSpace(input.TargetUnit))
                throw new ValidationException(
                    "TargetUnit is required for " + operation + " operation.");
        }

        private static void ValidateQuantity(QuantityDTO q, string fieldName)
        {
            if (!double.IsFinite(q.Value))
                throw new ValidationException(
                    fieldName + ".Value must be a finite number.");

            if (string.IsNullOrWhiteSpace(q.MeasurementType))
                throw new ValidationException(
                    fieldName + ".MeasurementType is required.");

            string type = q.MeasurementType.ToUpper();

            if (!ValidMeasurementTypes.Contains(type))
                throw new ValidationException(
                    fieldName + ".MeasurementType '" + q.MeasurementType
                    + "' is invalid. Valid values: LENGTH, WEIGHT, VOLUME, TEMPERATURE.");

            if (string.IsNullOrWhiteSpace(q.Unit))
                throw new ValidationException(
                    fieldName + ".Unit is required.");

            string unit = q.Unit.ToUpper();

            if (!ValidUnits[type].Contains(unit))
                throw new ValidationException(
                    fieldName + ".Unit '" + q.Unit
                    + "' is not valid for measurement type '" + type
                    + "'. Valid units: " + string.Join(", ", ValidUnits[type]));
        }

        private static void ValidateSameMeasurementType(QuantityInputDTO input)
        {
            if (input.ThatQuantity == null) return;

            if (input.ThisQuantity.MeasurementType.ToUpper()
                != input.ThatQuantity.MeasurementType.ToUpper())
            {
                throw new ValidationException(
                    "Both quantities must have the same MeasurementType. "
                    + "Got: " + input.ThisQuantity.MeasurementType
                    + " and " + input.ThatQuantity.MeasurementType + ".");
            }
        }

        private static void ValidateArithmeticSupported(
            QuantityInputDTO input, string operation)
        {
            string type = input.ThisQuantity.MeasurementType.ToUpper();

            if (ArithmeticUnsupported.Contains(type))
                throw new ValidationException(
                    type + " does not support " + operation
                    + ". Temperature arithmetic is not meaningful.");
        }

        private static void ValidateTargetUnitCompatible(QuantityInputDTO input)
        {
            if (string.IsNullOrWhiteSpace(input.TargetUnit)) return;

            string type       = input.ThisQuantity.MeasurementType.ToUpper();
            string targetUnit = input.TargetUnit.ToUpper();

            if (!ValidMeasurementTypes.Contains(type)) return;

            if (!ValidUnits[type].Contains(targetUnit))
                throw new ValidationException(
                    "TargetUnit '" + input.TargetUnit
                    + "' is not valid for measurement type '" + type
                    + "'. Valid units: " + string.Join(", ", ValidUnits[type]));
        }
    }
}