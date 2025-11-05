using AutoMapper;
using CourseApp.EntityLayer.Dto.InstructorDto;
using CourseApp.EntityLayer.Entity;

namespace CourseApp.ServiceLayer.Mapping;

public class InstructorMapping:Profile
{
    public InstructorMapping()
    {
        CreateMap<Instructor,GetAllInstructorDto>().ReverseMap();
        CreateMap<Instructor,GetByIdInstructorDto>().ReverseMap();
        CreateMap<Instructor,DeletedInstructorDto>().ReverseMap();
        // DÜZELTME: Gereksiz mapping kaldırıldı. UndefinedMappingDto mapping'i kaldırıldı, kullanılmayan ve hata üreten kod temizlendi.
    }
}
