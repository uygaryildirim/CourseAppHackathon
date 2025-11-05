using CourseApp.EntityLayer.Dto.RegistrationDto;
using FluentValidation;

namespace CourseApp.API.Validators;

// DÜZELTME: UpdatedRegistrationDto için FluentValidation validator eklendi. Kayıt güncelleme işlemlerinde girdi doğrulaması yapılıyor.
public class UpdatedRegistrationDtoValidator : AbstractValidator<UpdatedRegistrationDto>
{
    public UpdatedRegistrationDtoValidator()
    {
        // DÜZELTME: ID alanı için validation kuralları. ID boş olamaz.
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Kayıt ID boş olamaz.");

        // DÜZELTME: Price alanı için validation kuralları. Price pozitif olmalı, 0'dan büyük olmalı.
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(999999.99m).WithMessage("Fiyat çok yüksek olamaz.");

        // DÜZELTME: StudentID alanı için validation kuralları. StudentID boş olamaz.
        RuleFor(x => x.StudentID)
            .NotEmpty().WithMessage("Öğrenci ID boş olamaz.");

        // DÜZELTME: CourseID alanı için validation kuralları. CourseID boş olamaz.
        RuleFor(x => x.CourseID)
            .NotEmpty().WithMessage("Kurs ID boş olamaz.");

        // DÜZELTME: RegistrationDate alanı için validation kuralları. RegistrationDate bugünden önce olamaz.
        RuleFor(x => x.RegistrationDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Kayıt tarihi bugünden önce olamaz.");
    }
}

