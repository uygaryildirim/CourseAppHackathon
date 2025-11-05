using CourseApp.EntityLayer.Dto.LessonDto;
using FluentValidation;

namespace CourseApp.API.Validators;

// DÜZELTME: UpdateLessonDto için FluentValidation validator eklendi. Ders güncelleme işlemlerinde girdi doğrulaması yapılıyor.
public class UpdateLessonDtoValidator : AbstractValidator<UpdateLessonDto>
{
    public UpdateLessonDtoValidator()
    {
        // DÜZELTME: ID alanı için validation kuralları. ID boş olamaz.
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ders ID boş olamaz.");

        // DÜZELTME: Title alanı için validation kuralları. Title boş olamaz, minimum 3 karakter olmalı, maksimum 100 karakter olabilir.
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Ders başlığı boş olamaz.")
            .MinimumLength(3).WithMessage("Ders başlığı en az 3 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Ders başlığı en fazla 100 karakter olabilir.");

        // DÜZELTME: Date alanı için validation kuralları. Date gelecekte veya bugün olmalı.
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Ders tarihi boş olamaz.")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Ders tarihi bugünden önce olamaz.");

        // DÜZELTME: Duration alanı için validation kuralları. Duration 0'dan büyük olmalı, maksimum 240 dakika (4 saat) olabilir.
        RuleFor(x => x.Duration)
            .GreaterThan((byte)0).WithMessage("Ders süresi 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo((byte)240).WithMessage("Ders süresi en fazla 240 dakika (4 saat) olabilir.");

        // DÜZELTME: CourseID alanı için validation kuralları. CourseID boş olamaz.
        RuleFor(x => x.CourseID)
            .NotEmpty().WithMessage("Kurs ID boş olamaz.");

        // DÜZELTME: Content alanı için validation kuralları. Content isteğe bağlı, ancak doldurulmuşsa maksimum 5000 karakter olabilir.
        RuleFor(x => x.Content)
            .MaximumLength(5000).When(x => !string.IsNullOrEmpty(x.Content))
            .WithMessage("Ders içeriği en fazla 5000 karakter olabilir.");
    }
}

