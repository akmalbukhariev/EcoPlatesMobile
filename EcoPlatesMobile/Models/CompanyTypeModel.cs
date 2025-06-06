
using CommunityToolkit.Mvvm.ComponentModel;
using EcoPlatesMobile.Models;

public partial class CompanyTypeModel : ObservableObject
{
    [ObservableProperty] private string type;
    public string Type_value;
}