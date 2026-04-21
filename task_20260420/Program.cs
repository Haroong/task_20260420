using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using task_20260420.Common.Behaviors;
using task_20260420.Common.Middleware;
using task_20260420.Infrastructure;
using task_20260420.Services;

var builder = WebApplication.CreateBuilder(args);

// === Serilog ===
builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

// === Controllers ===
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// === Swagger/OpenAPI ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Employee Emergency Contact API",
        Version = "v1",
        Description = "직원 긴급 연락망 관리 API"
    });
});

// === EF Core + SQLite ===
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
                     ?? "Data Source=employees.db"));

// === MediatR (CQRS) ===
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// === FluentValidation ===
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// === MediatR Pipeline Behaviors ===
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// === Parsing Services ===
builder.Services.AddScoped<IEmployeeParser, CsvEmployeeParser>();
builder.Services.AddScoped<IEmployeeParser, JsonEmployeeParser>();
builder.Services.AddScoped<EmployeeParserFactory>();

var app = builder.Build();

// === DB 자동 생성 ===
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// === Middleware ===
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();

// 통합 테스트에서 WebApplicationFactory가 접근할 수 있도록 partial class 선언
public partial class Program { }
