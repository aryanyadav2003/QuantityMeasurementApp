using System;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Entity.Enums;
using QuantityMeasurementApp.Repository;
using QuantityMeasurementApp.Business.Implementations;
using QuantityMeasurementApp.Business.Exceptions;

namespace QuantityMeasurementApp.Business
{
    public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
    {
        private IQuantityMeasurementRepository _repository;

        public QuantityMeasurementServiceImpl(IQuantityMeasurementRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            _repository = repository;
        }

        // ── COMPARE ───────────────────────────────────────────

        public QuantityDTO Compare(QuantityDTO quantity1, QuantityDTO quantity2)
        {
            ValidateNotNull(quantity1, "quantity1");
            ValidateNotNull(quantity2, "quantity2");
            ValidateSameType(quantity1, quantity2);

            try
            {
                bool result = DispatchCompare(quantity1, quantity2);

                QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                    quantity1, quantity2, OperationType.COMPARE, result);
                _repository.Save(entity);

                string unitLabel = result ? "EQUAL" : "NOT_EQUAL";
                return new QuantityDTO(result ? 1 : 0, unitLabel, "COMPARISON");
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                SaveError(quantity1, quantity2, OperationType.COMPARE, ex.Message);
                throw new QuantityMeasurementException("Compare failed: " + ex.Message, ex);
            }
        }

        // ── CONVERT ───────────────────────────────────────────

        public QuantityDTO Convert(QuantityDTO quantity, string targetUnit)
        {
            ValidateNotNull(quantity, "quantity");

            if (targetUnit == null || targetUnit.Trim() == "")
                throw new QuantityMeasurementException("Target unit cannot be empty");

            try
            {
                QuantityDTO result = DispatchConvert(quantity, targetUnit);

                QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                    quantity, OperationType.CONVERT, result);
                _repository.Save(entity);

                return result;
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                SaveError(quantity, null, OperationType.CONVERT, ex.Message);
                throw new QuantityMeasurementException("Convert failed: " + ex.Message, ex);
            }
        }

        // ── ADD ───────────────────────────────────────────────

        public QuantityDTO Add(QuantityDTO quantity1, QuantityDTO quantity2, string targetUnit)
        {
            ValidateNotNull(quantity1, "quantity1");
            ValidateNotNull(quantity2, "quantity2");
            ValidateSameType(quantity1, quantity2);

            try
            {
                QuantityDTO result = DispatchAdd(quantity1, quantity2, targetUnit);

                QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                    quantity1, quantity2, OperationType.ADD, result);
                _repository.Save(entity);

                return result;
            }
            catch (NotSupportedException ex)
            {
                SaveError(quantity1, quantity2, OperationType.ADD, ex.Message);
                throw new QuantityMeasurementException("Add failed: " + ex.Message, ex);
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                SaveError(quantity1, quantity2, OperationType.ADD, ex.Message);
                throw new QuantityMeasurementException("Add failed: " + ex.Message, ex);
            }
        }

        // ── SUBTRACT ──────────────────────────────────────────

        public QuantityDTO Subtract(QuantityDTO quantity1, QuantityDTO quantity2, string targetUnit)
        {
            ValidateNotNull(quantity1, "quantity1");
            ValidateNotNull(quantity2, "quantity2");
            ValidateSameType(quantity1, quantity2);

            try
            {
                QuantityDTO result = DispatchSubtract(quantity1, quantity2, targetUnit);

                QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                    quantity1, quantity2, OperationType.SUBTRACT, result);
                _repository.Save(entity);

                return result;
            }
            catch (NotSupportedException ex)
            {
                SaveError(quantity1, quantity2, OperationType.SUBTRACT, ex.Message);
                throw new QuantityMeasurementException("Subtract failed: " + ex.Message, ex);
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                SaveError(quantity1, quantity2, OperationType.SUBTRACT, ex.Message);
                throw new QuantityMeasurementException("Subtract failed: " + ex.Message, ex);
            }
        }

        // ── DIVIDE ────────────────────────────────────────────

        public QuantityDTO Divide(QuantityDTO quantity1, QuantityDTO quantity2)
        {
            ValidateNotNull(quantity1, "quantity1");
            ValidateNotNull(quantity2, "quantity2");
            ValidateSameType(quantity1, quantity2);

            try
            {
                double scalar = DispatchDivide(quantity1, quantity2);

                QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                    quantity1, quantity2, OperationType.DIVIDE, scalar);
                _repository.Save(entity);

                return new QuantityDTO(scalar, "SCALAR", "RATIO");
            }
            catch (NotSupportedException ex)
            {
                SaveError(quantity1, quantity2, OperationType.DIVIDE, ex.Message);
                throw new QuantityMeasurementException("Divide failed: " + ex.Message, ex);
            }
            catch (ArithmeticException ex)
            {
                SaveError(quantity1, quantity2, OperationType.DIVIDE, ex.Message);
                throw new QuantityMeasurementException("Divide failed: " + ex.Message, ex);
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                SaveError(quantity1, quantity2, OperationType.DIVIDE, ex.Message);
                throw new QuantityMeasurementException("Divide failed: " + ex.Message, ex);
            }
        }

        // ── DISPATCH ──────────────────────────────────────────

        private bool DispatchCompare(QuantityDTO q1, QuantityDTO q2)
        {
            string type = q1.MeasurementType;
            if (type == "LENGTH")      return BuildQuantity<LengthUnit>(q1).Equals(BuildQuantity<LengthUnit>(q2));
            if (type == "WEIGHT")      return BuildQuantity<WeightUnit>(q1).Equals(BuildQuantity<WeightUnit>(q2));
            if (type == "VOLUME")      return BuildQuantity<VolumeUnit>(q1).Equals(BuildQuantity<VolumeUnit>(q2));
            if (type == "TEMPERATURE") return BuildQuantity<TemperatureUnit>(q1).Equals(BuildQuantity<TemperatureUnit>(q2));
            throw new QuantityMeasurementException("Unsupported measurement type: " + type);
        }

        private QuantityDTO DispatchConvert(QuantityDTO q, string targetUnit)
        {
            string type = q.MeasurementType;
            if (type == "LENGTH")
            {
                Quantity<LengthUnit> r = BuildQuantity<LengthUnit>(q).ConvertTo(ParseEnum<LengthUnit>(targetUnit));
                return new QuantityDTO(r.Value, r.Unit.ToString(), "LENGTH");
            }
            if (type == "WEIGHT")
            {
                Quantity<WeightUnit> r = BuildQuantity<WeightUnit>(q).ConvertTo(ParseEnum<WeightUnit>(targetUnit));
                return new QuantityDTO(r.Value, r.Unit.ToString(), "WEIGHT");
            }
            if (type == "VOLUME")
            {
                Quantity<VolumeUnit> r = BuildQuantity<VolumeUnit>(q).ConvertTo(ParseEnum<VolumeUnit>(targetUnit));
                return new QuantityDTO(r.Value, r.Unit.ToString(), "VOLUME");
            }
            if (type == "TEMPERATURE")
            {
                Quantity<TemperatureUnit> r = BuildQuantity<TemperatureUnit>(q).ConvertTo(ParseEnum<TemperatureUnit>(targetUnit));
                return new QuantityDTO(r.Value, r.Unit.ToString(), "TEMPERATURE");
            }
            throw new QuantityMeasurementException("Unsupported measurement type: " + type);
        }

        private QuantityDTO DispatchAdd(QuantityDTO q1, QuantityDTO q2, string targetUnit)
        {
            string type = q1.MeasurementType;
            if (type == "LENGTH")
            {
                Quantity<LengthUnit> r = BuildQuantity<LengthUnit>(q1).Add(BuildQuantity<LengthUnit>(q2), ParseEnum<LengthUnit>(targetUnit));
                return new QuantityDTO(r.Value, r.Unit.ToString(), "LENGTH");
            }
            if (type == "WEIGHT")
            {
                Quantity<WeightUnit> r = BuildQuantity<WeightUnit>(q1).Add(BuildQuantity<WeightUnit>(q2), ParseEnum<WeightUnit>(targetUnit));
                return new QuantityDTO(r.Value, r.Unit.ToString(), "WEIGHT");
            }
            if (type == "VOLUME")
            {
                Quantity<VolumeUnit> r = BuildQuantity<VolumeUnit>(q1).Add(BuildQuantity<VolumeUnit>(q2), ParseEnum<VolumeUnit>(targetUnit));
                return new QuantityDTO(r.Value, r.Unit.ToString(), "VOLUME");
            }
            if (type == "TEMPERATURE")
            {
                Quantity<TemperatureUnit> r = BuildQuantity<TemperatureUnit>(q1).Add(BuildQuantity<TemperatureUnit>(q2), ParseEnum<TemperatureUnit>(targetUnit));
                return new QuantityDTO(r.Value, r.Unit.ToString(), "TEMPERATURE");
            }
            throw new QuantityMeasurementException("Unsupported measurement type: " + type);
        }

        private QuantityDTO DispatchSubtract(QuantityDTO q1, QuantityDTO q2, string targetUnit)
        {
            string type = q1.MeasurementType;
            if (type == "LENGTH")
            {
                Quantity<LengthUnit> r = BuildQuantity<LengthUnit>(q1).Subtract(BuildQuantity<LengthUnit>(q2), ParseEnum<LengthUnit>(targetUnit));
                return new QuantityDTO(r.Value, r.Unit.ToString(), "LENGTH");
            }
            if (type == "WEIGHT")
            {
                Quantity<WeightUnit> r = BuildQuantity<WeightUnit>(q1).Subtract(BuildQuantity<WeightUnit>(q2), ParseEnum<WeightUnit>(targetUnit));
                return new QuantityDTO(r.Value, r.Unit.ToString(), "WEIGHT");
            }
            if (type == "VOLUME")
            {
                Quantity<VolumeUnit> r = BuildQuantity<VolumeUnit>(q1).Subtract(BuildQuantity<VolumeUnit>(q2), ParseEnum<VolumeUnit>(targetUnit));
                return new QuantityDTO(r.Value, r.Unit.ToString(), "VOLUME");
            }
            if (type == "TEMPERATURE")
            {
                Quantity<TemperatureUnit> r = BuildQuantity<TemperatureUnit>(q1).Subtract(BuildQuantity<TemperatureUnit>(q2), ParseEnum<TemperatureUnit>(targetUnit));
                return new QuantityDTO(r.Value, r.Unit.ToString(), "TEMPERATURE");
            }
            throw new QuantityMeasurementException("Unsupported measurement type: " + type);
        }

        private double DispatchDivide(QuantityDTO q1, QuantityDTO q2)
        {
            string type = q1.MeasurementType;
            if (type == "LENGTH")      return BuildQuantity<LengthUnit>(q1).Divide(BuildQuantity<LengthUnit>(q2));
            if (type == "WEIGHT")      return BuildQuantity<WeightUnit>(q1).Divide(BuildQuantity<WeightUnit>(q2));
            if (type == "VOLUME")      return BuildQuantity<VolumeUnit>(q1).Divide(BuildQuantity<VolumeUnit>(q2));
            if (type == "TEMPERATURE") return BuildQuantity<TemperatureUnit>(q1).Divide(BuildQuantity<TemperatureUnit>(q2));
            throw new QuantityMeasurementException("Unsupported measurement type: " + type);
        }

        // ── BUILD HELPERS ─────────────────────────────────────

        private Quantity<U> BuildQuantity<U>(QuantityDTO dto) where U : struct
        {
            U unit = ParseEnum<U>(dto.Unit);
            return new Quantity<U>(dto.Value, unit);
        }

        private U ParseEnum<U>(string name) where U : struct
        {
            U result;
            if (Enum.TryParse(name.ToUpper(), out result))
                return result;
            throw new QuantityMeasurementException(
                "Unknown unit '" + name + "' for type " + typeof(U).Name);
        }

        // ── VALIDATION HELPERS ────────────────────────────────

        private void ValidateNotNull(QuantityDTO dto, string paramName)
        {
            if (dto == null)
                throw new QuantityMeasurementException(paramName + " cannot be null");
        }

        private void ValidateSameType(QuantityDTO q1, QuantityDTO q2)
        {
            if (q1.MeasurementType != q2.MeasurementType)
                throw new QuantityMeasurementException(
                    "Cannot operate on different types: "
                    + q1.MeasurementType + " and " + q2.MeasurementType);
        }

        private void SaveError(QuantityDTO q1, QuantityDTO q2,
                               OperationType op, string message)
        {
            try
            {
                _repository.Save(new QuantityMeasurementEntity(q1, q2, op, message));
            }
            catch { }
        }
    }
}