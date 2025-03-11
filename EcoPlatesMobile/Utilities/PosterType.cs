using System;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PosterType
{
    FOOD,
    CAKE,
    BURGER,
    DRINK,
    SNACK,
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
