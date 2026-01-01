
namespace EcoPlatesMobile.Utilities
{ 
    public enum PosterSort
    {
        NEAR,
        CHEAP,
        DISCOUNT
    }

    public static class PosterSortExtensions
    {
        public static string GetValue(this PosterSort type)
        {
            return type.ToString();
        }

        public static PosterSort FromValue(string value)
        {
            if (Enum.TryParse<PosterSort>(value, true, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Unknown PosterSort value: {value}");
        }
    }
}
