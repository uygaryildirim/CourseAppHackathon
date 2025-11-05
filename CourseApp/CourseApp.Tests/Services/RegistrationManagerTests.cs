using AutoMapper;
using CourseApp.DataAccessLayer.Abstract;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.RegistrationDto;
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
/// RegistrationManager servis katmanı için unit testler. 
/// Bu testler, RegistrationManager'ın tüm metodlarının doğru çalıştığını doğrulamak için yazıldı.
/// Mock kullanarak veritabanı bağımlılığından izole edilmiş testler yazılıyor.
/// </summary>
public class RegistrationManagerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRegistrationRepository> _mockRegistrationRepository;
    private readonly RegistrationManager _registrationManager;

    /// <summary>
    /// Test constructor'ı. Her test öncesi mock nesneleri ve RegistrationManager instance'ı oluşturuluyor.
    /// Mock'lar sayesinde gerçek veritabanı kullanılmadan testler çalıştırılabiliyor.
    /// </summary>
    public RegistrationManagerTests()
    {
        // DÜZELTME: Mock nesneleri oluşturuluyor. Unit test'lerde gerçek veritabanı yerine mock kullanılıyor, böylece testler hızlı ve izole çalışıyor.
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockRegistrationRepository = new Mock<IRegistrationRepository>();
        
        // DÜZELTME: UnitOfWork'in Registrations property'si mock registration repository döndürüyor. Dependency injection benzeri bir yapı oluşturuluyor.
        _mockUnitOfWork.Setup(u => u.Registrations).Returns(_mockRegistrationRepository.Object);
        
        // DÜZELTME: RegistrationManager instance'ı oluşturuluyor. Mock'lar constructor'a enjekte ediliyor.
        _registrationManager = new RegistrationManager(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    #region GetAllAsync Tests

    /// <summary>
    /// GetAllAsync metodunun başarılı durumda kayıt listesi döndürmesini test eder.
    /// Mock veritabanından kayıt listesi döndürülüyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenRegistrationsExist_ReturnsSuccessDataResult()
    {
        // Arrange - Test verileri hazırlanıyor
        // DÜZELTME: Test verisi oluşturuluyor. Mock veritabanından dönecek kayıt listesi hazırlanıyor.
        var registrations = new List<Registration>
        {
            new Registration { ID = "1", Price = 1000, StudentID = "1", CourseID = "1", RegistrationDate = new DateTime(2024, 1, 1) },
            new Registration { ID = "2", Price = 1500, StudentID = "2", CourseID = "1", RegistrationDate = new DateTime(2024, 1, 2) }
        };

        var registrationDtos = new List<GetAllRegistrationDto>
        {
            new GetAllRegistrationDto { Id = "1", Price = 1000, StudentID = "1", CourseID = "1", RegistrationDate = new DateTime(2024, 1, 1) },
            new GetAllRegistrationDto { Id = "2", Price = 1500, StudentID = "2", CourseID = "1", RegistrationDate = new DateTime(2024, 1, 2) }
        };

        // DÜZELTME: Mock repository'nin GetAll metodu için setup yapılıyor. MockQueryable.Moq kullanılarak IQueryable mock'u oluşturuluyor, ToListAsync ile liste elde edilecek.
        var mockQueryable = registrations.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockRegistrationRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);
        
        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. Registration listesi GetAllRegistrationDto listesine map ediliyor.
        _mockMapper.Setup(m => m.Map<IEnumerable<GetAllRegistrationDto>>(It.IsAny<IEnumerable<Registration>>()))
            .Returns(registrationDtos);

        // Act - Test edilecek metod çağrılıyor
        var result = await _registrationManager.GetAllAsync();

        // Assert - Sonuçlar doğrulanıyor
        // DÜZELTME: FluentAssertions kullanılarak test sonuçları doğrulanıyor. Başarılı sonuç ve doğru veri döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<IEnumerable<GetAllRegistrationDto>>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.RegistrationListSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    /// <summary>
    /// GetAllAsync metodunun kayıt listesi boş olduğunda hata mesajı döndürmesini test eder.
    /// Boş liste durumunda kullanıcıya bilgilendirici mesaj gösteriliyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenNoRegistrationsExist_ReturnsErrorDataResult()
    {
        // Arrange - Boş liste durumu hazırlanıyor
        // DÜZELTME: Boş kayıt listesi oluşturuluyor. MockQueryable.Moq kullanılarak boş liste mock'u oluşturuluyor, Mock repository boş liste döndürüyor.
        var emptyRegistrations = new List<Registration>();
        var mockQueryable = emptyRegistrations.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockRegistrationRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);
        
        // DÜZELTME: Mock mapper boş liste döndürüyor. Liste boş olduğunda hata mesajı döndürülmesi bekleniyor.
        _mockMapper.Setup(m => m.Map<IEnumerable<GetAllRegistrationDto>>(It.IsAny<IEnumerable<Registration>>()))
            .Returns(new List<GetAllRegistrationDto>());

        // Act - Test edilecek metod çağrılıyor
        var result = await _registrationManager.GetAllAsync();

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Boş liste durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<IEnumerable<GetAllRegistrationDto>>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ConstantsMessages.RegistrationListEmptyMessage);
    }

    #endregion

    #region GetByIdAsync Tests

    /// <summary>
    /// GetByIdAsync metodunun geçerli ID ile kayıt bulduğunda başarılı sonuç döndürmesini test eder.
    /// Mock veritabanından ID'ye göre kayıt bulunuyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenRegistrationExists_ReturnsSuccessDataResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test kaydı oluşturuluyor. Mock repository ID'ye göre kayıt döndürüyor.
        var registrationId = "1";
        var registration = new Registration 
        { 
            ID = registrationId, 
            Price = 1000, 
            StudentID = "1", 
            CourseID = "1", 
            RegistrationDate = new DateTime(2024, 1, 1) 
        };

        var registrationDto = new GetByIdRegistrationDto 
        { 
            Id = registrationId, 
            Price = 1000, 
            StudentID = "1", 
            CourseID = "1", 
            RegistrationDate = new DateTime(2024, 1, 1) 
        };

        // DÜZELTME: Mock repository'nin GetByIdAsync metodu için setup yapılıyor. Belirli ID için kayıt döndürüyor.
        _mockRegistrationRepository.Setup(r => r.GetByIdAsync(registrationId, It.IsAny<bool>()))
            .ReturnsAsync(registration);
        
        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. Registration GetByIdRegistrationDto'ya map ediliyor.
        _mockMapper.Setup(m => m.Map<GetByIdRegistrationDto>(It.IsAny<Registration>()))
            .Returns(registrationDto);

        // Act - Test edilecek metod çağrılıyor
        var result = await _registrationManager.GetByIdAsync(registrationId);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessDataResult döndüğü ve doğru kayıt bilgilerinin döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<GetByIdRegistrationDto>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.RegistrationGetByIdSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(registrationId);
        result.Data.Price.Should().Be(1000);
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
        var result = await _registrationManager.GetByIdAsync(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null ID durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdRegistrationDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("ID parametresi boş olamaz");
    }

    /// <summary>
    /// GetByIdAsync metodunun var olmayan ID ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Kayıt bulunamadığında kullanıcıya bilgilendirici mesaj gösteriliyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenRegistrationNotFound_ReturnsErrorDataResult()
    {
        // Arrange - Var olmayan ID durumu hazırlanıyor
        // DÜZELTME: Var olmayan ID test ediliyor. Mock repository null döndürüyor, kayıt bulunamadı durumu simüle ediliyor.
        var nonExistentId = "999";
        _mockRegistrationRepository.Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<bool>()))
            .ReturnsAsync((Registration)null);

        // Act - Test edilecek metod çağrılıyor
        var result = await _registrationManager.GetByIdAsync(nonExistentId);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Kayıt bulunamadığında ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdRegistrationDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("kayıt bulunamadı");
    }

    #endregion

    #region CreateAsync Tests

    /// <summary>
    /// CreateAsync metodunun geçerli kayıt bilgileri ile başarılı oluşturma yapmasını test eder.
    /// Mock veritabanına kayıt ekleniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenValidRegistration_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test kaydı DTO'su oluşturuluyor. Mock veritabanına eklenecek kayıt bilgileri hazırlanıyor.
        var createRegistrationDto = new CreateRegistrationDto
        {
            Price = 1000,
            StudentID = "1",
            CourseID = "1",
            RegistrationDate = new DateTime(2024, 1, 1)
        };

        var registration = new Registration
        {
            ID = "1",
            Price = 1000,
            StudentID = "1",
            CourseID = "1",
            RegistrationDate = new DateTime(2024, 1, 1)
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. CreateRegistrationDto Registration entity'sine map ediliyor.
        _mockMapper.Setup(m => m.Map<Registration>(It.IsAny<CreateRegistrationDto>()))
            .Returns(registration);
        
        // DÜZELTME: Mock repository'nin CreateAsync metodu için setup yapılıyor. Kayıt ekleniyor.
        _mockRegistrationRepository.Setup(r => r.CreateAsync(It.IsAny<Registration>()))
            .Returns(Task.CompletedTask);
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (result > 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _registrationManager.CreateAsync(createRegistrationDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.RegistrationCreateSuccessMessage);
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
        var result = await _registrationManager.CreateAsync(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Kayıt bilgileri boş olamaz");
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
        var createRegistrationDto = new CreateRegistrationDto
        {
            Price = 1000,
            StudentID = "1",
            CourseID = "1",
            RegistrationDate = new DateTime(2024, 1, 1)
        };

        var registration = new Registration
        {
            ID = "1",
            Price = 1000,
            StudentID = "1",
            CourseID = "1",
            RegistrationDate = new DateTime(2024, 1, 1)
        };

        _mockMapper.Setup(m => m.Map<Registration>(It.IsAny<CreateRegistrationDto>()))
            .Returns(registration);
        
        _mockRegistrationRepository.Setup(r => r.CreateAsync(It.IsAny<Registration>()))
            .Returns(Task.CompletedTask);
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarısız commit (result = 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(0);

        // Act - Test edilecek metod çağrılıyor
        var result = await _registrationManager.CreateAsync(createRegistrationDto);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Commit başarısız olduğunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ConstantsMessages.RegistrationCreateFailedMessage);
    }

    #endregion

    #region Update Tests

    /// <summary>
    /// Update metodunun geçerli kayıt bilgileri ile başarılı güncelleme yapmasını test eder.
    /// Mock veritabanında kayıt güncelleniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task Update_WhenValidRegistration_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test kaydı DTO'su oluşturuluyor. Mock veritabanında güncellenecek kayıt bilgileri hazırlanıyor.
        var updateRegistrationDto = new UpdatedRegistrationDto
        {
            Id = "1",
            Price = 1500,
            StudentID = "1",
            CourseID = "1",
            RegistrationDate = new DateTime(2024, 1, 1)
        };

        var registration = new Registration
        {
            ID = "1",
            Price = 1500,
            StudentID = "1",
            CourseID = "1",
            RegistrationDate = new DateTime(2024, 1, 1)
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. UpdatedRegistrationDto Registration entity'sine map ediliyor.
        _mockMapper.Setup(m => m.Map<Registration>(It.IsAny<UpdatedRegistrationDto>()))
            .Returns(registration);
        
        // DÜZELTME: Mock repository'nin Update metodu için setup yapılıyor. Kayıt güncelleniyor.
        _mockRegistrationRepository.Setup(r => r.Update(It.IsAny<Registration>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (result > 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _registrationManager.Update(updateRegistrationDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.RegistrationUpdateSuccessMessage);
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
        var result = await _registrationManager.Update(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Güncellenecek kayıt bilgileri boş olamaz");
    }

    #endregion

    #region Remove Tests

    /// <summary>
    /// Remove metodunun geçerli kayıt bilgileri ile başarılı silme yapmasını test eder.
    /// Mock veritabanından kayıt siliniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task Remove_WhenValidRegistration_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test kaydı DTO'su oluşturuluyor. Mock veritabanından silinecek kayıt bilgileri hazırlanıyor.
        var deleteRegistrationDto = new DeleteRegistrationDto
        {
            Id = "1"
        };

        var registration = new Registration
        {
            ID = "1",
            Price = 1000,
            StudentID = "1",
            CourseID = "1",
            RegistrationDate = new DateTime(2024, 1, 1)
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. DeleteRegistrationDto Registration entity'sine map ediliyor.
        _mockMapper.Setup(m => m.Map<Registration>(It.IsAny<DeleteRegistrationDto>()))
            .Returns(registration);
        
        // DÜZELTME: Mock repository'nin Remove metodu için setup yapılıyor. Kayıt siliniyor.
        _mockRegistrationRepository.Setup(r => r.Remove(It.IsAny<Registration>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (result > 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _registrationManager.Remove(deleteRegistrationDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.RegistrationDeleteSuccessMessage);
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
        var result = await _registrationManager.Remove(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Silinecek kayıt bilgileri boş olamaz");
    }

    #endregion
}

