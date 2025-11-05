using AutoMapper;
using CourseApp.DataAccessLayer.Abstract;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.InstructorDto;
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
/// InstructorManager servis katmanı için unit testler. 
/// Bu testler, InstructorManager'ın tüm metodlarının doğru çalıştığını doğrulamak için yazıldı.
/// Mock kullanarak veritabanı bağımlılığından izole edilmiş testler yazılıyor.
/// </summary>
public class InstructorManagerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IInstructorRepository> _mockInstructorRepository;
    private readonly InstructorManager _instructorManager;

    /// <summary>
    /// Test constructor'ı. Her test öncesi mock nesneleri ve InstructorManager instance'ı oluşturuluyor.
    /// Mock'lar sayesinde gerçek veritabanı kullanılmadan testler çalıştırılabiliyor.
    /// </summary>
    public InstructorManagerTests()
    {
        // DÜZELTME: Mock nesneleri oluşturuluyor. Unit test'lerde gerçek veritabanı yerine mock kullanılıyor, böylece testler hızlı ve izole çalışıyor.
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockInstructorRepository = new Mock<IInstructorRepository>();
        
        // DÜZELTME: UnitOfWork'in Instructors property'si mock instructor repository döndürüyor. Dependency injection benzeri bir yapı oluşturuluyor.
        _mockUnitOfWork.Setup(u => u.Instructors).Returns(_mockInstructorRepository.Object);
        
        // DÜZELTME: InstructorManager instance'ı oluşturuluyor. Mock'lar constructor'a enjekte ediliyor.
        _instructorManager = new InstructorManager(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    #region GetAllAsync Tests

    /// <summary>
    /// GetAllAsync metodunun başarılı durumda eğitmen listesi döndürmesini test eder.
    /// Mock veritabanından eğitmen listesi döndürülüyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenInstructorsExist_ReturnsSuccessDataResult()
    {
        // Arrange - Test verileri hazırlanıyor
        // DÜZELTME: Test verisi oluşturuluyor. Mock veritabanından dönecek eğitmen listesi hazırlanıyor.
        var instructors = new List<Instructor>
        {
            new Instructor { ID = "1", Name = "Ahmet", Surname = "Yılmaz", Email = "ahmet@example.com", PhoneNumber = "5551234567" },
            new Instructor { ID = "2", Name = "Mehmet", Surname = "Demir", Email = "mehmet@example.com", PhoneNumber = "5551234568" }
        };

        var instructorDtos = new List<GetAllInstructorDto>
        {
            new GetAllInstructorDto { Id = "1", Name = "Ahmet", Surname = "Yılmaz", Email = "ahmet@example.com", PhoneNumber = "5551234567" },
            new GetAllInstructorDto { Id = "2", Name = "Mehmet", Surname = "Demir", Email = "mehmet@example.com", PhoneNumber = "5551234568" }
        };

        // DÜZELTME: Mock repository'nin GetAll metodu için setup yapılıyor. MockQueryable.Moq kullanılarak IQueryable mock'u oluşturuluyor, ToListAsync ile liste elde edilecek.
        var mockQueryable = instructors.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockInstructorRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);
        
        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. Instructor listesi GetAllInstructorDto listesine map ediliyor.
        _mockMapper.Setup(m => m.Map<IEnumerable<GetAllInstructorDto>>(It.IsAny<IEnumerable<Instructor>>()))
            .Returns(instructorDtos);

        // Act - Test edilecek metod çağrılıyor
        var result = await _instructorManager.GetAllAsync();

        // Assert - Sonuçlar doğrulanıyor
        // DÜZELTME: FluentAssertions kullanılarak test sonuçları doğrulanıyor. Başarılı sonuç ve doğru veri döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<IEnumerable<GetAllInstructorDto>>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.InstructorListSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    /// <summary>
    /// GetAllAsync metodunun eğitmen listesi boş olduğunda hata mesajı döndürmesini test eder.
    /// Boş liste durumunda kullanıcıya bilgilendirici mesaj gösteriliyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenNoInstructorsExist_ReturnsErrorDataResult()
    {
        // Arrange - Boş liste durumu hazırlanıyor
        // DÜZELTME: Boş eğitmen listesi oluşturuluyor. MockQueryable.Moq kullanılarak boş liste mock'u oluşturuluyor, Mock repository boş liste döndürüyor.
        var emptyInstructors = new List<Instructor>();
        var mockQueryable = emptyInstructors.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockInstructorRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);
        
        // DÜZELTME: Mock mapper boş liste döndürüyor. Liste boş olduğunda hata mesajı döndürülmesi bekleniyor.
        _mockMapper.Setup(m => m.Map<IEnumerable<GetAllInstructorDto>>(It.IsAny<IEnumerable<Instructor>>()))
            .Returns(new List<GetAllInstructorDto>());

        // Act - Test edilecek metod çağrılıyor
        var result = await _instructorManager.GetAllAsync();

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Boş liste durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<IEnumerable<GetAllInstructorDto>>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ConstantsMessages.InstructorListEmptyMessage);
    }

    #endregion

    #region GetByIdAsync Tests

    /// <summary>
    /// GetByIdAsync metodunun geçerli ID ile eğitmen bulduğunda başarılı sonuç döndürmesini test eder.
    /// Mock veritabanından ID'ye göre eğitmen bulunuyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenInstructorExists_ReturnsSuccessDataResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test eğitmeni oluşturuluyor. Mock repository ID'ye göre eğitmen döndürüyor.
        var instructorId = "1";
        var instructor = new Instructor 
        { 
            ID = instructorId, 
            Name = "Ahmet", 
            Surname = "Yılmaz", 
            Email = "ahmet@example.com", 
            PhoneNumber = "5551234567" 
        };

        var instructorDto = new GetByIdInstructorDto 
        { 
            Id = instructorId, 
            Name = "Ahmet", 
            Surname = "Yılmaz", 
            Email = "ahmet@example.com", 
            PhoneNumber = "5551234567" 
        };

        // DÜZELTME: Mock repository'nin GetByIdAsync metodu için setup yapılıyor. Belirli ID için eğitmen döndürüyor.
        _mockInstructorRepository.Setup(r => r.GetByIdAsync(instructorId, It.IsAny<bool>()))
            .ReturnsAsync(instructor);
        
        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. Instructor GetByIdInstructorDto'ya map ediliyor.
        _mockMapper.Setup(m => m.Map<GetByIdInstructorDto>(It.IsAny<Instructor>()))
            .Returns(instructorDto);

        // Act - Test edilecek metod çağrılıyor
        var result = await _instructorManager.GetByIdAsync(instructorId);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessDataResult döndüğü ve doğru eğitmen bilgilerinin döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<GetByIdInstructorDto>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.InstructorGetByIdSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(instructorId);
        result.Data.Name.Should().Be("Ahmet");
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
        var result = await _instructorManager.GetByIdAsync(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null ID durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdInstructorDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("ID parametresi boş olamaz");
    }

    /// <summary>
    /// GetByIdAsync metodunun var olmayan ID ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Eğitmen bulunamadığında kullanıcıya bilgilendirici mesaj gösteriliyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenInstructorNotFound_ReturnsErrorDataResult()
    {
        // Arrange - Var olmayan ID durumu hazırlanıyor
        // DÜZELTME: Var olmayan ID test ediliyor. Mock repository null döndürüyor, eğitmen bulunamadı durumu simüle ediliyor.
        var nonExistentId = "999";
        _mockInstructorRepository.Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<bool>()))
            .ReturnsAsync((Instructor)null);

        // Act - Test edilecek metod çağrılıyor
        var result = await _instructorManager.GetByIdAsync(nonExistentId);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Eğitmen bulunamadığında ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdInstructorDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("eğitmen bulunamadı");
    }

    #endregion

    #region CreateAsync Tests

    /// <summary>
    /// CreateAsync metodunun geçerli eğitmen bilgileri ile başarılı oluşturma yapmasını test eder.
    /// Mock veritabanına eğitmen ekleniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenValidInstructor_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test eğitmeni DTO'su oluşturuluyor. Mock veritabanına eklenecek eğitmen bilgileri hazırlanıyor.
        var createInstructorDto = new CreatedInstructorDto
        {
            Name = "Ahmet",
            Surname = "Yılmaz",
            Email = "ahmet@example.com",
            PhoneNumber = "5551234567"
        };

        var instructor = new Instructor
        {
            ID = "1",
            Name = "Ahmet",
            Surname = "Yılmaz",
            Email = "ahmet@example.com",
            PhoneNumber = "5551234567"
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. CreatedInstructorDto Instructor entity'sine map ediliyor.
        _mockMapper.Setup(m => m.Map<Instructor>(It.IsAny<CreatedInstructorDto>()))
            .Returns(instructor);
        
        // DÜZELTME: Mock repository'nin CreateAsync metodu için setup yapılıyor. Eğitmen ekleniyor.
        _mockInstructorRepository.Setup(r => r.CreateAsync(It.IsAny<Instructor>()))
            .Returns(Task.CompletedTask);
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (result > 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _instructorManager.CreateAsync(createInstructorDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.InstructorCreateSuccessMessage);
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
        var result = await _instructorManager.CreateAsync(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Eğitmen bilgileri boş olamaz");
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
        var createInstructorDto = new CreatedInstructorDto
        {
            Name = "Ahmet",
            Surname = "Yılmaz",
            Email = "ahmet@example.com",
            PhoneNumber = "5551234567"
        };

        var instructor = new Instructor
        {
            ID = "1",
            Name = "Ahmet",
            Surname = "Yılmaz",
            Email = "ahmet@example.com",
            PhoneNumber = "5551234567"
        };

        _mockMapper.Setup(m => m.Map<Instructor>(It.IsAny<CreatedInstructorDto>()))
            .Returns(instructor);
        
        _mockInstructorRepository.Setup(r => r.CreateAsync(It.IsAny<Instructor>()))
            .Returns(Task.CompletedTask);
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarısız commit (result = 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(0);

        // Act - Test edilecek metod çağrılıyor
        var result = await _instructorManager.CreateAsync(createInstructorDto);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Commit başarısız olduğunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ConstantsMessages.InstructorCreateFailedMessage);
    }

    #endregion

    #region Update Tests

    /// <summary>
    /// Update metodunun geçerli eğitmen bilgileri ile başarılı güncelleme yapmasını test eder.
    /// Mock veritabanında eğitmen güncelleniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task Update_WhenValidInstructor_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test eğitmeni DTO'su oluşturuluyor. Mock veritabanında güncellenecek eğitmen bilgileri hazırlanıyor.
        var updateInstructorDto = new UpdatedInstructorDto
        {
            Id = "1",
            Name = "Ahmet",
            Surname = "Yılmaz",
            Email = "ahmet@example.com",
            PhoneNumber = "5551234567"
        };

        var instructor = new Instructor
        {
            ID = "1",
            Name = "Ahmet",
            Surname = "Yılmaz",
            Email = "ahmet@example.com",
            PhoneNumber = "5551234567"
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. UpdatedInstructorDto Instructor entity'sine map ediliyor.
        _mockMapper.Setup(m => m.Map<Instructor>(It.IsAny<UpdatedInstructorDto>()))
            .Returns(instructor);
        
        // DÜZELTME: Mock repository'nin Update metodu için setup yapılıyor. Eğitmen güncelleniyor.
        _mockInstructorRepository.Setup(r => r.Update(It.IsAny<Instructor>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (result > 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _instructorManager.Update(updateInstructorDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.InstructorUpdateSuccessMessage);
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
        var result = await _instructorManager.Update(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Güncellenecek eğitmen bilgileri boş olamaz");
    }

    #endregion

    #region Remove Tests

    /// <summary>
    /// Remove metodunun geçerli eğitmen bilgileri ile başarılı silme yapmasını test eder.
    /// Mock veritabanından eğitmen siliniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task Remove_WhenValidInstructor_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test eğitmeni DTO'su oluşturuluyor. Mock veritabanından silinecek eğitmen bilgileri hazırlanıyor.
        var deleteInstructorDto = new DeletedInstructorDto
        {
            Id = "1"
        };

        var instructor = new Instructor
        {
            ID = "1",
            Name = "Ahmet",
            Surname = "Yılmaz",
            Email = "ahmet@example.com",
            PhoneNumber = "5551234567"
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. DeletedInstructorDto Instructor entity'sine map ediliyor.
        _mockMapper.Setup(m => m.Map<Instructor>(It.IsAny<DeletedInstructorDto>()))
            .Returns(instructor);
        
        // DÜZELTME: Mock repository'nin Remove metodu için setup yapılıyor. Eğitmen siliniyor.
        _mockInstructorRepository.Setup(r => r.Remove(It.IsAny<Instructor>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (result > 0) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _instructorManager.Remove(deleteInstructorDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.InstructorDeleteSuccessMessage);
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
        var result = await _instructorManager.Remove(null);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Silinecek eğitmen bilgileri boş olamaz");
    }

    #endregion
}

