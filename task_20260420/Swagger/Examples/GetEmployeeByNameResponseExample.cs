using Swashbuckle.AspNetCore.Filters;
using task_20260420.Common.Models;
using task_20260420.Features.Employees.Models;

namespace task_20260420.Swagger.Examples;

public class GetEmployeeByNameResponseExample
    : IMultipleExamplesProvider<BaseResponse<EmployeeDto>>
{
    public IEnumerable<SwaggerExample<BaseResponse<EmployeeDto>>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "성공 응답",
            new BaseResponse<EmployeeDto>(
                true,
                ResponseCode.Success,
                "요청이 정상 처리되었습니다.",
                new EmployeeDto("김민수", "minsu.kim@company.com", "010-1234-5678", "2023-03-15")));
    }
}
