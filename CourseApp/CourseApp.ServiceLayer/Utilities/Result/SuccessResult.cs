namespace CourseApp.ServiceLayer.Utilities.Result;

public class SuccessResult:Result
{

    public SuccessResult():base(true)
    {
        
    }

    public SuccessResult(string message):base(true,message) 
    {

    }
    
    // DÜZELTME: Gereksiz metod kaldırıldı. UseUndefinedUtility ve UndefinedUtilityClass kaldırıldı, kullanılmayan ve hata üreten kod temizlendi.
}
