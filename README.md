# Employee Emergency Contact API

직원 긴급 연락망 관리를 위한 Backend API입니다.

## 기술 스택

- **.NET 8** (ASP.NET Core Web API)
- **CQRS 패턴** (MediatR)
- **EF Core + SQLite** (영속화)
- **FluentValidation** (입력값 검증)
- **Serilog** (구조적 로깅)
- **CsvHelper** (CSV 파싱)
- **xUnit + FluentAssertions** (테스트)

## 실행 방법

```bash
git clone <repository-url>
cd task_20260420
dotnet build
dotnet run --project task_20260420
```

실행 후 Swagger UI: http://localhost:5081/swagger

## 테스트 실행

```bash
dotnet test
```

## Persistence 설정

SQLite를 사용하며, 앱 시작 시 `employees.db` 파일이 자동 생성됩니다.
별도의 DB 설치나 마이그레이션 명령이 필요하지 않습니다.

연결문자열은 `appsettings.json`에서 변경할 수 있습니다:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=employees.db"
  }
}
```

MSSQL 등 다른 DB로 전환 시 `Program.cs`에서 `UseSqlite()`를 `UseSqlServer()` 등으로 변경하면 됩니다.

## API 명세

### GET /api/employee?page={page}&pageSize={pageSize}

직원 목록을 페이징하여 조회합니다.

- **page**: 페이지 번호 (기본값: 1)
- **pageSize**: 페이지 크기 (기본값: 10, 최대: 100)

```json
{
  "items": [
    { "name": "김철수", "email": "charles@test.com", "tel": "01075312468", "joined": "2018-03-07" }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

### GET /api/employee/{name}

이름으로 직원을 조회합니다.

- **200 OK**: 직원 정보 반환
- **404 Not Found**: 해당 이름의 직원이 없음

### POST /api/employee

직원 연락 정보를 추가합니다. 4가지 입력 방식을 지원합니다:

| 입력 방식 | Content-Type | 필드 |
|-----------|-------------|------|
| CSV 파일 업로드 | multipart/form-data | file (*.csv) |
| JSON 파일 업로드 | multipart/form-data | file (*.json) |
| CSV 텍스트 직접 입력 | multipart/form-data | data (텍스트) |
| JSON 텍스트 직접 입력 | multipart/form-data | data (텍스트) |

CSV/JSON 포맷은 파일 확장자 또는 내용 기반으로 자동 감지됩니다.

**CSV 형식 예제:**
```
김철수, charles@test.com, 01075312468, 2018.03.07
박영희, matilda@test.com, 01087654321, 2021.04.28
```

**JSON 형식 예제:**
```json
[
  {"name":"김하늘", "email":"haneul@test.com", "tel":"010-1111-2424", "joined":"2012-01-05"},
  {"name":"박마블", "email":"md@test.com", "tel":"010-3535-7979", "joined":"2013-07-01"}
]
```

## 프로젝트 구조

```
task_20260420/
├── Controllers/          # API 엔드포인트 (thin layer)
├── Domain/               # 도메인 엔티티
├── Infrastructure/       # 데이터 액세스 (EF Core)
├── Features/Employees/   # CQRS Commands & Queries
├── Services/             # 입력 파싱 (CSV/JSON)
├── Common/               # 횡단 관심사 (로깅, 검증, 예외처리)
└── Program.cs            # DI 구성, 미들웨어
```

## 아키텍처: CQRS 패턴

읽기(Query)와 쓰기(Command)를 MediatR를 통해 구조적으로 분리합니다.

- **Query**: `GetEmployeesQuery`, `GetEmployeeByNameQuery`
- **Command**: `AddEmployeesCommand`
- **Pipeline Behaviors**: `LoggingBehavior`, `ValidationBehavior`
