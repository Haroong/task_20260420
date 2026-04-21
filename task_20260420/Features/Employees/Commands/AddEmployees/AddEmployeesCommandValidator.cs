using FluentValidation;

namespace task_20260420.Features.Employees.Commands.AddEmployees;

public class AddEmployeesCommandValidator : AbstractValidator<AddEmployeesCommand>
{
    public AddEmployeesCommandValidator()
    {
        RuleFor(x => x.Employees)
            .NotEmpty().WithMessage("직원 데이터가 비어 있습니다.");

        RuleForEach(x => x.Employees).ChildRules(employee =>
        {
            employee.RuleFor(e => e.Name)
                .NotEmpty().WithMessage("이름은 필수 항목입니다.");
            employee.RuleFor(e => e.Email)
                .NotEmpty().WithMessage("이메일은 필수 항목입니다.")
                .EmailAddress().WithMessage("올바른 이메일 형식이 아닙니다.");
            employee.RuleFor(e => e.Tel)
                .NotEmpty().WithMessage("전화번호는 필수 항목입니다.");
            employee.RuleFor(e => e.Joined)
                .NotEqual(default(DateTime)).WithMessage("입사일은 필수 항목입니다.");
        });
    }
}
