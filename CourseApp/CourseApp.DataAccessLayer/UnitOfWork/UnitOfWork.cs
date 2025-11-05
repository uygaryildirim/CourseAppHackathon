using CourseApp.DataAccessLayer.Abstract;
using CourseApp.DataAccessLayer.Concrete;
using Microsoft.EntityFrameworkCore.Storage;

namespace CourseApp.DataAccessLayer.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    // DÜZELTME: Thread-safe repository initialization. Lazy<T> kullanılarak thread-safe lazy initialization sağlanıyor, multi-threaded ortamda birden fazla instance oluşturulması önleniyor.
    private readonly Lazy<IStudentRepository> _studentRepository;
    private readonly Lazy<ILessonRepository> _lessonRepository;
    private readonly Lazy<ICourseRepository> _courseRepository;
    private readonly Lazy<IRegistrationRepository> _registrationRepository;
    private readonly Lazy<IExamRepository> _examRepository;
    private readonly Lazy<IExamResultRepository> _examResultRepository;
    private readonly Lazy<IInstructorRepository> _instructorRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        // DÜZELTME: Thread-safe lazy initialization. Her repository için Lazy<T> kullanılarak thread-safe initialization sağlanıyor.
        _studentRepository = new Lazy<IStudentRepository>(() => new StudentRepository(_context));
        _lessonRepository = new Lazy<ILessonRepository>(() => new LessonRepository(_context));
        _courseRepository = new Lazy<ICourseRepository>(() => new CourseRepository(_context));
        _registrationRepository = new Lazy<IRegistrationRepository>(() => new RegistrationRepository(_context));
        _examRepository = new Lazy<IExamRepository>(() => new ExamRepository(_context));
        _examResultRepository = new Lazy<IExamResultRepository>(() => new ExamResultRepository(_context));
        _instructorRepository = new Lazy<IInstructorRepository>(() => new InstructorRepository(_context));
    }

    // DÜZELTME: Thread-safe property access. Lazy<T>.Value kullanılarak thread-safe erişim sağlanıyor.
    public IStudentRepository Students => _studentRepository.Value;

    public ILessonRepository Lessons => _lessonRepository.Value;

    public ICourseRepository Courses => _courseRepository.Value;

    public IExamRepository Exams => _examRepository.Value;

    public IExamResultRepository ExamResults => _examResultRepository.Value;

    public IInstructorRepository Instructors => _instructorRepository.Value;

    public IRegistrationRepository Registrations => _registrationRepository.Value;

    // DÜZELTME: CommitAsync metodu düzgün implement edildi. SaveChangesAsync kullanılarak tüm değişiklikler tek seferde commit ediliyor.
    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    // DÜZELTME: Transaction yönetimi eklendi. BeginTransactionAsync metodu ile transaction başlatılıyor, birden fazla işlem atomik olarak yürütülebiliyor.
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    // DÜZELTME: Transaction commit metodu eklendi. Tüm değişiklikler başarılı olduğunda transaction commit ediliyor.
    public async Task CommitTransactionAsync()
    {
        var transaction = _context.Database.CurrentTransaction;
        if (transaction != null)
        {
            await transaction.CommitAsync();
        }
    }

    // DÜZELTME: Transaction rollback metodu eklendi. Hata durumunda tüm değişiklikler geri alınıyor, veri tutarlılığı sağlanıyor.
    public async Task RollbackTransactionAsync()
    {
        var transaction = _context.Database.CurrentTransaction;
        if (transaction != null)
        {
            await transaction.RollbackAsync();
        }
    }

    // DÜZELTME: DisposeAsync metodu düzgün implement edildi. DbContext dispose edilerek kaynak sızıntısı önleniyor.
    public async ValueTask DisposeAsync()
    {
        // DÜZELTME: Dispose öncesi aktif transaction varsa rollback ediliyor, kaynak sızıntısı önleniyor.
        var transaction = _context.Database.CurrentTransaction;
        if (transaction != null)
        {
            await transaction.RollbackAsync();
        }
        
        await _context.DisposeAsync();
    }
    
    // DÜZELTME: Gereksiz metod kaldırıldı. AccessMissingRepository metodu kaldırıldı, kullanılmayan ve hata üreten kod temizlendi.
}
