using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using System.Text.RegularExpressions;

namespace LLMAzureOpenAITemplate.Utils;

public class LLMService(IChatCompletionService chatCompletionService) : ILLMService
{
    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory)
    {
        try
        {
            return await chatCompletionService.GetChatMessageContentsAsync(chatHistory);
        }
        catch (HttpOperationException exception)
        {
            var errorMessage = exception.Message;
            var match = Regex.Match(errorMessage, "Please retry after ([0-9]+) seconds");
            if (match.Success)
            {
                var time = int.Parse(match.Groups[1].Value);

                SimpleConsole.WriteErrorLine($"Retrying after {time} seconds");
                await Task.Delay(time * 1000);

                return await GetChatMessageContentsAsync(chatHistory);
            }

            SimpleConsole.WriteErrorLine(exception.Message);

            SimpleConsole.WriteErrorLine("Retrying after default time 30 seconds");
            await Task.Delay(30_000);

            return await GetChatMessageContentsAsync(chatHistory);
        }
    }
}