using CourseApp.EntityLayer.Dto.CourseDto;
using FluentValidation;

namespace CourseApp.API.Validators;

// DÜZELTME: CreateCourseDto için FluentValidation validator eklendi. Kurs oluşturma işlemlerinde girdi doğrulaması yapılıyor.
public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
{
    public CreateCourseDtoValidator()
    {
        // DÜZELTME: CourseName alanı için validation kuralları. CourseName boş olamaz, minimum 3 karakter olmalı, maksimum 100 karakter olabilir.
        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("Kurs adı boş olamaz.")
            .MinimumLength(3).WithMessage("Kurs adı en az 3 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Kurs adı en fazla 100 karakter olabilir.");

        // DÜZELTME: StartDate alanı için validation kuralları. StartDate gelecekte veya bugün olmalı.
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlangıç tarihi boş olamaz.")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Başlangıç tarihi bugünden önce olamaz.");

        // DÜZELTME: EndDate alanı için validation kuralları. EndDate StartDate'den sonra olmalı.
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Bitiş tarihi boş olamaz.")
            .GreaterThan(x => x.StartDate).WithMessage("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

        // DÜZELTME: InstructorID alanı için validation kuralları. InstructorID boş olamaz.
        RuleFor(x => x.InstructorID)
            .NotEmpty().WithMessage("Eğitmen ID boş olamaz.");
    }
}

