using MediatR;
using Microsoft.AspNetCore.Mvc;
using task_20260420.Features.Employees.Commands.AddEmployees;
using task_20260420.Features.Employees.Queries.GetEmployeeByName;
using task_20260420.Features.Employees.Queries.GetEmployees;
using task_20260420.Services;

namespace task_20260420.Controllers;

/// <summary>
/// 직원 긴급 연락 정보를 관리하는 API Controller.
/// CQRS 패턴에 따라 Controller는 HTTP 요청/응답 관심사만 담당하고,
/// 비즈니스 로직은 MediatR를 통해 Command/Query Handler에 위임합니다.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly EmployeeParserFactory _parserFactory;
    private readonly IContentExtractor _contentExtractor;

    public EmployeeController(IMediator mediator, EmployeeParserFactory parserFactory, IContentExtractor contentExtractor)
    {
        _mediator = mediator;
        _parserFactory = parserFactory;
        _contentExtractor = contentExtractor;
    }

    /// <summary>
    /// 직원 목록을 페이징하여 조회합니다.
    /// </summary>
    /// <param name="page">페이지 번호 (1부터 시작, 기본값: 1)</param>
    /// <param name="pageSize">페이지당 항목 수 (1~100, 기본값: 10)</param>
    /// <returns>페이징된 직원 목록 (items, totalCount, page, pageSize, totalPages)</returns>
    /// <response code="200">정상 조회</response>
    /// <response code="400">잘못된 페이지 파라미터</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetEmployeesQuery(page, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// 이름으로 직원을 조회합니다.
    /// </summary>
    /// <param name="name">검색할 직원 이름 (정확히 일치)</param>
    /// <returns>직원 상세 연락 정보 (name, email, tel, joined)</returns>
    /// <response code="200">직원 정보 반환</response>
    /// <response code="404">해당 이름의 직원 없음</response>
    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName(string name)
    {
        var result = await _mediator.Send(new GetEmployeeByNameQuery(name));
        return Ok(result);
    }

    /// <summary>
    /// 직원 연락 정보를 추가합니다.
    /// 4가지 입력 방식을 지원합니다:
    /// 1) CSV 파일 업로드 (input type=file, .csv)
    /// 2) JSON 파일 업로드 (input type=file, .json)
    /// 3) CSV 텍스트 직접 입력 (textarea → data 필드)
    /// 4) JSON 텍스트 직접 입력 (textarea → data 필드)
    /// 포맷은 파일 확장자 또는 내용 기반으로 자동 감지됩니다.
    /// </summary>
    /// <param name="file">CSV 또는 JSON 파일 (선택)</param>
    /// <param name="data">CSV 또는 JSON 텍스트 (선택)</param>
    /// <returns>추가된 직원 수</returns>
    /// <response code="201">직원 정보 추가 성공</response>
    /// <response code="400">입력 데이터 형식 오류 또는 필수값 누락</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add(IFormFile? file, [FromForm] string? data)
    {
        var (content, fileName) = await _contentExtractor.ExtractAsync(file, data, Request);
        var employees = _parserFactory.Parse(content, fileName);
        var result = await _mediator.Send(new AddEmployeesCommand(employees));

        return Created(string.Empty, result);
    }
}
