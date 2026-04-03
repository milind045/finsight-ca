using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using FinSights.Data;
using FinSights.Tools;

namespace FinSights.Agents;

public static class AgentFactory
{
    // ─── AccountsAgent ─────────────────────────────────────────────────────────
    // Knows what Maya has: balances, contribution room, account details.
    public static AIAgent CreateAccountsAgent(
        IChatClient client, FinancialProfile profile)
    {
        var tools = new AccountTools(profile);

        return client
            .AsAIAgent(
                name: "AccountsAgent",
                description: "Returns current account balances, TFSA/RRSP/FHSA contribution room, " +
                             "and a full net worth snapshot for the user.",
                instructions: """
                    You are a precise financial data agent for a Canadian household.
                    Your job is to retrieve and report account information accurately.
                    Always use your tools to get data — never guess balances or room.
                    Report numbers in Canadian dollars. Be concise and factual.
                    """,
                tools:
                [
                    AIFunctionFactory.Create(tools.GetAccountBalances),
                    AIFunctionFactory.Create(tools.GetContributionRoom),
                    AIFunctionFactory.Create(tools.GetNetWorthSnapshot),
                ]
            );
    }

    // ─── CashflowAgent ─────────────────────────────────────────────────────────
    // Knows how money flows: burn rate, runway, monthly surplus, projections.
    public static AIAgent CreateCashflowAgent(
        IChatClient client, FinancialProfile profile)
    {
        var tools = new CashflowTools(profile);
        return client
            .AsAIAgent(
                name: "CashflowAgent",
                description: "Calculates monthly cashflow, burn rate, emergency fund runway, " +
                             "and projects savings milestones over time.",
                instructions: """
                    You are a cashflow analysis agent for a Canadian household.
                    Your job is to reason over income, expenses, and savings velocity.
                    Always check emergency fund status before making investment recommendations.
                    Show your math — state the numbers you used to reach a conclusion.
                    Flag clearly if the emergency fund is below 3 months expenses.
                    """,
                tools:
                [
                    AIFunctionFactory.Create(tools.GetMonthlySurplus),
                    AIFunctionFactory.Create(tools.GetEmergencyFundStatus),
                    AIFunctionFactory.Create(tools.ProjectSavingsGoal),
                    AIFunctionFactory.Create(tools.EstimateMonthsToGoal),
                ]
            );
    }

    // ─── OrchestratorAgent ─────────────────────────────────────────────────────
    // Receives the user's question, delegates to sub-agents, applies skill
    // playbooks, and synthesizes a final grounded Canadian answer.
    public static AIAgent CreateOrchestrator(
        IChatClient client,
        string name,
        string skillsDirectoryPath,
        AIAgent accountsAgent,
        AIAgent cashflowAgent,
        IEnumerable<AIFunction>? mcpTools = null)
    {
        // FileAgentSkillsProvider scans the /skills directory for SKILL.md files
        // and registers them as tools the orchestrator can load on demand.
#pragma warning disable MAAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        var skillsProvider = new AgentSkillsProvider(skillsDirectoryPath);
#pragma warning restore MAAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        var toolset = new List<AITool>
        {
            accountsAgent.AsAIFunction(),
            cashflowAgent.AsAIFunction(),
        };

        if (mcpTools is not null)
        {
            toolset.AddRange(mcpTools);
        }

        var options = new ChatClientAgentOptions()
        {
            Name = name,
            Description = null,
            ChatOptions = new ChatOptions()
            {
                Instructions = """
                    You are FinSights, a knowledgeable Canadian personal finance advisor.
                    You help Canadians make specific, grounded financial decisions.

                    ## How you work
                    - Always delegate data retrieval to your specialist agents (AccountsAgent, CashflowAgent).
                    - Load the relevant skill playbook when the question involves a known decision pattern.
                    - Never guess account balances or contribution room — always use your agents.
                    - Always check emergency fund status before recommending any investment.

                    ## How you answer
                    - Be specific — use real dollar amounts, not vague advice.
                    - Show the math behind your recommendation.
                    - State tradeoffs clearly: "Option A saves you X, Option B grows to Y."
                    - Keep the Canadian context front and center: marginal rates, account rules, CRA deadlines.
                    - If a question is outside your financial scope, say so clearly.

                    ## Key rules you always apply
                    - Emergency fund < 3 months expenses: flag this FIRST before any investment advice.
                    - TFSA room is restored January 1 after a withdrawal — mention if relevant.
                    - RRSP contribution deadline is 60 days after December 31 for prior-year deduction.
                    - FHSA is only available to first-time home buyers — confirm eligibility before including it.
                    - Self-employed users pay both sides of CPP — factor into tax set-aside calculations.

                    ## Tone
                    Friendly but precise. Like a smart friend who happens to know Canadian tax law.
                    Not a disclaimer-heavy institution. Specific and actionable.
                    """,
                Tools = toolset,
            },
            AIContextProviders = [skillsProvider],
        };
        return client
            .AsAIAgent(
                options
            );
    }
}