# 📚 CourseApp - Production-Ready Eğitim Yönetim Sistemi

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## 🚀 Hızlı Başlangıç (Quick Start)

### 1️⃣ Repository'yi Klonlayın

```bash
git clone https://github.com/uygaryildirim/CourseAppHackathon.git
cd CourseAppHackathon/CourseApp
```

### 2️⃣ Gereksinimler

- **.NET 9.0 SDK** veya üzeri
  - İndirme: [.NET Downloads](https://dotnet.microsoft.com/download)
  - Kontrol: `dotnet --version` (9.0.0 veya üzeri olmalı)

### 3️⃣ Projeyi Çalıştırın

```bash
# Proje dizinine gidin
cd CourseApp.API

# Paketleri geri yükleyin (ilk kez çalıştırıyorsanız)
dotnet restore

# Projeyi çalıştırın
dotnet run
```

### 4️⃣ Tarayıcıda Açın

Proje başladıktan sonra aşağıdaki URL'lerde uygulamaya erişebilirsiniz:

- 🌐 **Web UI**: [http://localhost:5232](http://localhost:5232) veya [https://localhost:7120](https://localhost:7120)
- 📚 **Swagger UI**: [http://localhost:5232/swagger](http://localhost:5232/swagger) veya [https://localhost:7120/swagger](https://localhost:7120/swagger)
- ❤️ **Health Check**: [http://localhost:5232/health](http://localhost:5232/health) veya [https://localhost:7120/health](https://localhost:7120/health)

### 5️⃣ Uygulamayı Durdurma

Terminal'de `Ctrl + C` tuşlarına basarak uygulamayı durdurabilirsiniz.

**macOS/Linux için alternatif:**
```bash
# Çalışan process'i bulup durdur
pkill -f "CourseApp.API"
```

---

## 📋 İçindekiler

1. [Hızlı Başlangıç](#-hızlı-başlangıç-quick-start)
2. [Proje Hakkında](#proje-hakkında)
3. [Mimari Yapı](#mimari-yapı)
4. [Yapılan İyileştirmeler](#yapılan-iyileştirmeler)
5. [Teknik Detaylar](#teknik-detaylar)
6. [Kurulum](#kurulum)
7. [Kullanım](#kullanım)
8. [API Dokümantasyonu](#api-dokümantasyonu)
9. [Güvenlik Önlemleri](#güvenlik-önlemleri)
10. [Performans İyileştirmeleri](#performans-iyileştirmeleri)

---

## 🎯 Proje Hakkında

**CourseApp**, bir hackathon projesi olarak başlamış ve **production-ready, sürdürülebilir, güvenli ve ölçeklenebilir** bir API'ye dönüştürülmüştür. Proje, .NET 9.0 kullanılarak geliştirilmiş, katmanlı mimari (layered architecture) prensiplerine uygun olarak tasarlanmış bir eğitim yönetim sistemidir.

### Proje Özellikleri

- ✅ **7 Ana Entity**: Student, Course, Instructor, Exam, ExamResult, Lesson, Registration
- ✅ **RESTful API**: Tüm CRUD işlemleri için standart HTTP metodları
- ✅ **Swagger UI**: Interaktif API dokümantasyonu
- ✅ **Custom Web UI**: Modern, responsive web arayüzü
- ✅ **Structured Logging**: Serilog ile JSON formatında loglama
- ✅ **Health Checks**: Sistem sağlık kontrolü
- ✅ **Rate Limiting**: DDoS koruması
- ✅ **Response Compression**: Gzip/Brotli sıkıştırma
- ✅ **Global Exception Handling**: Merkezi hata yönetimi
- ✅ **FluentValidation**: Otomatik veri doğrulama
- ✅ **Unit Tests**: xUnit, Moq, FluentAssertions ile test coverage

---

## 🏗️ Mimari Yapı

### Katmanlar (Layers)

```
CourseApp/
├── CourseApp.API/              # Presentation Layer (REST API)
│   ├── Controllers/            # API endpoint'leri
│   ├── Middleware/             # Custom middleware'ler
│   ├── Validators/             # FluentValidation validators
│   └── wwwroot/                # Static files (Web UI)
│
├── CourseApp.ServiceLayer/     # Business Logic Layer
│   ├── Abstract/                # Service interfaces
│   ├── Concrete/                # Service implementations (Managers)
│   ├── Mapping/                 # AutoMapper profiles
│   └── Utilities/               # Helper classes, constants
│
├── CourseApp.DataAccessLayer/  # Data Access Layer
│   ├── Abstract/                # Repository interfaces
│   ├── Concrete/                # Repository implementations
│   └── UnitOfWork/              # Unit of Work pattern
│
├── EntityLayer/                 # Domain Layer
│   ├── Entity/                  # Domain entities
│   ├── Dto/                     # Data Transfer Objects
│   └── Enums/                   # Enum types
│
└── CourseApp.Tests/             # Test Layer
    └── Services/                 # Unit tests
```

### Mimari Prensipler

1. **Separation of Concerns**: Her katman kendi sorumluluğuna odaklanır
2. **Dependency Injection**: Tüm bağımlılıklar DI container üzerinden yönetilir
3. **Repository Pattern**: Veri erişimi soyutlanmıştır
4. **Unit of Work Pattern**: Transaction yönetimi ve tutarlılık sağlanır
5. **AutoMapper**: Entity-DTO dönüşümleri otomatikleştirilmiştir
6. **FluentValidation**: Girdi doğrulama işlemleri merkezileştirilmiştir

---

## 🚀 Yapılan İyileştirmeler

### 1. Derleme Hataları (Build Errors) - 20+ Hata Düzeltildi

#### 1.1 Yazım Hataları (Typos)

**Sorun:** Method ve property isimlerinde yazım hataları
- `AddScopd` → `AddScoped`
- `ExamManagr` → `ExamManager`
- `MapContrllers` → `MapControllers`
- `GetByIdAsnc` → `GetByIdAsync`
- `CreatAsync` → `CreateAsync`
- `result.Succes` → `result.IsSuccess`
- `updateStudntDto` → `updateStudentDto`
- `BadReqest` → `BadRequest`
- `examtListMapping` → `examListMapping`

**Çözüm:** Tüm yazım hataları düzeltildi, doğru isimlendirmeler kullanıldı.

#### 1.2 Dosya İsimlendirme Hataları

**Sorun:** Dosya isimlerinde trailing space karakterleri
- `ICourseRepository .cs` → `ICourseRepository.cs`
- `IInstructorRepository .cs` → `IInstructorRepository.cs`

**Çözüm:** Dosya isimlerindeki boşluklar kaldırıldı.

#### 1.3 Eksik Noktalı Virgüller

**Sorun:** Bazı return statement'larında noktalı virgül eksikliği
- `return BadRequest(result)` → `return BadRequest(result);`

**Çözüm:** Tüm statement'lara noktalı virgül eklendi.

#### 1.4 Eksik Base Constructor Call

**Sorun:** `InstructorRepository` constructor'ında base class constructor çağrısı eksik
```csharp
public InstructorRepository(AppDbContext context) // Base constructor çağrısı yok
```

**Çözüm:** `: base(context)` eklendi
```csharp
public InstructorRepository(AppDbContext context) : base(context)
```

#### 1.5 Gereksiz/Nonsense Kodlar

**Sorun:** Var olmayan class'lara referanslar, kullanılmayan methodlar
- `NonExistentMethod()`
- `UseUndefinedType()`
- `MissingImplementation()`
- `AccessMissingRepository()`
- `CallMissingMethod()`
- `UseNonExistentNamespace()`
- `AccessNonExistentProperty()`
- `GetNonExistentAsync()`

**Çözüm:** Tüm gereksiz kodlar kaldırıldı.

#### 1.6 Mapping Hataları

**Sorun:** Var olmayan DTO'lara mapping tanımları
- `NonExistentStudentMappingDto`
- `UndefinedMappingDto`
- `MissingRegistrationMappingDto`
- `MissingMappingDto`
- `NonExistentDtoType`
- `MissingMappingClass`

**Çözüm:** Tüm hatalı mapping'ler kaldırıldı.

---

### 2. Runtime Hataları (Runtime Errors) - 40+ Hata Düzeltildi

#### 2.1 Null Reference Exceptions

**Sorun:** Null kontrolü yapılmadan nesne kullanımı

**Örnekler:**
```csharp
// ❌ Önceki Kod
var result = await _studentService.GetByIdAsync(id);
return Ok(result.Data); // result veya result.Data null olabilir

// ✅ Düzeltilmiş Kod
if (string.IsNullOrEmpty(id))
{
    return BadRequest(new { Message = "ID parametresi boş olamaz." });
}
var result = await _studentService.GetByIdAsync(id);
if (result != null && result.IsSuccess && result.Data != null)
{
    return Ok(result);
}
return BadRequest(result);
```

**Düzeltilen Yerler:**
- Tüm Controller'larda `GetById`, `Create`, `Update`, `Delete` metodları
- Tüm Manager sınıflarında `GetByIdAsync`, `CreateAsync`, `Update`, `Remove` metodları
- DTO parametreleri için null kontrolleri
- Service result'ları için null kontrolleri

#### 2.2 Index Out of Range Exceptions

**Sorun:** String ve array'lere güvenli olmayan erişim

**Örnekler:**
```csharp
// ❌ Önceki Kod
var idPrefix = id[5]; // id 5 karakterden kısa olabilir
var tcFirstDigit = entity.TC[0]; // TC boş olabilir
var firstExam = examList[0]; // Liste boş olabilir

// ✅ Düzeltilmiş Kod
// Doğrudan array/string erişimi kaldırıldı
// String.IsNullOrEmpty ve Length kontrolleri eklendi
// List<T>.Any() kontrolü eklendi
```

**Düzeltilen Yerler:**
- `StudentsController`: `id[10]` erişimi kaldırıldı
- `CoursesController`: `courseName[0]` erişimi kaldırıldı
- `StudentManager`: `entity.TC[0]` erişimi kaldırıldı
- `InstructorManager`: `id[5]` erişimi kaldırıldı
- `ExamManager`: `firstExam` erişimi kaldırıldı
- `RegistrationManager`: `registrationDataMapping.ToList()[0]` erişimi kaldırıldı

#### 2.3 Invalid Cast Exceptions

**Sorun:** Yanlış tip dönüşümleri

**Örnekler:**
```csharp
// ❌ Önceki Kod
var invalidId = (int)createStudentDto.Name; // string'i int'e cast etmeye çalışıyor
var invalidPrice = (int)updatedRegistration.Price; // decimal'i int'e cast ediyor

// ✅ Düzeltilmiş Kod
// Gereksiz cast'ler kaldırıldı
// Doğru tip dönüşümleri kullanıldı
```

**Düzeltilen Yerler:**
- `StudentsController`: `(int)createStudentDto.Name` kaldırıldı
- `StudentManager`: `(int)entity.TC` kaldırıldı
- `RegistrationManager`: `(int)updatedRegistration.Price` kaldırıldı

#### 2.4 Mantıksal Hatalar (Logical Errors)

**Sorun:** Yanlış result tipi ve mesaj döndürme

**Örnekler:**
```csharp
// ❌ Önceki Kod
if (!updateSuccess)
{
    return new SuccessResult(ConstantsMessages.StudentUpdateFailedMessage);
    // Hata durumunda SuccessResult döndürülüyor!
}

// ✅ Düzeltilmiş Kod
if (!updateSuccess)
{
    return new ErrorResult(ConstantsMessages.StudentUpdateFailedMessage);
    // Hata durumunda ErrorResult döndürülüyor
}
```

**Düzeltilen Yerler:**
- `StudentManager.Update`: `SuccessResult` → `ErrorResult`
- `InstructorManager.Update`: `SuccessResult` → `ErrorResult`
- `LessonsManager.Update`: `SuccessResult` → `ErrorResult`
- `RegistrationManager.Update`: `SuccessResult` → `ErrorResult`
- `ExamResultManager.GetByIdAsync`: Yanlış mesaj düzeltildi

#### 2.5 Empty List Handling

**Sorun:** Boş liste durumunda hata mesajı döndürülüyordu

**Önceki Kod:**
```csharp
if (!entities.Any())
{
    return new ErrorDataResult<List<GetAllStudentDto>>(ConstantsMessages.StudentListEmptyMessage);
    // 400 Bad Request döndürülüyordu
}
```

**Düzeltilmiş Kod:**
```csharp
if (!entities.Any())
{
    return new SuccessDataResult<List<GetAllStudentDto>>(new List<GetAllStudentDto>(), ConstantsMessages.StudentListEmptyMessage);
    // 200 OK ile boş liste döndürülüyor
}
```

**REST API Best Practice:** Boş liste bir hata değil, geçerli bir response'dur. Bu nedenle HTTP 200 OK ile boş array döndürülmelidir.

**Düzeltilen Manager'lar:**
- `StudentManager.GetAllAsync`
- `CourseManager.GetAllAsync`
- `ExamManager.GetAllAsync`
- `ExamResultManager.GetAllAsync`
- `InstructorManager.GetAllAsync`
- `LessonsManager.GetAllAsync`
- `RegistrationManager.GetAllAsync`

---

### 3. Mimari ve Performans Sorunları - 15+ Hata Düzeltildi

#### 3.1 Katman İhlali (Layer Violation)

**Sorun:** Controller'dan direkt DbContext erişimi

**Önceki Kod:**
```csharp
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly AppDbContext _dbContext; // ❌ Controller'da DbContext!
    
    // Controller'dan direkt veritabanı işlemleri yapılıyor
}
```

**Düzeltilmiş Kod:**
```csharp
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    // ✅ DbContext kaldırıldı, tüm işlemler Service layer üzerinden
}
```

**Neden Önemli:**
- Controller sadece HTTP isteklerini yönetmeli, business logic içermemeli
- Veri erişimi Service layer üzerinden yapılmalı
- Test edilebilirlik artar

**Düzeltilen Controller:**
- `StudentsController`: `_dbContext` field'ı ve tüm kullanımları kaldırıldı

#### 3.2 N+1 Query Problemi

**Sorun:** Her kayıt için ayrı veritabanı sorgusu

**Önceki Kod:**
```csharp
public async Task<IActionResult> GetAll()
{
    var exams = await _examService.GetAllAsync();
    foreach (var exam in exams.Data)
    {
        var detail = await _examService.GetByIdAsync(exam.Id); // ❌ N+1 Problem!
    }
}
```

**Düzeltilmiş Kod:**
```csharp
public async Task<IActionResult> GetAll()
{
    var result = await _examService.GetAllAsync(); // ✅ Tek sorgu
    return Ok(result);
}
```

**Neden Önemli:**
- Performans: 100 kayıt için 1 sorgu yerine 101 sorgu çalışıyordu
- Database yükü artıyordu
- Response time uzuyordu

**Düzeltilen Controller:**
- `ExamsController.GetAll()`: Gereksiz `foreach` ve `GetByIdAsync` çağrıları kaldırıldı

#### 3.3 Async/Await Anti-Patterns

**Sorun:** `.Result`, `.Wait()`, `.GetAwaiter().GetResult()` kullanımı

**Önceki Kod:**
```csharp
var result = _unitOfWork.CommitAsync().Result; // ❌ Deadlock riski!
_unitOfWork.CommitAsync().GetAwaiter().GetResult(); // ❌ Blocking!
await _unitOfWork.Exams.CreateAsync(entity).Wait(); // ❌ Yanlış kullanım!
```

**Düzeltilmiş Kod:**
```csharp
await _unitOfWork.CommitAsync(); // ✅ Async/await pattern
await _unitOfWork.Exams.CreateAsync(entity); // ✅ Doğru async kullanımı
```

**Neden Önemli:**
- `.Result` ve `.Wait()` deadlock'a neden olabilir
- Thread pool'u bloklar
- Scalability sorunları yaratır

**Düzeltilen Yerler:**
- `StudentManager`: `.Result` → `await`
- `ExamManager`: `.Wait()` → `await`
- `RegistrationManager`: `.GetAwaiter().GetResult()` → `await`
- `ExamResultManager`: `.GetAwaiter().GetResult()` → `await`

#### 3.4 Memory Leak

**Sorun:** Dispose edilmeyen kaynaklar

**Önceki Kod:**
```csharp
var tempContext = new AppDbContext(...); // ❌ Dispose edilmiyor!
// DbContext kullanılıyor ama dispose edilmiyor
```

**Düzeltilmiş Kod:**
```csharp
// Gereksiz DbContext oluşturma kaldırıldı
// DI container üzerinden yönetilen DbContext kullanılıyor
```

**Neden Önemli:**
- Memory leak'lere neden olur
- Database connection'lar açık kalır
- Uzun süreli çalışmada sistem çökebilir

**Düzeltilen Yerler:**
- `StudentsController`: `tempContext` kaldırıldı

#### 3.5 Thread-Safety Sorunları

**Sorun:** UnitOfWork'te repository'ler thread-safe değildi

**Önceki Kod:**
```csharp
public class UnitOfWork
{
    private IStudentRepository _studentRepository;
    
    public IStudentRepository Students
    {
        get
        {
            if (_studentRepository == null)
                _studentRepository = new StudentRepository(_context); // ❌ Thread-safe değil!
            return _studentRepository;
        }
    }
}
```

**Düzeltilmiş Kod:**
```csharp
public class UnitOfWork
{
    private readonly Lazy<IStudentRepository> _studentRepository;
    
    public IStudentRepository Students => _studentRepository.Value; // ✅ Thread-safe!
    
    public UnitOfWork(AppDbContext context)
    {
        _studentRepository = new Lazy<IStudentRepository>(() => new StudentRepository(context));
    }
}
```

**Neden Önemli:**
- Multi-threaded ortamlarda race condition riski
- Concurrent access sorunları
- Repository'lerin birden fazla kez oluşturulması

**Düzeltilen Yerler:**
- `UnitOfWork`: Tüm repository'ler `Lazy<T>` ile thread-safe hale getirildi

#### 3.6 Transaction Management (InMemory DB için kaldırıldı)

**Sorun:** InMemory Database transaction'ları desteklemiyor

**Önceki Kod:**
```csharp
await using var transaction = await _unitOfWork.BeginTransactionAsync();
try
{
    // İşlemler
    await _unitOfWork.CommitTransactionAsync();
}
catch
{
    await _unitOfWork.RollbackTransactionAsync();
    throw;
}
// ❌ InMemory DB transaction desteklemiyor!
```

**Düzeltilmiş Kod:**
```csharp
try
{
    // İşlemler
    await _unitOfWork.CommitAsync();
}
catch (Exception ex)
{
    return new ErrorResult($"Bir hata oluştu: {ex.Message}");
}
// ✅ InMemory DB için transaction kaldırıldı
```

**Not:** Production'da gerçek veritabanı kullanıldığında transaction'lar geri eklenebilir.

**Düzeltilen Yerler:**
- `ExamManager.CreateAsync`: Transaction kaldırıldı
- `ExamManager.Update`: Transaction kaldırıldı
- `ExamManager.Remove`: Transaction kaldırıldı

---

### 4. Framework ve Paket Güncellemeleri

#### 4.1 .NET 9.0 Upgrade

**Değişiklik:**
- Tüm projeler `.NET 8.0` → `.NET 9.0` güncellendi
- NuGet paketleri `.NET 9.0` uyumlu versiyonlara güncellendi

**Güncellenen Dosyalar:**
- `CourseApp.API.csproj`: `net8.0` → `net9.0`
- `CourseApp.DataAccessLayer.csproj`: `net8.0` → `net9.0`
- `CourseApp.BusinessLayer.csproj`: `net8.0` → `net9.0`
- `CourseApp.EntityLayer.csproj`: `net8.0` → `net9.0`
- `Microsoft.AspNetCore.OpenApi`: `9.0.0`
- `Microsoft.EntityFrameworkCore.InMemory`: `9.0.0`
- `Microsoft.EntityFrameworkCore.Design`: `9.0.0`
- `Microsoft.EntityFrameworkCore.SqlServer`: `9.0.0`
- `Microsoft.EntityFrameworkCore.Tools`: `9.0.0`

#### 4.2 InMemory Database Kullanımı

**Değişiklik:**
- SQL Server bağımlılığı kaldırıldı
- InMemory Database kullanılıyor

**Neden:**
- SQL Server kurulumu gerektirmez
- Hızlı test ve geliştirme
- Docker/container gerektirmez
- Taşınabilirlik

**Not:** Production'da gerçek veritabanına geçiş için sadece `UseInMemoryDatabase` → `UseSqlServer` değişikliği yeterlidir.

---

### 5. Validation ve Güvenlik İyileştirmeleri

#### 5.1 FluentValidation Entegrasyonu

**Eklendi:** Tüm DTO'lar için FluentValidation validators

**Validator'lar:**
- `CreateStudentDtoValidator`
- `UpdateStudentDtoValidator`
- `CreateCourseDtoValidator`
- `UpdateCourseDtoValidator`
- `CreateExamDtoValidator`
- `UpdateExamDtoValidator`
- `CreatedInstructorDtoValidator`
- `UpdatedInstructorDtoValidator`
- `CreateRegistrationDtoValidator`
- `UpdatedRegistrationDtoValidator`
- `CreateLessonDtoValidator`
- `UpdateLessonDtoValidator`
- `CreateExamResultDtoValidator`
- `UpdateExamResultDtoValidator`

**Örnek Validator:**
```csharp
public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ad boş olamaz.")
            .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");
        
        RuleFor(x => x.TC)
            .NotEmpty().WithMessage("TC Kimlik boş olamaz.")
            .Length(11).WithMessage("TC Kimlik 11 karakter olmalıdır.");
        
        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Today).WithMessage("Doğum tarihi bugünden önce olmalıdır.");
    }
}
```

**Faydaları:**
- Otomatik validation
- Tutarlı hata mesajları
- Controller'da tekrar eden validation kodları kaldırıldı
- Client-side validation desteği

#### 5.2 Global Exception Handler

**Eklendi:** `GlobalExceptionHandlerMiddleware`

**Özellikler:**
- Tüm exception'ları yakalar
- Exception tipine göre özel mesajlar
- Development'ta stack trace, Production'da sadece mesaj
- HTTP status code mapping
- Structured logging

**Desteklenen Exception Tipleri:**
- `ValidationException` → 400 Bad Request
- `ArgumentNullException` → 400 Bad Request
- `ArgumentException` → 400 Bad Request
- `KeyNotFoundException` → 404 Not Found
- `UnauthorizedAccessException` → 401 Unauthorized
- `DbUpdateException` → 500 Internal Server Error
- `DbUpdateConcurrencyException` → 409 Conflict
- `NotImplementedException` → 501 Not Implemented
- `TimeoutException` → 408 Request Timeout
- Generic Exception → 500 Internal Server Error

**Kod:**
```csharp
private async Task HandleExceptionAsync(HttpContext context, Exception exception)
{
    context.Response.ContentType = "application/json";
    
    var (statusCode, message, errors) = GetExceptionDetails(exception);
    
    context.Response.StatusCode = (int)statusCode;
    
    var response = new
    {
        isSuccess = false,
        message = message,
        errors = errors,
        stackTrace = _environment.IsDevelopment() ? exception.StackTrace : null
    };
    
    await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
}
```

#### 5.3 Request Logging Middleware

**Eklendi:** `RequestLoggingMiddleware`

**Özellikler:**
- Her HTTP isteği loglanır
- Request method, path, query string loglanır
- Response time ölçülür
- Status code loglanır

**Kod:**
```csharp
public async Task InvokeAsync(HttpContext context)
{
    var stopwatch = Stopwatch.StartNew();
    var method = context.Request.Method;
    var path = context.Request.Path;
    
    await _next(context);
    
    stopwatch.Stop();
    var statusCode = context.Response.StatusCode;
    var elapsedMs = stopwatch.ElapsedMilliseconds;
    
    _logger.LogInformation(
        "{Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
        method, path, statusCode, elapsedMs);
}
```

---

### 6. Logging ve Monitoring

#### 6.1 Structured Logging (Serilog)

**Eklendi:** Serilog ile structured logging

**Özellikler:**
- JSON formatında loglar
- Console ve dosyaya yazma
- Günlük log dosyaları (`logs/courseapp-YYYYMMDD.log`)
- 7 gün saklama
- Enrichment: MachineName, ThreadId, LogContext

**Konfigürasyon:**
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/courseapp-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]
  }
}
```

**Faydaları:**
- Log analizi kolaylaşır
- Production'da sorun tespiti hızlanır
- ELK Stack, Seq gibi log aggregation tool'ları ile entegrasyon

#### 6.2 Health Checks

**Eklendi:** `/health` endpoint'i

**Özellikler:**
- Database bağlantı kontrolü
- Sistem durumu raporlama
- Load balancer'lar için hazırlık kontrolü

**Kod:**
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(
        name: "database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "sql", "inmemory" });
```

**Kullanım:**
```bash
GET /health
```

**Response:**
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456",
  "entries": {
    "database": {
      "status": "Healthy",
      "duration": "00:00:00.0012345",
      "tags": ["db", "sql", "inmemory"]
    }
  }
}
```

---

### 7. Güvenlik İyileştirmeleri

#### 7.1 API Rate Limiting

**Eklendi:** AspNetCoreRateLimit ile IP bazlı rate limiting

**Limitler:**
- Genel: 100 istek/dakika
- POST: 20 istek/dakika
- PUT: 20 istek/dakika
- DELETE: 10 istek/dakika

**Konfigürasyon:**
```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      },
      {
        "Endpoint": "POST:*",
        "Period": "1m",
        "Limit": 20
      }
    ]
  }
}
```

**Faydaları:**
- DDoS koruması
- API abuse önleme
- Adil kullanım garantisi
- Sistem kaynaklarını koruma

**Response (Limit Aşıldığında):**
```json
{
  "statusCode": 429,
  "message": "API rate limit exceeded"
}
```

#### 7.2 Response Compression

**Eklendi:** Gzip ve Brotli compression

**Özellikler:**
- HTTPS için aktif
- Optimal compression level
- %50-70 network trafiği azalması

**Kod:**
```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
```

**Faydaları:**
- Daha hızlı veri transferi
- Mobil kullanıcılar için daha iyi deneyim
- Bandwidth tasarrufu

---

### 8. UI İyileştirmeleri

#### 8.1 Custom Web UI

**Eklendi:** Modern, responsive web arayüzü (`wwwroot/index.html`)

**Özellikler:**
- **7 API Endpoint Butonu**: Her entity için GET endpoint'leri
- **7 Test Butonu**: Her entity için örnek veri oluşturma
- **JSON/Info Cards View**: API response'ları iki farklı formatta görüntüleme
- **Interactive Background**: Mouse tracking ile gradient animasyon
- **Responsive Design**: Mobil, tablet, desktop uyumlu
- **Premium Dark Theme**: Modern, şık görünüm

**UI Bileşenleri:**
1. **Header**: Logo (mezuniyet kepi), CourseApp başlığı, slogan
2. **API Butonları**: Courses, ExamResults, Exams, Instructors, Lessons, Registrations, Students
3. **Test Butonları**: Her entity için örnek veri oluşturma butonları
4. **View Toggle**: JSON ve Info Cards arasında geçiş
5. **Response Area**: API response'larını gösteren alan

#### 8.2 Test Butonları ve Cooldown Mekanizması

**Özellikler:**
- Her entity için bağımlılıkları otomatik oluşturma
- 5 saniye cooldown (çift tıklama önleme)
- Duplicate kayıt önleme
- Promise-based deduplication
- Client-side duplicate click prevention

**Test Data Generation:**
- **Students**: Rastgele TC, doğum tarihi
- **Instructors**: Unique email, telefon
- **Courses**: Eğitmen bağımlılığı, tarih aralıkları
- **Exams**: Gelecek tarih
- **Lessons**: Kurs bağımlılığı, içerik
- **Registrations**: Öğrenci ve kurs bağımlılığı, para birimi desteği, geçmiş tarih
- **ExamResults**: Sınav ve öğrenci bağımlılığı

#### 8.3 Para Birimi Desteği

**Eklendi:** Currency enum ve para birimi desteği

**Desteklenen Para Birimleri:**
- TRY (₺) - Türk Lirası
- USD ($) - Amerikan Doları
- EUR (€) - Euro

**Özellikler:**
- Registration entity'sine Currency alanı eklendi
- Test verilerinde rastgele para birimi seçimi
- UI'da para birimi sembolü ile gösterim
- Intl.NumberFormat ile formatlanmış fiyat gösterimi

---

### 9. Unit Testing

**Eklendi:** xUnit, Moq, FluentAssertions ile unit testler

**Test Coverage:**
- `StudentManagerTests`
- `CourseManagerTests`
- `ExamManagerTests`
- `InstructorManagerTests`
- `LessonsManagerTests`
- `RegistrationManagerTests`
- `ExamResultManagerTests`

**Test Senaryoları:**
- `GetAllAsync`: Success, empty list
- `GetByIdAsync`: Success, null ID, not found
- `CreateAsync`: Success, null entity, commit failure
- `Update`: Success, null entity
- `Remove`: Success, null entity

**Test Örneği:**
```csharp
[Fact]
public async Task GetAllAsync_ShouldReturnSuccessDataResult_WhenStudentsExist()
{
    // Arrange
    var students = new List<Student> { /* test data */ };
    var mockQueryable = students.AsQueryable().BuildMock();
    
    // Act
    var result = await _studentManager.GetAllAsync();
    
    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Data.Should().HaveCount(students.Count);
}
```

---

## 🔧 Teknik Detaylar

### 10. AutoMapper Konfigürasyonu

**Mapping Profiles:**
- `StudentMapping`
- `CourseMapping`
- `ExamMapping`
- `InstructorMapping`
- `LessonMapping`
- `RegistrationMapping`
- `ExamResultMapping`

**Örnek Mapping:**
```csharp
public class StudentMapping : Profile
{
    public StudentMapping()
    {
        CreateMap<Student, GetAllStudentDto>().ReverseMap();
        CreateMap<Student, GetByIdStudentDto>().ReverseMap();
        CreateMap<Student, CreateStudentDto>().ReverseMap();
        CreateMap<Student, UpdateStudentDto>().ReverseMap();
        CreateMap<Student, DeleteStudentDto>().ReverseMap();
    }
}
```

**Eksik Mapping'ler Eklendi:**
- `CreatedInstructorDto` → `Instructor`
- `UpdatedInstructorDto` → `Instructor`
- `Exam` → `GetByIdExamDto`
- `UpdateExamDto` → `Exam`

### 11. Unit of Work Pattern

**Özellikler:**
- Tüm repository'ler tek bir UnitOfWork üzerinden yönetilir
- Transaction yönetimi (InMemory DB için kaldırıldı)
- Thread-safe lazy initialization
- Dispose pattern ile kaynak yönetimi

**Repository'ler:**
- `IStudentRepository`
- `ICourseRepository`
- `IExamRepository`
- `IExamResultRepository`
- `IInstructorRepository`
- `ILessonRepository`
- `IRegistrationRepository`

### 12. Repository Pattern

**Generic Repository:**
- `IGenericRepository<T>` interface
- `GenericRepository<T>` base implementation
- Ortak CRUD işlemleri: GetAll, GetById, Create, Update, Delete

**Specific Repositories:**
- Her entity için özel repository (gerekirse custom metodlar için)

### 13. Result Pattern

**Result Types:**
- `IResult`: Başarı/hata durumu
- `IDataResult<T>`: Veri ile birlikte result
- `SuccessResult`: Başarılı işlem
- `ErrorResult`: Hatalı işlem
- `SuccessDataResult<T>`: Başarılı işlem + veri
- `ErrorDataResult<T>`: Hatalı işlem + veri

**Kullanım:**
```csharp
public async Task<IDataResult<List<GetAllStudentDto>>> GetAllAsync()
{
    var students = await _unitOfWork.Students.GetAll(false);
    if (!students.Any())
    {
        return new SuccessDataResult<List<GetAllStudentDto>>(
            new List<GetAllStudentDto>(), 
            ConstantsMessages.StudentListEmptyMessage);
    }
    
    var studentDtos = _mapper.Map<List<GetAllStudentDto>>(students);
    return new SuccessDataResult<List<GetAllStudentDto>>(
        studentDtos, 
        ConstantsMessages.StudentListSuccessMessage);
}
```

---

## 📦 Kurulum ve Çalıştırma

### Gereksinimler

- **.NET 9.0 SDK** veya üzeri
  - Windows: [.NET 9.0 SDK İndir](https://dotnet.microsoft.com/download/dotnet/9.0)
  - macOS: `brew install dotnet` veya [Resmi İndirme Sayfası](https://dotnet.microsoft.com/download/dotnet/9.0)
  - Linux: [Linux Kurulum Rehberi](https://learn.microsoft.com/en-us/dotnet/core/install/linux)
- **Git** (Repository'yi klonlamak için)
- **IDE** (Opsiyonel): Visual Studio 2022, VS Code, Rider, vb.

### Adım Adım Kurulum

#### 1. Repository'yi Klonlayın

```bash
# GitHub repository'sini klonlayın
git clone https://github.com/uygaryildirim/CourseAppHackathon.git

# Proje dizinine gidin
cd CourseAppHackathon/CourseApp
```

#### 2. .NET SDK Versiyonunu Kontrol Edin

```bash
# .NET SDK versiyonunu kontrol edin (9.0.0 veya üzeri olmalı)
dotnet --version

# Eğer 9.0.0'dan düşükse, .NET 9.0 SDK'yı yükleyin
```

#### 3. Paketleri Geri Yükleyin

**Not:** Bu bir **.NET (C#) projesi**dir, Python değil. .NET'te `requirements.txt` dosyası yok, bunun yerine `.csproj` dosyalarında paket bağımlılıkları tanımlanır.

```bash
# Tüm NuGet paketlerini geri yükleyin
# dotnet restore komutu .csproj dosyalarındaki paketleri otomatik olarak indirir
dotnet restore

# Veya sadece API projesi için
cd CourseApp.API
dotnet restore
```

**Paket Yönetimi:**
- ✅ **.NET'te:** Paketler `.csproj` dosyalarında `<PackageReference>` olarak tanımlanır
- ❌ **Python'da:** `requirements.txt` dosyası kullanılır
- 🔄 **Otomatik:** `dotnet restore` komutu tüm `.csproj` dosyalarını tarar ve paketleri indirir

#### 4. Projeyi Derleyin (Opsiyonel)

```bash
# Tüm projeyi derleyin
dotnet build

# Veya sadece API projesi için
cd CourseApp.API
dotnet build
```

#### 5. Projeyi Çalıştırın

```bash
# API projesi dizinine gidin
cd CourseApp.API

# Projeyi çalıştırın
dotnet run
```

**Alternatif (Tek Komut):**
```bash
# Proje root'undan direkt çalıştırma
dotnet run --project CourseApp.API/CourseApp.API.csproj
```

#### 6. Uygulamaya Erişin

Proje başarıyla çalıştıktan sonra terminal'de şu mesajı göreceksiniz:
```
Now listening on: http://localhost:5232
Now listening on: https://localhost:7120
```

**Erişim URL'leri:**

| Servis | HTTP URL | HTTPS URL |
|--------|----------|-----------|
| **Web UI** | [http://localhost:5232](http://localhost:5232) | [https://localhost:7120](https://localhost:7120) |
| **Swagger UI** | [http://localhost:5232/swagger](http://localhost:5232/swagger) | [https://localhost:7120/swagger](https://localhost:7120/swagger) |
| **Health Check** | [http://localhost:5232/health](http://localhost:5232/health) | [https://localhost:7120/health](https://localhost:7120/health) |
| **API Base** | `http://localhost:5232/api` | `https://localhost:7120/api` |

### Uygulamayı Durdurma

**Windows/macOS/Linux:**
```bash
# Terminal'de Ctrl + C tuşlarına basın
```

**macOS/Linux (Alternatif):**
```bash
# Process'i bulup durdur
pkill -f "CourseApp.API"

# Veya process ID ile
ps aux | grep CourseApp.API
kill <PID>
```

### Sorun Giderme

#### Port Zaten Kullanılıyor Hatası

Eğer port zaten kullanılıyorsa, `launchSettings.json` dosyasındaki port numaralarını değiştirebilirsiniz:

```json
{
  "applicationUrl": "http://localhost:5000;https://localhost:5001"
}
```

#### .NET SDK Bulunamadı

```bash
# .NET SDK'nın yüklü olup olmadığını kontrol edin
dotnet --version

# Yüklü değilse, yukarıdaki gereksinimler bölümündeki linklerden yükleyin
```

#### Paket Geri Yükleme Hatası

```bash
# NuGet cache'i temizleyin
dotnet nuget locals all --clear

# Tekrar restore edin
dotnet restore
```

#### Build Hatası

```bash
# Clean build yapın
dotnet clean
dotnet build
```

---

## 🚀 Kullanım

### API Endpoint'leri

**Base URL:** `https://localhost:5001/api` veya `http://localhost:5000/api`

#### Students
- `GET /api/Students` - Tüm öğrencileri listele
- `GET /api/Students/{id}` - Öğrenci detayı
- `POST /api/Students` - Yeni öğrenci oluştur
- `PUT /api/Students` - Öğrenci güncelle
- `DELETE /api/Students` - Öğrenci sil

#### Courses
- `GET /api/Courses` - Tüm kursları listele
- `GET /api/Courses/{id}` - Kurs detayı
- `GET /api/Courses/detail` - Tüm kurs detayları (eğitmen bilgisi ile)
- `POST /api/Courses` - Yeni kurs oluştur
- `PUT /api/Courses` - Kurs güncelle
- `DELETE /api/Courses` - Kurs sil

#### Instructors
- `GET /api/Instructors` - Tüm eğitmenleri listele
- `GET /api/Instructors/{id}` - Eğitmen detayı
- `POST /api/Instructors` - Yeni eğitmen oluştur
- `PUT /api/Instructors` - Eğitmen güncelle
- `DELETE /api/Instructors` - Eğitmen sil

#### Exams
- `GET /api/Exams` - Tüm sınavları listele
- `GET /api/Exams/{id}` - Sınav detayı
- `POST /api/Exams` - Yeni sınav oluştur
- `PUT /api/Exams` - Sınav güncelle
- `DELETE /api/Exams` - Sınav sil

#### Lessons
- `GET /api/Lessons` - Tüm dersleri listele
- `GET /api/Lessons/{id}` - Ders detayı
- `GET /api/Lessons/detail` - Tüm ders detayları
- `POST /api/Lessons` - Yeni ders oluştur
- `PUT /api/Lessons` - Ders güncelle
- `DELETE /api/Lessons` - Ders sil

#### Registrations
- `GET /api/Registrations` - Tüm kayıtları listele
- `GET /api/Registrations/{id}` - Kayıt detayı
- `GET /api/Registrations/detail` - Tüm kayıt detayları
- `POST /api/Registrations` - Yeni kayıt oluştur
- `PUT /api/Registrations` - Kayıt güncelle
- `DELETE /api/Registrations` - Kayıt sil

#### ExamResults
- `GET /api/ExamResults` - Tüm sınav sonuçlarını listele
- `GET /api/ExamResults/{id}` - Sınav sonucu detayı
- `GET /api/ExamResults/detail` - Tüm sınav sonuç detayları
- `POST /api/ExamResults` - Yeni sınav sonucu oluştur
- `PUT /api/ExamResults` - Sınav sonucu güncelle
- `DELETE /api/ExamResults` - Sınav sonucu sil

### Web UI Kullanımı

1. Tarayıcıda `http://localhost:5000` adresini açın
2. **API Endpoint Butonları** ile verileri görüntüleyin
3. **Test Butonları** ile örnek veriler oluşturun
4. **JSON/Info Cards** toggle butonu ile görünümü değiştirin

---

## 🔒 Güvenlik Önlemleri

### 1. Rate Limiting
- IP bazlı istek sınırlama
- DDoS koruması
- Endpoint bazlı farklı limitler

### 2. Input Validation
- FluentValidation ile otomatik validation
- Tüm DTO'lar için validator'lar
- Client-side ve server-side validation

### 3. Global Exception Handling
- Hassas bilgilerin log'larda gizlenmesi
- Production'da stack trace gizleme
- Tutarlı hata mesajları

### 4. HTTPS Redirection
- HTTP istekleri otomatik HTTPS'e yönlendirilir
- Güvenli veri transferi

### 5. CORS Configuration
- Şu anda tüm origin'lere açık (development için)
- Production'da spesifik origin'ler belirlenebilir

---

## ⚡ Performans İyileştirmeleri

### 1. Response Compression
- Gzip/Brotli sıkıştırma
- %50-70 network trafiği azalması

### 2. Async/Await Pattern
- Tüm veritabanı işlemleri async
- Thread blocking önlendi
- Scalability arttı

### 3. N+1 Query Önleme
- Gereksiz foreach döngüleri kaldırıldı
- Tek sorgu ile tüm veriler çekiliyor

### 4. Thread-Safe Repository Initialization
- Lazy<T> ile thread-safe initialization
- Race condition önlendi

---

## 📊 Test Coverage

### Unit Tests
- 7 Manager sınıfı için testler
- Success, error, null, empty senaryoları
- Mock kullanımı ile izole testler

### Test Çalıştırma
```bash
cd CourseApp.Tests
dotnet test
```

---

## 📝 Log Dosyaları

Log dosyaları `logs/` klasöründe saklanır:
- Format: `courseapp-YYYYMMDD.log`
- Saklama süresi: 7 gün
- Format: JSON structured logs

---

## 📞 İletişim ve Katkı

### 🔗 Repository

**GitHub:** [https://github.com/uygaryildirim/CourseAppHackathon](https://github.com/uygaryildirim/CourseAppHackathon)

### 🤝 Katkıda Bulunma

1. Repository'yi fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request açın

---

## 📄 Lisans

Bu proje hackathon amaçlı geliştirilmiştir.

---

**Son Güncelleme:** 2025-11-05  
**Versiyon:** 1.0.0  
**.NET Version:** 9.0  
**Repository:** [https://github.com/uygaryildirim/CourseAppHackathon](https://github.com/uygaryildirim/CourseAppHackathon)
