using CourseApp.EntityLayer.Dto.ExamResultDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamResultsController : ControllerBase
{
    private readonly IExamResultService _examResultService;

    public ExamResultsController(IExamResultService examResultService)
    {
        _examResultService = examResultService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // ZOR: N+1 Problemi - Her examResult için ayrı sorgu
        var result = await _examResultService.GetAllAsync();
        // ORTA: Null reference - result.Data null olabilir
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IDataResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess && result.Data != null)
        {
            // ZOR: N+1 - Her examResult için detay çekiliyor
            var examResults = result.Data.ToList();
            foreach (var examResult in examResults)
            {
                // Her examResult için ayrı sorgu
                var detail = await _examResultService.GetByIdExamResultDetailAsync(examResult.Id);
            }
            return Ok(result);
        }
        // DÜZELTME: BadReqest yazım hatası düzeltildi - BadRequest olarak değiştirildi. Doğru HTTP status code metodu kullanılıyor.
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _examResultService.GetByIdAsync(id);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetAllDetail()
    {
        var result = await _examResultService.GetAllExamResultDetailAsync();
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> GetByIdDetail(string id)
    {
        var result = await _examResultService.GetByIdExamResultDetailAsync(id);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExamResultDto createExamResultDto)
    {
        var result = await _examResultService.CreateAsync(createExamResultDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateExamResultDto updateExamResultDto)
    {
        var result = await _examResultService.Update(updateExamResultDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteExamResultDto deleteExamResultDto)
    {
        var result = await _examResultService.Remove(deleteExamResultDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
