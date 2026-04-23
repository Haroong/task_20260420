using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace task_20260420.Swagger.Filters;

public class AddEmployeeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.HttpMethod != "POST"
            || !context.ApiDescription.RelativePath!.Equals("api/employee", StringComparison.OrdinalIgnoreCase))
            return;

        operation.RequestBody ??= new OpenApiRequestBody();

        AddJsonExamples(operation);
        AddTextPlainExamples(operation);
        AddMultipartDescription(operation);
    }

    private static void AddJsonExamples(OpenApiOperation operation)
    {
        if (!operation.RequestBody.Content.TryGetValue("application/json", out var jsonMedia))
        {
            jsonMedia = new OpenApiMediaType();
            operation.RequestBody.Content["application/json"] = jsonMedia;
        }

        jsonMedia.Examples["JSON 배열 입력"] = new OpenApiExample
        {
            Summary = "여러 직원을 JSON 배열로 등록",
            Value = new OpenApiArray
            {
                Employee("김민수", "minsu.kim@company.com", "010-1234-5678", "2023-03-15"),
                Employee("이지은", "jieun.lee@company.com", "010-9876-5432", "2024-01-10")
            }
        };

        jsonMedia.Examples["JSON 단건 입력"] = new OpenApiExample
        {
            Summary = "직원 1명을 JSON 객체로 등록",
            Value = Employee("김민수", "minsu.kim@company.com", "010-1234-5678", "2023-03-15")
        };
    }

    private static void AddTextPlainExamples(OpenApiOperation operation)
    {
        if (!operation.RequestBody.Content.TryGetValue("text/plain", out var textMedia))
        {
            textMedia = new OpenApiMediaType();
            operation.RequestBody.Content["text/plain"] = textMedia;
        }

        textMedia.Examples["CSV 텍스트 입력"] = new OpenApiExample
        {
            Summary = "CSV 형식으로 직원 등록",
            Value = new OpenApiString(
                "이름,이메일,전화번호,입사일\n김민수,minsu.kim@company.com,010-1234-5678,2023-03-15\n이지은,jieun.lee@company.com,010-9876-5432,2024-01-10")
        };
    }

    private static void AddMultipartDescription(OpenApiOperation operation)
    {
        if (!operation.RequestBody.Content.TryGetValue("multipart/form-data", out var formMedia))
            return;

        operation.RequestBody.Description =
            "CSV 또는 JSON 데이터를 전송합니다. 다음 중 하나를 선택하세요:\n" +
            "• **file**: `.csv` 또는 `.json` 파일 업로드\n" +
            "• **data**: CSV 또는 JSON 텍스트 직접 입력\n" +
            "• **Body (application/json)**: JSON 배열 또는 단건 객체를 직접 전송\n" +
            "• **Body (text/plain)**: CSV 텍스트를 직접 전송";
    }

    private static OpenApiObject Employee(string name, string email, string tel, string joined) =>
        new()
        {
            ["name"] = new OpenApiString(name),
            ["email"] = new OpenApiString(email),
            ["tel"] = new OpenApiString(tel),
            ["joined"] = new OpenApiString(joined)
        };
}
