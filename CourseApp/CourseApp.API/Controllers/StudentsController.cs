using CourseApp.EntityLayer.Dto.StudentDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;
// DÜZELTME: Gereksiz using kaldırıldı. Controller'dan direkt DataAccessLayer'a erişim kaldırıldığı için AppDbContext using'i kaldırıldı.

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    // DÜZELTME: Katman ihlali kaldırıldı. Controller'dan direkt DbContext erişimi kaldırıldı, tüm işlemler Service layer üzerinden yönetiliyor.
    // DÜZELTME: Kullanılmayan değişkenler kaldırıldı. _cachedStudents ve _dbContext dependency injection'dan kaldırıldı.

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // DÜZELTME: Kullanılmayan cache kontrolü kaldırıldı. _cachedStudents değişkeni kaldırıldığı için cache kontrolü de kaldırıldı.
        var result = await _studentService.GetAllAsync();
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        // DÜZELTME: Null ve empty kontrolü eklendi. String parametre null veya boş olabilir, bu durumda IndexOutOfRangeException oluşmadan önce kontrol ediliyor.
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest(new { Message = "ID parametresi boş olamaz." });
        }
        
        var result = await _studentService.GetByIdAsync(id);
        // DÜZELTME: Null reference exception önlendi. result.Data null olabilir, bu durumda result.Success kontrolü yapılmadan önce null kontrolü ekleniyor.
        if (result.Success && result.Data != null)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto createStudentDto)
    {
        // DÜZELTME: Null check eklendi. createStudentDto null olabilir, bu durumda BadRequest döndürülüyor.
        if (createStudentDto == null)
        {
            return BadRequest(new { Message = "Öğrenci bilgileri boş olamaz." });
        }
        
        // DÜZELTME: Katman ihlali kaldırıldı. Controller'dan direkt DbContext'e erişim kaldırıldı, business logic Service layer üzerinden yönetiliyor.
        // DÜZELTME: Invalid cast exception önlendi. Gereksiz tip dönüşümü kaldırıldı, sadece service metoduna yönlendiriliyor.
        var result = await _studentService.CreateAsync(createStudentDto);
        if (result.Success)
        {
            return Ok(result);
        }
        // DÜZELTME: Eksik noktalı virgül eklendi. C# syntax gereği her statement'ın sonunda noktalı virgül olmalı, derleme hatası önlendi.
        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateStudentDto updateStudentDto)
    {
        // DÜZELTME: Null check eklendi. updateStudentDto null olabilir, bu durumda BadRequest döndürülüyor.
        if (updateStudentDto == null)
        {
            return BadRequest(new { Message = "Güncellenecek öğrenci bilgileri boş olamaz." });
        }
        
        var result = await _studentService.Update(updateStudentDto);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteStudentDto deleteStudentDto)
    {
        // DÜZELTME: Null check eklendi. deleteStudentDto null olabilir, bu durumda BadRequest döndürülüyor.
        if (deleteStudentDto == null)
        {
            return BadRequest(new { Message = "Silinecek öğrenci bilgileri boş olamaz." });
        }
        
        // DÜZELTME: Memory leak önlendi. Gereksiz DbContext oluşturma ve kullanımı kaldırıldı, DI container üzerinden yönetilen context kullanılıyor.
        var result = await _studentService.Remove(deleteStudentDto);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
