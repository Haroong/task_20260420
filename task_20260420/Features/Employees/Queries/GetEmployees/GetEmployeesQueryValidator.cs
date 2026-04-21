using FluentValidation;

namespace task_20260420.Features.Employees.Queries.GetEmployees;

public class GetEmployeesQueryValidator : AbstractValidator<GetEmployeesQuery>
{
    public GetEmployeesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("page는 1 이상이어야 합니다.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("pageSize는 1~100 사이여야 합니다.");
    }
}
