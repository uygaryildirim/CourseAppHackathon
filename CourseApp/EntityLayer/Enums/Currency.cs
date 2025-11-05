namespace CourseApp.EntityLayer.Enums;

// DÜZELTME: Para birimi enum'u eklendi. TL, USD, EUR para birimlerini desteklemek için. Farklı para birimlerinde kayıt yapılabilmesini sağlıyor.
public enum Currency
{
    // DÜZELTME: Türk Lirası para birimi. Standart para birimi.
    TRY = 1,
    // DÜZELTME: Amerikan Doları para birimi. Dolar cinsinden kayıt yapılabilmesini sağlıyor.
    USD = 2,
    // DÜZELTME: Euro para birimi. Euro cinsinden kayıt yapılabilmesini sağlıyor.
    EUR = 3
}

