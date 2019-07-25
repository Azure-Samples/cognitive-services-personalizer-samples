using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PersonalizerBusinessDemo.Repositories;
using PersonalizerBusinessDemo.Services;
using PersonalizerBusinessDemo.Services.ActionFeaturizer;

namespace PersonalizerBusinessDemo
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
            var personalizationEndPoint = Configuration["PersonalizationEndpoint"];
            var personalizationApiKey = Configuration["PersonalizationApiKey"];
            var cognitiveTextAnalyticsSubscriptionKey = Configuration["TextAnalyticsKey"];
            var cognitiveTextAnalyticsEndpoint = Configuration["TextAnalyticsEndpoint"];

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSingleton<ITextAnalyticsClient>(s => new TextAnalyticsClient(new ApiKeyServiceClientCredentials(cognitiveTextAnalyticsSubscriptionKey))
            {
                // Cognitive Service Endpoint
                Endpoint = cognitiveTextAnalyticsEndpoint.Split("/text/analytics")[0]
            });
            services.AddSingleton<CognitiveTextAnalyzer, CognitiveTextAnalyzer>();
            services.AddSingleton<IActionFeaturizer, CognitiveTextAnalyticsFeaturizer>();
            services.AddSingleton<IPersonalizerClient>(s => CreateClient(personalizationEndPoint, personalizationApiKey));
            services.AddSingleton<IActionsRepository, ActionsRepository>();
            services.AddSingleton<IPersonalizerService, PersonalizerService>();
            services.AddSingleton<IArticleRepository, ArticleRepository>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private IPersonalizerClient CreateClient(string uri, string ApiKey)
        {
            return new PersonalizerClient(
                new ApiKeyServiceClientCredentials(ApiKey))
            {
                Endpoint = uri
            };
        }
    }
}