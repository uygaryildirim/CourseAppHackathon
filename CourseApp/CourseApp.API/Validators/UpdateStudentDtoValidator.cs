using CourseApp.EntityLayer.Dto.StudentDto;
using FluentValidation;

namespace CourseApp.API.Validators;

// DÜZELTME: UpdateStudentDto için FluentValidation validator eklendi. Öğrenci güncelleme işlemlerinde girdi doğrulaması yapılıyor.
public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentDtoValidator()
    {
        // DÜZELTME: ID alanı için validation kuralları. ID boş olamaz.
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Öğrenci ID boş olamaz.");

        // DÜZELTME: Name alanı için validation kuralları. Name boş olamaz, minimum 2 karakter olmalı, maksimum 50 karakter olabilir.
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Öğrenci adı boş olamaz.")
            .MinimumLength(2).WithMessage("Öğrenci adı en az 2 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Öğrenci adı en fazla 50 karakter olabilir.");

        // DÜZELTME: Surname alanı için validation kuralları. Surname boş olamaz, minimum 2 karakter olmalı, maksimum 50 karakter olabilir.
        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Öğrenci soyadı boş olamaz.")
            .MinimumLength(2).WithMessage("Öğrenci soyadı en az 2 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Öğrenci soyadı en fazla 50 karakter olabilir.");

        // DÜZELTME: TC (T.C. Kimlik No) alanı için validation kuralları. TC boş olamaz, tam olarak 11 karakter olmalı, sadece rakam içermeli.
        RuleFor(x => x.TC)
            .NotEmpty().WithMessage("T.C. Kimlik No boş olamaz.")
            .Length(11).WithMessage("T.C. Kimlik No tam olarak 11 karakter olmalıdır.")
            .Matches(@"^\d+$").WithMessage("T.C. Kimlik No sadece rakam içermelidir.");

        // DÜZELTME: BirthDate alanı için validation kuralları. BirthDate geçmişte olmalı, 18 yaşından küçük olamaz.
        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Doğum tarihi boş olamaz.")
            .LessThan(DateTime.Now).WithMessage("Doğum tarihi geçmişte olmalıdır.")
            .Must(BeAtLeast18YearsOld).WithMessage("Öğrenci en az 18 yaşında olmalıdır.");
    }

    // DÜZELTME: Yaş kontrolü için yardımcı metod. Öğrencinin 18 yaşında veya daha büyük olması kontrol ediliyor.
    private bool BeAtLeast18YearsOld(DateTime birthDate)
    {
        var age = DateTime.Now.Year - birthDate.Year;
        if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
            age--;
        return age >= 18;
    }
}

