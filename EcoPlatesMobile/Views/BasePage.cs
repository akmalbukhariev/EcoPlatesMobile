using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Views
{
    public class BasePage : ContentPage
    {
        protected BasePage() 
        {
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
