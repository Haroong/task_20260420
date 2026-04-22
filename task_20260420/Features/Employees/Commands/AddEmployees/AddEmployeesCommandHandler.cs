using MediatR;
using task_20260420.Infrastructure;
using task_20260420.Services;

namespace task_20260420.Features.Employees.Commands.AddEmployees;

public class AddEmployeesCommandHandler : IRequestHandler<AddEmployeesCommand, AddEmployeesResult>
{
    private readonly AppDbContext _db;
    private readonly EmployeeParserFactory _parserFactory;

    public AddEmployeesCommandHandler(AppDbContext db, EmployeeParserFactory parserFactory)
    {
        _db = db;
        _parserFactory = parserFactory;
    }

    public async Task<AddEmployeesResult> Handle(AddEmployeesCommand request, CancellationToken cancellationToken)
    {
        var employees = _parserFactory.Parse(request.Content, request.FileName);

        _db.Employees.AddRange(employees);
        await _db.SaveChangesAsync(cancellationToken);

        return new AddEmployeesResult(employees.Count);
    }
}
