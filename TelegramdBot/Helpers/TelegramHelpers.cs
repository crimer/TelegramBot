using Telegram.Bot.Types;

namespace TelegramBot.Helpers;

public class TelegramHelpers
{
    /// <summary>
    /// Метод создания имени пользователя
    /// </summary>
    /// <param name="userFrom">Телеграм пользователь</param>
    /// <returns>Имя пользователя</returns>
    public static string MakeUserName(User userFrom)
    {
        if (!string.IsNullOrWhiteSpace(userFrom.Username))
            return $"@{userFrom.Username}";
            
        return $"{userFrom.LastName} {userFrom.FirstName}";
    }
}