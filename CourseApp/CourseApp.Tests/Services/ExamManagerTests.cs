using AutoMapper;
using CourseApp.DataAccessLayer.Abstract;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.ExamDto;
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
/// ExamManager servis katmanı için unit testler. 
/// Bu testler, ExamManager'ın tüm metodlarının doğru çalıştığını doğrulamak için yazıldı.
/// Mock kullanarak veritabanı bağımlılığından izole edilmiş testler yazılıyor.
/// </summary>
public class ExamManagerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IExamRepository> _mockExamRepository;
    private readonly ExamManager _examManager;

    /// <summary>
    /// Test constructor'ı. Her test öncesi mock nesneleri ve ExamManager instance'ı oluşturuluyor.
    /// Mock'lar sayesinde gerçek veritabanı kullanılmadan testler çalıştırılabiliyor.
    /// </summary>
    public ExamManagerTests()
    {
        // DÜZELTME: Mock nesneleri oluşturuluyor. Unit test'lerde gerçek veritabanı yerine mock kullanılıyor, böylece testler hızlı ve izole çalışıyor.
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockExamRepository = new Mock<IExamRepository>();
        
        // DÜZELTME: UnitOfWork'in Exams property'si mock exam repository döndürüyor. Dependency injection benzeri bir yapı oluşturuluyor.
        _mockUnitOfWork.Setup(u => u.Exams).Returns(_mockExamRepository.Object);
        
        // DÜZELTME: ExamManager instance'ı oluşturuluyor. Mock'lar constructor'a enjekte ediliyor.
        _examManager = new ExamManager(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    #region GetAllAsync Tests

    /// <summary>
    /// GetAllAsync metodunun başarılı durumda sınav listesi döndürmesini test eder.
    /// Mock veritabanından sınav listesi döndürülüyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenExamsExist_ReturnsSuccessDataResult()
    {
        // Arrange - Test verileri hazırlanıyor
        // DÜZELTME: Test verisi oluşturuluyor. Mock veritabanından dönecek sınav listesi hazırlanıyor.
        var exams = new List<Exam>
        {
            new Exam { ID = "1", Name = "Midterm Exam", Date = new DateTime(2024, 3, 15) },
            new Exam { ID = "2", Name = "Final Exam", Date = new DateTime(2024, 6, 20) }
        };

        var examDtos = new List<GetAllExamDto>
        {
            new GetAllExamDto { Id = "1", Name = "Midterm Exam", Date = new DateTime(2024, 3, 15) },
            new GetAllExamDto { Id = "2", Name = "Final Exam", Date = new DateTime(2024, 6, 20) }
        };

        // DÜZELTME: Mock repository'nin GetAll metodu için setup yapılıyor. MockQueryable.Moq kullanılarak IQueryable mock'u oluşturuluyor, ToListAsync ile liste elde edilecek.
        var mockQueryable = exams.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockExamRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);
        
        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. Exam listesi GetAllExamDto listesine map ediliyor.
        _mockMapper.Setup(m => m.Map<IEnumerable<GetAllExamDto>>(It.IsAny<IEnumerable<Exam>>()))
            .Returns(examDtos);

        // Act - Test edilecek metod çağrılıyor
        var result = await _examManager.GetAllAsync();

        // Assert - Sonuçlar doğrulanıyor
        // DÜZELTME: FluentAssertions kullanılarak test sonuçları doğrulanıyor. Başarılı sonuç ve doğru veri döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<IEnumerable<GetAllExamDto>>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.ExamListSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    /// <summary>
    /// GetAllAsync metodunun sınav listesi boş olduğunda hata mesajı döndürmesini test eder.
    /// Boş liste durumunda kullanıcıya bilgilendirici mesaj gösteriliyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenNoExamsExist_ReturnsErrorDataResult()
    {
        // Arrange - Boş liste durumu hazırlanıyor
        // DÜZELTME: Boş sınav listesi oluşturuluyor. MockQueryable.Moq kullanılarak boş liste mock'u oluşturuluyor, Mock repository boş liste döndürüyor.
        var emptyExams = new List<Exam>();
        var mockQueryable = emptyExams.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockExamRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);
        
        // DÜZELTME: Mock mapper boş liste döndürüyor. Liste boş olduğunda hata mesajı döndürülmesi bekleniyor.
        _mockMapper.Setup(m => m.Map<IEnumerable<GetAllExamDto>>(It.IsAny<IEnumerable<Exam>>()))
            .Returns(new List<GetAllExamDto>());

        // Act - Test edilecek metod çağrılıyor
        var result = await _examManager.GetAllAsync();

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Boş liste durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<IEnumerable<GetAllExamDto>>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ConstantsMessages.ExamListEmptyMessage);
    }

    #endregion

    #region GetByIdAsync Tests

    /// <summary>
    /// GetByIdAsync metodunun geçerli ID ile sınav bulduğunda başarılı sonuç döndürmesini test eder.
    /// Mock veritabanından ID'ye göre sınav bulunuyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenExamExists_ReturnsSuccessDataResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test sınavı oluşturuluyor. Mock repository ID'ye göre sınav döndürüyor.
        var examId = "1";
        var exam = new Exam 
        { 
            ID = examId, 
            Name = "Midterm Exam", 
            Date = new DateTime(2024, 3, 15) 
        };

        var examDto = new GetByIdExamDto 
        { 
            Id = examId, 
            Name = "Midterm Exam", 
            Date = new DateTime(2024, 3, 15) 
        };

        // DÜZELTME: Mock repository'nin GetByIdAsync metodu için setup yapılıyor. Belirli ID için sınav döndürüyor.
        _mockExamRepository.Setup(r => r.GetByIdAsync(examId, It.IsAny<bool>()))
            .ReturnsAsync(exam);
        
        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. Exam GetByIdExamDto'ya map ediliyor.
        _mockMapper.Setup(m => m.Map<GetByIdExamDto>(It.IsAny<Exam>()))
            .Returns(examDto);

        // Act - Test edilecek metod çağrılıyor
        var result = await _examManager.GetByIdAsync(examId);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessDataResult döndüğü ve doğru sınav bilgilerinin döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<GetByIdExamDto>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.ExamGetByIdSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(examId);
        result.Data.Name.Should().Be("Midterm Exam");
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
        var result = await _examManager.GetByIdAsync(nullId!);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null ID durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdExamDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("ID parametresi boş olamaz.");
    }

    /// <summary>
    /// GetByIdAsync metodunun sınav bulunamadığında hata mesajı döndürmesini test eder.
    /// Mock veritabanından sınav bulunamadığında null döndürülüyor ve hata mesajı bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenExamNotFound_ReturnsErrorDataResult()
    {
        // Arrange - Sınav bulunamama durumu hazırlanıyor
        // DÜZELTME: Mock repository null döndürüyor. Sınav bulunamadığında null check'in doğru çalıştığı kontrol ediliyor.
        var examId = "999";
        _mockExamRepository.Setup(r => r.GetByIdAsync(examId, It.IsAny<bool>()))
            .ReturnsAsync((Exam?)null);

        // Act - Test edilecek metod çağrılıyor
        var result = await _examManager.GetByIdAsync(examId);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Sınav bulunamadığında ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdExamDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Belirtilen ID'ye sahip sınav bulunamadı.");
    }

    #endregion

    #region CreateAsync Tests

    /// <summary>
    /// CreateAsync metodunun geçerli sınav bilgileri ile başarılı oluşturma yapmasını test eder.
    /// Mock veritabanına sınav ekleniyor ve başarılı sonuç bekleniyor. Transaction kullanılıyor.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenValidExam_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test sınavı oluşturuluyor. Mock repository ve mapper için setup yapılıyor.
        var createExamDto = new CreateExamDto
        {
            Name = "Midterm Exam",
            Date = new DateTime(2024, 3, 15)
        };

        var exam = new Exam
        {
            Name = "Midterm Exam",
            Date = new DateTime(2024, 3, 15)
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. CreateExamDto Exam'e map ediliyor.
        _mockMapper.Setup(m => m.Map<Exam>(It.IsAny<CreateExamDto>()))
            .Returns(exam);
        
        // DÜZELTME: Mock repository'nin CreateAsync metodu için setup yapılıyor. Sınav ekleniyor.
        _mockExamRepository.Setup(r => r.CreateAsync(It.IsAny<Exam>()))
            .Returns(Task.CompletedTask);
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync ve transaction metodları için setup yapılıyor. Başarılı commit (1 row affected) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync())
            .ReturnsAsync(1);
        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync())
            .ReturnsAsync(Mock.Of<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>());
        _mockUnitOfWork.Setup(u => u.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act - Test edilecek metod çağrılıyor
        var result = await _examManager.CreateAsync(createExamDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor. CreateAsync ve CommitAsync metodlarının çağrıldığı doğrulanıyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.ExamCreateSuccessMessage);
        
        // DÜZELTME: Mock metodların çağrıldığı doğrulanıyor. Veritabanı işlemlerinin doğru sırada yapıldığı kontrol ediliyor.
        _mockExamRepository.Verify(r => r.CreateAsync(It.IsAny<Exam>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    /// <summary>
    /// CreateAsync metodunun null sınav bilgileri ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null entity durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenEntityIsNull_ReturnsErrorResult()
    {
        // Arrange - Null entity durumu hazırlanıyor
        // DÜZELTME: Null CreateExamDto ile test yapılıyor. Null check'in doğru çalıştığı kontrol ediliyor.
        CreateExamDto? nullDto = null;

        // Act - Test edilecek metod çağrılıyor
        var result = await _examManager.CreateAsync(nullDto!);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Sınav bilgileri boş olamaz.");
        
        // DÜZELTME: Mock metodların çağrılmadığı doğrulanıyor. Null check sonrası erken return yapılıyor.
        _mockExamRepository.Verify(r => r.CreateAsync(It.IsAny<Exam>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    #endregion

    #region Remove Tests

    /// <summary>
    /// Remove metodunun geçerli sınav bilgileri ile başarılı silme yapmasını test eder.
    /// Mock veritabanından sınav siliniyor ve başarılı sonuç bekleniyor. Transaction kullanılıyor.
    /// </summary>
    [Fact]
    public async Task Remove_WhenValidExam_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test sınavı silme verisi oluşturuluyor. Mock repository ve mapper için setup yapılıyor.
        var deleteExamDto = new DeleteExamDto
        {
            Id = "1",
            Name = "Midterm Exam",
            Date = new DateTime(2024, 3, 15)
        };

        var exam = new Exam
        {
            ID = "1",
            Name = "Midterm Exam",
            Date = new DateTime(2024, 3, 15)
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. DeleteExamDto Exam'e map ediliyor.
        _mockMapper.Setup(m => m.Map<Exam>(It.IsAny<DeleteExamDto>()))
            .Returns(exam);
        
        // DÜZELTME: Mock repository'nin Remove metodu için setup yapılıyor. Sınav siliniyor.
        _mockExamRepository.Setup(r => r.Remove(It.IsAny<Exam>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync ve transaction metodları için setup yapılıyor. Başarılı commit (1 row affected) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync())
            .ReturnsAsync(1);
        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync())
            .ReturnsAsync(Mock.Of<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>());
        _mockUnitOfWork.Setup(u => u.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act - Test edilecek metod çağrılıyor
        var result = await _examManager.Remove(deleteExamDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor. Remove ve CommitAsync metodlarının çağrıldığı doğrulanıyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.ExamDeleteSuccessMessage);
        
        // DÜZELTME: Mock metodların çağrıldığı doğrulanıyor. Veritabanı işlemlerinin doğru sırada yapıldığı kontrol ediliyor.
        _mockExamRepository.Verify(r => r.Remove(It.IsAny<Exam>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    /// <summary>
    /// Remove metodunun null sınav bilgileri ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null entity durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task Remove_WhenEntityIsNull_ReturnsErrorResult()
    {
        // Arrange - Null entity durumu hazırlanıyor
        // DÜZELTME: Null DeleteExamDto ile test yapılıyor. Null check'in doğru çalıştığı kontrol ediliyor.
        DeleteExamDto? nullDto = null;

        // Act - Test edilecek metod çağrılıyor
        var result = await _examManager.Remove(nullDto!);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Silinecek sınav bilgileri boş olamaz.");
        
        // DÜZELTME: Mock metodların çağrılmadığı doğrulanıyor. Null check sonrası erken return yapılıyor.
        _mockExamRepository.Verify(r => r.Remove(It.IsAny<Exam>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    #endregion

    #region Update Tests

    /// <summary>
    /// Update metodunun geçerli sınav bilgileri ile başarılı güncelleme yapmasını test eder.
    /// Mock veritabanında sınav güncelleniyor ve başarılı sonuç bekleniyor. Transaction kullanılıyor.
    /// </summary>
    [Fact]
    public async Task Update_WhenValidExam_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test sınavı güncelleme verisi oluşturuluyor. Mock repository ve mapper için setup yapılıyor.
        var updateExamDto = new UpdateExamDto
        {
            Id = "1",
            Name = "Updated Midterm Exam",
            Date = new DateTime(2024, 3, 20)
        };

        var exam = new Exam
        {
            ID = "1",
            Name = "Updated Midterm Exam",
            Date = new DateTime(2024, 3, 20)
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. UpdateExamDto Exam'e map ediliyor.
        _mockMapper.Setup(m => m.Map<Exam>(It.IsAny<UpdateExamDto>()))
            .Returns(exam);
        
        // DÜZELTME: Mock repository'nin Update metodu için setup yapılıyor. Sınav güncelleniyor.
        _mockExamRepository.Setup(r => r.Update(It.IsAny<Exam>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync ve transaction metodları için setup yapılıyor. Başarılı commit (1 row affected) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync())
            .ReturnsAsync(1);
        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync())
            .ReturnsAsync(Mock.Of<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>());
        _mockUnitOfWork.Setup(u => u.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act - Test edilecek metod çağrılıyor
        var result = await _examManager.Update(updateExamDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor. Update ve CommitAsync metodlarının çağrıldığı doğrulanıyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.ExamUpdateSuccessMessage);
        
        // DÜZELTME: Mock metodların çağrıldığı doğrulanıyor. Veritabanı işlemlerinin doğru sırada yapıldığı kontrol ediliyor.
        _mockExamRepository.Verify(r => r.Update(It.IsAny<Exam>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    /// <summary>
    /// Update metodunun null sınav bilgileri ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null entity durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task Update_WhenEntityIsNull_ReturnsErrorResult()
    {
        // Arrange - Null entity durumu hazırlanıyor
        // DÜZELTME: Null UpdateExamDto ile test yapılıyor. Null check'in doğru çalıştığı kontrol ediliyor.
        UpdateExamDto? nullDto = null;

        // Act - Test edilecek metod çağrılıyor
        var result = await _examManager.Update(nullDto!);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Güncellenecek sınav bilgileri boş olamaz.");
        
        // DÜZELTME: Mock metodların çağrılmadığı doğrulanıyor. Null check sonrası erken return yapılıyor.
        _mockExamRepository.Verify(r => r.Update(It.IsAny<Exam>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    #endregion
}

