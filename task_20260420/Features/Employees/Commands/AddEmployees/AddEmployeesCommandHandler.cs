using MediatR;
using task_20260420.Infrastructure;

namespace task_20260420.Features.Employees.Commands.AddEmployees;

public class AddEmployeesCommandHandler : IRequestHandler<AddEmployeesCommand, AddEmployeesResult>
{
    private readonly AppDbContext _db;

    public AddEmployeesCommandHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AddEmployeesResult> Handle(AddEmployeesCommand request, CancellationToken cancellationToken)
    {
        _db.Employees.AddRange(request.Employees);
        await _db.SaveChangesAsync(cancellationToken);

        return new AddEmployeesResult(request.Employees.Count);
    }
}
