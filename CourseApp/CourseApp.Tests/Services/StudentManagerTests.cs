using AutoMapper;
using CourseApp.DataAccessLayer.Abstract;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.StudentDto;
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
/// StudentManager servis katmanı için unit testler. 
/// Bu testler, StudentManager'ın tüm metodlarının doğru çalıştığını doğrulamak için yazıldı.
/// Mock kullanarak veritabanı bağımlılığından izole edilmiş testler yazılıyor.
/// </summary>
public class StudentManagerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly StudentManager _studentManager;

    /// <summary>
    /// Test constructor'ı. Her test öncesi mock nesneleri ve StudentManager instance'ı oluşturuluyor.
    /// Mock'lar sayesinde gerçek veritabanı kullanılmadan testler çalıştırılabiliyor.
    /// </summary>
    public StudentManagerTests()
    {
        // DÜZELTME: Mock nesneleri oluşturuluyor. Unit test'lerde gerçek veritabanı yerine mock kullanılıyor, böylece testler hızlı ve izole çalışıyor.
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockStudentRepository = new Mock<IStudentRepository>();
        
        // DÜZELTME: UnitOfWork'in Students property'si mock student repository döndürüyor. Dependency injection benzeri bir yapı oluşturuluyor.
        _mockUnitOfWork.Setup(u => u.Students).Returns(_mockStudentRepository.Object);
        
        // DÜZELTME: StudentManager instance'ı oluşturuluyor. Mock'lar constructor'a enjekte ediliyor.
        _studentManager = new StudentManager(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    #region GetAllAsync Tests

    /// <summary>
    /// GetAllAsync metodunun başarılı durumda öğrenci listesi döndürmesini test eder.
    /// Mock veritabanından öğrenci listesi döndürülüyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenStudentsExist_ReturnsSuccessDataResult()
    {
        // Arrange - Test verileri hazırlanıyor
        // DÜZELTME: Test verisi oluşturuluyor. Mock veritabanından dönecek öğrenci listesi hazırlanıyor.
        var students = new List<Student>
        {
            new Student { ID = "1", Name = "Ahmet", Surname = "Yılmaz", TC = "12345678901", BirthDate = new DateTime(1990, 1, 1) },
            new Student { ID = "2", Name = "Mehmet", Surname = "Demir", TC = "12345678902", BirthDate = new DateTime(1991, 2, 2) }
        };

        var studentDtos = new List<GetAllStudentDto>
        {
            new GetAllStudentDto { Id = "1", Name = "Ahmet", Surname = "Yılmaz", TC = "12345678901", BirthDate = new DateTime(1990, 1, 1) },
            new GetAllStudentDto { Id = "2", Name = "Mehmet", Surname = "Demir", TC = "12345678902", BirthDate = new DateTime(1991, 2, 2) }
        };

        // DÜZELTME: Mock repository'nin GetAll metodu için setup yapılıyor. MockQueryable.Moq kullanılarak IQueryable mock'u oluşturuluyor, ToListAsync ile liste elde edilecek.
        var mockQueryable = students.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockStudentRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);
        
        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. Student listesi GetAllStudentDto listesine map ediliyor.
        _mockMapper.Setup(m => m.Map<IEnumerable<GetAllStudentDto>>(It.IsAny<IEnumerable<Student>>()))
            .Returns(studentDtos);

        // Act - Test edilecek metod çağrılıyor
        var result = await _studentManager.GetAllAsync();

        // Assert - Sonuçlar doğrulanıyor
        // DÜZELTME: FluentAssertions kullanılarak test sonuçları doğrulanıyor. Başarılı sonuç ve doğru veri döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<IEnumerable<GetAllStudentDto>>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.StudentListSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
    }

    /// <summary>
    /// GetAllAsync metodunun öğrenci listesi boş olduğunda hata mesajı döndürmesini test eder.
    /// Boş liste durumunda kullanıcıya bilgilendirici mesaj gösteriliyor.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WhenNoStudentsExist_ReturnsErrorDataResult()
    {
        // Arrange - Boş liste durumu hazırlanıyor
        // DÜZELTME: Boş öğrenci listesi oluşturuluyor. MockQueryable.Moq kullanılarak boş liste mock'u oluşturuluyor, Mock repository boş liste döndürüyor.
        var emptyStudents = new List<Student>();
        var mockQueryable = emptyStudents.AsQueryable().BuildMockDbSet().Object.AsQueryable();
        _mockStudentRepository.Setup(r => r.GetAll(It.IsAny<bool>())).Returns(mockQueryable);
        
        // DÜZELTME: Mock mapper boş liste döndürüyor. Liste boş olduğunda hata mesajı döndürülmesi bekleniyor.
        _mockMapper.Setup(m => m.Map<IEnumerable<GetAllStudentDto>>(It.IsAny<IEnumerable<Student>>()))
            .Returns(new List<GetAllStudentDto>());

        // Act - Test edilecek metod çağrılıyor
        var result = await _studentManager.GetAllAsync();

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Boş liste durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<IEnumerable<GetAllStudentDto>>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ConstantsMessages.StudentListEmptyMessage);
    }

    #endregion

    #region GetByIdAsync Tests

    /// <summary>
    /// GetByIdAsync metodunun geçerli ID ile öğrenci bulduğunda başarılı sonuç döndürmesini test eder.
    /// Mock veritabanından ID'ye göre öğrenci bulunuyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenStudentExists_ReturnsSuccessDataResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test öğrencisi oluşturuluyor. Mock repository ID'ye göre öğrenci döndürüyor.
        var studentId = "1";
        var student = new Student 
        { 
            ID = studentId, 
            Name = "Ahmet", 
            Surname = "Yılmaz", 
            TC = "12345678901", 
            BirthDate = new DateTime(1990, 1, 1) 
        };

        var studentDto = new GetByIdStudentDto 
        { 
            Id = studentId, 
            Name = "Ahmet", 
            Surname = "Yılmaz", 
            TC = "12345678901", 
            BirthDate = new DateTime(1990, 1, 1) 
        };

        // DÜZELTME: Mock repository'nin GetByIdAsync metodu için setup yapılıyor. Belirli ID için öğrenci döndürüyor.
        _mockStudentRepository.Setup(r => r.GetByIdAsync(studentId, It.IsAny<bool>()))
            .ReturnsAsync(student);
        
        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. Student GetByIdStudentDto'ya map ediliyor.
        _mockMapper.Setup(m => m.Map<GetByIdStudentDto>(It.IsAny<Student>()))
            .Returns(studentDto);

        // Act - Test edilecek metod çağrılıyor
        var result = await _studentManager.GetByIdAsync(studentId);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessDataResult döndüğü ve doğru öğrenci bilgilerinin döndüğü kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessDataResult<GetByIdStudentDto>>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.StudentGetByIdSuccessMessage);
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(studentId);
        result.Data.Name.Should().Be("Ahmet");
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
        var result = await _studentManager.GetByIdAsync(nullId!);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null ID durumunda ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdStudentDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("ID parametresi boş olamaz.");
    }

    /// <summary>
    /// GetByIdAsync metodunun öğrenci bulunamadığında hata mesajı döndürmesini test eder.
    /// Mock veritabanından öğrenci bulunamadığında null döndürülüyor ve hata mesajı bekleniyor.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenStudentNotFound_ReturnsErrorDataResult()
    {
        // Arrange - Öğrenci bulunamama durumu hazırlanıyor
        // DÜZELTME: Mock repository null döndürüyor. Öğrenci bulunamadığında null check'in doğru çalıştığı kontrol ediliyor.
        var studentId = "999";
        _mockStudentRepository.Setup(r => r.GetByIdAsync(studentId, It.IsAny<bool>()))
            .ReturnsAsync((Student?)null);

        // Act - Test edilecek metod çağrılıyor
        var result = await _studentManager.GetByIdAsync(studentId);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Öğrenci bulunamadığında ErrorDataResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorDataResult<GetByIdStudentDto>>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Belirtilen ID'ye sahip öğrenci bulunamadı.");
    }

    #endregion

    #region CreateAsync Tests

    /// <summary>
    /// CreateAsync metodunun geçerli öğrenci bilgileri ile başarılı oluşturma yapmasını test eder.
    /// Mock veritabanına öğrenci ekleniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenValidStudent_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test öğrencisi oluşturuluyor. Mock repository ve mapper için setup yapılıyor.
        var createStudentDto = new CreateStudentDto
        {
            Name = "Ahmet",
            Surname = "Yılmaz",
            TC = "12345678901",
            BirthDate = new DateTime(1990, 1, 1)
        };

        var student = new Student
        {
            Name = "Ahmet",
            Surname = "Yılmaz",
            TC = "12345678901",
            BirthDate = new DateTime(1990, 1, 1)
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. CreateStudentDto Student'e map ediliyor.
        _mockMapper.Setup(m => m.Map<Student>(It.IsAny<CreateStudentDto>()))
            .Returns(student);
        
        // DÜZELTME: Mock repository'nin CreateAsync metodu için setup yapılıyor. Öğrenci ekleniyor.
        _mockStudentRepository.Setup(r => r.CreateAsync(It.IsAny<Student>()))
            .Returns(Task.CompletedTask);
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (1 row affected) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync())
            .ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _studentManager.CreateAsync(createStudentDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor. CreateAsync ve CommitAsync metodlarının çağrıldığı doğrulanıyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.StudentCreateSuccessMessage);
        
        // DÜZELTME: Mock metodların çağrıldığı doğrulanıyor. Veritabanı işlemlerinin doğru sırada yapıldığı kontrol ediliyor.
        _mockStudentRepository.Verify(r => r.CreateAsync(It.IsAny<Student>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    /// <summary>
    /// CreateAsync metodunun null öğrenci bilgileri ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null entity durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenEntityIsNull_ReturnsErrorResult()
    {
        // Arrange - Null entity durumu hazırlanıyor
        // DÜZELTME: Null CreateStudentDto ile test yapılıyor. Null check'in doğru çalıştığı kontrol ediliyor.
        CreateStudentDto? nullDto = null;

        // Act - Test edilecek metod çağrılıyor
        var result = await _studentManager.CreateAsync(nullDto!);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Öğrenci bilgileri boş olamaz.");
        
        // DÜZELTME: Mock metodların çağrılmadığı doğrulanıyor. Null check sonrası erken return yapılıyor.
        _mockStudentRepository.Verify(r => r.CreateAsync(It.IsAny<Student>()), Times.Never);
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
        var createStudentDto = new CreateStudentDto
        {
            Name = "Ahmet",
            Surname = "Yılmaz",
            TC = "12345678901",
            BirthDate = new DateTime(1990, 1, 1)
        };

        var student = new Student
        {
            Name = "Ahmet",
            Surname = "Yılmaz",
            TC = "12345678901",
            BirthDate = new DateTime(1990, 1, 1)
        };

        _mockMapper.Setup(m => m.Map<Student>(It.IsAny<CreateStudentDto>()))
            .Returns(student);
        
        _mockStudentRepository.Setup(r => r.CreateAsync(It.IsAny<Student>()))
            .Returns(Task.CompletedTask);
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu 0 döndürüyor. Commit başarısız olduğunda hata mesajı döndürülmesi bekleniyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync())
            .ReturnsAsync(0);

        // Act - Test edilecek metod çağrılıyor
        var result = await _studentManager.CreateAsync(createStudentDto);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Commit başarısız olduğunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(ConstantsMessages.StudentCreateFailedMessage);
    }

    #endregion

    #region Update Tests

    /// <summary>
    /// Update metodunun geçerli öğrenci bilgileri ile başarılı güncelleme yapmasını test eder.
    /// Mock veritabanında öğrenci güncelleniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task Update_WhenValidStudent_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test öğrencisi güncelleme verisi oluşturuluyor. Mock repository ve mapper için setup yapılıyor.
        var updateStudentDto = new UpdateStudentDto
        {
            Id = "1",
            Name = "Ahmet Updated",
            Surname = "Yılmaz Updated",
            TC = "12345678901",
            BirthDate = new DateTime(1990, 1, 1)
        };

        var student = new Student
        {
            ID = "1",
            Name = "Ahmet Updated",
            Surname = "Yılmaz Updated",
            TC = "12345678901",
            BirthDate = new DateTime(1990, 1, 1)
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. UpdateStudentDto Student'e map ediliyor.
        _mockMapper.Setup(m => m.Map<Student>(It.IsAny<UpdateStudentDto>()))
            .Returns(student);
        
        // DÜZELTME: Mock repository'nin Update metodu için setup yapılıyor. Öğrenci güncelleniyor.
        _mockStudentRepository.Setup(r => r.Update(It.IsAny<Student>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (1 row affected) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync())
            .ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _studentManager.Update(updateStudentDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor. Update ve CommitAsync metodlarının çağrıldığı doğrulanıyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.StudentUpdateSuccessMessage);
        
        // DÜZELTME: Mock metodların çağrıldığı doğrulanıyor. Veritabanı işlemlerinin doğru sırada yapıldığı kontrol ediliyor.
        _mockStudentRepository.Verify(r => r.Update(It.IsAny<Student>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    /// <summary>
    /// Update metodunun null öğrenci bilgileri ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null entity durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task Update_WhenEntityIsNull_ReturnsErrorResult()
    {
        // Arrange - Null entity durumu hazırlanıyor
        // DÜZELTME: Null UpdateStudentDto ile test yapılıyor. Null check'in doğru çalıştığı kontrol ediliyor.
        UpdateStudentDto? nullDto = null;

        // Act - Test edilecek metod çağrılıyor
        var result = await _studentManager.Update(nullDto!);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Güncellenecek öğrenci bilgileri boş olamaz.");
        
        // DÜZELTME: Mock metodların çağrılmadığı doğrulanıyor. Null check sonrası erken return yapılıyor.
        _mockStudentRepository.Verify(r => r.Update(It.IsAny<Student>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    #endregion

    #region Remove Tests

    /// <summary>
    /// Remove metodunun geçerli öğrenci bilgileri ile başarılı silme yapmasını test eder.
    /// Mock veritabanından öğrenci siliniyor ve başarılı sonuç bekleniyor.
    /// </summary>
    [Fact]
    public async Task Remove_WhenValidStudent_ReturnsSuccessResult()
    {
        // Arrange - Test verisi hazırlanıyor
        // DÜZELTME: Test öğrencisi silme verisi oluşturuluyor. Mock repository ve mapper için setup yapılıyor.
        var deleteStudentDto = new DeleteStudentDto
        {
            Id = "1",
            Name = "Ahmet",
            Surname = "Yılmaz",
            TC = "12345678901",
            BirthDate = new DateTime(1990, 1, 1)
        };

        var student = new Student
        {
            ID = "1",
            Name = "Ahmet",
            Surname = "Yılmaz",
            TC = "12345678901",
            BirthDate = new DateTime(1990, 1, 1)
        };

        // DÜZELTME: Mock mapper'ın Map metodu için setup yapılıyor. DeleteStudentDto Student'e map ediliyor.
        _mockMapper.Setup(m => m.Map<Student>(It.IsAny<DeleteStudentDto>()))
            .Returns(student);
        
        // DÜZELTME: Mock repository'nin Remove metodu için setup yapılıyor. Öğrenci siliniyor.
        _mockStudentRepository.Setup(r => r.Remove(It.IsAny<Student>()));
        
        // DÜZELTME: Mock UnitOfWork'in CommitAsync metodu için setup yapılıyor. Başarılı commit (1 row affected) döndürüyor.
        _mockUnitOfWork.Setup(u => u.CommitAsync())
            .ReturnsAsync(1);

        // Act - Test edilecek metod çağrılıyor
        var result = await _studentManager.Remove(deleteStudentDto);

        // Assert - Başarılı sonuç doğrulanıyor
        // DÜZELTME: SuccessResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor. Remove ve CommitAsync metodlarının çağrıldığı doğrulanıyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<SuccessResult>();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(ConstantsMessages.StudentDeleteSuccessMessage);
        
        // DÜZELTME: Mock metodların çağrıldığı doğrulanıyor. Veritabanı işlemlerinin doğru sırada yapıldığı kontrol ediliyor.
        _mockStudentRepository.Verify(r => r.Remove(It.IsAny<Student>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    /// <summary>
    /// Remove metodunun null öğrenci bilgileri ile çağrıldığında hata mesajı döndürmesini test eder.
    /// Null entity durumunda validasyon hatası döndürülüyor.
    /// </summary>
    [Fact]
    public async Task Remove_WhenEntityIsNull_ReturnsErrorResult()
    {
        // Arrange - Null entity durumu hazırlanıyor
        // DÜZELTME: Null DeleteStudentDto ile test yapılıyor. Null check'in doğru çalıştığı kontrol ediliyor.
        DeleteStudentDto? nullDto = null;

        // Act - Test edilecek metod çağrılıyor
        var result = await _studentManager.Remove(nullDto!);

        // Assert - Hata sonucu doğrulanıyor
        // DÜZELTME: Null entity durumunda ErrorResult döndüğü ve doğru mesajın gösterildiği kontrol ediliyor.
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorResult>();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Silinecek öğrenci bilgileri boş olamaz.");
        
        // DÜZELTME: Mock metodların çağrılmadığı doğrulanıyor. Null check sonrası erken return yapılıyor.
        _mockStudentRepository.Verify(r => r.Remove(It.IsAny<Student>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    #endregion
}


