using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using task_20260420.Common;
using task_20260420.Domain;

namespace task_20260420.Services;

public class JsonEmployeeParser : IEmployeeParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new FlexibleDateTimeConverter() }
    };

    public bool CanParse(string content, string? fileName)
    {
        if (fileName is not null)
            return fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase);

        var trimmed = content.TrimStart();
        return trimmed.StartsWith('[') || trimmed.StartsWith('{');
    }

    public List<Employee> Parse(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new FormatException("JSON 데이터가 비어 있습니다.");

        try
        {
            var trimmed = content.TrimStart();
            List<EmployeeJsonDto> dtos;

            if (trimmed.StartsWith('['))
            {
                dtos = JsonSerializer.Deserialize<List<EmployeeJsonDto>>(content, Options)
                       ?? throw new FormatException("JSON 배열 파싱에 실패했습니다.");
            }
            else
            {
                var single = JsonSerializer.Deserialize<EmployeeJsonDto>(content, Options)
                             ?? throw new FormatException("JSON 객체 파싱에 실패했습니다.");
                dtos = [single];
            }

            if (dtos.Count == 0)
                throw new FormatException("파싱된 직원 데이터가 없습니다.");

            return dtos.Select(dto =>
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new FormatException("이름은 필수 항목입니다.");
                if (string.IsNullOrWhiteSpace(dto.Email))
                    throw new FormatException("이메일은 필수 항목입니다.");
                if (string.IsNullOrWhiteSpace(dto.Tel))
                    throw new FormatException("전화번호는 필수 항목입니다.");

                return new Employee
                {
                    Name = dto.Name.Trim(),
                    Email = dto.Email.Trim(),
                    Tel = dto.Tel.Trim(),
                    Joined = dto.Joined
                };
            }).ToList();
        }
        catch (JsonException ex)
        {
            throw new FormatException($"JSON 형식이 올바르지 않습니다: {ex.Message}", ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new FormatException($"JSON 데이터 타입이 올바르지 않습니다: {ex.Message}", ex);
        }
    }

    private sealed class EmployeeJsonDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Tel { get; set; } = string.Empty;
        public DateTime Joined { get; set; }
    }

    private sealed class FlexibleDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string[] Formats = DateFormats.Supported;

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString()
                      ?? throw new JsonException("날짜 값이 null입니다.");

            if (DateTime.TryParseExact(str, Formats, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date))
                return date;

            throw new JsonException($"날짜 형식이 올바르지 않습니다: '{str}'");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
        }
    }
}
