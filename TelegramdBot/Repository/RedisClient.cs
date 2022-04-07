using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TelegramBot.Configuration;

namespace TelegramBot.Repository;

/// <summary>
/// Клиент для работы с Redis
/// </summary>
public class RedisClient
{
    private readonly ConnectionMultiplexer _connection;
    private readonly int _redisDatabaseNumber;

    /// <summary>
    /// Констуктор
    /// </summary>
    /// <param name="options">Конфиг</param>
    public RedisClient(IOptions<AppConfig> options)
    {
        _connection = ConnectionMultiplexer.Connect(options.Value.RedisConnection);
        _redisDatabaseNumber = options.Value.RedisDatabaseNumber;
    }
        
    /// <summary>
    /// Получение <see cref="IDatabase"/>
    /// </summary>
    public IDatabase GetDatabase()
    {
        return _connection.GetDatabase(_redisDatabaseNumber);
    }
}