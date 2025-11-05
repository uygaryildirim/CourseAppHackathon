using CourseApp.EntityLayer.Enums;

namespace CourseApp.EntityLayer.Dto.RegistrationDto;

public class UpdatedRegistrationDto
{
    public string Id { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public decimal Price { get; set; }
    // DÜZELTME: Currency alanı eklendi. TL, USD, EUR para birimlerinden birini seçebilmek için. Standart değer TRY.
    public Currency Currency { get; set; } = Currency.TRY;
    public string? StudentID { get; set; }
    public string? CourseID { get; set; }
}
