using AutoMapper;
using CourseApp.DataAccessLayer.Abstract;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.ExamResultDto;
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
/// ExamResultManager servis katmanı için unit testler. 
/// Bu testler, ExamResultManager'ın tüm metodlarının doğru çalıştığını doğrulamak için yazıldı.
/// Mock kullanarak veritabanı bağımlılığından izole edilmiş testler yazılıyor.
/// </summary>
public class ExamResultManagerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IExamResultRepository> _mockExamResultRepository;
    private readonly ExamResultManager _examResultManager;

    /// <summary>
    /// Test constructor'ı. Her test öncesi mock nesneleri ve ExamResultManager instance'ı oluşturuluyor.
    /// Mock'lar sayesinde gerçek veritabanı kullanılmadan testler çalıştırılabiliyor.
    /// </summary>
    public ExamResultManagerTests()
    {
        // DÜZELTME: Mock nesneleri oluşturuluyor. Unit test'lerde gerçek veritabanı yerine mock kullanılıyor, böylece testler hızlı ve izole çalışıyor.
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockExamResultRepository = new Mock<IExamResultRepository>();
        
        // DÜZELTME: UnitOfWork'in ExamResults property'si mock exam result repository döndürüyor. Dependency injection benzeri bir yapı oluşturuluyor.
        _mockUnitOfWork.Setup(u => u.ExamResults).Returns(_mockExamResultRepository.Object);
        
        // DÜZELTME: ExamResultManager instance'ı oluşturuluyor. Mock'lar constructor'a enjekte ediliyor.
        _examResultManager = new ExamResultManager(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    #region GetAllAsync Tests

    /// <summary>
    /// GetAllAsync metodunun başarılı durumda sınav sonucu listesi döndürmesini test eder.
    /// Mock veritabanından sınav sonucu listesi döndürülüyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenExamResultsExist_ReturnsSuccessDataResult()
    {
        // Arrange - Test verileri hazırlanıyor
        // DÜZELTME: Test verisi oluşturuluyor. Mock veritabanından dönecek sınav sonucu listesi hazırlanıyor.
        var examResults = new List<ExamResult>
        {
            new ExamResult { ID = "1", Grade = 85, ExamID = "1", StudentID = "1" },
            new ExamResult { ID = "2", Grade = 90, ExamID = "1", StudentID = "2" }
        };

        var examResultDtos = new List<GetAllExamResultDto>
        {
            new GetAllExamResultDto { Id = "1", Grade = 85, ExamID = "1", StudentID = "1" },
            new GetAllExamResultDto { Id = "2", Grade = 90, ExamID = "1", StudentID = "2" }
        };

        // DÜZELTME: Mock repository'nin GetAll metodu için setup yapılıyor. MockQueryable.Moq kullanılarak IQueryable mock'u oluşturuluyor, ToListAsync ile liste elde edilecek.
        var mockQueryable = examResults.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockExamResultRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);
        
        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. ExamResult listesi GetAllExamResultDto listesine map ediliyor.
        _mockMapper.Setup(m => m.Map<IEnumerable<GetAllExamResultDto>>(It.IsAny<IEnumerable<ExamResult>>()))
            .Returns(examResultDtos);

        // Act - Test edilecek metod çağrılıyor
        var result = await _examResultManager.GetAllAsync();

        // Assert - Sonuçlar doğrulanıyor
        // DÜZELTME: FluentAssertions kullanılarak test sonuçları doğrulanıyor. Başarılı sonuç ve doğru veri döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<IEnumerable<GetAllExamResultDto>>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.ExamResultListSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    /// <summary>
    /// GetAllAsync metodunun sınav sonucu listesi boş olduğunda hata mesajı döndürmesini test eder.
    /// Boş liste durumunda kullanıcıya bilgilendirici mesaj gösteriliyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenNoExamResultsExist_ReturnsErrorDataResult()
    {
        // Arrange - Boş liste durumu hazırlanıyor
        // DÜZELTME: Boş sınav sonucu listesi oluşturuluyor. MockQueryable.Moq kullanılarak boş liste mock'u oluşturuluyor, Mock repository boş liste döndürüyor.
        var emptyExamResults = new List<ExamResult>();
        var mockQueryable = emptyExamResults.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockExamResultRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);
        
        // DÜZELTME: Mock mapper boş liste döndürüyor. Liste boş olduğunda hata mesajı döndürülmesi bekleniyor.
        _mockMapper.Setup(m => m.Map<IEnumerable<GetAllExamResultDto>>(It.IsAny<IEnumerable<ExamResult>>()))
            .Returns(new List<GetAllExamResultDto>());

        // Act - Test edilecek metod çağrılıyor
        var result = await _examResultManager.GetAllAsync();

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Boş liste durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<IEnumerable<GetAllExamResultDto>>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ConstantsMessages.ExamResultListEmptyMessage);
    }

    #endregion

    #region GetByIdAsync Tests

    /// <summary>
    /// GetByIdAsync metodunun geçerli ID ile sınav sonucu bulduğunda başarılı sonuç döndürmesini test eder.
    /// Mock veritabanından ID'ye göre sınav sonucu bulunuyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenExamResultExists_ReturnsSuccessDataResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test sınav sonucu oluşturuluyor. Mock repository ID'ye göre sınav sonucu döndürüyor.
        var examResultId = "1";
        var examResult = new ExamResult 
        { 
            ID = examResultId, 
            Grade = 85, 
            ExamID = "1", 
            StudentID = "1" 
        };

        var examResultDto = new GetByIdExamResultDto 
        { 
            Id = examResultId, 
            Grade = 85, 
            ExamID = "1", 
            StudentID = "1" 
        };

        // DÜZELTME: Mock repository'nin GetByIdAsync metodu için setup yapılıyor. Belirli ID için sınav sonucu döndürüyor.
        _mockExamResultRepository.Setup(r => r.GetByIdAsync(examResultId, It.IsAny<bool>()))
            .ReturnsAsync(examResult);
        
        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. ExamResult GetByIdExamResultDto'ya map ediliyor.
        _mockMapper.Setup(m => m.Map<GetByIdExamResultDto>(It.IsAny<ExamResult>()))
            .Returns(examResultDto);

        // Act - Test edilecek metod çağrılıyor
        var result = await _examResultManager.GetByIdAsync(examResultId);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessDataResult döndüğü ve doğru sınav sonucu bilgilerinin döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<GetByIdExamResultDto>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.ExamResultGetByIdSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(examResultId);
        result.Data.Grade.Should().Be(85);
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
        var result = await _examResultManager.GetByIdAsync(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null ID durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdExamResultDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("ID parametresi boş olamaz");
    }

    /// <summary>
    /// GetByIdAsync metodunun var olmayan ID ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Sınav sonucu bulunamadığında kullanıcıya bilgilendirici mesaj gösteriliyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenExamResultNotFound_ReturnsErrorDataResult()
    {
        // Arrange - Var olmayan ID durumu hazırlanıyor
        // DÜZELTME: Var olmayan ID test ediliyor. Mock repository null döndürüyor, sınav sonucu bulunamadı durumu simüle ediliyor.
        var nonExistentId = "999";
        _mockExamResultRepository.Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<bool>()))
            .ReturnsAsync((ExamResult)null);

        // Act - Test edilecek metod çağrılıyor
        var result = await _examResultManager.GetByIdAsync(nonExistentId);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Sınav sonucu bulunamadığında ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdExamResultDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("sınav sonucu bulunamadı");
    }

    #endregion

    #region CreateAsync Tests

    /// <summary>
    /// CreateAsync metodunun geçerli sınav sonucu bilgileri ile başarılı oluşturma yapmasını test eder.
    /// Mock veritabanına sınav sonucu ekleniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenValidExamResult_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test sınav sonucu DTO'su oluşturuluyor. Mock veritabanına eklenecek sınav sonucu bilgileri hazırlanıyor.
        var createExamResultDto = new CreateExamResultDto
        {
            Grade = 85,
            ExamID = "1",
            StudentID = "1"
        };

        var examResult = new ExamResult
        {
            ID = "1",
            Grade = 85,
            ExamID = "1",
            StudentID = "1"
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. CreateExamResultDto ExamResult entity'sine map ediliyor.
        _mockMapper.Setup(m => m.Map<ExamResult>(It.IsAny<CreateExamResultDto>()))
            .Returns(examResult);
        
        // DÜZELTME: Mock repository'nin CreateAsync metodu için setup yapılıyor. Sınav sonucu ekleniyor.
        _mockExamResultRepository.Setup(r => r.CreateAsync(It.IsAny<ExamResult>()))
            .Returns(Task.CompletedTask);
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (result > 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _examResultManager.CreateAsync(createExamResultDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.ExamResultCreateSuccessMessage);
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
        var result = await _examResultManager.CreateAsync(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Sınav sonucu bilgileri boş olamaz");
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
        var createExamResultDto = new CreateExamResultDto
        {
            Grade = 85,
            ExamID = "1",
            StudentID = "1"
        };

        var examResult = new ExamResult
        {
            ID = "1",
            Grade = 85,
            ExamID = "1",
            StudentID = "1"
        };

        _mockMapper.Setup(m => m.Map<ExamResult>(It.IsAny<CreateExamResultDto>()))
            .Returns(examResult);
        
        _mockExamResultRepository.Setup(r => r.CreateAsync(It.IsAny<ExamResult>()))
            .Returns(Task.CompletedTask);
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarısız commit (result = 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(0);

        // Act - Test edilecek metod çağrılıyor
        var result = await _examResultManager.CreateAsync(createExamResultDto);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Commit başarısız olduğunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ConstantsMessages.ExamResultCreateFailedMessage);
    }

    #endregion

    #region Update Tests

    /// <summary>
    /// Update metodunun geçerli sınav sonucu bilgileri ile başarılı güncelleme yapmasını test eder.
    /// Mock veritabanında sınav sonucu güncelleniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task Update_WhenValidExamResult_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test sınav sonucu DTO'su oluşturuluyor. Mock veritabanında güncellenecek sınav sonucu bilgileri hazırlanıyor.
        var updateExamResultDto = new UpdateExamResultDto
        {
            Id = "1",
            Grade = 90,
            ExamID = "1",
            StudentID = "1"
        };

        var examResult = new ExamResult
        {
            ID = "1",
            Grade = 90,
            ExamID = "1",
            StudentID = "1"
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. UpdateExamResultDto ExamResult entity'sine map ediliyor.
        _mockMapper.Setup(m => m.Map<ExamResult>(It.IsAny<UpdateExamResultDto>()))
            .Returns(examResult);
        
        // DÜZELTME: Mock repository'nin Update metodu için setup yapılıyor. Sınav sonucu güncelleniyor.
        _mockExamResultRepository.Setup(r => r.Update(It.IsAny<ExamResult>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (result > 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _examResultManager.Update(updateExamResultDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.ExamResultUpdateSuccessMessage);
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
        var result = await _examResultManager.Update(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Güncellenecek sınav sonucu bilgileri boş olamaz");
    }

    #endregion

    #region Remove Tests

    /// <summary>
    /// Remove metodunun geçerli sınav sonucu bilgileri ile başarılı silme yapmasını test eder.
    /// Mock veritabanından sınav sonucu siliniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task Remove_WhenValidExamResult_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test sınav sonucu DTO'su oluşturuluyor. Mock veritabanından silinecek sınav sonucu bilgileri hazırlanıyor.
        var deleteExamResultDto = new DeleteExamResultDto
        {
            Id = "1"
        };

        var examResult = new ExamResult
        {
            ID = "1",
            Grade = 85,
            ExamID = "1",
            StudentID = "1"
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. DeleteExamResultDto ExamResult entity'sine map ediliyor.
        _mockMapper.Setup(m => m.Map<ExamResult>(It.IsAny<DeleteExamResultDto>()))
            .Returns(examResult);
        
        // DÜZELTME: Mock repository'nin Remove metodu için setup yapılıyor. Sınav sonucu siliniyor.
        _mockExamResultRepository.Setup(r => r.Remove(It.IsAny<ExamResult>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (result > 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _examResultManager.Remove(deleteExamResultDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.ExamResultDeleteSuccessMessage);
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
        var result = await _examResultManager.Remove(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Silinecek sınav sonucu bilgileri boş olamaz");
    }

    #endregion
}

