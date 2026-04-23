using Swashbuckle.AspNetCore.Filters;
using task_20260420.Common.Models;
using task_20260420.Features.Employees.Models;

namespace task_20260420.Swagger.Examples;

public class GetEmployeesResponseExample
    : IMultipleExamplesProvider<BaseResponse<PagedResult<EmployeeDto>>>
{
    public IEnumerable<SwaggerExample<BaseResponse<PagedResult<EmployeeDto>>>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "성공 응답",
            new BaseResponse<PagedResult<EmployeeDto>>(
                true,
                ResponseCode.Success,
                "요청이 정상 처리되었습니다.",
                new PagedResult<EmployeeDto>(
                    new List<EmployeeDto>
                    {
                        new("김민수", "minsu.kim@company.com", "010-1234-5678", "2023-03-15"),
                        new("이지은", "jieun.lee@company.com", "010-9876-5432", "2024-01-10")
                    },
                    TotalCount: 25,
                    Page: 1,
                    PageSize: 10)));
    }
}
