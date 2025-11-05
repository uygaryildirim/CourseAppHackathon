using AutoMapper;
using CourseApp.DataAccessLayer.Abstract;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.LessonDto;
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
/// LessonsManager servis katmanı için unit testler. 
/// Bu testler, LessonsManager'ın tüm metodlarının doğru çalıştığını doğrulamak için yazıldı.
/// Mock kullanarak veritabanı bağımlılığından izole edilmiş testler yazılıyor.
/// </summary>
public class LessonsManagerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILessonRepository> _mockLessonRepository;
    private readonly LessonsManager _lessonsManager;

    /// <summary>
    /// Test constructor'ı. Her test öncesi mock nesneleri ve LessonsManager instance'ı oluşturuluyor.
    /// Mock'lar sayesinde gerçek veritabanı kullanılmadan testler çalıştırılabiliyor.
    /// </summary>
    public LessonsManagerTests()
    {
        // DÜZELTME: Mock nesneleri oluşturuluyor. Unit test'lerde gerçek veritabanı yerine mock kullanılıyor, böylece testler hızlı ve izole çalışıyor.
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockLessonRepository = new Mock<ILessonRepository>();
        
        // DÜZELTME: UnitOfWork'in Lessons property'si mock lesson repository döndürüyor. Dependency injection benzeri bir yapı oluşturuluyor.
        _mockUnitOfWork.Setup(u => u.Lessons).Returns(_mockLessonRepository.Object);
        
        // DÜZELTME: LessonsManager instance'ı oluşturuluyor. Mock'lar constructor'a enjekte ediliyor.
        _lessonsManager = new LessonsManager(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    #region GetAllAsync Tests

    /// <summary>
    /// GetAllAsync metodunun başarılı durumda ders listesi döndürmesini test eder.
    /// Mock veritabanından ders listesi döndürülüyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenLessonsExist_ReturnsSuccessDataResult()
    {
        // Arrange - Test verileri hazırlanıyor
        // DÜZELTME: Test verisi oluşturuluyor. Mock veritabanından dönecek ders listesi hazırlanıyor.
        var lessons = new List<Lesson>
        {
            new Lesson { ID = "1", Title = "Ders 1", Date = new DateTime(2024, 1, 1), Duration = 60, CourseID = "1", Content = "İçerik 1" },
            new Lesson { ID = "2", Title = "Ders 2", Date = new DateTime(2024, 1, 2), Duration = 90, CourseID = "1", Content = "İçerik 2" }
        };

        var lessonDtos = new List<GetAllLessonDto>
        {
            new GetAllLessonDto { Id = "1", Title = "Ders 1", Date = new DateTime(2024, 1, 1), Duration = 60, CourseID = "1", Content = "İçerik 1" },
            new GetAllLessonDto { Id = "2", Title = "Ders 2", Date = new DateTime(2024, 1, 2), Duration = 90, CourseID = "1", Content = "İçerik 2" }
        };

        // DÜZELTME: Mock repository'nin GetAll metodu için setup yapılıyor. MockQueryable.Moq kullanılarak IQueryable mock'u oluşturuluyor, ToListAsync ile liste elde edilecek.
        var mockQueryable = lessons.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockLessonRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);
        
        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. Lesson listesi GetAllLessonDto listesine map ediliyor.
        _mockMapper.Setup(m => m.Map<IEnumerable<GetAllLessonDto>>(It.IsAny<IEnumerable<Lesson>>()))
            .Returns(lessonDtos);

        // Act - Test edilecek metod çağrılıyor
        var result = await _lessonsManager.GetAllAsync();

        // Assert - Sonuçlar doğrulanıyor
        // DÜZELTME: FluentAssertions kullanılarak test sonuçları doğrulanıyor. Başarılı sonuç ve doğru veri döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<IEnumerable<GetAllLessonDto>>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.LessonListSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    /// <summary>
    /// GetAllAsync metodunun ders listesi boş olduğunda hata mesajı döndürmesini test eder.
    /// Boş liste durumunda kullanıcıya bilgilendirici mesaj gösteriliyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenNoLessonsExist_ReturnsErrorDataResult()
    {
        // Arrange - Boş liste durumu hazırlanıyor
        // DÜZELTME: Boş ders listesi oluşturuluyor. MockQueryable.Moq kullanılarak boş liste mock'u oluşturuluyor, Mock repository boş liste döndürüyor.
        var emptyLessons = new List<Lesson>();
        var mockQueryable = emptyLessons.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockLessonRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);
        
        // DÜZELTME: Mock mapper boş liste döndürüyor. Liste boş olduğunda hata mesajı döndürülmesi bekleniyor.
        _mockMapper.Setup(m => m.Map<IEnumerable<GetAllLessonDto>>(It.IsAny<IEnumerable<Lesson>>()))
            .Returns(new List<GetAllLessonDto>());

        // Act - Test edilecek metod çağrılıyor
        var result = await _lessonsManager.GetAllAsync();

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Boş liste durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<IEnumerable<GetAllLessonDto>>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ConstantsMessages.LessonListEmptyMessage);
    }

    #endregion

    #region GetByIdAsync Tests

    /// <summary>
    /// GetByIdAsync metodunun geçerli ID ile ders bulduğunda başarılı sonuç döndürmesini test eder.
    /// Mock veritabanından ID'ye göre ders bulunuyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenLessonExists_ReturnsSuccessDataResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test dersi oluşturuluyor. Mock repository ID'ye göre ders döndürüyor.
        var lessonId = "1";
        var lesson = new Lesson 
        { 
            ID = lessonId, 
            Title = "Ders 1", 
            Date = new DateTime(2024, 1, 1), 
            Duration = 60, 
            CourseID = "1", 
            Content = "İçerik 1" 
        };

        var lessonDto = new GetByIdLessonDto 
        { 
            Id = lessonId, 
            Title = "Ders 1", 
            Date = new DateTime(2024, 1, 1), 
            Duration = 60, 
            CourseID = "1", 
            Content = "İçerik 1" 
        };

        // DÜZELTME: Mock repository'nin GetByIdAsync metodu için setup yapılıyor. Belirli ID için ders döndürüyor.
        _mockLessonRepository.Setup(r => r.GetByIdAsync(lessonId, It.IsAny<bool>()))
            .ReturnsAsync(lesson);
        
        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. Lesson GetByIdLessonDto'ya map ediliyor.
        _mockMapper.Setup(m => m.Map<GetByIdLessonDto>(It.IsAny<Lesson>()))
            .Returns(lessonDto);

        // Act - Test edilecek metod çağrılıyor
        var result = await _lessonsManager.GetByIdAsync(lessonId);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessDataResult döndüğü ve doğru ders bilgilerinin döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<GetByIdLessonDto>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.LessonGetByIdSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(lessonId);
        result.Data.Title.Should().Be("Ders 1");
    }

    /// <summary>
    /// GetByIdAsync metodunun null veya boş ID ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null/empty ID durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenIdIsNullOrEmpty_ReturnsErrorDataResult()
    {
        // Arrange - Null ID durumu hazırlanıyor
        // DÜZELTME: Null ID test ediliyor. Null veya empty ID durumunda hata mesajı döndürülmesi bekleniyor.

        // Act - Test edilecek metod çağrılıyor
        var result = await _lessonsManager.GetByIdAsync(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null ID durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdLessonDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("ID parametresi boş olamaz");
    }

    /// <summary>
    /// GetByIdAsync metodunun var olmayan ID ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Ders bulunamadığında kullanıcıya bilgilendirici mesaj gösteriliyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenLessonNotFound_ReturnsErrorDataResult()
    {
        // Arrange - Var olmayan ID durumu hazırlanıyor
        // DÜZELTME: Var olmayan ID test ediliyor. Mock repository null döndürüyor, ders bulunamadı durumu simüle ediliyor.
        var nonExistentId = "999";
        _mockLessonRepository.Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<bool>()))
            .ReturnsAsync((Lesson)null);

        // Act - Test edilecek metod çağrılıyor
        var result = await _lessonsManager.GetByIdAsync(nonExistentId);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Ders bulunamadığında ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdLessonDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("ders bulunamadı");
    }

    #endregion

    #region CreateAsync Tests

    /// <summary>
    /// CreateAsync metodunun geçerli ders bilgileri ile başarılı oluşturma yapmasını test eder.
    /// Mock veritabanına ders ekleniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenValidLesson_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test dersi DTO'su oluşturuluyor. Mock veritabanına eklenecek ders bilgileri hazırlanıyor.
        var createLessonDto = new CreateLessonDto
        {
            Title = "Ders 1",
            Date = new DateTime(2024, 1, 1),
            Duration = 60,
            CourseID = "1",
            Content = "İçerik 1"
        };

        var lesson = new Lesson
        {
            ID = "1",
            Title = "Ders 1",
            Date = new DateTime(2024, 1, 1),
            Duration = 60,
            CourseID = "1",
            Content = "İçerik 1"
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. CreateLessonDto Lesson entity'sine map ediliyor.
        _mockMapper.Setup(m => m.Map<Lesson>(It.IsAny<CreateLessonDto>()))
            .Returns(lesson);
        
        // DÜZELTME: Mock repository'nin CreateAsync metodu için setup yapılıyor. Ders ekleniyor.
        _mockLessonRepository.Setup(r => r.CreateAsync(It.IsAny<Lesson>()))
            .Returns(Task.CompletedTask);
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (result > 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _lessonsManager.CreateAsync(createLessonDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.LessonCreateSuccessMessage);
    }

    /// <summary>
    /// CreateAsync metodunun null entity ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null entity durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenEntityIsNull_ReturnsErrorResult()
    {
        // Arrange - Null entity durumu hazırlanıyor
        // DÜZELTME: Null entity test ediliyor. Null entity durumunda hata mesajı döndürülmesi bekleniyor.

        // Act - Test edilecek metod çağrılıyor
        var result = await _lessonsManager.CreateAsync(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Ders bilgileri boş olamaz");
    }

    /// <summary>
    /// CreateAsync metodunun commit başarısız olduğunda hata mesajı döndürmesini test eder.
    /// Veritabanı commit başarısız olduğunda kullanıcıya bilgilendirici mesaj gösteriliyor.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenCommitFails_ReturnsErrorResult()
    {
        // Arrange - Commit başarısız durumu hazırlanıyor
        // DÜZELTME: Commit başarısız test ediliyor. Mock UnitOfWork CommitAsync 0 döndürüyor, başarısız commit durumu simüle ediliyor.
        var createLessonDto = new CreateLessonDto
        {
            Title = "Ders 1",
            Date = new DateTime(2024, 1, 1),
            Duration = 60,
            CourseID = "1",
            Content = "İçerik 1"
        };

        var lesson = new Lesson
        {
            ID = "1",
            Title = "Ders 1",
            Date = new DateTime(2024, 1, 1),
            Duration = 60,
            CourseID = "1",
            Content = "İçerik 1"
        };

        _mockMapper.Setup(m => m.Map<Lesson>(It.IsAny<CreateLessonDto>()))
            .Returns(lesson);
        
        _mockLessonRepository.Setup(r => r.CreateAsync(It.IsAny<Lesson>()))
            .Returns(Task.CompletedTask);
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarısız commit (result = 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(0);

        // Act - Test edilecek metod çağrılıyor
        var result = await _lessonsManager.CreateAsync(createLessonDto);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Commit başarısız olduğunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ConstantsMessages.LessonCreateFailedMessage);
    }

    #endregion

    #region Update Tests

    /// <summary>
    /// Update metodunun geçerli ders bilgileri ile başarılı güncelleme yapmasını test eder.
    /// Mock veritabanında ders güncelleniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task Update_WhenValidLesson_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test dersi DTO'su oluşturuluyor. Mock veritabanında güncellenecek ders bilgileri hazırlanıyor.
        var updateLessonDto = new UpdateLessonDto
        {
            Id = "1",
            Title = "Ders 1 Güncellendi",
            Date = new DateTime(2024, 1, 1),
            Duration = 90,
            CourseID = "1",
            Content = "Güncellenmiş İçerik"
        };

        var lesson = new Lesson
        {
            ID = "1",
            Title = "Ders 1 Güncellendi",
            Date = new DateTime(2024, 1, 1),
            Duration = 90,
            CourseID = "1",
            Content = "Güncellenmiş İçerik"
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. UpdateLessonDto Lesson entity'sine map ediliyor.
        _mockMapper.Setup(m => m.Map<Lesson>(It.IsAny<UpdateLessonDto>()))
            .Returns(lesson);
        
        // DÜZELTME: Mock repository'nin Update metodu için setup yapılıyor. Ders güncelleniyor.
        _mockLessonRepository.Setup(r => r.Update(It.IsAny<Lesson>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (result > 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _lessonsManager.Update(updateLessonDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.LessonUpdateSuccessMessage);
    }

    /// <summary>
    /// Update metodunun null entity ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null entity durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task Update_WhenEntityIsNull_ReturnsErrorResult()
    {
        // Arrange - Null entity durumu hazırlanıyor
        // DÜZELTME: Null entity test ediliyor. Null entity durumunda hata mesajı döndürülmesi bekleniyor.

        // Act - Test edilecek metod çağrılıyor
        var result = await _lessonsManager.Update(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Güncellenecek ders bilgileri boş olamaz");
    }

    #endregion

    #region Remove Tests

    /// <summary>
    /// Remove metodunun geçerli ders bilgileri ile başarılı silme yapmasını test eder.
    /// Mock veritabanından ders siliniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task Remove_WhenValidLesson_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test dersi DTO'su oluşturuluyor. Mock veritabanından silinecek ders bilgileri hazırlanıyor.
        var deleteLessonDto = new DeleteLessonDto
        {
            Id = "1"
        };

        var lesson = new Lesson
        {
            ID = "1",
            Title = "Ders 1",
            Date = new DateTime(2024, 1, 1),
            Duration = 60,
            CourseID = "1",
            Content = "İçerik 1"
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. DeleteLessonDto Lesson entity'sine map ediliyor.
        _mockMapper.Setup(m => m.Map<Lesson>(It.IsAny<DeleteLessonDto>()))
            .Returns(lesson);
        
        // DÜZELTME: Mock repository'nin Remove metodu için setup yapılıyor. Ders siliniyor.
        _mockLessonRepository.Setup(r => r.Remove(It.IsAny<Lesson>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (result > 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _lessonsManager.Remove(deleteLessonDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.LessonDeleteSuccessMessage);
    }

    /// <summary>
    /// Remove metodunun null entity ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null entity durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task Remove_WhenEntityIsNull_ReturnsErrorResult()
    {
        // Arrange - Null entity durumu hazırlanıyor
        // DÜZELTME: Null entity test ediliyor. Null entity durumunda hata mesajı döndürülmesi bekleniyor.

        // Act - Test edilecek metod çağrılıyor
        var result = await _lessonsManager.Remove(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Silinecek ders bilgileri boş olamaz");
    }

    #endregion
}

