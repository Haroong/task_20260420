using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;

namespace task_20260420.Tests.Integration;

public class EmployeeEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EmployeeEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // === POST: CSV 파일 업로드 ===

    [Fact]
    public async Task PostCsvFile_ReturnsCreated()
    {
        var csv = "김철수, charles@test.com, 01075312468, 2018.03.07\n박영희, matilda@test.com, 01087654321, 2021.04.28";
        var content = CreateFileContent(csv, "employees.csv", "text/csv");

        var response = await _client.PostAsync("/api/employee", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await DeserializeResponse(response);
        body.GetProperty("isSuccess").GetBoolean().Should().BeTrue();
        body.GetProperty("code").GetString().Should().Be("COMMON201");
        body.GetProperty("result").GetProperty("addedCount").GetInt32().Should().Be(2);
    }

    // === POST: JSON 파일 업로드 ===

    [Fact]
    public async Task PostJsonFile_ReturnsCreated()
    {
        var json = """[{"name":"김클로","email":"clo@test.com","tel":"010-1111-2424","joined":"2012-01-05"}]""";
        var content = CreateFileContent(json, "employees.json", "application/json");

        var response = await _client.PostAsync("/api/employee", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await DeserializeResponse(response);
        body.GetProperty("isSuccess").GetBoolean().Should().BeTrue();
        body.GetProperty("result").GetProperty("addedCount").GetInt32().Should().Be(1);
    }

    // === POST: CSV 텍스트 직접 입력 ===

    [Fact]
    public async Task PostCsvText_ReturnsCreated()
    {
        var csv = "홍길동, gildong@test.com, 01012345678, 2015.08.15";
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(csv), "data");

        var response = await _client.PostAsync("/api/employee", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // === POST: JSON 텍스트 직접 입력 ===

    [Fact]
    public async Task PostJsonText_ReturnsCreated()
    {
        var json = """[{"name":"박마블","email":"md@test.com","tel":"010-3535-7979","joined":"2013-07-01"}]""";
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(json), "data");

        var response = await _client.PostAsync("/api/employee", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // === POST: 실패 케이스 ===

    [Fact]
    public async Task PostEmptyForm_ReturnsBadRequest()
    {
        var content = new MultipartFormDataContent();

        var response = await _client.PostAsync("/api/employee", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await DeserializeResponse(response);
        body.GetProperty("status").GetInt32().Should().Be(400);
        body.GetProperty("errors").ValueKind.Should().Be(JsonValueKind.Object);
    }

    [Fact]
    public async Task PostInvalidCsv_ReturnsBadRequest()
    {
        var csv = "잘못된데이터";
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(csv), "data");

        var response = await _client.PostAsync("/api/employee", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await DeserializeResponse(response);
        body.GetProperty("isSuccess").GetBoolean().Should().BeFalse();
        body.GetProperty("code").GetString().Should().Be("FORMAT400");
        body.GetProperty("message").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task PostInvalidJson_ReturnsBadRequest()
    {
        var json = "{invalid json}";
        var content = CreateFileContent(json, "bad.json", "application/json");

        var response = await _client.PostAsync("/api/employee", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await DeserializeResponse(response);
        body.GetProperty("isSuccess").GetBoolean().Should().BeFalse();
        body.GetProperty("code").GetString().Should().Be("FORMAT400");
        body.GetProperty("message").GetString().Should().NotBeNullOrEmpty();
    }

    // === GET: 페이징 조회 ===

    [Fact]
    public async Task GetAll_AfterPost_ReturnsEmployees()
    {
        // Arrange: 데이터 추가
        var csv = "페이징A, paginga@test.com, 01011111111, 2020.01.01\n페이징B, pagingb@test.com, 01022222222, 2021.01.01";
        var postContent = CreateFileContent(csv, "test.csv", "text/csv");
        var postResponse = await _client.PostAsync("/api/employee", postContent);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act
        var response = await _client.GetAsync("/api/employee?page=1&pageSize=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await DeserializeResponse(response);
        body.GetProperty("isSuccess").GetBoolean().Should().BeTrue();
        var result = body.GetProperty("result");
        result.GetProperty("totalCount").GetInt32().Should().BeGreaterThanOrEqualTo(2);
        result.GetProperty("items").GetArrayLength().Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetAll_InvalidPage_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/employee?page=0&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await DeserializeResponse(response);
        body.GetProperty("isSuccess").GetBoolean().Should().BeFalse();
        body.GetProperty("code").GetString().Should().Be("VALIDATION400");
        body.GetProperty("message").GetString().Should().NotBeNullOrEmpty();
    }

    // === GET: 이름 조회 ===

    [Fact]
    public async Task GetByName_ExistingEmployee_ReturnsOk()
    {
        // Arrange
        const string name = "SearchTest";
        var json = """[{"name":"SearchTest","email":"search@test.com","tel":"01099999999","joined":"2020-06-15"}]""";
        var postContent = CreateFileContent(json, "test.json", "application/json");
        var postResponse = await _client.PostAsync("/api/employee", postContent);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act
        var response = await _client.GetAsync($"/api/employee/{name}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await DeserializeResponse(response);
        body.GetProperty("isSuccess").GetBoolean().Should().BeTrue();
        var result = body.GetProperty("result");
        result.GetProperty("name").GetString().Should().Be(name);
        result.GetProperty("email").GetString().Should().Be("search@test.com");
    }

    [Fact]
    public async Task GetByName_NonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/employee/존재하지않는직원");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await DeserializeResponse(response);
        body.GetProperty("isSuccess").GetBoolean().Should().BeFalse();
        body.GetProperty("code").GetString().Should().Be("NOTFOUND404");
        body.GetProperty("message").GetString().Should().NotBeNullOrEmpty();
    }

    // === POST: 중복 처리 ===

    [Fact]
    public async Task PostDuplicateEmail_UpdatesAndReturnsCorrectCounts()
    {
        // Arrange: 최초 등록
        var csv1 = "중복테스트, duptest@test.com, 01011111111, 2020.01.01";
        var content1 = CreateFileContent(csv1, "first.csv", "text/csv");
        var response1 = await _client.PostAsync("/api/employee", content1);
        response1.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act: 같은 이메일로 재등록
        var csv2 = "중복테스트_수정, duptest@test.com, 01099999999, 2024.06.01";
        var content2 = CreateFileContent(csv2, "second.csv", "text/csv");
        var response2 = await _client.PostAsync("/api/employee", content2);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await DeserializeResponse(response2);
        body.GetProperty("isSuccess").GetBoolean().Should().BeTrue();
        var result = body.GetProperty("result");
        result.GetProperty("addedCount").GetInt32().Should().Be(0);
        result.GetProperty("updatedCount").GetInt32().Should().Be(1);
    }

    [Fact]
    public async Task PostBatchWithInternalDuplicates_DeduplicatesCorrectly()
    {
        // Act: 배치 내 동일 이메일 2건
        var csv = "첫번째, batchdup@test.com, 01011111111, 2020.01.01\n두번째, batchdup@test.com, 01022222222, 2024.06.01";
        var content = CreateFileContent(csv, "batch-dup.csv", "text/csv");
        var response = await _client.PostAsync("/api/employee", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await DeserializeResponse(response);
        body.GetProperty("result").GetProperty("addedCount").GetInt32().Should().Be(1);
    }

    // === 헬퍼 메서드 ===

    private static MultipartFormDataContent CreateFileContent(string content, string fileName, string mediaType)
    {
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(content));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

        var form = new MultipartFormDataContent();
        form.Add(fileContent, "file", fileName);
        return form;
    }

    private static async Task<JsonElement> DeserializeResponse(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<JsonElement>(json);
    }
}
