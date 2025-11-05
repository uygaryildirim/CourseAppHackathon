using CourseApp.DataAccessLayer.Abstract;
using CourseApp.EntityLayer.Entity;

namespace CourseApp.DataAccessLayer.Concrete;

public class ExamRepository : GenericRepository<Exam>, IExamRepository
{
    public ExamRepository(AppDbContext context) : base(context)
    {
    }
    
    // DÜZELTME: Gereksiz metod kaldırıldı. InvalidMethod ve ExamHelperUtility kaldırıldı, kullanılmayan ve hata üreten kod temizlendi.
}
