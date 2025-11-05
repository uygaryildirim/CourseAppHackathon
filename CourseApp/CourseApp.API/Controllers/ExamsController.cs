using CourseApp.EntityLayer.Dto.ExamDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamsController : ControllerBase
{
    private readonly IExamService _examService;

    public ExamsController(IExamService examService)
    {
        _examService = examService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // DÜZELTME: N+1 problemi kaldırıldı. Gereksiz foreach döngüsü ve GetByIdAsync çağrıları kaldırıldı, sadece GetAllAsync kullanılıyor.
        var result = await _examService.GetAllAsync();
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
        var result = await _examService.GetByIdAsync(id);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExamDto createExamDto)
    {
        var result = await _examService.CreateAsync(createExamDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateExamDto updateExamDto)
    {
        var result = await _examService.Update(updateExamDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteExamDto deleteExamDto)
    {
        var result = await _examService.Remove(deleteExamDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
