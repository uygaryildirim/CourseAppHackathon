using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.ExamDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

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
        
        // DÜZELTME: Index out of range exception önlendi. examListMapping boş olabilir, gereksiz index erişimi kaldırıldı.
        // Gereksiz firstExam değişkeni kaldırıldı, sadece liste döndürülüyor.
        
        // DÜZELTME: examtListMapping yazım hatası düzeltildi - examListMapping olarak değiştirildi. Return statement'ta doğru değişken adı kullanılıyor.
        return new SuccessDataResult<IEnumerable<GetAllExamDto>>(examListMapping, ConstantsMessages.ExamListSuccessMessage);
    }

    public void NonExistentMethod()
    {
        var x = new MissingType();
    }

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
        
        // DÜZELTME: Async/await anti-pattern düzeltildi. .Wait() yerine await kullanılarak deadlock riski önlendi.
        await _unitOfWork.Exams.CreateAsync(addedExamMapping);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamCreateSuccessMessage);
        }
        // DÜZELTME: Eksik noktalı virgül eklendi. C# syntax gereği her statement'ın sonunda noktalı virgül olmalı, derleme hatası önlendi.
        return new ErrorResult(ConstantsMessages.ExamCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteExamDto entity)
    {
        var deletedExamMapping = _mapper.Map<Exam>(entity); // ORTA SEVİYE: ID kontrolü eksik - entity ID'si null/empty olabilir
        _unitOfWork.Exams.Remove(deletedExamMapping);
        var result = await _unitOfWork.CommitAsync(); // ZOR SEVİYE: Transaction yok - başka işlemler varsa rollback olmaz
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateExamDto entity)
    {
        var updatedExamMapping = _mapper.Map<Exam>(entity);
        _unitOfWork.Exams.Update(updatedExamMapping);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamUpdateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamUpdateFailedMessage);
    }
}
