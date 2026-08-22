namespace Tijori.Application.Common;

public static class BucketCategoryKeys
{
    public const string Contract = "contract";
    public const string Warranties = "warranties";
    public const string Insurance = "insurance";
    public const string PersonalDoc = "personal-doc";
    public const string MyTrips = "my-trips";
    public const string MyAppointments = "my-appointments";
    public const string MyMedicine = "my-medicine";
    public const string Custom = "custom";
    public const string CustomSub = "custom-sub";

    public static bool IsContract(string? iconKey) =>
        string.Equals(iconKey, Contract, StringComparison.OrdinalIgnoreCase);

    public static bool IsWarranty(string? iconKey) =>
        string.Equals(iconKey, Warranties, StringComparison.OrdinalIgnoreCase);

    public static bool IsMyTrips(string? iconKey) =>
        string.Equals(iconKey, MyTrips, StringComparison.OrdinalIgnoreCase);

    public static bool IsMyAppointments(string? iconKey) =>
        string.Equals(iconKey, MyAppointments, StringComparison.OrdinalIgnoreCase);

    public static bool IsMyMedicine(string? iconKey) =>
        string.Equals(iconKey, MyMedicine, StringComparison.OrdinalIgnoreCase);

    public static bool IsCustomGroup(string? iconKey) =>
        string.Equals(iconKey, Custom, StringComparison.OrdinalIgnoreCase);

    public static bool IsCustomSubCategory(string? iconKey) =>
        string.Equals(iconKey, CustomSub, StringComparison.OrdinalIgnoreCase);

    public static bool IsCustom(string? iconKey) => IsCustomSubCategory(iconKey);
}

public static class TripItemTypeKeys
{
    public const string Passport = "passport";
    public const string Transportation = "transportation";
    public const string Accommodation = "accommodation";
    public const string CarRental = "car_rental";
    public const string Club = "club";
    public const string Match = "match";
    public const string Invoice = "invoice";

    private static readonly HashSet<string> ValidTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        Passport,
        Transportation,
        Accommodation,
        CarRental,
        Club,
        Match,
        Invoice
    };

    public static bool IsValid(string? itemType) =>
        !string.IsNullOrWhiteSpace(itemType) && ValidTypes.Contains(itemType);
}

public static class WarrantySubCategoryKeys
{
    public const string Watches = "watches";
    public const string Jewelaries = "jewelaries";
    public const string Bags = "bags";
    public const string Others = "others";
}
