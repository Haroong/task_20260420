using MediatR;
using Microsoft.EntityFrameworkCore;
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
        var parsed = _parserFactory.Parse(request.Content, request.FileName);

        var deduped = parsed
            .GroupBy(e => e.Email.ToLowerInvariant())
            .Select(g => g.Last())
            .ToList();

        var emailSet = deduped.Select(e => e.Email.ToLowerInvariant()).ToHashSet();

        var existingList = await _db.Employees
            .Where(e => emailSet.Contains(e.Email.ToLower()))
            .ToListAsync(cancellationToken);

        var existing = existingList
            .GroupBy(e => e.Email.ToLowerInvariant())
            .ToDictionary(g => g.Key, g => g.First());

        int added = 0, updated = 0;

        foreach (var emp in deduped)
        {
            var key = emp.Email.ToLowerInvariant();
            if (existing.TryGetValue(key, out var existingEmp))
            {
                existingEmp.Name = emp.Name;
                existingEmp.Tel = emp.Tel;
                existingEmp.Joined = emp.Joined;
                updated++;
            }
            else
            {
                _db.Employees.Add(emp);
                added++;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return new AddEmployeesResult(added, updated);
    }
}
