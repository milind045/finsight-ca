using System.ComponentModel;
using FinSights.Data;

namespace FinSights.Tools;

public class AccountTools(FinancialProfile profile)
{
    [Description("Returns all current account balances: TFSA, RRSP, FHSA, chequing, and HELOC.")]
    public AccountBalancesResult GetAccountBalances()
    {
        return new AccountBalancesResult(
            TFSA:     profile.Accounts.TFSA.Balance,
            RRSP:     profile.Accounts.RRSP.Balance,
            FHSA:     profile.Accounts.FHSA.Balance,
            Chequing: profile.Accounts.Chequing.Balance,
            HELOC:    profile.Accounts.HELOC.Balance
        );
    }

    [Description("Returns remaining contribution room for TFSA, RRSP, and FHSA for the current year, " +
                 "including year-to-date contributions already made.")]
    public ContributionRoomResult GetContributionRoom()
    {
        return new ContributionRoomResult(
            TFSARoom:           profile.Accounts.TFSA.AvailableRoom,
            TFSAYtd:            profile.Accounts.TFSA.YtdContributions,
            RRSPRoom:           profile.Accounts.RRSP.AvailableRoom,
            RRSPYtd:            profile.Accounts.RRSP.YtdContributions,
            FHSARoom:           profile.Accounts.FHSA.AvailableRoom,
            FHSAYtd:            profile.Accounts.FHSA.YtdContributions,
            IsFirstTimeBuyer:   profile.IsFirstTimeBuyer
        );
    }

    [Description("Returns a full net worth snapshot: total assets, any liabilities, and net worth figure.")]
    public NetWorthResult GetNetWorthSnapshot()
    {
        var totalAssets =
            profile.Accounts.TFSA.Balance +
            profile.Accounts.RRSP.Balance +
            profile.Accounts.FHSA.Balance +
            profile.Accounts.Chequing.Balance;

        var totalLiabilities = profile.Accounts.HELOC.Balance;

        return new NetWorthResult(
            TotalAssets:      totalAssets,
            TotalLiabilities: totalLiabilities,
            NetWorth:         totalAssets - totalLiabilities
        );
    }
}

// ─── Result records ────────────────────────────────────────────────────────────

public record AccountBalancesResult(
    decimal TFSA,
    decimal RRSP,
    decimal FHSA,
    decimal Chequing,
    decimal HELOC);

public record ContributionRoomResult(
    decimal TFSARoom,
    decimal TFSAYtd,
    decimal RRSPRoom,
    decimal RRSPYtd,
    decimal FHSARoom,
    decimal FHSAYtd,
    bool    IsFirstTimeBuyer);

public record NetWorthResult(
    decimal TotalAssets,
    decimal TotalLiabilities,
    decimal NetWorth);