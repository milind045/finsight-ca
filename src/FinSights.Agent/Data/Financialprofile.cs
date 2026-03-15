using System.Text.Json;

namespace FinSights.Data;

// ─── Domain model ──────────────────────────────────────────────────────────────

public record FinancialProfile
{
    public string Name           { get; init; } = "";
    public int    Age            { get; init; }
    public string Province       { get; init; } = "";
    public decimal AnnualIncome  { get; init; }
    public bool   IsFirstTimeBuyer { get; init; }
    public bool   IsSelfEmployed   { get; init; }

    public Accounts        Accounts        { get; init; } = new();
    public MonthlyExpenses MonthlyExpenses { get; init; } = new();
    public List<Goal>      Goals           { get; init; } = [];

    // Derived helpers used by tools
    public decimal MonthlyIncome       => AnnualIncome / 12;
    public decimal TotalMonthlyExpenses => MonthlyExpenses.Total;
    public decimal MonthlySurplus      => MonthlyIncome - TotalMonthlyExpenses;

    public static FinancialProfile LoadFromFile(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<FinancialProfile>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Could not deserialize financial profile.");
    }
}

public record Accounts
{
    public Account TFSA     { get; init; } = new();
    public Account RRSP     { get; init; } = new();
    public Account FHSA     { get; init; } = new();
    public Account Chequing { get; init; } = new();
    public Account HELOC    { get; init; } = new();
}

public record Account
{
    public decimal Balance           { get; init; }
    public decimal YtdContributions  { get; init; }
    public decimal AvailableRoom     { get; init; }
    public decimal Limit             { get; init; }   // for HELOC
}

public record MonthlyExpenses
{
    public decimal Housing       { get; init; }
    public decimal Food          { get; init; }
    public decimal Transport     { get; init; }
    public decimal Subscriptions { get; init; }
    public decimal Other         { get; init; }

    public decimal Total => Housing + Food + Transport + Subscriptions + Other;
}

public record Goal
{
    public string  Name         { get; init; } = "";
    public decimal TargetAmount { get; init; }
    public int     TargetMonths { get; init; }   // for emergency fund: months of expenses
    public string  Deadline     { get; init; } = "";
    public int     Priority     { get; init; }
}