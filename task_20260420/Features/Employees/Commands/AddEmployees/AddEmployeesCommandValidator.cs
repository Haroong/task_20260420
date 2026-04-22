using FluentValidation;

namespace task_20260420.Features.Employees.Commands.AddEmployees;

public class AddEmployeesCommandValidator : AbstractValidator<AddEmployeesCommand>
{
    public AddEmployeesCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("직원 데이터가 비어 있습니다.");
    }
}
