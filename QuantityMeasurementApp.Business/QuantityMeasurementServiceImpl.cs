using System;
using System.Collections.Generic;
using QuantityMeasurementApp.Business.Exceptions;
using QuantityMeasurementApp.Business.Implementations;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Entity.DTOs;
using QuantityMeasurementApp.Entity.Enums;
using QuantityMeasurementApp.Repository;


namespace QuantityMeasurementApp.Business
{
    public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
    {
        private readonly IQuantityMeasurementRepository _repository;

        public QuantityMeasurementServiceImpl(IQuantityMeasurementRepository repository)
        {
            _repository = repository;
        }

        // ── COMPARE ──────────────────────────────────────────

        public QuantityDTO Compare(QuantityDTO q1, QuantityDTO q2)
        {
            if (q1 == null || q2 == null)
                throw new QuantityMeasurementException("Both quantities are required for comparison.");

            try
            {
                ValidateSameType(q1, q2);
                bool isEqual = ResolveAndCompare(q1, q2);

                _repository.Save(QuantityMeasurementEntity.ForCompare(
                    q1, q2, OperationType.COMPARE, isEqual));

                return new QuantityDTO(isEqual ? 1 : 0, isEqual ? "EQUAL" : "NOT_EQUAL",
                    q1.MeasurementType);
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new QuantityMeasurementException("Compare failed: " + ex.Message, ex);
            }
        }

        // ── CONVERT ──────────────────────────────────────────

        public QuantityDTO Convert(QuantityDTO quantity, string targetUnit)
        {
            if (quantity == null)
                throw new QuantityMeasurementException("Quantity is required for conversion.");
            if (string.IsNullOrEmpty(targetUnit))
                throw new QuantityMeasurementException("Target unit is required for conversion.");

            try
            {
                QuantityDTO result = ResolveAndConvert(quantity, targetUnit);

                _repository.Save(QuantityMeasurementEntity.ForConvert(
                    quantity, OperationType.CONVERT, result));

                return result;
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new QuantityMeasurementException("Convert failed: " + ex.Message, ex);
            }
        }

        // ── ADD ──────────────────────────────────────────────

        public QuantityDTO Add(QuantityDTO q1, QuantityDTO q2, string targetUnit)
        {
            if (q1 == null || q2 == null)
                throw new QuantityMeasurementException("Both quantities are required for addition.");

            try
            {
                ValidateSameType(q1, q2);
                QuantityDTO result = ResolveAndAdd(q1, q2, targetUnit);

                _repository.Save(QuantityMeasurementEntity.ForArithmetic(
                    q1, q2, OperationType.ADD, result));

                return result;
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new QuantityMeasurementException("Add failed: " + ex.Message, ex);
            }
        }

        // ── SUBTRACT ─────────────────────────────────────────

        public QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2, string targetUnit)
        {
            if (q1 == null || q2 == null)
                throw new QuantityMeasurementException("Both quantities are required for subtraction.");

            try
            {
                ValidateSameType(q1, q2);
                QuantityDTO result = ResolveAndSubtract(q1, q2, targetUnit);

                _repository.Save(QuantityMeasurementEntity.ForArithmetic(
                    q1, q2, OperationType.SUBTRACT, result));

                return result;
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new QuantityMeasurementException("Subtract failed: " + ex.Message, ex);
            }
        }

        // ── DIVIDE ───────────────────────────────────────────

        public QuantityDTO Divide(QuantityDTO q1, QuantityDTO q2)
        {
            if (q1 == null || q2 == null)
                throw new QuantityMeasurementException("Both quantities are required for division.");

            try
            {
                ValidateSameType(q1, q2);
                double scalar = ResolveAndDivide(q1, q2);

                _repository.Save(QuantityMeasurementEntity.ForDivide(
                    q1, q2, OperationType.DIVIDE, scalar));

                return new QuantityDTO(scalar, "SCALAR", q1.MeasurementType);
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new QuantityMeasurementException("Divide failed: " + ex.Message, ex);
            }
        }

        // ── HISTORY ──────────────────────────────────────────

        public IList<QuantityMeasurementEntity> GetAllMeasurements()
            => _repository.GetAll();

        public IList<QuantityMeasurementEntity> GetMeasurementsByOperation(string operation)
            => _repository.GetByOperation(operation);

        public IList<QuantityMeasurementEntity> GetMeasurementsByType(string measurementType)
            => _repository.GetByMeasurementType(measurementType);

        public int GetTotalCount()
            => _repository.GetTotalCount();

        // ── PRIVATE HELPERS ──────────────────────────────────

        private void ValidateSameType(QuantityDTO q1, QuantityDTO q2)
        {
            if (q1.MeasurementType != q2.MeasurementType)
                throw new QuantityMeasurementException(
                    "Cannot operate on different measurement types: "
                    + q1.MeasurementType + " vs " + q2.MeasurementType);
        }

        private bool ResolveAndCompare(QuantityDTO q1, QuantityDTO q2)
        {
            string type = q1.MeasurementType;

            if (type == "LENGTH")
            {
                LengthUnit u1 = ParseEnum<LengthUnit>(q1.Unit);
                LengthUnit u2 = ParseEnum<LengthUnit>(q2.Unit);
                return new Quantity<LengthUnit>(q1.Value, u1)
                    .Equals(new Quantity<LengthUnit>(q2.Value, u2));
            }
            if (type == "WEIGHT")
            {
                WeightUnit u1 = ParseEnum<WeightUnit>(q1.Unit);
                WeightUnit u2 = ParseEnum<WeightUnit>(q2.Unit);
                return new Quantity<WeightUnit>(q1.Value, u1)
                    .Equals(new Quantity<WeightUnit>(q2.Value, u2));
            }
            if (type == "VOLUME")
            {
                VolumeUnit u1 = ParseEnum<VolumeUnit>(q1.Unit);
                VolumeUnit u2 = ParseEnum<VolumeUnit>(q2.Unit);
                return new Quantity<VolumeUnit>(q1.Value, u1)
                    .Equals(new Quantity<VolumeUnit>(q2.Value, u2));
            }
            if (type == "TEMPERATURE")
            {
                TemperatureUnit u1 = ParseEnum<TemperatureUnit>(q1.Unit);
                TemperatureUnit u2 = ParseEnum<TemperatureUnit>(q2.Unit);
                return new Quantity<TemperatureUnit>(q1.Value, u1)
                    .Equals(new Quantity<TemperatureUnit>(q2.Value, u2));
            }
            throw new QuantityMeasurementException("Unknown measurement type: " + type);
        }

        private QuantityDTO ResolveAndConvert(QuantityDTO q, string targetUnit)
        {
            string type = q.MeasurementType;

            if (type == "LENGTH")
            {
                LengthUnit from   = ParseEnum<LengthUnit>(q.Unit);
                LengthUnit to     = ParseEnum<LengthUnit>(targetUnit);
                Quantity<LengthUnit> result =
                    new Quantity<LengthUnit>(q.Value, from).ConvertTo(to);
                return new QuantityDTO(result.Value, result.Unit.ToString(), type);
            }
            if (type == "WEIGHT")
            {
                WeightUnit from   = ParseEnum<WeightUnit>(q.Unit);
                WeightUnit to     = ParseEnum<WeightUnit>(targetUnit);
                Quantity<WeightUnit> result =
                    new Quantity<WeightUnit>(q.Value, from).ConvertTo(to);
                return new QuantityDTO(result.Value, result.Unit.ToString(), type);
            }
            if (type == "VOLUME")
            {
                VolumeUnit from   = ParseEnum<VolumeUnit>(q.Unit);
                VolumeUnit to     = ParseEnum<VolumeUnit>(targetUnit);
                Quantity<VolumeUnit> result =
                    new Quantity<VolumeUnit>(q.Value, from).ConvertTo(to);
                return new QuantityDTO(result.Value, result.Unit.ToString(), type);
            }
            if (type == "TEMPERATURE")
            {
                TemperatureUnit from = ParseEnum<TemperatureUnit>(q.Unit);
                TemperatureUnit to   = ParseEnum<TemperatureUnit>(targetUnit);
                Quantity<TemperatureUnit> result =
                    new Quantity<TemperatureUnit>(q.Value, from).ConvertTo(to);
                return new QuantityDTO(result.Value, result.Unit.ToString(), type);
            }
            throw new QuantityMeasurementException("Unknown measurement type: " + type);
        }

        private QuantityDTO ResolveAndAdd(QuantityDTO q1, QuantityDTO q2, string targetUnit)
        {
            string type = q1.MeasurementType;

            if (type == "LENGTH")
            {
                LengthUnit u1  = ParseEnum<LengthUnit>(q1.Unit);
                LengthUnit u2  = ParseEnum<LengthUnit>(q2.Unit);
                LengthUnit tgt = ParseEnum<LengthUnit>(targetUnit);
                Quantity<LengthUnit> result =
                    new Quantity<LengthUnit>(q1.Value, u1)
                        .Add(new Quantity<LengthUnit>(q2.Value, u2), tgt);
                return new QuantityDTO(result.Value, result.Unit.ToString(), type);
            }
            if (type == "WEIGHT")
            {
                WeightUnit u1  = ParseEnum<WeightUnit>(q1.Unit);
                WeightUnit u2  = ParseEnum<WeightUnit>(q2.Unit);
                WeightUnit tgt = ParseEnum<WeightUnit>(targetUnit);
                Quantity<WeightUnit> result =
                    new Quantity<WeightUnit>(q1.Value, u1)
                        .Add(new Quantity<WeightUnit>(q2.Value, u2), tgt);
                return new QuantityDTO(result.Value, result.Unit.ToString(), type);
            }
            if (type == "VOLUME")
            {
                VolumeUnit u1  = ParseEnum<VolumeUnit>(q1.Unit);
                VolumeUnit u2  = ParseEnum<VolumeUnit>(q2.Unit);
                VolumeUnit tgt = ParseEnum<VolumeUnit>(targetUnit);
                Quantity<VolumeUnit> result =
                    new Quantity<VolumeUnit>(q1.Value, u1)
                        .Add(new Quantity<VolumeUnit>(q2.Value, u2), tgt);
                return new QuantityDTO(result.Value, result.Unit.ToString(), type);
            }
            throw new QuantityMeasurementException(
                type + " does not support Add.");
        }

        private QuantityDTO ResolveAndSubtract(QuantityDTO q1, QuantityDTO q2, string targetUnit)
        {
            string type = q1.MeasurementType;

            if (type == "LENGTH")
            {
                LengthUnit u1  = ParseEnum<LengthUnit>(q1.Unit);
                LengthUnit u2  = ParseEnum<LengthUnit>(q2.Unit);
                LengthUnit tgt = ParseEnum<LengthUnit>(targetUnit);
                Quantity<LengthUnit> result =
                    new Quantity<LengthUnit>(q1.Value, u1)
                        .Subtract(new Quantity<LengthUnit>(q2.Value, u2), tgt);
                return new QuantityDTO(result.Value, result.Unit.ToString(), type);
            }
            if (type == "WEIGHT")
            {
                WeightUnit u1  = ParseEnum<WeightUnit>(q1.Unit);
                WeightUnit u2  = ParseEnum<WeightUnit>(q2.Unit);
                WeightUnit tgt = ParseEnum<WeightUnit>(targetUnit);
                Quantity<WeightUnit> result =
                    new Quantity<WeightUnit>(q1.Value, u1)
                        .Subtract(new Quantity<WeightUnit>(q2.Value, u2), tgt);
                return new QuantityDTO(result.Value, result.Unit.ToString(), type);
            }
            if (type == "VOLUME")
            {
                VolumeUnit u1  = ParseEnum<VolumeUnit>(q1.Unit);
                VolumeUnit u2  = ParseEnum<VolumeUnit>(q2.Unit);
                VolumeUnit tgt = ParseEnum<VolumeUnit>(targetUnit);
                Quantity<VolumeUnit> result =
                    new Quantity<VolumeUnit>(q1.Value, u1)
                        .Subtract(new Quantity<VolumeUnit>(q2.Value, u2), tgt);
                return new QuantityDTO(result.Value, result.Unit.ToString(), type);
            }
            throw new QuantityMeasurementException(
                type + " does not support Subtract.");
        }

        private double ResolveAndDivide(QuantityDTO q1, QuantityDTO q2)
        {
            string type = q1.MeasurementType;

            if (type == "LENGTH")
            {
                LengthUnit u1 = ParseEnum<LengthUnit>(q1.Unit);
                LengthUnit u2 = ParseEnum<LengthUnit>(q2.Unit);
                return new Quantity<LengthUnit>(q1.Value, u1)
                    .Divide(new Quantity<LengthUnit>(q2.Value, u2));
            }
            if (type == "WEIGHT")
            {
                WeightUnit u1 = ParseEnum<WeightUnit>(q1.Unit);
                WeightUnit u2 = ParseEnum<WeightUnit>(q2.Unit);
                return new Quantity<WeightUnit>(q1.Value, u1)
                    .Divide(new Quantity<WeightUnit>(q2.Value, u2));
            }
            if (type == "VOLUME")
            {
                VolumeUnit u1 = ParseEnum<VolumeUnit>(q1.Unit);
                VolumeUnit u2 = ParseEnum<VolumeUnit>(q2.Unit);
                return new Quantity<VolumeUnit>(q1.Value, u1)
                    .Divide(new Quantity<VolumeUnit>(q2.Value, u2));
            }
            throw new QuantityMeasurementException(
                type + " does not support Divide.");
        }

        private T ParseEnum<T>(string value) where T : struct
        {
            if (Enum.TryParse<T>(value.ToUpper(), out T result))
                return result;
            throw new QuantityMeasurementException(
                "Invalid unit '" + value + "' for type " + typeof(T).Name);
        }
    }
}