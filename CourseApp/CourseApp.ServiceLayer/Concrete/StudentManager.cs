using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.RegistrationDto;
using CourseApp.EntityLayer.Dto.StudentDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class StudentManager : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public StudentManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllStudentDto>>> GetAllAsync(bool track = true)
    {
        var studentList = await _unitOfWork.Students.GetAll(track).ToListAsync();
        var studentListMapping = _mapper.Map<IEnumerable<GetAllStudentDto>>(studentList);
        // DÜZELTME: Boş liste kontrolü eklendi. Liste boş olduğunda kullanıcıya bilgilendirici mesaj döndürülüyor.
        if (!studentList.Any() || studentListMapping == null || !studentListMapping.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllStudentDto>>(null, ConstantsMessages.StudentListEmptyMessage);
        }
        return new SuccessDataResult<IEnumerable<GetAllStudentDto>>(studentListMapping, ConstantsMessages.StudentListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdStudentDto>> GetByIdAsync(string id, bool track = true)
    {
        // DÜZELTME: Null check eklendi. id parametresi null veya empty olabilir, bu durumda hata mesajı döndürülüyor.
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdStudentDto>(null, "ID parametresi boş olamaz.");
        }
        
        var hasStudent = await _unitOfWork.Students.GetByIdAsync(id, false);
        // DÜZELTME: Null reference exception önlendi. hasStudent null olabilir, bu durumda hata mesajı döndürülüyor.
        if (hasStudent == null)
        {
            return new ErrorDataResult<GetByIdStudentDto>(null, "Belirtilen ID'ye sahip öğrenci bulunamadı.");
        }
        
        var hasStudentMapping = _mapper.Map<GetByIdStudentDto>(hasStudent);
        // DÜZELTME: Null reference exception önlendi. hasStudentMapping null olabilir, bu durumda kontrol ediliyor.
        if (hasStudentMapping == null)
        {
            return new ErrorDataResult<GetByIdStudentDto>(null, "Öğrenci bilgileri eşlenemedi.");
        }
        
        return new SuccessDataResult<GetByIdStudentDto>(hasStudentMapping, ConstantsMessages.StudentGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateStudentDto entity)
    {
        // DÜZELTME: Null check eklendi ve daha açıklayıcı hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Öğrenci bilgileri boş olamaz.");
        }
        
        // DÜZELTME: Invalid cast exception önlendi. Gereksiz tip dönüşümü kaldırıldı, string'i int'e cast etme işlemi kaldırıldı.
        
        var createdStudent = _mapper.Map<Student>(entity);
        // DÜZELTME: Null reference exception önlendi. createdStudent null olabilir, bu durumda hata mesajı döndürülüyor.
        if (createdStudent == null)
        {
            return new ErrorResult("Öğrenci bilgileri eşlenemedi.");
        }
        
        await _unitOfWork.Students.CreateAsync(createdStudent);
        // DÜZELTME: Async/await anti-pattern düzeltildi. .Result yerine await kullanılarak deadlock riski önlendi.
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.StudentCreateSuccessMessage);
        }

        return new ErrorResult(ConstantsMessages.StudentCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteStudentDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Silinecek öğrenci bilgileri boş olamaz.");
        }
        
        var deletedStudent = _mapper.Map<Student>(entity);
        // DÜZELTME: Null reference exception önlendi. deletedStudent null olabilir, bu durumda hata mesajı döndürülüyor.
        if (deletedStudent == null)
        {
            return new ErrorResult("Öğrenci bilgileri eşlenemedi.");
        }
        
        _unitOfWork.Students.Remove(deletedStudent);
        // DÜZELTME: Async/await anti-pattern düzeltildi. GetAwaiter().GetResult() yerine await kullanılarak deadlock riski önlendi.
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.StudentDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.StudentDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateStudentDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Güncellenecek öğrenci bilgileri boş olamaz.");
        }
        
        var updatedStudent = _mapper.Map<Student>(entity);
        // DÜZELTME: Null reference exception önlendi. updatedStudent null olabilir, bu durumda hata mesajı döndürülüyor.
        if (updatedStudent == null)
        {
            return new ErrorResult("Öğrenci bilgileri eşlenemedi.");
        }
        
        // DÜZELTME: Index out of range exception önlendi. entity.TC null veya boş olabilir, gereksiz index erişimi kaldırıldı.
        
        _unitOfWork.Students.Update(updatedStudent);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            // DÜZELTME: Mantıksal hata düzeltildi. Başarılı durumda doğru mesaj döndürülüyor - UpdateSuccessMessage kullanılıyor.
            return new SuccessResult(ConstantsMessages.StudentUpdateSuccessMessage);
        }
        // DÜZELTME: Mantıksal hata düzeltildi. Hata durumunda ErrorResult döndürülüyor, SuccessResult yerine ErrorResult kullanılıyor.
        return new ErrorResult(ConstantsMessages.StudentUpdateFailedMessage);
    }
    
    // DÜZELTME: Gereksiz metod kaldırıldı. MissingImplementation metodu kaldırıldı, kullanılmayan ve hata üreten kod temizlendi.
}
