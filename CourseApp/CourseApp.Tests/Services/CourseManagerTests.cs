using CourseApp.DataAccessLayer.Abstract;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.CourseDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Concrete;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace CourseApp.Tests.Services;

/// <summary>
/// CourseManager servis katmanı için unit testler. 
/// Bu testler, CourseManager'ın tüm metodlarının doğru çalıştığını doğrulamak için yazıldı.
/// Mock kullanarak veritabanı bağımlılığından izole edilmiş testler yazılıyor.
/// </summary>
public class CourseManagerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly CourseManager _courseManager;

    /// <summary>
    /// Test constructor'ı. Her test öncesi mock nesneleri ve CourseManager instance'ı oluşturuluyor.
    /// Mock'lar sayesinde gerçek veritabanı kullanılmadan testler çalıştırılabiliyor.
    /// </summary>
    public CourseManagerTests()
    {
        // DÜZELTME: Mock nesneleri oluşturuluyor. Unit test'lerde gerçek veritabanı yerine mock kullanılıyor, böylece testler hızlı ve izole çalışıyor.
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        
        // DÜZELTME: UnitOfWork'in Courses property'si mock course repository döndürüyor. Dependency injection benzeri bir yapı oluşturuluyor.
        _mockUnitOfWork.Setup(u => u.Courses).Returns(_mockCourseRepository.Object);
        
        // DÜZELTME: CourseManager instance'ı oluşturuluyor. Mock'lar constructor'a enjekte ediliyor.
        _courseManager = new CourseManager(_mockUnitOfWork.Object);
    }

    #region GetAllAsync Tests

    /// <summary>
    /// GetAllAsync metodunun başarılı durumda kurs listesi döndürmesini test eder.
    /// Mock veritabanından kurs listesi döndürülüyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenCoursesExist_ReturnsSuccessDataResult()
    {
        // Arrange - Test verileri hazırlanıyor
        // DÜZELTME: Test verisi oluşturuluyor. Mock veritabanından dönecek kurs listesi hazırlanıyor.
        var courses = new List<Course>
        {
            new Course 
            { 
                ID = "1", 
                CourseName = "C# Programming", 
                StartDate = new DateTime(2024, 1, 1),
                EndDate = new DateTime(2024, 6, 30),
                InstructorID = "instructor1",
                IsActive = true,
                CreatedDate = DateTime.Now
            },
            new Course 
            { 
                ID = "2", 
                CourseName = "ASP.NET Core", 
                StartDate = new DateTime(2024, 2, 1),
                EndDate = new DateTime(2024, 7, 30),
                InstructorID = "instructor2",
                IsActive = true,
                CreatedDate = DateTime.Now
            }
        };

        // DÜZELTME: Mock repository'nin GetAll metodu için setup yapılıyor. MockQueryable.Moq kullanılarak IQueryable mock'u oluşturuluyor, ToListAsync ile liste elde edilecek.
        var mockQueryable = courses.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockCourseRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);

        // Act - Test edilecek metod çağrılıyor
        var result = await _courseManager.GetAllAsync();

        // Assert - Sonuçlar doğrulanıyor
        // DÜZELTME: FluentAssertions kullanılarak test sonuçları doğrulanıyor. Başarılı sonuç ve doğru veri döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<IEnumerable<GetAllCourseDto>>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.CourseListSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    /// <summary>
    /// GetAllAsync metodunun kurs listesi boş olduğunda hata mesajı döndürmesini test eder.
    /// Boş liste durumunda kullanıcıya bilgilendirici mesaj gösteriliyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenNoCoursesExist_ReturnsErrorDataResult()
    {
        // Arrange - Boş liste durumu hazırlanıyor
        // DÜZELTME: Boş kurs listesi oluşturuluyor. MockQueryable.Moq kullanılarak boş liste mock'u oluşturuluyor, Mock repository boş liste döndürüyor.
        var emptyCourses = new List<Course>();
        var mockQueryable = emptyCourses.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockCourseRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);

        // Act - Test edilecek metod çağrılıyor
        var result = await _courseManager.GetAllAsync();

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Boş liste durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<IEnumerable<GetAllCourseDto>>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ConstantsMessages.CourseListEmptyMessage);
    }

    #endregion

    #region GetByIdAsync Tests

    /// <summary>
    /// GetByIdAsync metodunun geçerli ID ile kurs bulduğunda başarılı sonuç döndürmesini test eder.
    /// Mock veritabanından ID'ye göre kurs bulunuyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenCourseExists_ReturnsSuccessDataResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test kursu oluşturuluyor. Mock repository ID'ye göre kurs döndürüyor.
        var courseId = "1";
        var course = new Course 
        { 
            ID = courseId, 
            CourseName = "C# Programming",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 6, 30),
            InstructorID = "instructor1",
            IsActive = true,
            CreatedDate = DateTime.Now
        };

        // DÜZELTME: Mock repository'nin GetByIdAsync metodu için setup yapılıyor. Belirli ID için kurs döndürüyor.
        _mockCourseRepository.Setup(r => r.GetByIdAsync(courseId, It.IsAny<bool>()))
            .ReturnsAsync(course);

        // Act - Test edilecek metod çağrılıyor
        var result = await _courseManager.GetByIdAsync(courseId);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessDataResult döndüğü ve doğru kurs bilgilerinin döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<GetByIdCourseDto>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.CourseGetByIdSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(courseId);
        result.Data.CourseName.Should().Be("C# Programming");
    }

    /// <summary>
    /// GetByIdAsync metodunun null veya boş ID ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null/empty ID durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenIdIsNullOrEmpty_ReturnsErrorDataResult()
    {
        // Arrange - Boş ID durumu hazırlanıyor
        // DÜZELTME: Null ID ile test yapılıyor. Null check'in doğru çalıştığı kontrol ediliyor.
        string? nullId = null;

        // Act - Test edilecek metod çağrılıyor
        var result = await _courseManager.GetByIdAsync(nullId!);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null ID durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdCourseDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("ID parametresi boş olamaz.");
    }

    /// <summary>
    /// GetByIdAsync metodunun kurs bulunamadığında hata mesajı döndürmesini test eder.
    /// Mock veritabanından kurs bulunamadığında null döndürülüyor ve hata mesajı bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenCourseNotFound_ReturnsErrorDataResult()
    {
        // Arrange - Kurs bulunamama durumu hazırlanıyor
        // DÜZELTME: Mock repository null döndürüyor. Kurs bulunamadığında null check'in doğru çalıştığı kontrol ediliyor.
        var courseId = "999";
        _mockCourseRepository.Setup(r => r.GetByIdAsync(courseId, It.IsAny<bool>()))
            .ReturnsAsync((Course?)null);

        // Act - Test edilecek metod çağrılıyor
        var result = await _courseManager.GetByIdAsync(courseId);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Kurs bulunamadığında ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdCourseDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Belirtilen ID'ye sahip kurs bulunamadı.");
    }

    #endregion

    #region CreateAsync Tests

    /// <summary>
    /// CreateAsync metodunun geçerli kurs bilgileri ile başarılı oluşturma yapmasını test eder.
    /// Mock veritabanına kurs ekleniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenValidCourse_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test kursu oluşturuluyor. Mock repository ve mapper için setup yapılıyor.
        var createCourseDto = new CreateCourseDto
        {
            CourseName = "C# Programming",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 6, 30),
            InstructorID = "instructor1",
            IsActive = true,
            CreatedDate = DateTime.Now
        };

        // DÜZELTME: Mock repository'nin CreateAsync metodu için setup yapılıyor. Kurs ekleniyor.
        _mockCourseRepository.Setup(r => r.CreateAsync(It.IsAny<Course>()))
            .Returns(Task.CompletedTask);
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (1 row affected) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync())
            .ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _courseManager.CreateAsync(createCourseDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor. CreateAsync ve CommitAsync metodlarının çağrıldığı doğrulanıyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.CourseCreateSuccessMessage);
        
        // DÜZELTME: Mock metodların çağrıldığı doğrulanıyor. Veritabanı işlemlerinin doğru sırada yapıldığı kontrol ediliyor.
        _mockCourseRepository.Verify(r => r.CreateAsync(It.IsAny<Course>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    /// <summary>
    /// CreateAsync metodunun null kurs bilgileri ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null entity durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenEntityIsNull_ReturnsErrorResult()
    {
        // Arrange - Null entity durumu hazırlanıyor
        // DÜZELTME: Null CreateCourseDto ile test yapılıyor. Null check'in doğru çalıştığı kontrol ediliyor.
        CreateCourseDto? nullDto = null;

        // Act - Test edilecek metod çağrılıyor
        var result = await _courseManager.CreateAsync(nullDto!);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Kurs bilgileri boş olamaz.");
        
        // DÜZELTME: Mock metodların çağrılmadığı doğrulanıyor. Null check sonrası erken return yapılıyor.
        _mockCourseRepository.Verify(r => r.CreateAsync(It.IsAny<Course>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    /// <summary>
    /// CreateAsync metodunun commit başarısız olduğunda hata mesajı döndürmesini test eder.
    /// Veritabanı commit işlemi başarısız olduğunda (0 row affected) hata mesajı döndürülüyor.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenCommitFails_ReturnsErrorResult()
    {
        // Arrange - Commit başarısız durumu hazırlanıyor
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu 0 döndürüyor. Commit başarısız olduğunda hata mesajı döndürülmesi bekleniyor.
        var createCourseDto = new CreateCourseDto
        {
            CourseName = "C# Programming",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 6, 30),
            InstructorID = "instructor1",
            IsActive = true,
            CreatedDate = DateTime.Now
        };

        _mockCourseRepository.Setup(r => r.CreateAsync(It.IsAny<Course>()))
            .Returns(Task.CompletedTask);
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu 0 döndürüyor. Commit başarısız olduğunda hata mesajı döndürülmesi bekleniyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync())
            .ReturnsAsync(0);

        // Act - Test edilecek metod çağrılıyor
        var result = await _courseManager.CreateAsync(createCourseDto);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Commit başarısız olduğunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ConstantsMessages.CourseCreateFailedMessage);
    }

    #endregion

    #region Update Tests

    /// <summary>
    /// Update metodunun geçerli kurs bilgileri ile başarılı güncelleme yapmasını test eder.
    /// Mock veritabanında kurs güncelleniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task Update_WhenValidCourse_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test kursu güncelleme verisi oluşturuluyor. Mock repository ve mapper için setup yapılıyor.
        var updateCourseDto = new UpdateCourseDto
        {
            Id = "1",
            CourseName = "C# Advanced Programming",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 6, 30),
            InstructorID = "instructor1",
            IsActive = true,
            CreatedDate = DateTime.Now
        };

        var existingCourse = new Course
        {
            ID = "1",
            CourseName = "C# Programming",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 6, 30),
            InstructorID = "instructor1",
            IsActive = true,
            CreatedDate = DateTime.Now
        };

        // DÜZELTME: Mock repository'nin GetByIdAsync ve Update metodları için setup yapılıyor. Kurs güncelleniyor.
        _mockCourseRepository.Setup(r => r.GetByIdAsync(updateCourseDto.Id, It.IsAny<bool>()))
            .ReturnsAsync(existingCourse);
        _mockCourseRepository.Setup(r => r.Update(It.IsAny<Course>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (1 row affected) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync())
            .ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _courseManager.Update(updateCourseDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor. Update ve CommitAsync metodlarının çağrıldığı doğrulanıyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.CourseUpdateSuccessMessage);
        
        // DÜZELTME: Mock metodların çağrıldığı doğrulanıyor. Veritabanı işlemlerinin doğru sırada yapıldığı kontrol ediliyor.
        _mockCourseRepository.Verify(r => r.GetByIdAsync(updateCourseDto.Id, It.IsAny<bool>()), Times.Once);
        _mockCourseRepository.Verify(r => r.Update(It.IsAny<Course>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    /// <summary>
    /// Update metodunun null kurs bilgileri ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null entity durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task Update_WhenEntityIsNull_ReturnsErrorResult()
    {
        // Arrange - Null entity durumu hazırlanıyor
        // DÜZELTME: Null UpdateCourseDto ile test yapılıyor. Null check'in doğru çalıştığı kontrol ediliyor.
        UpdateCourseDto? nullDto = null;

        // Act - Test edilecek metod çağrılıyor
        var result = await _courseManager.Update(nullDto!);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Güncellenecek kurs bilgileri boş olamaz.");
        
        // DÜZELTME: Mock metodların çağrılmadığı doğrulanıyor. Null check sonrası erken return yapılıyor.
        _mockCourseRepository.Verify(r => r.Update(It.IsAny<Course>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    #endregion

    #region Remove Tests

    /// <summary>
    /// Remove metodunun geçerli kurs bilgileri ile başarılı silme yapmasını test eder.
    /// Mock veritabanından kurs siliniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task Remove_WhenValidCourse_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test kursu silme verisi oluşturuluyor. Mock repository ve mapper için setup yapılıyor.
        var deleteCourseDto = new DeleteCourseDto
        {
            Id = "1",
            CourseName = "C# Programming",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 6, 30),
            InstructorID = "instructor1",
            IsActive = true,
            CreatedDate = DateTime.Now
        };

        // DÜZELTME: Mock repository'nin Remove metodu için setup yapılıyor. Kurs siliniyor.
        _mockCourseRepository.Setup(r => r.Remove(It.IsAny<Course>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (1 row affected) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync())
            .ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _courseManager.Remove(deleteCourseDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor. Remove ve CommitAsync metodlarının çağrıldığı doğrulanıyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.CourseDeleteSuccessMessage);
        
        // DÜZELTME: Mock metodların çağrıldığı doğrulanıyor. Veritabanı işlemlerinin doğru sırada yapıldığı kontrol ediliyor.
        _mockCourseRepository.Verify(r => r.Remove(It.IsAny<Course>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    /// <summary>
    /// Remove metodunun null kurs bilgileri ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null entity durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task Remove_WhenEntityIsNull_ReturnsErrorResult()
    {
        // Arrange - Null entity durumu hazırlanıyor
        // DÜZELTME: Null DeleteCourseDto ile test yapılıyor. Null check'in doğru çalıştığı kontrol ediliyor.
        DeleteCourseDto? nullDto = null;

        // Act - Test edilecek metod çağrılıyor
        var result = await _courseManager.Remove(nullDto!);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Silinecek kurs bilgileri boş olamaz.");
        
        // DÜZELTME: Mock metodların çağrılmadığı doğrulanıyor. Null check sonrası erken return yapılıyor.
        _mockCourseRepository.Verify(r => r.Remove(It.IsAny<Course>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    #endregion
}

