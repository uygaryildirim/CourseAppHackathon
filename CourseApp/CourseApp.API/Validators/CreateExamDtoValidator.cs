using CourseApp.EntityLayer.Dto.ExamDto;
using FluentValidation;

namespace CourseApp.API.Validators;

// DÜZELTME: CreateExamDto için FluentValidation validator eklendi. Sınav oluşturma işlemlerinde girdi doğrulaması yapılıyor.
public class CreateExamDtoValidator : AbstractValidator<CreateExamDto>
{
    public CreateExamDtoValidator()
    {
        // DÜZELTME: Name alanı için validation kuralları. Name boş olamaz, minimum 3 karakter olmalı, maksimum 100 karakter olabilir.
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Sınav adı boş olamaz.")
            .MinimumLength(3).WithMessage("Sınav adı en az 3 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Sınav adı en fazla 100 karakter olabilir.");

        // DÜZELTME: Date alanı için validation kuralları. Date gelecekte veya bugün olmalı.
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Sınav tarihi boş olamaz.")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Sınav tarihi bugünden önce olamaz.");
    }
}

