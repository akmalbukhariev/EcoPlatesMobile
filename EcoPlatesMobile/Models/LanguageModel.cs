using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models
{
    public class LanguageModel :BaseModel
    {
        public string Name { get { return GetValue<string>(); } set => SetValue(value); }
        public string Flag { get { return GetValue<string>(); } set => SetValue(value); }
        public bool IsSelected { get { return GetValue<bool>(); } set => SetValue(value); }
    }
}
