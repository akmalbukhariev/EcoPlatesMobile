using System;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PosterType
{
    // --- Food ---
    FOOD_GENERAL,          // Oziq-ovqat
    DRINKS,                // Ichimliklar
    DAIRY,                 // Sut mahsulotlari
    MEAT,                  // Go‘sht / Kolbasa
    BAKERY,                // Non / Shirinlik
    CHOCOLATE,             // Shokolad
    FRUITS_VEGETABLES,     // Meva / Sabzavot
    FROZEN,                // Muzlatilgan
    READY_MEALS,           // Tayyor ovqat
    SNACK,                 // Gazaklar
    CAKE,                  // Tortlar
    BURGER,                // Burger / Fast food

    // --- Daily goods ---
    HYGIENE,               // Gigiyena
    HOUSEHOLD,             // Uy-ro‘zg‘or
    KIDS,                  // Bolalar

    // --- Non-food / future ---
    ELECTRONICS,
    CLOTHING,
    FURNITURE,
    BEAUTY,
    SPORTS,
    BOOKS,
    TOYS,

    OTHER
}

public static class PosterTypeExtensions
{
    public static string GetValue(this PosterType type)
    {
        return type.ToString();
    }

    public static PosterType FromValue(string value)
    {
        if (Enum.TryParse<PosterType>(value, true, out var result))
        {
            return result;
        }

        throw new ArgumentException($"Unknown PosterType value: {value}");
    }
}
