using LLMAzureOpenAITemplate.Utils;
using Microsoft.SemanticKernel.ChatCompletion;

namespace LLMAzureOpenAITemplate.Exercises
{
    internal class Exercise2(ILLMService llmService) : IExercise
    {
        public async Task Run()
        {
            SimpleConsole.WriteLineAsAI("AI: How can I help?");
            SimpleConsole.WriteAsUser("User: ");
            var prompt = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(prompt))
            {
                SimpleConsole.WriteErrorLine("Please enter a valid prompt.");
                return;
            }

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("As senior software developer response to questions.");
            chatHistory.AddUserMessage(prompt!);

            var response = await llmService.GetChatMessageContentsAsync(chatHistory);

            SimpleConsole.WriteLineAsAI($"AI: {response.Last()}");
        }
    }
}
