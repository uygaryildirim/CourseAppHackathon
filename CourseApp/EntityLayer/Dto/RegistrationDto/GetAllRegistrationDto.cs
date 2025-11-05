using CourseApp.EntityLayer.Enums;

namespace CourseApp.EntityLayer.Dto.RegistrationDto;

public class GetAllRegistrationDto
{
    public string Id { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public decimal Price { get; set; }
    // DÜZELTME: Currency alanı eklendi. Para birimi bilgisini göstermek için.
    public Currency Currency { get; set; } = Currency.TRY;
    public string? StudentID { get; set; }
    public string? CourseID { get; set; }
}
