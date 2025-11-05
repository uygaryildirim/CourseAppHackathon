using CourseApp.DataAccessLayer.Abstract;
using CourseApp.EntityLayer.Entity;

namespace CourseApp.DataAccessLayer.Concrete;

public class InstructorRepository : GenericRepository<Instructor>, IInstructorRepository
{
    // DÜZELTME: Base constructor çağrısı eklendi - base(context) parametresi eklendi. GenericRepository base class'ının AppDbContext'e ihtiyacı olduğu için context parametresi geçiriliyor.
    public InstructorRepository(AppDbContext context) : base(context)
    {
    }
}
