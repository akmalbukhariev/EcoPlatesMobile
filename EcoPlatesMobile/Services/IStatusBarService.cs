using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Services
{
    public interface IStatusBarService
    {
        void SetStatusBarColor(string hexColor, bool darkStatusBarTint);
    }
}
