using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JNogueira.Logger.Discord
{
    public class DiscordLoggerProvider : ILoggerProvider
    {
        private readonly IHttpContextAccessor _httpContextAcessor;
        private readonly DiscordLoggerOptions _options;
        private readonly IHttpClientFactory _clientFactory;

        private DiscordLogger _logger;

        public DiscordLoggerProvider(DiscordLoggerOptions options, IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAcessor = null)
        {
            _options            = options;
            _clientFactory      = clientFactory;
            _httpContextAcessor = httpContextAcessor;
        }

        public ILogger CreateLogger(string name)
        {
            _logger = new DiscordLogger(_options, _httpContextAcessor, _clientFactory);

            return _logger;
        }

        public void Dispose()
        {
            _logger = null;
        }
    }

    public static class DiscordLoggerProviderExtensions
    {
        public static ILoggerFactory AddDiscord(this ILoggerFactory loggerFactory, DiscordLoggerOptions options, IHttpContextAccessor httpContextAccessor = null)
        {
            var services = new ServiceCollection();
            services.AddDiscordHttpClient();
            var provider = services.BuildServiceProvider();
            var httpClientFactory = provider.GetService<IHttpClientFactory>();
            loggerFactory.AddProvider(new DiscordLoggerProvider(options, httpClientFactory, httpContextAccessor));

            return loggerFactory;
        }
        
        public static ILoggerFactory AddDiscord(this ILoggerFactory loggerFactory, 
            IServiceCollection services,
            DiscordLoggerOptions options, 
            IHttpContextAccessor httpContextAccessor = null)
        {
            services.AddDiscordHttpClient();
            var provider = services.BuildServiceProvider();
            loggerFactory.AddProvider(new DiscordLoggerProvider(options, provider.GetService<IHttpClientFactory>(), httpContextAccessor));

            return loggerFactory;
        }

        private static void AddDiscordHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<DiscordLogger>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });
        }
    }
}
