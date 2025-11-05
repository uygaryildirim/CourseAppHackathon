using CourseApp.EntityLayer.Dto.InstructorDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstructorsController : ControllerBase
{
    private readonly IInstructorService _instructorService;

    public InstructorsController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _instructorService.GetAllAsync();
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _instructorService.GetByIdAsync(id);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatedInstructorDto createdInstructorDto)
    {
        // DÜZELTME: Null check eklendi. createdInstructorDto null olabilir, bu durumda BadRequest döndürülüyor.
        if (createdInstructorDto == null)
        {
            return BadRequest(new { Message = "Eğitmen bilgileri boş olamaz." });
        }
        
        // DÜZELTME: Null ve empty kontrolü eklendi. Name null veya boş olabilir, IndexOutOfRangeException oluşmadan önce kontrol ediliyor.
        if (string.IsNullOrWhiteSpace(createdInstructorDto.Name))
        {
            return BadRequest(new { Message = "Eğitmen adı boş olamaz." });
        }
        
        // DÜZELTME: Invalid cast exception önlendi. Gereksiz tip dönüşümü kaldırıldı, string'i int'e cast etme işlemi kaldırıldı.
        var result = await _instructorService.CreateAsync(createdInstructorDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatedInstructorDto updatedInstructorDto)
    {
        var result = await _instructorService.Update(updatedInstructorDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeletedInstructorDto deletedInstructorDto)
    {
        var result = await _instructorService.Remove(deletedInstructorDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
