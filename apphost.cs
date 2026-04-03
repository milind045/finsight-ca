#:package Aspire.Hosting.Azure@13.*
#:package Aspire.Hosting.OpenAI@13.*
#:sdk Aspire.AppHost.Sdk@13.2.1
#:property UserSecretsId=7ae1635d-7ac9-43dd-b458-5f56d1b1ee02
#:project ./src/FinSights.Agent/FinSights.Agent.csproj
#:project ./src/FinSights.WebUI/FinSights.WebUI.csproj

using Microsoft.Extensions.Configuration;

const string RESOURCE_PROJECT_AGENT = "agent";
const string RESOURCE_PROJECT_WEBUI = "webui";

var builder = DistributedApplication.CreateBuilder(args);

var config = builder.Configuration
    .AddJsonFile("apphost.settings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets(typeof(Program).Assembly, optional: true, reloadOnChange: true)
    .Build();

var llm = builder.AddConnectionString("llm");

var agent = builder.AddProject<Projects.FinSights_Agent>(RESOURCE_PROJECT_AGENT)
    .WithExternalHttpEndpoints()
    .WithReference(llm);

var webUI = builder.AddProject<Projects.FinSights_WebUI>(RESOURCE_PROJECT_WEBUI)
    .WithExternalHttpEndpoints()
    .WithReference(agent)
    .WaitFor(agent);

await builder.Build().RunAsync();
