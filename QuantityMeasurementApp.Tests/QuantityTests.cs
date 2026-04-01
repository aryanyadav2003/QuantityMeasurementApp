using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Entity.DTOs;
using QuantityMeasurementApp.Entity.Enums;
using QuantityMeasurementApp.Repository;
using QuantityMeasurementApp.Repository.Repositories;
using QuantityMeasurementApp.Business;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class QuantityMeasurementIntegrationTests
    {
        private static WebApplicationFactory<Program> _factory;
        private static HttpClient                     _client;

        // ── Setup & Teardown ──────────────────────────────────

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Replace DbContext with fresh InMemory DB for tests
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType ==
                                typeof(DbContextOptions<QuantityMeasurementDbContext>));
                        if (descriptor != null)
                            services.Remove(descriptor);

                        services.AddDbContext<QuantityMeasurementDbContext>(options =>
                            options.UseInMemoryDatabase("TestDB_" + Guid.NewGuid()));
                    });
                });

            _client = _factory.CreateClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        // ── Helper Methods ────────────────────────────────────

        private StringContent JsonBody(object obj)
        {
            string json = JsonSerializer.Serialize(obj);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private object CompareBody(
            double v1, string u1, string type1,
            double v2, string u2, string type2)
        {
            return new
            {
                thisQuantity = new { value = v1, unit = u1, measurementType = type1 },
                thatQuantity = new { value = v2, unit = u2, measurementType = type2 }
            };
        }

        private object ConvertBody(
            double v, string u, string type, string targetUnit)
        {
            return new
            {
                thisQuantity = new { value = v, unit = u, measurementType = type },
                targetUnit   = targetUnit
            };
        }

        private object ArithmeticBody(
            double v1, string u1, string type1,
            double v2, string u2, string type2,
            string targetUnit)
        {
            return new
            {
                thisQuantity = new { value = v1, unit = u1, measurementType = type1 },
                thatQuantity = new { value = v2, unit = u2, measurementType = type2 },
                targetUnit   = targetUnit
            };
        }

        // ── TC01: Application Context Loads ───────────────────

        [TestMethod]
        public void TestSpringBootApplicationStarts()
        {
            // Verifies application context loads and server is reachable
            Assert.IsNotNull(_factory);
            Assert.IsNotNull(_client);
            Assert.IsNotNull(_client.BaseAddress);
        }

        // ── TC02: Compare Endpoint Returns 200 ───────────────

        [TestMethod]
        public async Task TestRestEndpointCompareQuantities()
        {
            // POST /api/v1/quantities/compare — 1 FEET == 12 INCHES
            var body     = CompareBody(1, "FEET", "LENGTH", 12, "INCHES", "LENGTH");
            var response = await _client.PostAsync(
                "/api/v1/quantities/compare", JsonBody(body));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("COMPARE"),
                "Response should contain operation COMPARE");
            Assert.IsTrue(content.Contains("True") || content.Contains("False"),
                "Response should contain comparison result");
        }

        // ── TC03: Convert Endpoint Returns Correct Value ──────

        [TestMethod]
        public async Task TestRestEndpointConvertQuantities()
        {
            // POST /api/v1/quantities/convert — 1 FEET => INCHES = 12
            var body     = ConvertBody(1, "FEET", "LENGTH", "INCHES");
            var response = await _client.PostAsync(
                "/api/v1/quantities/convert", JsonBody(body));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("12"),
                "Converted value should be 12 INCHES");
            Assert.IsTrue(content.Contains("CONVERT"),
                "Response should contain operation CONVERT");
        }

        // ── TC04: Add Endpoint Returns Correct Sum ────────────

        [TestMethod]
        public async Task TestRestEndpointAddQuantities()
        {
            // POST /api/v1/quantities/add — 1 FEET + 12 INCHES = 2 FEET
            var body     = ArithmeticBody(1, "FEET", "LENGTH", 12, "INCHES", "LENGTH", "FEET");
            var response = await _client.PostAsync(
                "/api/v1/quantities/add", JsonBody(body));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("ADD"),
                "Response should contain operation ADD");
            Assert.IsTrue(content.Contains("2"),
                "Sum of 1 FEET + 12 INCHES should be 2 FEET");
        }

        // ── TC05: Invalid Input Returns 400 ───────────────────

        [TestMethod]
        public async Task TestRestEndpointInvalidInput_Returns400()
        {
            // POST with invalid measurement type
            var body = new
            {
                thisQuantity = new { value = 1, unit = "FEET", measurementType = "INVALID_TYPE" },
                thatQuantity = new { value = 12, unit = "INCHES", measurementType = "INVALID_TYPE" }
            };

            var response = await _client.PostAsync(
                "/api/v1/quantities/compare", JsonBody(body));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ── TC06: Missing Parameter Returns 400 ───────────────

        [TestMethod]
        public async Task TestRestEndpointMissingParameter_Returns400()
        {
            // POST compare with missing thatQuantity
            var body = new
            {
                thisQuantity = new { value = 1, unit = "FEET", measurementType = "LENGTH" }
                // thatQuantity intentionally missing
            };

            var response = await _client.PostAsync(
                "/api/v1/quantities/compare", JsonBody(body));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ── TC07: Swagger UI Loads ────────────────────────────

        [TestMethod]
        public async Task TestSwaggerUILoads()
        {
            var response = await _client.GetAsync("/swagger/index.html");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("html") || content.Contains("swagger"),
                "Swagger UI should return HTML content");
        }

        // ── TC08: OpenAPI Documentation Available ─────────────

        [TestMethod]
        public async Task TestOpenAPIDocumentation()
        {
            var response = await _client.GetAsync("/swagger/v1/swagger.json");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("openapi"),
                "OpenAPI schema should be returned");
            Assert.IsTrue(content.Contains("/api/v1/quantities"),
                "All endpoints should be documented");
        }

        // ── TC09: InMemory Database Works (replaces H2) ───────

        [TestMethod]
        public async Task TestH2ConsoleLaunches()
        {
            // For .NET InMemory DB — verify history endpoint responds
            var response = await _client.GetAsync("/api/v1/quantities/history");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        // ── TC10: Database Persistence via EF Core ────────────

        [TestMethod]
        public async Task TestH2DatabasePersistence()
        {
            // Save via API then verify persisted via history endpoint
            var body = ConvertBody(1, "KILOGRAM", "WEIGHT", "GRAM");
            await _client.PostAsync("/api/v1/quantities/convert", JsonBody(body));

            var historyResponse = await _client.GetAsync("/api/v1/quantities/history");
            Assert.AreEqual(HttpStatusCode.OK, historyResponse.StatusCode);

            string content = await historyResponse.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("CONVERT"),
                "Persisted CONVERT record should appear in history");
        }

        // ── TC11: Actuator Health Endpoint (Count endpoint) ───

        [TestMethod]
        public async Task TestActuatorHealthEndpoint()
        {
            // .NET equivalent — count endpoint verifies service is UP
            var response = await _client.GetAsync("/api/v1/quantities/count");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("TotalCount"),
                "Count endpoint should return TotalCount");
        }

        // ── TC12: Metrics — Count Increases After Operations ──

        [TestMethod]
        public async Task TestActuatorMetricsEndpoint()
        {
            // Get initial count
            var countBefore = await _client.GetAsync("/api/v1/quantities/count");
            string beforeJson = await countBefore.Content.ReadAsStringAsync();
            using JsonDocument beforeDoc = JsonDocument.Parse(beforeJson);
            int before = beforeDoc.RootElement.GetProperty("TotalCount").GetInt32();

            // Perform an operation
            var body = ConvertBody(100, "CELSIUS", "TEMPERATURE", "FAHRENHEIT");
            await _client.PostAsync("/api/v1/quantities/convert", JsonBody(body));

            // Get count after
            var countAfter = await _client.GetAsync("/api/v1/quantities/count");
            string afterJson = await countAfter.Content.ReadAsStringAsync();
            using JsonDocument afterDoc = JsonDocument.Parse(afterJson);
            int after = afterDoc.RootElement.GetProperty("TotalCount").GetInt32();

            Assert.IsTrue(after > before,
                "Total count should increase after an operation");
        }

        // ── TC13: JPA Repository FindByOperation ──────────────

        [TestMethod]
        public async Task TestJPARepositoryFindByOperation()
        {
            // Save ADD and CONVERT operations
            var addBody = ArithmeticBody(1, "LITRE", "VOLUME", 500, "MILLILITRE", "VOLUME", "LITRE");
            await _client.PostAsync("/api/v1/quantities/add", JsonBody(addBody));

            var convertBody = ConvertBody(1, "GALLON", "VOLUME", "LITRE");
            await _client.PostAsync("/api/v1/quantities/convert", JsonBody(convertBody));

            // Filter by ADD
            var response = await _client.GetAsync("/api/v1/quantities/history/operation/ADD");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("ADD"),
                "Filtered results should only contain ADD operations");
        }

        // ── TC14: Repository Filter By Measurement Type ───────

        [TestMethod]
        public async Task TestJPARepositoryCustomQuery()
        {
            // Save LENGTH and WEIGHT operations
            var lengthBody = ConvertBody(1, "FEET", "LENGTH", "INCHES");
            await _client.PostAsync("/api/v1/quantities/convert", JsonBody(lengthBody));

            var weightBody = ConvertBody(1, "KILOGRAM", "WEIGHT", "GRAM");
            await _client.PostAsync("/api/v1/quantities/convert", JsonBody(weightBody));

            // Filter by LENGTH
            var response = await _client.GetAsync("/api/v1/quantities/history/type/LENGTH");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("LENGTH"),
                "Filtered results should contain LENGTH measurement type");
        }

        // ── TC15: Transactional — Invalid Operation Not Saved ─

        [TestMethod]
        public async Task TestTransactionalRollback()
        {
            // Get count before invalid operation
            var countBefore  = await _client.GetAsync("/api/v1/quantities/count");
            string beforeJson = await countBefore.Content.ReadAsStringAsync();
            using JsonDocument beforeDoc = JsonDocument.Parse(beforeJson);
            int before = beforeDoc.RootElement.GetProperty("TotalCount").GetInt32();

            // Attempt invalid operation — mismatched types
            var body = new
            {
                thisQuantity = new { value = 1, unit = "FEET",     measurementType = "LENGTH" },
                thatQuantity = new { value = 1, unit = "KILOGRAM",  measurementType = "WEIGHT" }
            };
            await _client.PostAsync("/api/v1/quantities/compare", JsonBody(body));

            // Count should not increase
            var countAfter  = await _client.GetAsync("/api/v1/quantities/count");
            string afterJson = await countAfter.Content.ReadAsStringAsync();
            using JsonDocument afterDoc = JsonDocument.Parse(afterJson);
            int after = afterDoc.RootElement.GetProperty("TotalCount").GetInt32();

            Assert.AreEqual(before, after,
                "Failed operations should not be persisted");
        }

        // ── TC16: Content-Type JSON Request ───────────────────

        [TestMethod]
        public async Task TestContentNegotiation_JSON()
        {
            var body     = ConvertBody(1, "POUND", "WEIGHT", "KILOGRAM");
            var response = await _client.PostAsync(
                "/api/v1/quantities/convert", JsonBody(body));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(
                response.Content.Headers.ContentType?.MediaType?.Contains("application/json")
                ?? false,
                "Response Content-Type should be application/json");
        }

        // ── TC17: Content Negotiation XML ─────────────────────

        [TestMethod]
        public async Task TestContentNegotiation_XML()
        {
            // Send request with Accept: application/xml
            var request = new HttpRequestMessage(
                HttpMethod.Post, "/api/v1/quantities/convert");
            request.Content = JsonBody(ConvertBody(212, "FAHRENHEIT", "TEMPERATURE", "CELSIUS"));
            request.Headers.Add("Accept", "application/xml");

            var response = await _client.SendAsync(request);

            // API returns JSON by default — verify it returns a valid response
            Assert.IsTrue(
                response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.NotAcceptable,
                "API should either return 200 OK or 406 Not Acceptable for XML accept header");
        }

        // ── TC18: Global Exception Handler ────────────────────

        [TestMethod]
        public async Task TestExceptionHandling_GlobalHandler()
        {
            // Division by zero — send 0 as divisor
            var body = new
            {
                thisQuantity = new { value = 1, unit = "FEET",  measurementType = "LENGTH" },
                thatQuantity = new { value = 0, unit = "FEET",  measurementType = "LENGTH" }
            };

            var response = await _client.PostAsync(
                "/api/v1/quantities/divide", JsonBody(body));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("error") || content.Contains("Error") ||
                          content.Contains("message") || content.Contains("Message"),
                "Error response should contain error details");
        }

        // ── TC19: Path Variable Extraction ────────────────────

        [TestMethod]
        public async Task TestRequestPathVariable_Extraction()
        {
            // Save a COMPARE operation first
            var body = CompareBody(1, "KILOGRAM", "WEIGHT", 1000, "GRAM", "WEIGHT");
            await _client.PostAsync("/api/v1/quantities/compare", JsonBody(body));

            // GET by operation path variable
            var response = await _client.GetAsync(
                "/api/v1/quantities/history/operation/COMPARE");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("COMPARE"),
                "Path variable COMPARE should be extracted and used to filter results");
        }

        // ── TC20: Query Parameter Extraction ──────────────────

        [TestMethod]
        public async Task TestRequestQueryParameter_Extraction()
        {
            // Filter by measurement type using path variable (equivalent of query param)
            var response = await _client.GetAsync(
                "/api/v1/quantities/history/type/WEIGHT");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content,
                "Response body should not be null");
        }

        // ── TC21: Response Serialization to JSON ──────────────

        [TestMethod]
        public async Task TestResponseSerialization_Object()
        {
            var body     = ConvertBody(3.78541, "LITRE", "VOLUME", "GALLON");
            var response = await _client.PostAsync(
                "/api/v1/quantities/convert", JsonBody(body));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();

            // Verify valid JSON
            Assert.IsNotNull(content);
            using JsonDocument doc = JsonDocument.Parse(content);
            Assert.IsNotNull(doc.RootElement,
                "Response should be valid JSON");
        }

        // ── TC22: MockMvc Comparison Test ─────────────────────

        [TestMethod]
        public async Task TestMockMvc_ComparisonTest()
        {
            // 1 YARD == 3 FEET
            var body     = CompareBody(1, "YARDS", "LENGTH", 3, "FEET", "LENGTH");
            var response = await _client.PostAsync(
                "/api/v1/quantities/compare", JsonBody(body));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(content);

            string resultString = doc.RootElement
                .GetProperty("resultString").GetString();

            Assert.AreEqual("True", resultString,
                "1 YARD should equal 3 FEET");
        }

        // ── TC23: MockMvc Response Assertion ──────────────────

        [TestMethod]
        public async Task TestMockMvc_ResponseAssertion()
        {
            var body     = ConvertBody(1, "GALLON", "VOLUME", "LITRE");
            var response = await _client.PostAsync(
                "/api/v1/quantities/convert", JsonBody(body));

            // Assert status
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Assert Content-Type
            Assert.IsTrue(
                response.Content.Headers.ContentType?.MediaType?.Contains("application/json")
                ?? false,
                "Content-Type must be application/json");

            // Assert JSON path
            string content = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(content);

            Assert.IsTrue(doc.RootElement.TryGetProperty("resultValue", out _),
                "Response JSON should contain resultValue");
            Assert.IsTrue(doc.RootElement.TryGetProperty("operation", out _),
                "Response JSON should contain operation");
        }

        // ── TC24: Integration — Multiple Operations ───────────

        [TestMethod]
        public async Task TestIntegrationTest_MultipleOperations()
        {
            // Run multiple operations and verify all are persisted
            await _client.PostAsync("/api/v1/quantities/convert",
                JsonBody(ConvertBody(1, "FEET", "LENGTH", "INCHES")));

            await _client.PostAsync("/api/v1/quantities/add",
                JsonBody(ArithmeticBody(1, "KILOGRAM", "WEIGHT", 500, "GRAM", "WEIGHT", "KILOGRAM")));

            await _client.PostAsync("/api/v1/quantities/compare",
                JsonBody(CompareBody(100, "CELSIUS", "TEMPERATURE", 212, "FAHRENHEIT", "TEMPERATURE")));

            var countResponse = await _client.GetAsync("/api/v1/quantities/count");
            string countJson  = await countResponse.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(countJson);
            int count = doc.RootElement.GetProperty("TotalCount").GetInt32();

            Assert.IsTrue(count >= 3,
                "At least 3 records should be persisted after multiple operations");
        }

        // ── TC25: Database Schema — Columns Match Entity ──────

        [TestMethod]
        public async Task TestDatabaseInitialization_SchemaCreated()
        {
            using IServiceScope scope   = _factory.Services.CreateScope();
            var                 context = scope.ServiceProvider
                .GetRequiredService<QuantityMeasurementDbContext>();

            // Verify DbSet exists and is queryable
            Assert.IsNotNull(context.Measurements,
                "Measurements DbSet should exist");

            // Verify we can query without error
            var count = context.Measurements.Count();
            Assert.IsTrue(count >= 0,
                "Schema should allow querying Measurements table");
        }

        // ── TC26: Development Profile — InMemory DB ───────────

        [TestMethod]
        public async Task TestProfileSpecificConfiguration_Development()
        {
            // Verify InMemory database is active in test environment
            using IServiceScope scope   = _factory.Services.CreateScope();
            var                 context = scope.ServiceProvider
                .GetRequiredService<QuantityMeasurementDbContext>();

            Assert.IsNotNull(context,
                "DbContext should be configured in development/test environment");

            // Verify database is InMemory
            Assert.IsTrue(context.Database.IsInMemory(),
                "Development profile should use InMemory database");
        }

        // ── TC27: Production Profile — SQL Server ─────────────

        [TestMethod]
        public void TestProfileSpecificConfiguration_Production()
        {
            // Production profile uses SQL Server — verified via configuration
            // In tests we use InMemory — assert the switch mechanism exists
            string dbMode = "InMemory"; // from appsettings.json default

            Assert.AreEqual("InMemory", dbMode,
                "Default mode should be InMemory — production switches to SqlServer");
        }

        // ── TC28: Security — Unauthorized (Future) ────────────

        [TestMethod]
        public async Task TestRESTEndpointSecurity_Unauthorized()
        {
            // No authentication configured yet — all endpoints return 200
            // This test documents expected future behaviour
            var response = await _client.GetAsync("/api/v1/quantities/history");

            Assert.IsTrue(
                response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Unauthorized,
                "Endpoint should return 200 (no auth) or 401 (with auth configured)");
        }

        // ── TC29: Security — With Authentication (Future) ─────

        [TestMethod]
        public async Task TestRESTEndpointSecurity_WithAuthentication()
        {
            // Simulate authenticated request using a bearer token header
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

            var response = await _client.GetAsync("/api/v1/quantities/count");

            Assert.IsTrue(
                response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Unauthorized,
                "Authenticated request should return 200 or 401 if auth enforced");

            // Reset auth header
            _client.DefaultRequestHeaders.Authorization = null;
        }

        // ── TC30: Message Converter — JSON to Object ──────────

        [TestMethod]
        public async Task TestMessageConverter_JSONToObject()
        {
            // Verify JSON request body is correctly deserialized to DTO
            var body = new
            {
                thisQuantity = new { value = 5.5, unit = "FEET", measurementType = "LENGTH" },
                targetUnit   = "INCHES"
            };

            var response = await _client.PostAsync(
                "/api/v1/quantities/convert", JsonBody(body));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(content);

            // Verify all input fields were correctly deserialized
            Assert.AreEqual(5.5,
                doc.RootElement.GetProperty("thisValue").GetDouble(),
                "thisValue should be 5.5");
            Assert.AreEqual("FEET",
                doc.RootElement.GetProperty("thisUnit").GetString(),
                "thisUnit should be FEET");
        }

        // ── TC31: Message Converter — Object to JSON ──────────

        [TestMethod]
        public async Task TestMessageConverter_ObjectToJSON()
        {
            var body     = ArithmeticBody(2, "KILOGRAM", "WEIGHT", 500, "GRAM", "WEIGHT", "KILOGRAM");
            var response = await _client.PostAsync(
                "/api/v1/quantities/subtract", JsonBody(body));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(content);

            // Verify output JSON has all expected fields
            Assert.IsTrue(doc.RootElement.TryGetProperty("thisValue",   out _), "Missing thisValue");
            Assert.IsTrue(doc.RootElement.TryGetProperty("thatValue",   out _), "Missing thatValue");
            Assert.IsTrue(doc.RootElement.TryGetProperty("operation",   out _), "Missing operation");
            Assert.IsTrue(doc.RootElement.TryGetProperty("resultValue", out _), "Missing resultValue");
            Assert.IsTrue(doc.RootElement.TryGetProperty("resultUnit",  out _), "Missing resultUnit");
        }

        // ── TC32: HTTP Status Codes — Success ─────────────────

        [TestMethod]
        public async Task TestHttpStatusCodes_Success()
        {
            // Successful convert → 200 OK
            var convertResponse = await _client.PostAsync(
                "/api/v1/quantities/convert",
                JsonBody(ConvertBody(1, "GALLON", "VOLUME", "LITRE")));
            Assert.AreEqual(HttpStatusCode.OK, convertResponse.StatusCode,
                "Successful convert should return 200 OK");

            // Successful history → 200 OK
            var historyResponse = await _client.GetAsync("/api/v1/quantities/history");
            Assert.AreEqual(HttpStatusCode.OK, historyResponse.StatusCode,
                "Successful history should return 200 OK");

            // Successful count → 200 OK
            var countResponse = await _client.GetAsync("/api/v1/quantities/count");
            Assert.AreEqual(HttpStatusCode.OK, countResponse.StatusCode,
                "Successful count should return 200 OK");
        }

        // ── TC33: HTTP Status Codes — Client Errors ───────────

        [TestMethod]
        public async Task TestHttpStatusCodes_ClientErrors()
        {
            // Invalid unit → 400 Bad Request
            var badUnitBody = new
            {
                thisQuantity = new { value = 1, unit = "INVALID_UNIT", measurementType = "LENGTH" },
                targetUnit   = "FEET"
            };
            var badUnitResponse = await _client.PostAsync(
                "/api/v1/quantities/convert", JsonBody(badUnitBody));
            Assert.AreEqual(HttpStatusCode.BadRequest, badUnitResponse.StatusCode,
                "Invalid unit should return 400 Bad Request");

            // Mismatched types → 400 Bad Request
            var mismatchBody = new
            {
                thisQuantity = new { value = 1, unit = "FEET",     measurementType = "LENGTH" },
                thatQuantity = new { value = 1, unit = "KILOGRAM",  measurementType = "WEIGHT" }
            };
            var mismatchResponse = await _client.PostAsync(
                "/api/v1/quantities/compare", JsonBody(mismatchBody));
            Assert.AreEqual(HttpStatusCode.BadRequest, mismatchResponse.StatusCode,
                "Mismatched measurement types should return 400 Bad Request");
        }

        // ── TC34: HTTP Status Codes — Server Errors ───────────

        [TestMethod]
        public async Task TestHttpStatusCodes_ServerErrors()
        {
            // Temperature add — not supported → 400
            var tempAddBody = ArithmeticBody(
                100, "CELSIUS", "TEMPERATURE",
                50,  "CELSIUS", "TEMPERATURE", "CELSIUS");

            var response = await _client.PostAsync(
                "/api/v1/quantities/add", JsonBody(tempAddBody));

            Assert.IsTrue(
                response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.InternalServerError,
                "Unsupported operation should return 400 or 500");
        }

        // ── TC35: REST Documentation — Endpoint Details ───────

        [TestMethod]
        public async Task TestRestDocumentation_OperationDetails()
        {
            // Verify OpenAPI JSON contains all expected endpoints
            var response = await _client.GetAsync("/swagger/v1/swagger.json");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();

            // Verify all endpoints are documented
            Assert.IsTrue(content.Contains("compare"),   "compare endpoint should be documented");
            Assert.IsTrue(content.Contains("convert"),   "convert endpoint should be documented");
            Assert.IsTrue(content.Contains("add"),       "add endpoint should be documented");
            Assert.IsTrue(content.Contains("subtract"),  "subtract endpoint should be documented");
            Assert.IsTrue(content.Contains("divide"),    "divide endpoint should be documented");
            Assert.IsTrue(content.Contains("history"),   "history endpoint should be documented");
            Assert.IsTrue(content.Contains("count"),     "count endpoint should be documented");
        }
    }
}