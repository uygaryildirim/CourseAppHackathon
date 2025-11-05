using CourseApp.EntityLayer.Dto.InstructorDto;
using FluentValidation;

namespace CourseApp.API.Validators;

// DÜZELTME: CreatedInstructorDto için FluentValidation validator eklendi. Eğitmen oluşturma işlemlerinde girdi doğrulaması yapılıyor.
public class CreatedInstructorDtoValidator : AbstractValidator<CreatedInstructorDto>
{
    public CreatedInstructorDtoValidator()
    {
        // DÜZELTME: Name alanı için validation kuralları. Name boş olamaz, minimum 2 karakter olmalı, maksimum 50 karakter olabilir.
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Eğitmen adı boş olamaz.")
            .MinimumLength(2).WithMessage("Eğitmen adı en az 2 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Eğitmen adı en fazla 50 karakter olabilir.");

        // DÜZELTME: Surname alanı için validation kuralları. Surname boş olamaz, minimum 2 karakter olmalı, maksimum 50 karakter olabilir.
        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Eğitmen soyadı boş olamaz.")
            .MinimumLength(2).WithMessage("Eğitmen soyadı en az 2 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Eğitmen soyadı en fazla 50 karakter olabilir.");

        // DÜZELTME: Email alanı için validation kuralları. Email boş olamaz, geçerli email formatında olmalı.
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        // DÜZELTME: PhoneNumber alanı için validation kuralları. PhoneNumber isteğe bağlı, ancak doldurulmuşsa geçerli formatında olmalı.
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^(\+90|0)?[0-9]{10}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Geçerli bir telefon numarası formatı giriniz (örn: 05551234567 veya +905551234567).");
    }
}

