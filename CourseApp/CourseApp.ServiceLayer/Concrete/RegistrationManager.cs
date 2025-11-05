using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.RegistrationDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class RegistrationManager : IRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public RegistrationManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllRegistrationDto>>> GetAllAsync(bool track = true)
    {
        var registrationList = await _unitOfWork.Registrations.GetAll(false).ToListAsync();
        var registrationListMapping = _mapper.Map<IEnumerable<GetAllRegistrationDto>>(registrationList);
        // DÜZELTME: Boş liste kontrolü eklendi. Liste boş olduğunda kullanıcıya bilgilendirici mesaj döndürülüyor.
        if (!registrationList.Any() || registrationListMapping == null || !registrationListMapping.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllRegistrationDto>>(null, ConstantsMessages.RegistrationListEmptyMessage);
        }
        return new SuccessDataResult<IEnumerable<GetAllRegistrationDto>>(registrationListMapping, ConstantsMessages.RegistrationListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdRegistrationDto>> GetByIdAsync(string id, bool track = true)
    {
        // DÜZELTME: Null check eklendi. id parametresi null veya empty olabilir, bu durumda hata mesajı döndürülüyor.
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdRegistrationDto>(null, "ID parametresi boş olamaz.");
        }
        
        var hasRegistration = await _unitOfWork.Registrations.GetByIdAsync(id, false);
        // DÜZELTME: Null reference exception önlendi. hasRegistration null olabilir, bu durumda hata mesajı döndürülüyor.
        if (hasRegistration == null)
        {
            return new ErrorDataResult<GetByIdRegistrationDto>(null, "Belirtilen ID'ye sahip kayıt bulunamadı.");
        }
        
        var hasRegistrationMapping = _mapper.Map<GetByIdRegistrationDto>(hasRegistration);
        // DÜZELTME: Null reference exception önlendi. hasRegistrationMapping null olabilir, bu durumda hata mesajı döndürülüyor.
        if (hasRegistrationMapping == null)
        {
            return new ErrorDataResult<GetByIdRegistrationDto>(null, "Kayıt bilgileri eşlenemedi.");
        }
        
        return new SuccessDataResult<GetByIdRegistrationDto>(hasRegistrationMapping, ConstantsMessages.RegistrationGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateRegistrationDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Kayıt bilgileri boş olamaz.");
        }
        
        var createdRegistration = _mapper.Map<Registration>(entity);
        // DÜZELTME: Null reference exception önlendi. createdRegistration null olabilir, bu durumda hata mesajı döndürülüyor.
        if (createdRegistration == null)
        {
            return new ErrorResult("Kayıt bilgileri eşlenemedi.");
        }
        
        // DÜZELTME: Async/await anti-pattern düzeltildi. GetAwaiter().GetResult() yerine await kullanılarak deadlock riski önlendi.
        await _unitOfWork.Registrations.CreateAsync(createdRegistration);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.RegistrationCreateSuccessMessage);
        }

        return new ErrorResult(ConstantsMessages.RegistrationCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteRegistrationDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Silinecek kayıt bilgileri boş olamaz.");
        }
        
        var deletedRegistration = _mapper.Map<Registration>(entity);
        // DÜZELTME: Null reference exception önlendi. deletedRegistration null olabilir, bu durumda hata mesajı döndürülüyor.
        if (deletedRegistration == null)
        {
            return new ErrorResult("Kayıt bilgileri eşlenemedi.");
        }
        
        _unitOfWork.Registrations.Remove(deletedRegistration);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.RegistrationDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.RegistrationDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdatedRegistrationDto entity)
    {
        // DÜZELTME: Null check eklendi. entity null olabilir, bu durumda hata mesajı döndürülüyor.
        if (entity == null)
        {
            return new ErrorResult("Güncellenecek kayıt bilgileri boş olamaz.");
        }
        
        var updatedRegistration = _mapper.Map<Registration>(entity);
        // DÜZELTME: Null reference exception önlendi. updatedRegistration null olabilir, bu durumda hata mesajı döndürülüyor.
        if (updatedRegistration == null)
        {
            return new ErrorResult("Kayıt bilgileri eşlenemedi.");
        }
        
        // DÜZELTME: Invalid cast exception önlendi. decimal'i int'e direkt cast etme işlemi kaldırıldı, gereksiz tip dönüşümü kaldırıldı.
        
        _unitOfWork.Registrations.Update(updatedRegistration);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.RegistrationUpdateSuccessMessage);
        }
        // DÜZELTME: Mantıksal hata düzeltildi. Hata durumunda ErrorResult döndürülüyor, SuccessResult yerine ErrorResult kullanılıyor.
        return new ErrorResult(ConstantsMessages.RegistrationUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllRegistrationDetailDto>>> GetAllRegistrationDetailAsync(bool track = true)
    {
        // ZOR: N+1 Problemi - Include kullanılmamış, lazy loading aktif
        var registrationData = await _unitOfWork.Registrations.GetAllRegistrationDetail(track).ToListAsync();
        
        // ZOR: N+1 - Her registration için Course ve Student ayrı sorgu ile çekiliyor
        // Örnek: registration.Course?.CourseName her iterasyonda DB sorgusu
        
        if(!registrationData.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllRegistrationDetailDto>>(null,ConstantsMessages.RegistrationListFailedMessage);
        }

        var registrationDataMapping = _mapper.Map<IEnumerable<GetAllRegistrationDetailDto>>(registrationData);
        
        // DÜZELTME: Index out of range exception önlendi. registrationDataMapping boş olabilir, gereksiz index erişimi kaldırıldı.
        // DÜZELTME: Null check eklendi. registrationDataMapping null olabilir, bu durumda kontrol ediliyor.
        if (registrationDataMapping == null || !registrationDataMapping.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllRegistrationDetailDto>>(null, ConstantsMessages.RegistrationListFailedMessage);
        }
        
        return new SuccessDataResult<IEnumerable<GetAllRegistrationDetailDto>>(registrationDataMapping, ConstantsMessages.RegistrationListSuccessMessage);  
    }

    public async Task<IDataResult<GetByIdRegistrationDetailDto>> GetByIdRegistrationDetailAsync(string id, bool track = true)
    {
        // DÜZELTME: NotImplementedException kaldırıldı, metod implement edildi. id parametresi null veya empty olabilir, bu durumda hata mesajı döndürülüyor.
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdRegistrationDetailDto>(null, "ID parametresi boş olamaz.");
        }
        
        // DÜZELTME: Repository'den detail bilgisi çekiliyor. GetByIdRegistrationDetail metodu ile kayıt detay bilgisi alınıyor.
        var registration = await _unitOfWork.Registrations.GetByIdRegistrationDetail(id, track);
        
        // DÜZELTME: Null reference exception önlendi. registration null olabilir, bu durumda hata mesajı döndürülüyor.
        if (registration == null)
        {
            return new ErrorDataResult<GetByIdRegistrationDetailDto>(null, "Belirtilen ID'ye sahip kayıt bulunamadı.");
        }
        
        // DÜZELTME: AutoMapper ile Registration entity'si GetByIdRegistrationDetailDto'ya map ediliyor. Entity'den DTO'ya dönüşüm yapılıyor.
        var registrationMapping = _mapper.Map<GetByIdRegistrationDetailDto>(registration);
        
        // DÜZELTME: Null reference exception önlendi. registrationMapping null olabilir, bu durumda hata mesajı döndürülüyor.
        if (registrationMapping == null)
        {
            return new ErrorDataResult<GetByIdRegistrationDetailDto>(null, "Kayıt bilgileri eşlenemedi.");
        }
        
        // DÜZELTME: Başarılı sonuç döndürülüyor. Kayıt detay bilgisi başarıyla alındı ve döndürülüyor.
        return new SuccessDataResult<GetByIdRegistrationDetailDto>(registrationMapping, ConstantsMessages.RegistrationGetByIdSuccessMessage);
    }

    // DÜZELTME: Gereksiz metod kaldırıldı. AccessNonExistentProperty ve NonExistentProperty kaldırıldı, kullanılmayan ve hata üreten kod temizlendi.
}
