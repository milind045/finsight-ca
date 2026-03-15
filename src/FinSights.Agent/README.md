# FinSights Agent

FinSights is a Canadian personal finance assistant built with .NET 10, .NET Aspire, and the Microsoft Agents SDK.

The current solution runs:
- `finsights-agent`, the main finance assistant
- `echo-agent`, a simple uppercase echo agent for quick DevUI validation

## Prerequisites

- .NET 10 SDK
- .NET Aspire 13 tooling
- A valid `llm` connection string available to the app host

## Solution Structure

```text
fin-sights.slnx
apphost.cs
apphost.settings.json
src/
  FinSights.Agent/
    Program.cs
    Agents/
      Agentfactory.cs
    Data/
      Financialprofile.cs
      maya.json
    Tools/
      AccountTools.cs
      Cashflowtools.cs
    Skills/
      rrsp-vs-tfsa/SKILL.md
      freelance-tax-setaside/SKILL.md
  FinSights.ServiceDefaults/
```

## Current Architecture

### Agents

| Agent | Purpose | Tools |
|---|---|---|
| `AccountsAgent` | Account balances, contribution room, and net worth snapshots | `GetAccountBalances`, `GetContributionRoom`, `GetNetWorthSnapshot` |
| `CashflowAgent` | Budget, emergency fund, and savings projections | `GetMonthlySurplus`, `GetEmergencyFundStatus`, `ProjectSavingsGoal`, `EstimateMonthsToGoal` |
| `finsights-agent` | Orchestrates responses using specialist agents plus markdown skill playbooks | Wraps `AccountsAgent` and `CashflowAgent` as AI functions and loads `Skills/*/SKILL.md` |
| `echo-agent` | Dev/test agent that replies in uppercase | Inline instruction-based behavior |

### Runtime Notes

- The seeded profile in `Data/maya.json` is loaded at startup and registered as a singleton.
- Session state for `finsights-agent` uses the in-memory session store.
- The orchestrator now accepts an optional list of extra AI functions, which leaves room for future MCP or external tool integration.
- DevUI is the primary interface for testing and asking questions to the agents.
- The agent service exposes the default Aspire health endpoints in development.

## Packages Used by FinSights.Agent

```xml
<PackageReference Include="Aspire.Azure.AI.Inference" Version="13.*-*" />
<PackageReference Include="Microsoft.Agents.AI.DevUI" Version="1.*-*" />
<PackageReference Include="Microsoft.Agents.AI.Hosting.OpenAI" Version="1.*-*" />
<PackageReference Include="Microsoft.Agents.AI.Workflows" Version="1.*-*" />
<PackageReference Include="Microsoft.Agents.AI.Hosting.AGUI.AspNetCore" Version="1.*-*" />
```

## Configuration

The app host expects a single Aspire connection string named `llm`:

```json
{
  "ConnectionStrings": {
    "llm": "<your-llm-connection-string>"
  }
}
```

You can provide it through:
- `apphost.settings.json`
- app host user secrets
- environment variable `ConnectionStrings__llm`

## Running the Solution

From the repository root:

```bash
dotnet run apphost.cs
```

This starts the Aspire app host and launches:
- `agent` (`FinSights.Agent`)

The agent service also maps:
- DevUI endpoints
- OpenAI Responses API endpoints
- OpenAI Conversations API endpoints
- health endpoints in development via `MapDefaultEndpoints()`

## Example Prompts

1. "What's my current net worth?"
2. "How much TFSA, RRSP, and FHSA room do I still have?"
3. "How many months of emergency fund coverage do I have?"
4. "Can I reach my savings goal if I add $500 per month?"
5. "I got a $15,000 bonus. Where should it go first?"
6. "If I freelance for an extra $20,000, how much tax should I set aside?"