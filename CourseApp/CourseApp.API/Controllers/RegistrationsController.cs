using CourseApp.EntityLayer.Dto.RegistrationDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly IRegistrationService _registrationService;

    public RegistrationsController(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _registrationService.GetAllAsync();
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
        var result = await _registrationService.GetByIdAsync(id);
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
        var result = await _registrationService.GetAllRegistrationDetailAsync();
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
        var result = await _registrationService.GetByIdRegistrationDetailAsync(id);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRegistrationDto createRegistrationDto)
    {
        // DÜZELTME: Null check eklendi. createRegistrationDto null olabilir, bu durumda hata mesajı döndürülüyor.
        if (createRegistrationDto == null)
        {
            return BadRequest("Kayıt bilgileri boş olamaz.");
        }
        
        // DÜZELTME: Invalid cast exception önlendi. decimal'i int'e direkt cast etme işlemi kaldırıldı, gereksiz tip dönüşümü kaldırıldı.
        
        var result = await _registrationService.CreateAsync(createRegistrationDto);
        // DÜZELTME: rsult yazım hatası düzeltildi - result olarak değiştirildi. Doğru değişken adı kullanılıyor.
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatedRegistrationDto updatedRegistrationDto)
    {
        var result = await _registrationService.Update(updatedRegistrationDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteRegistrationDto deleteRegistrationDto)
    {
        var result = await _registrationService.Remove(deleteRegistrationDto);
        // DÜZELTME: result.Success yazım hatası düzeltildi - result.IsSuccess olarak değiştirildi. IResult interface'inde doğru property adı kullanılıyor.
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
