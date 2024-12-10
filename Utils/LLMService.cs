using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using System.Text.RegularExpressions;
using Microsoft.SemanticKernel.Embeddings;

namespace LLMAzureOpenAITemplate.Utils;

#pragma warning disable SKEXP0001
public class LLMService(IChatCompletionService chatCompletionService, ITextEmbeddingGenerationService embeddingGenerationService) : ILLMService
#pragma warning restore SKEXP0001
{
    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory)
    {
        return await TrySend(() => chatCompletionService.GetChatMessageContentsAsync(chatHistory));
    }

    public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> messages)
    {
        return await TrySend(() => embeddingGenerationService.GenerateEmbeddingsAsync(messages));
    }


    public async Task<T> TrySend<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
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

                return await TrySend(action);
            }

            SimpleConsole.WriteErrorLine(exception.Message);

            SimpleConsole.WriteErrorLine("Retrying after default time 30 seconds");
            await Task.Delay(30_000);

            return await TrySend(action);
        }
    }
}