using CourseApp.DataAccessLayer.Concrete;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Concrete;
using CourseApp.ServiceLayer.Mapping;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

// DÜZELTME: MapContrllers yazım hatası düzeltildi - MapControllers olarak değiştirildi. Controller routing'i için doğru metod adı kullanılıyor.
app.MapControllers();

// ZOR: Memory leak - app Dispose edilmiyor ama burada normal (app.Run() son satır)
app.Run();
// KOLAY: Noktalı virgül eksikliği yok - burada sorun yok ama ekstra bir satır var