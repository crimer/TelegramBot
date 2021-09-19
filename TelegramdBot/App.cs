using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TelegramBot.Telegram;

namespace TelegramBot
{
    /// <summary>
    /// Точка входа
    /// </summary>
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly TelegramBotManager _telegramBotManager;
        
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="telegramBotManager">Клиент Телеграм Бота</param>
        public App(ILogger<App> logger, TelegramBotManager telegramBotManager)
        {
            _logger = logger;
            _telegramBotManager = telegramBotManager;
        }
        
        /// <summary>
        /// Запуск
        /// </summary>
        /// <param name="args"></param>
        public async Task Run(string[] args)
        {
            try
            {
                _logger.LogInformation("Стартуем...");
                _telegramBotManager.Start();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            
            await Task.CompletedTask;
        }
    }
}