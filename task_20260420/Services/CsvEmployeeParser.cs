using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using task_20260420.Common;
using task_20260420.Domain;

namespace task_20260420.Services;

public class CsvEmployeeParser : IEmployeeParser
{

    public bool CanParse(string content, string? fileName)
    {
        if (fileName is not null)
            return fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);

        var trimmed = content.TrimStart();
        return !trimmed.StartsWith('[') && !trimmed.StartsWith('{');
    }

    public List<Employee> Parse(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new FormatException("CSV 데이터가 비어 있습니다.");

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null
        };

        using var reader = new StringReader(content);
        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap<EmployeeCsvMap>();

        try
        {
            var employees = new List<Employee>();
            while (csv.Read())
            {
                var name = csv.GetField<string>(0)?.Trim();
                var email = csv.GetField<string>(1)?.Trim();
                var tel = csv.GetField<string>(2)?.Trim();
                var dateStr = csv.GetField<string>(3)?.Trim();

                if (string.IsNullOrWhiteSpace(name))
                    throw new FormatException("이름은 필수 항목입니다.");
                if (string.IsNullOrWhiteSpace(email))
                    throw new FormatException("이메일은 필수 항목입니다.");
                if (string.IsNullOrWhiteSpace(tel))
                    throw new FormatException("전화번호는 필수 항목입니다.");
                if (string.IsNullOrWhiteSpace(dateStr))
                    throw new FormatException("입사일은 필수 항목입니다.");

                if (!DateTime.TryParseExact(dateStr, DateFormats.Supported, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out var joined))
                    throw new FormatException($"날짜 형식이 올바르지 않습니다: '{dateStr}'");

                employees.Add(new Employee
                {
                    Name = name,
                    Email = email,
                    Tel = tel,
                    Joined = joined
                });
            }

            if (employees.Count == 0)
                throw new FormatException("파싱된 직원 데이터가 없습니다.");

            return employees;
        }
        catch (FormatException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FormatException($"CSV 형식이 올바르지 않습니다: {ex.Message}", ex);
        }
    }

    private sealed class EmployeeCsvMap : ClassMap<Employee>
    {
        public EmployeeCsvMap()
        {
            Map(m => m.Name).Index(0);
            Map(m => m.Email).Index(1);
            Map(m => m.Tel).Index(2);
        }
    }
}
