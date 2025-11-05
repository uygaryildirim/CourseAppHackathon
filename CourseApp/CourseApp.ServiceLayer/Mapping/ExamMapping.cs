using AutoMapper;
using CourseApp.EntityLayer.Dto.ExamDto;
using CourseApp.EntityLayer.Entity;

namespace CourseApp.ServiceLayer.Mapping;

public class ExamMapping:Profile
{
    public ExamMapping()
    {
        CreateMap<Exam,GetAllExamDto>().ReverseMap();
        CreateMap<Exam,CreateExamDto>().ReverseMap();
        CreateMap<Exam,DeleteExamDto>().ReverseMap();
        // DÜZELTME: GetByIdExamDto mapping'i eklendi. Exam -> GetByIdExamDto mapping'i eksikti, eklendi.
        CreateMap<Exam,GetByIdExamDto>().ReverseMap();
        // DÜZELTME: UpdateExamDto mapping'i eklendi. Exam güncelleme işlemleri için gerekli mapping eksikti, eklendi.
        CreateMap<UpdateExamDto, Exam>();
        // DÜZELTME: Gereksiz mapping kaldırıldı. MissingMappingDto mapping'i kaldırıldı, kullanılmayan ve hata üreten kod temizlendi.
    }
}
