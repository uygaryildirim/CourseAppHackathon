using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.ExamResultDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CourseApp.ServiceLayer.Concrete;

public class ExamResultManager : IExamResultService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamResultManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllExamResultDto>>> GetAllAsync(bool track = true)
    {
        var examResultList = await _unitOfWork.ExamResults.GetAll(false).ToListAsync();
        var examResultListMapping = _mapper.Map<IEnumerable<GetAllExamResultDto>>(examResultList);
        // DÜZELTME: Boş liste kontrolü eklendi. Liste boş olduğunda HTTP 200 OK ile bilgilendirici mesaj döndürülüyor. Boş liste bir hata değil, geçerli bir durumdur.
        if (!examResultList.Any() || examResultListMapping == null || !examResultListMapping.Any())
        {
            return new SuccessDataResult<IEnumerable<GetAllExamResultDto>>(new List<GetAllExamResultDto>(), ConstantsMessages.ExamResultListEmptyMessage);
        }
        return new SuccessDataResult<IEnumerable<GetAllExamResultDto>>(examResultListMapping, ConstantsMessages.ExamResultListSuccessMessage);

    }

    public async Task<IDataResult<GetByIdExamResultDto>> GetByIdAsync(string id, bool track = true)
    {
        // DÜZELTME: Null check eklendi. id parametresi null veya empty olabilir, bu durumda hata mesajı döndürülüyor.
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdExamResultDto>(null, "ID parametresi boş olamaz.");
        }
        
        var hasExamResult = await _unitOfWork.ExamResults.GetByIdAsync(id, false);
        // DÜZELTME: Null reference exception önlendi. hasExamResult null olabilir, bu durumda hata mesajı döndürülüyor.
        if (hasExamResult == null)
        {
            return new ErrorDataResult<GetByIdExamResultDto>(null, "Belirtilen ID'ye sahip sınav sonucu bulunamadı.");
        }
        
        var examResultMapping = _mapper.Map<GetByIdExamResultDto>(hasExamResult);
        // DÜZELTME: Null reference exception önlendi. examResultMapping null olabilir, bu durumda hata mesajı döndürülüyor.
        if (examResultMapping == null)
        {
            return new ErrorDataResult<GetByIdExamResultDto>(null, "Sınav sonucu bilgileri eşlenemedi.");
        }
        
        // DÜZELTME: Yanlış mesaj düzeltildi. ExamResultListSuccessMessage yerine ExamResultGetByIdSuccessMessage kullanılıyor.
        return new SuccessDataResult<GetByIdExamResultDto>(examResultMapping, ConstantsMessages.ExamResultGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateExamResultDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Sınav sonucu bilgileri boş olamaz.");
        }
        
        var addedExamResultMapping = _mapper.Map<ExamResult>(entity);
        // DÜZELTME: Null reference exception önlendi. addedExamResultMapping null olabilir, bu durumda hata mesajı döndürülüyor.
        if (addedExamResultMapping == null)
        {
            return new ErrorResult("Sınav sonucu bilgileri eşlenemedi.");
        }
        
        await _unitOfWork.ExamResults.CreateAsync(addedExamResultMapping);
        // DÜZELTME: Async/await anti-pattern düzeltildi. GetAwaiter().GetResult() yerine await kullanılarak deadlock riski önlendi.
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamResultCreateSuccessMessage);
        }
        // DÜZELTME: Eksik noktalı virgül eklendi. C# syntax gereği her statement'ın sonunda noktalı virgül olmalı, derleme hatası önlendi.
        return new ErrorResult(ConstantsMessages.ExamResultCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteExamResultDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Silinecek sınav sonucu bilgileri boş olamaz.");
        }
        
        var deletedExamResultMapping = _mapper.Map<ExamResult>(entity);
        // DÜZELTME: Null reference exception önlendi. deletedExamResultMapping null olabilir, bu durumda hata mesajı döndürülüyor.
        if (deletedExamResultMapping == null)
        {
            return new ErrorResult("Sınav sonucu bilgileri eşlenemedi.");
        }
        
        _unitOfWork.ExamResults.Remove(deletedExamResultMapping);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamResultDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamResultDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateExamResultDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Güncellenecek sınav sonucu bilgileri boş olamaz.");
        }
        
        var updatedExamResultMapping = _mapper.Map<ExamResult>(entity);
        // DÜZELTME: Null reference exception önlendi. updatedExamResultMapping null olabilir, bu durumda hata mesajı döndürülüyor.
        if (updatedExamResultMapping == null)
        {
            return new ErrorResult("Sınav sonucu bilgileri eşlenemedi.");
        }
        
        _unitOfWork.ExamResults.Update(updatedExamResultMapping);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamResultUpdateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamResultUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllExamResultDetailDto>>> GetAllExamResultDetailAsync(bool track = true)
    {
        // ZOR: N+1 Problemi - Include kullanılmamış, lazy loading aktif
        var examResultList = await _unitOfWork.ExamResults.GetAllExamResultDetail(false).ToListAsync();
        
        // ZOR: N+1 - Her examResult için Student ve Exam ayrı sorgu ile çekiliyor
        // Örnek: examResult.Student?.Name ve examResult.Exam?.Name her iterasyonda DB sorgusu
        
        if (!examResultList.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllExamResultDetailDto>>(null, ConstantsMessages.ExamResultListFailedMessage);
        }

        var examResultListMapping = _mapper.Map<IEnumerable<GetAllExamResultDetailDto>>(examResultList);
        
        // DÜZELTME: Index out of range exception önlendi. examResultListMapping boş olabilir, gereksiz index erişimi kaldırıldı.
        // Gereksiz firstResult değişkeni kaldırıldı, sadece liste döndürülüyor.
        
        return new SuccessDataResult<IEnumerable<GetAllExamResultDetailDto>>(examResultListMapping, ConstantsMessages.ExamResultListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdExamResultDetailDto>> GetByIdExamResultDetailAsync(string id, bool track = true)
    {
        // DÜZELTME: NotImplementedException kaldırıldı, metod implement edildi. id parametresi null veya empty olabilir, bu durumda hata mesajı döndürülüyor.
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdExamResultDetailDto>(null, "ID parametresi boş olamaz.");
        }
        
        // DÜZELTME: Repository'den detail bilgisi çekiliyor. GetByIdExamResultDetailAsync metodu ile sınav sonucu detay bilgisi alınıyor.
        var examResult = await _unitOfWork.ExamResults.GetByIdExamResultDetailAsync(id, track);
        
        // DÜZELTME: Null reference exception önlendi. examResult null olabilir, bu durumda hata mesajı döndürülüyor.
        if (examResult == null)
        {
            return new ErrorDataResult<GetByIdExamResultDetailDto>(null, "Belirtilen ID'ye sahip sınav sonucu bulunamadı.");
        }
        
        // DÜZELTME: AutoMapper ile ExamResult entity'si GetByIdExamResultDetailDto'ya map ediliyor. Entity'den DTO'ya dönüşüm yapılıyor.
        var examResultMapping = _mapper.Map<GetByIdExamResultDetailDto>(examResult);
        
        // DÜZELTME: Null reference exception önlendi. examResultMapping null olabilir, bu durumda hata mesajı döndürülüyor.
        if (examResultMapping == null)
        {
            return new ErrorDataResult<GetByIdExamResultDetailDto>(null, "Sınav sonucu bilgileri eşlenemedi.");
        }
        
        // DÜZELTME: Başarılı sonuç döndürülüyor. Sınav sonucu detay bilgisi başarıyla alındı ve döndürülüyor.
        return new SuccessDataResult<GetByIdExamResultDetailDto>(examResultMapping, ConstantsMessages.ExamResultGetByIdSuccessMessage);
    }

    // DÜZELTME: Gereksiz metod kaldırıldı. CallMissingMethod ve MissingMethodHelper kaldırıldı, kullanılmayan ve hata üreten kod temizlendi.
}
