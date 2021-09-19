using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Di;

namespace TelegramBot
{
    /// <summary>
    /// Главный файл
    /// </summary>
    class Program
    {
        /// <summary>
        /// Главный метод
        /// </summary>
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.ConfigureServices();
            
            var serviceProvider = services.BuildServiceProvider();

            // Точка входа
            Task.Run(async () => await serviceProvider.GetRequiredService<App>().Run(args));
            
            Console.ReadKey();
        }
    }
}