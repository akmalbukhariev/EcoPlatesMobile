using Microsoft.Maui.Controls.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Helper
{
    public class CustomPin : Pin
    {
        public long CompanyId { get; set; }
        public string LogoUrl { get; set; }
        public string PinImage { get; set; }
        public bool IsPin { get; set; } = false;
    }
}
