using CourseApp.EntityLayer.Dto.CourseDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _courseService.GetAllAsync();
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
        // DÜZELTME: Null ve empty kontrolü eklendi. String parametre null veya boş olabilir, bu durumda BadRequest döndürülüyor.
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest(new { Message = "ID parametresi boş olamaz." });
        }
        
        // DÜZELTME: GetByIdAsnc yazım hatası düzeltildi - GetByIdAsync olarak değiştirildi. ICourseService interface'indeki doğru async metod adı kullanılıyor.
        var result = await _courseService.GetByIdAsync(id);
        // DÜZELTME: Null reference exception önlendi. result null olabilir, bu durumda result.Success kontrolü yapılmadan önce null kontrolü ekleniyor.
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IDataResult interface'inde doğru property adı kullanılıyor.
        if (result != null && result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetAllDetail()
    {
        var result = await _courseService.GetAllCourseDetail();
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto createCourseDto)
    {
        // DÜZELTME: Null check eklendi. createCourseDto null olabilir, bu durumda BadRequest döndürülüyor.
        if (createCourseDto == null)
        {
            return BadRequest(new { Message = "Kurs bilgileri boş olamaz." });
        }
        
        // DÜZELTME: Null ve empty kontrolü eklendi. CourseName null veya boş olabilir, IndexOutOfRangeException oluşmadan önce kontrol ediliyor.
        if (string.IsNullOrWhiteSpace(createCourseDto.CourseName))
        {
            return BadRequest(new { Message = "Kurs adı boş olamaz." });
        }
        
        var result = await _courseService.CreateAsync(createCourseDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        // DÜZELTME: Eksik noktalı virgül eklendi. C# syntax gereği her statement'ın sonunda noktalı virgül olmalı, derleme hatası önlendi.
        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCourseDto updateCourseDto)
    {
        var result = await _courseService.Update(updateCourseDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteCourseDto deleteCourseDto)
    {
        var result = await _courseService.Remove(deleteCourseDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
