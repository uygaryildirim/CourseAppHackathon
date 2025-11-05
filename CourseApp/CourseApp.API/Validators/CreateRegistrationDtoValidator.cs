using CourseApp.EntityLayer.Dto.RegistrationDto;
using FluentValidation;

namespace CourseApp.API.Validators;

// DÜZELTME: CreateRegistrationDto için FluentValidation validator eklendi. Kayıt oluşturma işlemlerinde girdi doğrulaması yapılıyor.
public class CreateRegistrationDtoValidator : AbstractValidator<CreateRegistrationDto>
{
    public CreateRegistrationDtoValidator()
    {
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

