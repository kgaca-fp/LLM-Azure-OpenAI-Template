using LLMAzureOpenAITemplate.Exercises;
using LLMAzureOpenAITemplate.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;

namespace LLMAzureOpenAITemplate
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings
            {
                Args = args
            });

            builder.Configuration.AddJsonFile("appsettings.json");
            var apiKey = builder.Configuration["secrest:openai-key"] ??
                         throw new ArgumentNullException("builder.Configuration[\"secrest:openai-key\"]");
            var endpoint = builder.Configuration["openai-endpoint"] ??
                           throw new ArgumentNullException("builder.Configuration[\"openai-endpoint\"]");

            builder.Services.AddAzureOpenAIChatCompletion("gpt-4o", endpoint, apiKey,
                modelId: "gpt-4o");

            builder.Services.AddSingleton<ILLMService, LLMService>();
            builder.Services.AddSingleton<Exercise1>();
            builder.Services.AddSingleton<Exercise2>();
            builder.Services.AddSingleton<Exercise3>();
            builder.Services.AddSingleton<Exercise4>();
            builder.Services.AddSingleton<Exercise5>();

            var app = builder.Build();

            //TODO change to current exercise
            var exercise = app.Services.GetService<Exercise1>();

            await exercise!.Run();
        }
    }
}
