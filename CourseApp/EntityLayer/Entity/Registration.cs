using CourseApp.EntityLayer.Enums;

namespace CourseApp.EntityLayer.Entity;

public class Registration : BaseEntity
{
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public decimal Price { get; set; }
    // DÜZELTME: Currency alanı eklendi. TL, USD, EUR gibi farklı para birimlerinde kayıt yapılabilmesini sağlıyor. Standart değer TRY.
    public Currency Currency { get; set; } = Currency.TRY;
    public string? StudentID { get; set; }
    public string? CourseID { get; set; }
    public Course? Course { get; set; }
    public Student? Student { get; set; }
}
