using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.Business;
using QuantityMeasurementApp.Business.Exceptions;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Entity.DTOs;

namespace QuantityMeasurementApp.API.Controllers
{
    /// <summary>
    /// REST API Controller for Quantity Measurement operations.
    /// Supports Compare, Convert, Add, Subtract, Divide operations
    /// across Length, Weight, Volume, and Temperature measurement types.
    /// All endpoints require a valid JWT Bearer token.
    /// Base route: api/v1/quantities
    /// </summary>
    [ApiController]
    [Route("api/v1/quantities")]
    [Authorize]
    public class QuantityMeasurementController : ControllerBase
    {
        /// <summary>
        /// Service layer dependency injected via constructor.
        /// </summary>
        private readonly IQuantityMeasurementService _service;

        /// <summary>
        /// Initializes a new instance of <see cref="QuantityMeasurementController"/>.
        /// </summary>
        /// <param name="service">
        /// The quantity measurement service injected by the DI container.
        /// </param>
        public QuantityMeasurementController(IQuantityMeasurementService service)
        {
            _service = service;
        }

        /// <summary>
        /// Compares two quantities and returns whether they are equal.
        /// Both quantities must be of the same measurement type.
        /// Accessible by USER and ADMIN roles.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/quantities/compare
        ///     {
        ///         "thisQuantity": { "value": 1,  "unit": "FEET",   "measurementType": "LENGTH" },
        ///         "thatQuantity": { "value": 12, "unit": "INCHES", "measurementType": "LENGTH" }
        ///     }
        ///
        /// </remarks>
        /// <param name="input">
        /// Input DTO containing <c>ThisQuantity</c> and <c>ThatQuantity</c>.
        /// Both are required for this operation.
        /// </param>
        /// <returns>
        /// 200 OK with <see cref="QuantityMeasurementDTO"/> containing the comparison result.
        /// 400 Bad Request if measurement types differ or units are invalid.
        /// 401 Unauthorized if JWT token is missing or invalid.
        /// 500 Internal Server Error for unexpected failures.
        /// </returns>
        [HttpPost("compare")]
        public IActionResult Compare(QuantityInputDTO input)
        {
            try
            {
                QuantityDTO result  = _service.Compare(input.ThisQuantity, input.ThatQuantity);
                bool        isEqual = result.Value == 1;
                return Ok(QuantityMeasurementDTO.FromCompare(
                    input.ThisQuantity, input.ThatQuantity, isEqual));
            }
            catch (QuantityMeasurementException ex)
            {
                return BadRequest(QuantityMeasurementDTO.FromError("COMPARE", ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, QuantityMeasurementDTO.FromError("COMPARE", ex.Message));
            }
        }

        /// <summary>
        /// Converts a quantity from one unit to another unit of the same measurement type.
        /// Supports Length, Weight, Volume, and Temperature conversions.
        /// Accessible by USER and ADMIN roles.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/quantities/convert
        ///     {
        ///         "thisQuantity": { "value": 1, "unit": "FEET", "measurementType": "LENGTH" },
        ///         "targetUnit": "INCHES"
        ///     }
        ///
        /// Supported units:
        /// - LENGTH      : FEET, INCHES, YARDS, CENTIMETERS
        /// - WEIGHT      : KILOGRAM, GRAM, POUND
        /// - VOLUME      : LITRE, MILLILITRE, GALLON
        /// - TEMPERATURE : CELSIUS, FAHRENHEIT
        /// </remarks>
        /// <param name="input">
        /// Input DTO containing <c>ThisQuantity</c> and <c>TargetUnit</c>.
        /// </param>
        /// <returns>
        /// 200 OK with <see cref="QuantityMeasurementDTO"/> containing the converted value and unit.
        /// 400 Bad Request if the unit is invalid or target unit is missing.
        /// 401 Unauthorized if JWT token is missing or invalid.
        /// 500 Internal Server Error for unexpected failures.
        /// </returns>
        [HttpPost("convert")]
        public IActionResult Convert(QuantityInputDTO input)
        {
            try
            {
                QuantityDTO result = _service.Convert(input.ThisQuantity, input.TargetUnit);
                return Ok(QuantityMeasurementDTO.FromConvert(input.ThisQuantity, result));
            }
            catch (QuantityMeasurementException ex)
            {
                return BadRequest(QuantityMeasurementDTO.FromError("CONVERT", ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, QuantityMeasurementDTO.FromError("CONVERT", ex.Message));
            }
        }

        /// <summary>
        /// Adds two quantities and returns the result in the specified target unit.
        /// Both quantities must be of the same measurement type.
        /// Temperature addition is not supported.
        /// Accessible by USER and ADMIN roles.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/quantities/add
        ///     {
        ///         "thisQuantity": { "value": 1,  "unit": "FEET",   "measurementType": "LENGTH" },
        ///         "thatQuantity": { "value": 12, "unit": "INCHES", "measurementType": "LENGTH" },
        ///         "targetUnit": "FEET"
        ///     }
        ///
        /// </remarks>
        /// <param name="input">
        /// Input DTO containing <c>ThisQuantity</c>, <c>ThatQuantity</c>, and <c>TargetUnit</c>.
        /// All three fields are required for this operation.
        /// </param>
        /// <returns>
        /// 200 OK with <see cref="QuantityMeasurementDTO"/> containing the sum in the target unit.
        /// 400 Bad Request if types differ, units are invalid, or operation is unsupported.
        /// 401 Unauthorized if JWT token is missing or invalid.
        /// 500 Internal Server Error for unexpected failures.
        /// </returns>
        [HttpPost("add")]
        public IActionResult Add(QuantityInputDTO input)
        {
            try
            {
                QuantityDTO result = _service.Add(
                    input.ThisQuantity, input.ThatQuantity, input.TargetUnit);
                return Ok(QuantityMeasurementDTO.FromArithmetic(
                    input.ThisQuantity, input.ThatQuantity, result, "ADD"));
            }
            catch (QuantityMeasurementException ex)
            {
                return BadRequest(QuantityMeasurementDTO.FromError("ADD", ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, QuantityMeasurementDTO.FromError("ADD", ex.Message));
            }
        }

        /// <summary>
        /// Subtracts the second quantity from the first and returns the result
        /// in the specified target unit.
        /// Both quantities must be of the same measurement type.
        /// Temperature subtraction is not supported.
        /// Accessible by USER and ADMIN roles.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/quantities/subtract
        ///     {
        ///         "thisQuantity": { "value": 2,   "unit": "KILOGRAM", "measurementType": "WEIGHT" },
        ///         "thatQuantity": { "value": 500, "unit": "GRAM",     "measurementType": "WEIGHT" },
        ///         "targetUnit": "KILOGRAM"
        ///     }
        ///
        /// </remarks>
        /// <param name="input">
        /// Input DTO containing <c>ThisQuantity</c>, <c>ThatQuantity</c>, and <c>TargetUnit</c>.
        /// All three fields are required for this operation.
        /// </param>
        /// <returns>
        /// 200 OK with <see cref="QuantityMeasurementDTO"/> containing the difference in the target unit.
        /// 400 Bad Request if types differ, units are invalid, or operation is unsupported.
        /// 401 Unauthorized if JWT token is missing or invalid.
        /// 500 Internal Server Error for unexpected failures.
        /// </returns>
        [HttpPost("subtract")]
        public IActionResult Subtract(QuantityInputDTO input)
        {
            try
            {
                QuantityDTO result = _service.Subtract(
                    input.ThisQuantity, input.ThatQuantity, input.TargetUnit);
                return Ok(QuantityMeasurementDTO.FromArithmetic(
                    input.ThisQuantity, input.ThatQuantity, result, "SUBTRACT"));
            }
            catch (QuantityMeasurementException ex)
            {
                return BadRequest(QuantityMeasurementDTO.FromError("SUBTRACT", ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, QuantityMeasurementDTO.FromError("SUBTRACT", ex.Message));
            }
        }

        /// <summary>
        /// Divides the first quantity by the second and returns a scalar (dimensionless) result.
        /// Both quantities must be of the same measurement type.
        /// Temperature division is not supported.
        /// Division by zero returns a 400 Bad Request.
        /// Accessible by USER and ADMIN roles.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/quantities/divide
        ///     {
        ///         "thisQuantity": { "value": 1, "unit": "GALLON", "measurementType": "VOLUME" },
        ///         "thatQuantity": { "value": 1, "unit": "LITRE",  "measurementType": "VOLUME" }
        ///     }
        ///
        /// The result is a scalar — it has no unit.
        /// </remarks>
        /// <param name="input">
        /// Input DTO containing <c>ThisQuantity</c> and <c>ThatQuantity</c>.
        /// <c>TargetUnit</c> is not required for this operation.
        /// </param>
        /// <returns>
        /// 200 OK with <see cref="QuantityMeasurementDTO"/> containing the scalar result.
        /// 400 Bad Request if types differ, units are invalid, or division by zero occurs.
        /// 401 Unauthorized if JWT token is missing or invalid.
        /// 500 Internal Server Error for unexpected failures.
        /// </returns>
        [HttpPost("divide")]
        public IActionResult Divide(QuantityInputDTO input)
        {
            try
            {
                QuantityDTO result = _service.Divide(
                    input.ThisQuantity, input.ThatQuantity);
                return Ok(QuantityMeasurementDTO.FromDivide(
                    input.ThisQuantity, input.ThatQuantity, result.Value));
            }
            catch (QuantityMeasurementException ex)
            {
                return BadRequest(QuantityMeasurementDTO.FromError("DIVIDE", ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, QuantityMeasurementDTO.FromError("DIVIDE", ex.Message));
            }
        }

        /// <summary>
        /// Retrieves the complete history of all quantity measurement operations.
        /// Accessible by ADMIN role only.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/v1/quantities/history
        ///
        /// Returns all records ordered by timestamp descending.
        /// </remarks>
        /// <returns>
        /// 200 OK with a list of <see cref="QuantityMeasurementEntity"/> records.
        /// 401 Unauthorized if JWT token is missing or invalid.
        /// 403 Forbidden if the user does not have ADMIN role.
        /// 500 Internal Server Error if history retrieval fails.
        /// </returns>
        [HttpGet("history")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetHistory()
        {
            try
            {
                return Ok(_service.GetAllMeasurements());
            }
            catch (Exception ex)
            {
                return StatusCode(500, QuantityMeasurementDTO.FromError("HISTORY", ex.Message));
            }
        }

        /// <summary>
        /// Retrieves the history of operations filtered by operation type.
        /// Accessible by ADMIN role only.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/v1/quantities/history/operation/ADD
        ///
        /// Valid operation values: COMPARE, CONVERT, ADD, SUBTRACT, DIVIDE
        /// </remarks>
        /// <param name="operation">
        /// The operation type to filter by. Case-insensitive.
        /// </param>
        /// <returns>
        /// 200 OK with a filtered list of <see cref="QuantityMeasurementEntity"/> records.
        /// 401 Unauthorized if JWT token is missing or invalid.
        /// 403 Forbidden if the user does not have ADMIN role.
        /// 500 Internal Server Error if retrieval fails.
        /// </returns>
        [HttpGet("history/operation/{operation}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetByOperation(string operation)
        {
            try
            {
                return Ok(_service.GetMeasurementsByOperation(operation));
            }
            catch (Exception ex)
            {
                return StatusCode(500, QuantityMeasurementDTO.FromError("HISTORY", ex.Message));
            }
        }

        /// <summary>
        /// Retrieves the history of operations filtered by measurement type.
        /// Accessible by ADMIN role only.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/v1/quantities/history/type/LENGTH
        ///
        /// Valid measurement type values: LENGTH, WEIGHT, VOLUME, TEMPERATURE
        /// </remarks>
        /// <param name="measurementType">
        /// The measurement type to filter by. Case-insensitive.
        /// </param>
        /// <returns>
        /// 200 OK with a filtered list of <see cref="QuantityMeasurementEntity"/> records.
        /// 401 Unauthorized if JWT token is missing or invalid.
        /// 403 Forbidden if the user does not have ADMIN role.
        /// 500 Internal Server Error if retrieval fails.
        /// </returns>
        [HttpGet("history/type/{measurementType}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetByType(string measurementType)
        {
            try
            {
                return Ok(_service.GetMeasurementsByType(measurementType));
            }
            catch (Exception ex)
            {
                return StatusCode(500, QuantityMeasurementDTO.FromError("HISTORY", ex.Message));
            }
        }

        /// <summary>
        /// Returns the total count of all quantity measurement operations recorded.
        /// Accessible by ADMIN role only.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/v1/quantities/count
        ///
        /// </remarks>
        /// <returns>
        /// 200 OK with an object containing <c>TotalCount</c> as an integer.
        /// 401 Unauthorized if JWT token is missing or invalid.
        /// 403 Forbidden if the user does not have ADMIN role.
        /// 500 Internal Server Error if count retrieval fails.
        /// </returns>
        [HttpGet("count")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetCount()
        {
            try
            {
                return Ok(new { TotalCount = _service.GetTotalCount() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, QuantityMeasurementDTO.FromError("COUNT", ex.Message));
            }
        }
    }
}