using FinSights.Agents;
using FinSights.Data;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddAzureChatCompletionsClient("llm")
    .AddChatClient();
var skillsPath  = Path.Combine(AppContext.BaseDirectory, "Skills");

// ─── Load seeded financial profile ───────────────────────────────────────────
var profile = FinancialProfile.LoadFromFile(
    Path.Combine(AppContext.BaseDirectory,
                 "Data", "maya.json"));
builder.Services.AddSingleton(profile);

// ─── Build agents ─────────────────────────────────────────────────────────────

builder.AddAIAgent("finsights-agent", (sp, name) => {
    var client = sp.GetRequiredService<IChatClient>();
    var accountsAgent  = AgentFactory.CreateAccountsAgent(client, profile);
    var cashflowAgent  = AgentFactory.CreateCashflowAgent(client, profile);
    var orchestrator   = AgentFactory.CreateOrchestrator(client, name, skillsPath,
        accountsAgent, cashflowAgent, []);
    return orchestrator;
})
.WithInMemorySessionStore();

builder.AddDevUI();
builder.AddAIAgent("echo-agent", (sp, name) =>
{
    var client = sp.GetRequiredService<IChatClient>();
    var agentBuilder = new AIAgentBuilder(client.AsAIAgent(
        name: name,
        instructions: "You are a ECHO assistant that repeats back whatever the user says, but in uppercase."
    ));
    agentBuilder.UseOpenTelemetry(null, cfg =>
    {
        cfg.EnableSensitiveData = true;
    });
    return agentBuilder.Build();
});
builder.AddOpenAIResponses();
builder.AddOpenAIConversations();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapOpenAIResponses();
app.MapOpenAIConversations();
app.MapDevUI();

app.Run();
