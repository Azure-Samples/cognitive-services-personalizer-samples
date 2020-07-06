// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChatbotExample.Services
{
    public class PersonalizerCoffeeClient : IDisposable
    {
        private readonly PersonalizerClient _personalizerClient;

        public PersonalizerCoffeeClient(IConfiguration configuration)
        {
            string personalizerEndpoint = configuration["PersonalizerServiceEndpoint"];
            string personalizerApiKey = configuration["PersonalizerAPIKey"];
            var personalizerIsConfigured = !string.IsNullOrEmpty(personalizerEndpoint) && !string.IsNullOrEmpty(personalizerApiKey);

            if (personalizerIsConfigured)
            {
                _personalizerClient = new PersonalizerClient(new ApiKeyServiceClientCredentials(personalizerApiKey))
                {
                    Endpoint = personalizerEndpoint
                };
            }
        }

        // Returns true if Personalizer is configured in the appsettings.json and initialized.
        public virtual bool IsConfigured => _personalizerClient != null;

        public Task RankAsync(RankRequest rankRequest, CancellationToken cancellationToken) => _personalizerClient.RankAsync(rankRequest, cancellationToken);

        public Task RewardAsync(string eventId, RewardRequest reward, CancellationToken cancellationToken) => _personalizerClient.RewardAsync(eventId, reward, cancellationToken);

        public void Dispose() => _personalizerClient.Dispose();

    }
}
