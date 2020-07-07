// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace ChatbotSample.Services
{
    public class CoffeeRecognizer : IRecognizer
    {
        private readonly LuisRecognizer _recognizer;

        public CoffeeRecognizer(IConfiguration configuration)
        {
            string luisAppId = configuration["LuisAppId"];
            string luisApiKey = configuration["LuisAPIKey"];
            string luisServiceEndpoint = configuration["LuisServiceEndpoint"];

            var luisIsConfigured = !string.IsNullOrEmpty(luisAppId) && !string.IsNullOrEmpty(luisApiKey) && !string.IsNullOrEmpty(luisServiceEndpoint);
            if (luisIsConfigured)
            {
                var luisApplication = new LuisApplication(
                    luisAppId,
                    luisApiKey,
                    luisServiceEndpoint);
                // Set the recognizer options depending on which endpoint version you want to use.
                // More details can be found in https://docs.microsoft.com/en-gb/azure/cognitive-services/luis/luis-migration-api-v3
                var recognizerOptions = new LuisRecognizerOptionsV3(luisApplication)
                {
                    PredictionOptions = new Microsoft.Bot.Builder.AI.LuisV3.LuisPredictionOptions
                    {
                        IncludeInstanceData = true,
                    }
                };

                _recognizer = new LuisRecognizer(recognizerOptions);
            }
        }

        // Returns true if luis is configured in the appsettings.json and initialized.
        public virtual bool IsConfigured => _recognizer != null;

        public virtual async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
            => await _recognizer.RecognizeAsync(turnContext, cancellationToken);

        public virtual async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken)
            where T : IRecognizerConvert, new()
            => await _recognizer.RecognizeAsync<T>(turnContext, cancellationToken);
    }
}
