using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.InstructorDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class InstructorManager : IInstructorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public InstructorManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllInstructorDto>>> GetAllAsync(bool track = true)
    {
        var instructorList = await _unitOfWork.Instructors.GetAll(false).ToListAsync();
        var instructorListMapping = _mapper.Map<IEnumerable<GetAllInstructorDto>>(instructorList);
        // DÜZELTME: Boş liste kontrolü eklendi. Liste boş olduğunda HTTP 200 OK ile bilgilendirici mesaj döndürülüyor. Boş liste bir hata değil, geçerli bir durumdur.
        if (!instructorList.Any() || instructorListMapping == null || !instructorListMapping.Any())
        {
            return new SuccessDataResult<IEnumerable<GetAllInstructorDto>>(new List<GetAllInstructorDto>(), ConstantsMessages.InstructorListEmptyMessage);
        }
        return new SuccessDataResult<IEnumerable<GetAllInstructorDto>>(instructorListMapping, ConstantsMessages.InstructorListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdInstructorDto>> GetByIdAsync(string id, bool track = true)
    {
        // DÜZELTME: Null check eklendi. id parametresi null veya empty olabilir, bu durumda hata mesajı döndürülüyor.
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdInstructorDto>(null, "ID parametresi boş olamaz.");
        }
        
        // DÜZELTME: Index out of range exception önlendi. id uzunluğu kontrol edilmeden index erişimi yapılıyordu, gereksiz index erişimi kaldırıldı.
        
        var hasInstructor = await _unitOfWork.Instructors.GetByIdAsync(id, false);
        // DÜZELTME: Null reference exception önlendi. hasInstructor null olabilir, bu durumda hata mesajı döndürülüyor.
        if (hasInstructor == null)
        {
            return new ErrorDataResult<GetByIdInstructorDto>(null, "Belirtilen ID'ye sahip eğitmen bulunamadı.");
        }
        
        var hasInstructorMapping = _mapper.Map<GetByIdInstructorDto>(hasInstructor);
        // DÜZELTME: Null reference exception önlendi. hasInstructorMapping null olabilir, bu durumda hata mesajı döndürülüyor.
        if (hasInstructorMapping == null)
        {
            return new ErrorDataResult<GetByIdInstructorDto>(null, "Eğitmen bilgileri eşlenemedi.");
        }
        
        return new SuccessDataResult<GetByIdInstructorDto>(hasInstructorMapping, ConstantsMessages.InstructorGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreatedInstructorDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Eğitmen bilgileri boş olamaz.");
        }
        
        var createdInstructor = _mapper.Map<Instructor>(entity);
        // DÜZELTME: Null reference exception önlendi. createdInstructor null olabilir, bu durumda hata mesajı döndürülüyor.
        if (createdInstructor == null)
        {
            return new ErrorResult("Eğitmen bilgileri eşlenemedi.");
        }
        
        await _unitOfWork.Instructors.CreateAsync(createdInstructor);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.InstructorCreateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.InstructorCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeletedInstructorDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Silinecek eğitmen bilgileri boş olamaz.");
        }
        
        var deletedInstructor = _mapper.Map<Instructor>(entity);
        // DÜZELTME: Null reference exception önlendi. deletedInstructor null olabilir, bu durumda hata mesajı döndürülüyor.
        if (deletedInstructor == null)
        {
            return new ErrorResult("Eğitmen bilgileri eşlenemedi.");
        }
        
        _unitOfWork.Instructors.Remove(deletedInstructor);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.InstructorDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.InstructorDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdatedInstructorDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Güncellenecek eğitmen bilgileri boş olamaz.");
        }
        
        var updatedInstructor = _mapper.Map<Instructor>(entity);
        // DÜZELTME: Null reference exception önlendi. updatedInstructor null olabilir, bu durumda hata mesajı döndürülüyor.
        if (updatedInstructor == null)
        {
            return new ErrorResult("Eğitmen bilgileri eşlenemedi.");
        }
        
        _unitOfWork.Instructors.Update(updatedInstructor);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.InstructorUpdateSuccessMessage);
        }
        // DÜZELTME: Mantıksal hata düzeltildi. Hata durumunda ErrorResult döndürülüyor, SuccessResult yerine ErrorResult kullanılıyor.
        return new ErrorResult(ConstantsMessages.InstructorUpdateFailedMessage);
    }

    // DÜZELTME: Gereksiz metod kaldırıldı. UseNonExistentNamespace ve NonExistentNamespace kaldırıldı, kullanılmayan ve hata üreten kod temizlendi.
}
