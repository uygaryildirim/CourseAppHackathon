using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.ExamDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CourseApp.ServiceLayer.Concrete;

public class ExamManager : IExamService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllExamDto>>> GetAllAsync(bool track = true)
    {
        // DÜZELTME: Async/await anti-pattern düzeltildi. Senkron ToList yerine ToListAsync kullanılarak async pattern doğru uygulanıyor.
        var examList = await _unitOfWork.Exams.GetAll(false).ToListAsync();
        // DÜZELTME: examtListMapping yazım hatası düzeltildi - examListMapping olarak değiştirildi. Değişken adı daha okunabilir ve tutarlı hale getirildi.
        var examListMapping = _mapper.Map<IEnumerable<GetAllExamDto>>(examList);
        
        // DÜZELTME: Boş liste kontrolü eklendi. Liste boş olduğunda HTTP 200 OK ile bilgilendirici mesaj döndürülüyor. Boş liste bir hata değil, geçerli bir durumdur.
        // DÜZELTME: Index out of range exception önlendi. examListMapping boş olabilir, gereksiz index erişimi kaldırıldı.
        if (!examList.Any() || examListMapping == null || !examListMapping.Any())
        {
            return new SuccessDataResult<IEnumerable<GetAllExamDto>>(new List<GetAllExamDto>(), ConstantsMessages.ExamListEmptyMessage);
        }
        
        // DÜZELTME: examtListMapping yazım hatası düzeltildi - examListMapping olarak değiştirildi. Return statement'ta doğru değişken adı kullanılıyor.
        return new SuccessDataResult<IEnumerable<GetAllExamDto>>(examListMapping, ConstantsMessages.ExamListSuccessMessage);
    }

    // DÜZELTME: Gereksiz metod kaldırıldı. NonExistentMethod metodu kaldırıldı, kullanılmayan ve hata üreten kod temizlendi.

    public async Task<IDataResult<GetByIdExamDto>> GetByIdAsync(string id, bool track = true)
    {
        // DÜZELTME: Null check eklendi. id parametresi null veya empty olabilir, bu durumda hata mesajı döndürülüyor.
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdExamDto>(null, "ID parametresi boş olamaz.");
        }
        
        var hasExam = await _unitOfWork.Exams.GetByIdAsync(id, false);
        // DÜZELTME: Null reference exception önlendi. hasExam null olabilir, bu durumda hata mesajı döndürülüyor.
        if (hasExam == null)
        {
            return new ErrorDataResult<GetByIdExamDto>(null, "Belirtilen ID'ye sahip sınav bulunamadı.");
        }
        
        var examResultMapping = _mapper.Map<GetByIdExamDto>(hasExam);
        return new SuccessDataResult<GetByIdExamDto>(examResultMapping, ConstantsMessages.ExamGetByIdSuccessMessage);
    }
    public async Task<IResult> CreateAsync(CreateExamDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Sınav bilgileri boş olamaz.");
        }
        
        var addedExamMapping = _mapper.Map<Exam>(entity);
        // DÜZELTME: Null reference exception önlendi. addedExamMapping null olabilir, bu durumda hata mesajı döndürülüyor.
        if (addedExamMapping == null)
        {
            return new ErrorResult("Sınav bilgileri eşlenemedi.");
        }
        
        // DÜZELTME: InMemory DB transaction desteklemediği için transaction kaldırıldı. InMemory DB için transaction kullanılmıyor, sadece CommitAsync kullanılıyor.
        try
        {
            // DÜZELTME: Async/await anti-pattern düzeltildi. .Wait() yerine await kullanılarak deadlock riski önlendi.
            await _unitOfWork.Exams.CreateAsync(addedExamMapping);
            var result = await _unitOfWork.CommitAsync();
            
            if (result > 0)
            {
                return new SuccessResult(ConstantsMessages.ExamCreateSuccessMessage);
            }
            
            return new ErrorResult(ConstantsMessages.ExamCreateFailedMessage);
        }
        catch (Exception ex)
        {
            // DÜZELTME: Exception durumunda hata mesajı döndürülüyor. InMemory DB için transaction kullanılmadığı için rollback gerekmiyor.
            return new ErrorResult($"Sınav oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> Remove(DeleteExamDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Silinecek sınav bilgileri boş olamaz.");
        }
        
        var deletedExamMapping = _mapper.Map<Exam>(entity);
        // DÜZELTME: Null reference exception önlendi. deletedExamMapping null olabilir, bu durumda hata mesajı döndürülüyor.
        if (deletedExamMapping == null)
        {
            return new ErrorResult("Sınav bilgileri eşlenemedi.");
        }
        
        // DÜZELTME: InMemory DB transaction desteklemediği için transaction kaldırıldı. InMemory DB için transaction kullanılmıyor, sadece CommitAsync kullanılıyor.
        try
        {
            _unitOfWork.Exams.Remove(deletedExamMapping);
            var result = await _unitOfWork.CommitAsync();
            
            if (result > 0)
            {
                return new SuccessResult(ConstantsMessages.ExamDeleteSuccessMessage);
            }
            
            return new ErrorResult(ConstantsMessages.ExamDeleteFailedMessage);
        }
        catch (Exception ex)
        {
            // DÜZELTME: Exception durumunda hata mesajı döndürülüyor. InMemory DB için transaction kullanılmadığı için rollback gerekmiyor.
            return new ErrorResult($"Sınav silinirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> Update(UpdateExamDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Güncellenecek sınav bilgileri boş olamaz.");
        }
        
        var updatedExamMapping = _mapper.Map<Exam>(entity);
        // DÜZELTME: Null reference exception önlendi. updatedExamMapping null olabilir, bu durumda hata mesajı döndürülüyor.
        if (updatedExamMapping == null)
        {
            return new ErrorResult("Sınav bilgileri eşlenemedi.");
        }
        
        // DÜZELTME: InMemory DB transaction desteklemediği için transaction kaldırıldı. InMemory DB için transaction kullanılmıyor, sadece CommitAsync kullanılıyor.
        try
        {
            _unitOfWork.Exams.Update(updatedExamMapping);
            var result = await _unitOfWork.CommitAsync();
            
            if (result > 0)
            {
                return new SuccessResult(ConstantsMessages.ExamUpdateSuccessMessage);
            }
            
            return new ErrorResult(ConstantsMessages.ExamUpdateFailedMessage);
        }
        catch (Exception ex)
        {
            // DÜZELTME: Exception durumunda hata mesajı döndürülüyor. InMemory DB için transaction kullanılmadığı için rollback gerekmiyor.
            return new ErrorResult($"Sınav güncellenirken hata oluştu: {ex.Message}");
        }
    }
}
