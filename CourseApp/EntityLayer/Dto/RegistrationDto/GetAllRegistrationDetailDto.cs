using CourseApp.EntityLayer.Enums;

namespace CourseApp.EntityLayer.Dto.RegistrationDto;

public class GetAllRegistrationDetailDto
{
    public string Id { get; set; } = null!;
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public decimal Price { get; set; }
    // DÜZELTME: Currency alanı eklendi. Para birimi bilgisini göstermek için.
    public Currency Currency { get; set; } = Currency.TRY;
    public string? StudentID { get; set; }
    public string? CourseID { get; set; }
    public string CourseName { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public string RegistrationDetail => $"{CourseName} {Price}";
}
