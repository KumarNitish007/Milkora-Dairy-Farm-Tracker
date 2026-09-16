namespace Milkora.Domain.Constants;

// Allowed string values kept as constants (the DB columns are NVARCHAR for
// sync flexibility). Validators reference these sets so the API and the DB
// agree on the vocabulary without an enum/lookup-table round-trip.

public static class AnimalStatuses
{
    public const string Milking = "Milking";
    public const string Pregnant = "Pregnant";
    public const string Dry = "Dry";
    public const string Sick = "Sick";
    public const string Sold = "Sold";
    public const string Dead = "Dead";
    public static readonly string[] All = { Milking, Pregnant, Dry, Sick, Sold, Dead };
}

public static class PaymentStatuses
{
    public const string Paid = "Paid";
    public const string Pending = "Pending";
    public static readonly string[] All = { Paid, Pending };
}

public static class PaymentModes
{
    public static readonly string[] All = { "Cash", "UPI", "Bank" };
}

public static class ExpenseCategories
{
    public static readonly string[] All =
        { "Feed", "Medicine", "Labour", "Electricity", "Equipment", "Transport", "Animal Purchase", "Miscellaneous" };
}

public static class IncomeCategories
{
    public static readonly string[] All = { "Milk Sale", "Calf Sale", "Manure Sale", "Other" };
}

public static class HealthRecordTypes
{
    public static readonly string[] All = { "Vaccine", "Deworming", "Checkup", "Treatment" };
}

public static class PregnancyStatuses
{
    public const string Inseminated = "Inseminated";
    public const string Confirmed = "Confirmed";
    public const string NotPregnant = "NotPregnant";
    public const string Delivered = "Delivered";
    public static readonly string[] All = { Inseminated, Confirmed, NotPregnant, Delivered };
}
