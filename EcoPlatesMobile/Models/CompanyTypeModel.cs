
using EcoPlatesMobile.Models;

public class CompanyTypeModel : BaseModel
{
    public string Type { get { return GetValue<string>(); } set => SetValue(value); }
}