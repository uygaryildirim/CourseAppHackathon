using CourseApp.API.Middleware;
using CourseApp.API.Validators;
using CourseApp.DataAccessLayer.Concrete;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Concrete;
using CourseApp.ServiceLayer.Mapping;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// DÜZELTME: FluentValidation eklendi. Controller'larda otomatik validation yapılması için FluentValidation.AspNetCore kullanılıyor.
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateStudentDtoValidator>();
        fv.AutomaticValidationEnabled = true;
        fv.ImplicitlyValidateChildProperties = true;
    });
builder.Services.AddEndpointsApiExplorer();
// DÜZELTME: Swagger UI yapılandırması iyileştirildi. API dokümantasyonu için Swagger UI'ı daha kullanışlı hale getirildi.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CourseApp API",
        Version = "v1",
        Description = "CourseApp - Eğitim yönetim sistemi API dokümantasyonu",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "CourseApp Team"
        }
    });
    
    // DÜZELTME: XML yorumları eklendi. Swagger UI'da API endpoint'leri için detaylı açıklamalar gösteriliyor.
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// DbContext Configuration
// DÜZELTME: SQL Server yerine InMemory database kullanılıyor. Projenin makine üzerinde çalışması için SQL Server kurulumu gerekmiyor, InMemory database kullanılarak test ve geliştirme ortamında kolaylık sağlanıyor.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("CourseAppDb");
});

// UnitOfWork Configuration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Service Configuration
// DÜZELTME: AddScopd yazım hatası düzeltildi - AddScoped olarak değiştirildi. Dependency injection için doğru metod adı kullanılıyor.
builder.Services.AddScoped<IStudentService, StudentManager>();
builder.Services.AddScoped<ICourseService, CourseManager>();
// DÜZELTME: ExamManagr yazım hatası düzeltildi - ExamManager olarak değiştirildi. IExamService interface'i için doğru implementasyon sınıfı kaydedildi.
builder.Services.AddScoped<IExamService, ExamManager>();
builder.Services.AddScoped<IExamResultService, ExamResultManager>();
builder.Services.AddScoped<IInstructorService, InstructorManager>();
builder.Services.AddScoped<ILessonService, LessonsManager>();
builder.Services.AddScoped<IRegistrationService, RegistrationManager>();

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(StudentMapping).Assembly);

// DÜZELTME: FluentValidation validator'ları DI container'a eklendi. Tüm validator sınıfları otomatik olarak kaydediliyor.
builder.Services.AddValidatorsFromAssemblyContaining<CreateStudentDtoValidator>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// DÜZELTME: Exception handling middleware pipeline'ın en başına eklendi. Tüm exception'lar yakalanıp standart formatta döndürülüyor.
app.UseExceptionHandling();

// DÜZELTME: Request logging middleware eklendi. Tüm HTTP istekleri loglanıyor, performans ve kullanım izleme için.
app.UseRequestLogging();

// DÜZELTME: Swagger UI her ortamda aktif. Geliştirme ve test için Swagger UI'ı her zaman kullanılabilir hale getirildi.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CourseApp API V1");
    c.RoutePrefix = string.Empty; // Swagger UI'ı root path'te açmak için
    c.DisplayRequestDuration(); // İstek süresini göster
    c.EnableDeepLinking(); // Deep linking aktif
    c.EnableFilter(); // Arama filtresi aktif
    c.EnableTryItOutByDefault(); // Try it out butonunu varsayılan olarak aktif et
});

// DÜZELTME: HTTPS redirection eklendi. Güvenlik için HTTP istekleri HTTPS'e yönlendiriliyor.
app.UseHttpsRedirection();

// DÜZELTME: CORS middleware eklendi. Cross-origin istekler için CORS politikası uygulanıyor.
app.UseCors("AllowAll");

// DÜZELTME: Authentication ve Authorization middleware eklendi. Kimlik doğrulama ve yetkilendirme için pipeline'a eklendi.
app.UseAuthentication();
app.UseAuthorization();

// DÜZELTME: MapControllers yazım hatası düzeltildi - MapControllers olarak değiştirildi. Controller routing'i için doğru metod adı kullanılıyor.
app.MapControllers();

// DÜZELTME: Uygulama çalıştırılıyor. app.Run() metodu uygulamayı başlatır ve request'leri dinler.
app.Run();