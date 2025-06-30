using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models
{
    public partial class LanguageModel : ObservableObject
    {
        [ObservableProperty] private string code;
        [ObservableProperty] private string name;
        [ObservableProperty] private string flag;
        [ObservableProperty] private bool isSelected;
    }
}
