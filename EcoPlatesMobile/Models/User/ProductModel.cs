using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.User
{
    public class ProductModel : BaseModel
    {
        public string Image { get { return GetValue<string>(); } set => SetValue(value); }
        public string Count { get { return GetValue<string>(); } set => SetValue(value); }
        public string Name { get { return GetValue<string>(); } set => SetValue(value); }
        public string ComapnyName { get { return GetValue<string>(); } set => SetValue(value); }
        public string NewPrice { get { return GetValue<string>(); } set => SetValue(value); }
        public string OldPrice { get { return GetValue<string>(); } set => SetValue(value); }
        public string Stars { get { return GetValue<string>(); } set => SetValue(value); }
        public string Distance { get { return GetValue<string>(); } set => SetValue(value); }
    }
}
