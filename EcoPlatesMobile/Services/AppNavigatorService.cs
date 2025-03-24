
public static class AppNavigatorService
{
    public static async Task NavigateTo(string route)
    {
        await Shell.Current.GoToAsync(route);
    }

    /*public static async Task NavigateToProduct(string productId)
    {
        await Shell.Current.GoToAsync($"{AppRoutes.ProductDetailsPage}?id={productId}");
    }*/
}