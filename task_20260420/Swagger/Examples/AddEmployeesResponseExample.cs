using Swashbuckle.AspNetCore.Filters;
using task_20260420.Common.Models;
using task_20260420.Features.Employees.Commands.AddEmployees;

namespace task_20260420.Swagger.Examples;

public class AddEmployeesResponseExample
    : IMultipleExamplesProvider<BaseResponse<AddEmployeesResult>>
{
    public IEnumerable<SwaggerExample<BaseResponse<AddEmployeesResult>>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "신규 등록",
            new BaseResponse<AddEmployeesResult>(
                true,
                ResponseCode.Created,
                "리소스가 정상 생성되었습니다.",
                new AddEmployeesResult(3, 0)));

        yield return SwaggerExample.Create(
            "일부 업데이트",
            new BaseResponse<AddEmployeesResult>(
                true,
                ResponseCode.Created,
                "리소스가 정상 생성되었습니다.",
                new AddEmployeesResult(2, 1)));
    }
}
