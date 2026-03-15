using System.ComponentModel;
using FinSights.Data;

namespace FinSights.Tools;

/// <summary>
/// Tools used by CashflowAgent. Pure math over the seeded profile.
/// No side effects — easy to unit test.
/// </summary>
public class CashflowTools(FinancialProfile profile)
{
    [Description("Returns monthly income, total expenses broken down by category, and monthly surplus.")]
    public MonthlySurplusResult GetMonthlySurplus()
    {
        return new MonthlySurplusResult(
            MonthlyIncome:    profile.MonthlyIncome,
            Housing:          profile.MonthlyExpenses.Housing,
            Food:             profile.MonthlyExpenses.Food,
            Transport:        profile.MonthlyExpenses.Transport,
            Subscriptions:    profile.MonthlyExpenses.Subscriptions,
            Other:            profile.MonthlyExpenses.Other,
            TotalExpenses:    profile.TotalMonthlyExpenses,
            MonthlySurplus:   profile.MonthlySurplus
        );
    }

    [Description("Returns emergency fund status: current liquid balance, target amount (3 months expenses), " +
                 "current coverage in months, and whether the fund is adequate.")]
    public EmergencyFundResult GetEmergencyFundStatus()
    {
        var targetMonths   = 3;
        var targetAmount   = profile.TotalMonthlyExpenses * targetMonths;
        var liquidBalance  = profile.Accounts.Chequing.Balance;
        var monthsCovered  = liquidBalance / profile.TotalMonthlyExpenses;
        var isAdequate     = monthsCovered >= targetMonths;
        var shortfall      = isAdequate ? 0 : targetAmount - liquidBalance;

        return new EmergencyFundResult(
            LiquidBalance:  liquidBalance,
            TargetAmount:   targetAmount,
            MonthsCovered:  Math.Round(monthsCovered, 1),
            IsAdequate:     isAdequate,
            Shortfall:      shortfall,
            TargetMonths:   targetMonths
        );
    }

    [Description("Projects how long it will take to reach a savings goal given a monthly contribution amount. " +
                 "Provide the goal name, target amount, and monthly contribution.")]
    public ProjectionResult ProjectSavingsGoal(
        [Description("Name of the savings goal, e.g. 'Emergency Fund', 'TFSA top-up'")] string goalName,
        [Description("The total dollar amount to reach")] decimal targetAmount,
        [Description("The monthly amount being contributed toward this goal")] decimal monthlyContribution)
    {
        if (monthlyContribution <= 0)
            return new ProjectionResult(goalName, targetAmount, monthlyContribution, 0, "Cannot project — monthly contribution must be greater than zero.");

        var months   = (int)Math.Ceiling(targetAmount / monthlyContribution);
        var years    = months / 12;
        var remMonths = months % 12;

        var timeDescription = years > 0
            ? $"{years} year{(years > 1 ? "s" : "")} and {remMonths} month{(remMonths != 1 ? "s" : "")}"
            : $"{months} month{(months != 1 ? "s" : "")}";

        return new ProjectionResult(goalName, targetAmount, monthlyContribution, months, timeDescription);
    }

    [Description("Estimates how many months until a specific dollar goal is reached, " +
                 "given the current surplus and any existing balance toward that goal.")]
    public EstimateResult EstimateMonthsToGoal(
        [Description("Current balance already saved toward this goal")] decimal currentBalance,
        [Description("Total target balance to reach")] decimal targetBalance,
        [Description("Monthly amount being allocated to this goal")] decimal monthlyAllocation)
    {
        if (currentBalance >= targetBalance)
            return new EstimateResult(0, "Goal already reached.");

        if (monthlyAllocation <= 0)
            return new EstimateResult(-1, "Cannot estimate — monthly allocation must be greater than zero.");

        var remaining = targetBalance - currentBalance;
        var months    = (int)Math.Ceiling(remaining / monthlyAllocation);

        return new EstimateResult(months, $"Approximately {months} month{(months != 1 ? "s" : "")} at ${monthlyAllocation:N0}/month.");
    }
}

// ─── Result records ────────────────────────────────────────────────────────────

public record MonthlySurplusResult(
    decimal MonthlyIncome,
    decimal Housing,
    decimal Food,
    decimal Transport,
    decimal Subscriptions,
    decimal Other,
    decimal TotalExpenses,
    decimal MonthlySurplus);

public record EmergencyFundResult(
    decimal LiquidBalance,
    decimal TargetAmount,
    decimal MonthsCovered,
    bool    IsAdequate,
    decimal Shortfall,
    int     TargetMonths);

public record ProjectionResult(
    string  GoalName,
    decimal TargetAmount,
    decimal MonthlyContribution,
    int     MonthsToGoal,
    string  Summary);

public record EstimateResult(
    int    MonthsRemaining,
    string Summary);