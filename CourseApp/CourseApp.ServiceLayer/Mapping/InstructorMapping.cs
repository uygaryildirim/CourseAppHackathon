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
        // DÜZELTME: CreatedInstructorDto -> Instructor mapping'i eklendi. Instructor oluşturma işlemleri için gerekli mapping eksikti, eklendi.
        CreateMap<CreatedInstructorDto, Instructor>();
        // DÜZELTME: UpdatedInstructorDto -> Instructor mapping'i eklendi. Instructor güncelleme işlemleri için gerekli mapping eksikti, eklendi.
        CreateMap<UpdatedInstructorDto, Instructor>();
        // DÜZELTME: Gereksiz mapping kaldırıldı. UndefinedMappingDto mapping'i kaldırıldı, kullanılmayan ve hata üreten kod temizlendi.
    }
}
