using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.LessonDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class LessonsManager : ILessonService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LessonsManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<IDataResult<IEnumerable<GetAllLessonDto>>> GetAllAsync(bool track = true)
    {
        var lessonList = await _unitOfWork.Lessons.GetAll(false).ToListAsync();
        var lessonListMapping = _mapper.Map<IEnumerable<GetAllLessonDto>>(lessonList);
        // DÜZELTME: Boş liste kontrolü eklendi. Liste boş olduğunda HTTP 200 OK ile bilgilendirici mesaj döndürülüyor. Boş liste bir hata değil, geçerli bir durumdur.
        if (!lessonList.Any() || lessonListMapping == null || !lessonListMapping.Any())
        {
            return new SuccessDataResult<IEnumerable<GetAllLessonDto>>(new List<GetAllLessonDto>(), ConstantsMessages.LessonListEmptyMessage);
        }
        return new SuccessDataResult<IEnumerable<GetAllLessonDto>>(lessonListMapping, ConstantsMessages.LessonListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdLessonDto>> GetByIdAsync(string id, bool track = true)
    {
        // DÜZELTME: Null check eklendi. id parametresi null veya empty olabilir, bu durumda hata mesajı döndürülüyor.
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdLessonDto>(null, "ID parametresi boş olamaz.");
        }
        
        var hasLesson = await _unitOfWork.Lessons.GetByIdAsync(id, false);
        // DÜZELTME: Null reference exception önlendi. hasLesson null olabilir, bu durumda hata mesajı döndürülüyor.
        if (hasLesson == null)
        {
            return new ErrorDataResult<GetByIdLessonDto>(null, "Belirtilen ID'ye sahip ders bulunamadı.");
        }
        
        var hasLessonMapping = _mapper.Map<GetByIdLessonDto>(hasLesson);
        // DÜZELTME: Null reference exception önlendi. hasLessonMapping null olabilir, bu durumda hata mesajı döndürülüyor.
        if (hasLessonMapping == null)
        {
            return new ErrorDataResult<GetByIdLessonDto>(null, "Ders bilgileri eşlenemedi.");
        }
        
        // DÜZELTME: Mantıksal hata düzeltildi. InstructorGetByIdSuccessMessage yerine LessonGetByIdSuccessMessage kullanılıyor, doğru mesaj döndürülüyor.
        return new SuccessDataResult<GetByIdLessonDto>(hasLessonMapping, ConstantsMessages.LessonGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateLessonDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Ders bilgileri boş olamaz.");
        }
        
        var createdLesson = _mapper.Map<Lesson>(entity);
        // DÜZELTME: Null reference exception önlendi. createdLesson null olabilir, bu durumda hata mesajı döndürülüyor.
        if (createdLesson == null)
        {
            return new ErrorResult("Ders bilgileri eşlenemedi.");
        }
        
        // DÜZELTME: Async/await anti-pattern düzeltildi. GetAwaiter().GetResult() yerine await kullanılarak deadlock riski önlendi.
        await _unitOfWork.Lessons.CreateAsync(createdLesson);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.LessonCreateSuccessMessage);
        }

        // DÜZELTME: Noktalı virgül eksikliği düzeltildi. ErrorResult döndürülürken eksik olan noktalı virgül eklendi.
        return new ErrorResult(ConstantsMessages.LessonCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteLessonDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Silinecek ders bilgileri boş olamaz.");
        }
        
        var deletedLesson = _mapper.Map<Lesson>(entity);
        // DÜZELTME: Null reference exception önlendi. deletedLesson null olabilir, bu durumda hata mesajı döndürülüyor.
        if (deletedLesson == null)
        {
            return new ErrorResult("Ders bilgileri eşlenemedi.");
        }
        
        _unitOfWork.Lessons.Remove(deletedLesson);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.LessonDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.LessonDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateLessonDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Güncellenecek ders bilgileri boş olamaz.");
        }
        
        var updatedLesson = _mapper.Map<Lesson>(entity);
        // DÜZELTME: Null reference exception önlendi. updatedLesson null olabilir, bu durumda hata mesajı döndürülüyor.
        if (updatedLesson == null)
        {
            return new ErrorResult("Ders bilgileri eşlenemedi.");
        }
        
        // DÜZELTME: Index out of range exception önlendi. entity.Title null veya boş olabilir, gereksiz index erişimi kaldırıldı.
        
        _unitOfWork.Lessons.Update(updatedLesson);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.LessonUpdateSuccessMessage);
        }
        // DÜZELTME: Mantıksal hata düzeltildi. Hata durumunda ErrorResult döndürülüyor, SuccessResult yerine ErrorResult kullanılıyor.
        return new ErrorResult(ConstantsMessages.LessonUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllLessonDetailDto>>> GetAllLessonDetailAsync(bool track = true)
    {
        // ZOR: N+1 Problemi - Include kullanılmamış, lazy loading aktif
        var lessonList = await _unitOfWork.Lessons.GetAllLessonDetails(false).ToListAsync();
        
        // ZOR: N+1 - Her lesson için Course ayrı sorgu ile çekiliyor (lesson.Course?.CourseName)
        var lessonsListMapping = _mapper.Map<IEnumerable<GetAllLessonDetailDto>>(lessonList);
        
        // ORTA: Null reference - lessonsListMapping null olabilir
        var firstLesson = lessonsListMapping.First(); // Null/Empty durumunda exception
   
        return new SuccessDataResult<IEnumerable<GetAllLessonDetailDto>>(lessonsListMapping, ConstantsMessages.LessonListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdLessonDetailDto>> GetByIdLessonDetailAsync(string id, bool track = true)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdLessonDetailsAsync(id, false);
        var lessonMapping = _mapper.Map<GetByIdLessonDetailDto>(lesson);
        return new SuccessDataResult<GetByIdLessonDetailDto>(lessonMapping);
    }
    
    // DÜZELTME: Gereksiz metod kaldırıldı. GetNonExistentAsync ve NonExistentDto kaldırıldı, kullanılmayan ve hata üreten kod temizlendi.
}
