using System.Text.Json.Serialization;

namespace task_20260420.Common.Models;

public record ErrorResponse(
    [property: JsonPropertyName("status")] int Status,
    [property: JsonPropertyName("message")] string Message);
