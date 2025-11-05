using CourseApp.EntityLayer.Dto.ExamResultDto;
using FluentValidation;

namespace CourseApp.API.Validators;

// DÜZELTME: UpdateExamResultDto için FluentValidation validator eklendi. Sınav sonucu güncelleme işlemlerinde girdi doğrulaması yapılıyor.
public class UpdateExamResultDtoValidator : AbstractValidator<UpdateExamResultDto>
{
    public UpdateExamResultDtoValidator()
    {
        // DÜZELTME: ID alanı için validation kuralları. ID boş olamaz.
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Sınav sonucu ID boş olamaz.");

        // DÜZELTME: Grade alanı için validation kuralları. Grade 0 ile 100 arasında olmalı.
        RuleFor(x => x.Grade)
            .GreaterThanOrEqualTo((byte)0).WithMessage("Not 0'dan küçük olamaz.")
            .LessThanOrEqualTo((byte)100).WithMessage("Not 100'den büyük olamaz.");

        // DÜZELTME: ExamID alanı için validation kuralları. ExamID boş olamaz.
        RuleFor(x => x.ExamID)
            .NotEmpty().WithMessage("Sınav ID boş olamaz.");

        // DÜZELTME: StudentID alanı için validation kuralları. StudentID boş olamaz.
        RuleFor(x => x.StudentID)
            .NotEmpty().WithMessage("Öğrenci ID boş olamaz.");
    }
}

