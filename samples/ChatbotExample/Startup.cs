// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using ChatbotSample.ReinforcementLearning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace ChatbotSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Check that all configuration values have been specified
            IConfiguration chatbotConfig = Configuration.GetSection("PersonalizerChatbot");
            var missingConfigValues = chatbotConfig.GetChildren().Where(c => string.IsNullOrEmpty(c.Value));
            if (missingConfigValues.Count() > 0)
            {
                throw new Exception($"Missing config values in appsettings. Please check the README to ensure you have filled in all the necessary Personalizer and LUIS config values. {string.Join(",", missingConfigValues.Select(cv => cv.Key).ToArray())}");
            }

            services.AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the reinforcement learning feature manager
            services.AddSingleton<RLContextManager>();

            // Register LUIS recognizer
            services.AddSingleton(recognizer =>
            {
                var luisApplication = new LuisApplication(
                    chatbotConfig["LuisAppId"],
                    chatbotConfig["LuisAPIKey"],
                    chatbotConfig["LuisServiceEndpoint"]);
                // Set the recognizer options depending on which endpoint version you want to use.
                // More details can be found in https://docs.microsoft.com/en-gb/azure/cognitive-services/luis/luis-migration-api-v3
                var recognizerOptions = new LuisRecognizerOptionsV3(luisApplication)
                {
                    PredictionOptions = new Microsoft.Bot.Builder.AI.LuisV3.LuisPredictionOptions
                    {
                        IncludeInstanceData = true,
                    }
                };

                return new LuisRecognizer(recognizerOptions);
            });

            // Register Personalizer client
            services.AddSingleton(client =>
            {
                return new PersonalizerClient(new ApiKeyServiceClientCredentials(chatbotConfig["PersonalizerAPIKey"]))
                {
                    Endpoint = chatbotConfig["PersonalizerServiceEndpoint"],
                };
            });

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, Bots.PersonalizerChatbot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
