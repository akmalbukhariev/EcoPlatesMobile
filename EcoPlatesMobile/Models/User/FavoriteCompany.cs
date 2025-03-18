
namespace EcoPlatesMobile.Models.User
{
    public class FavoriteCompany : BaseModel
    {
        public string logo { get { return GetValue<string>(); } set => SetValue(value); }
        public string company_name { get { return GetValue<string>(); } set => SetValue(value); }
        public string working_time { get { return GetValue<string>(); } set => SetValue(value); }
        public string stars { get { return GetValue<string>(); } set => SetValue(value); }
        public string distance { get { return GetValue<string>(); } set => SetValue(value); }
    }
}
