namespace TelegramBot.Configuration
{
    /// <summary>
    /// Класс конфигурации приложения
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Токен бота
        /// </summary>
        public string BotToken { get; set; }

        /// <summary>
        /// Строка подключение к Redis
        /// </summary>
        public string RedisConnection { get; set; }
        
        /// <summary>
        /// Номер БД Redis
        /// </summary>
        public int RedisDatabaseNumber { get; set; }
    }
}