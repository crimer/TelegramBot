using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TelegramBot.Configuration;
using TelegramBot.Repository;
using TelegramBot.Telegram;

namespace TelegramBot.Di
{
    /// <summary>
    /// Регистрация зависимостей приложения
    /// </summary>
    public static class RegisterDependencies
    {
        private const string AppSettingsFileName = "appsettings.json";
        
        /// <summary>
        /// Конфигурация зависимостей приложения
        /// </summary>
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(AppSettingsFileName, optional: false)
                .Build();

            services.Configure<AppConfig>(configuration.GetSection("Config"));

            services.AddSingleton<App>();

            // Сервисы
            services.AddSingleton<TelegramBotManager>();
            services.AddSingleton<RedisClient>();
            services.AddSingleton<RedisPersistenceRepository>();
        }
    }
}