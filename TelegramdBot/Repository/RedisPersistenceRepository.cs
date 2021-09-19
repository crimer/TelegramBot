using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TelegramBot.Models;

namespace TelegramBot.Repository
{
    /// <summary>
    /// Репозиторий для работы с Redis
    /// </summary>
    public class RedisPersistenceRepository
    {
        private readonly RedisClient _redisClient;
        private readonly ILogger<RedisPersistenceRepository> _logger;
        private readonly string _usersKey = "telegram:bot:users";
        
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="redisClient">Клиент Redis</param>
        /// <param name="logger">Логгер</param>
        public RedisPersistenceRepository(RedisClient redisClient, ILogger<RedisPersistenceRepository> logger)
        {
            _redisClient = redisClient;
            _logger = logger;
        }
        
        /// <summary>
        /// Получение списка участников чата
        /// </summary>
        /// <returns>Список участников чата</returns>
        public async Task<Result<List<TgChatMember>>> GetAllChatMembersAsync()
        {
            try
            {
                var redis = _redisClient.GetDatabase();
                var chatsJson = await redis.HashGetAllAsync(_usersKey);
                var chats = new List<TgChatMember>(chatsJson.Length);
                
                foreach (var chatJson in chatsJson)
                    chats.Add(JsonConvert.DeserializeObject<TgChatMember>(chatJson.Value));

                return Result.Ok(chats);
            }
            catch (Exception e)
            {
                _logger.LogError($"При получении списка участников чата произошла ошибка: {e}");
                return Result.Fail<List<TgChatMember>>($"При получении списка участников чата произошла ошибка");
            }
        }

        /// <summary>
        /// Сохранения участника чата
        /// </summary>
        /// <param name="chatId">Идентификатор чата</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="chetMemberName">Имя пользователя</param>
        public async Task<Result> SaveChatMemberAsync(long chatId, long userId, string chetMemberName)
        {
            try
            {
                var redis = _redisClient.GetDatabase();
                var chatMember = new TgChatMember()
                {
                    Name = chetMemberName,
                    ChatId = chatId,
                    UserId = userId
                };
                var jsonUser = JsonConvert.SerializeObject(chatMember); 
                
                var containUser = await redis.SetContainsAsync(GetKey(chatId, chetMemberName), jsonUser);
                
                if(!containUser)
                    await redis.HashSetAsync(_usersKey, GetKey(chatId, chetMemberName), jsonUser);
                
                return Result.Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"При сохранении информации о участнике чата произошла ошибка: {e}");
                return Result.Fail<List<Chat>>($"При сохранении информации о участнике чата произошла ошибка");
            }
        }
        
        /// <summary>
        /// Удаление участника чата
        /// </summary>
        /// <param name="chatId">Идентификатор чата</param>
        /// <param name="userName">Имя пользователя</param>
        public async Task<Result> DeleteChatMemberAsync(long chatId, string userName)
        {
            try
            {
                var redis = _redisClient.GetDatabase();
                await redis.HashDeleteAsync(_usersKey, GetKey(chatId, userName));
                
                return Result.Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"При удалении информации о участнике чата произошла ошибка: {e}");
                return Result.Fail<List<Chat>>($"При удалении информации о участнике чата произошла ошибка");
            }
        }
        
        /// <summary>
        /// Получение участника чата 
        /// </summary>
        /// <param name="chatId">Идентификатор чата</param>
        /// <param name="userName">Имя пользователя</param>
        /// <returns></returns>
        public async Task<Result<TgChatMember>> GetChatMemberAsync(long chatId, string userName)
        {
            try
            {
                var redis = _redisClient.GetDatabase();
                var redisData = redis.HashGet(_usersKey, GetKey(chatId, userName));
                
                if (!redisData.HasValue)
                    return Result.Fail($"Не удалось найти запись: {GetKey(chatId, userName)}");
                
                var chatMember = JsonConvert.DeserializeObject<TgChatMember>(redisData);

                return Result.Ok(chatMember);
            }
            catch (Exception e)
            {
                _logger.LogError($"При получении информации о участнике чата произошла ошибка: {e}");
                return Result.Fail($"При получении информации о участнике чата произошла ошибка");
            }
        }
        
        
        /// <summary>
        /// Получение ключа для записи Redis
        /// </summary>
        /// <param name="chatId">Идентификатор чата</param>
        /// <param name="chetMemberName">Имя участника чата</param>
        /// <example>
        /// 11111_nikita
        /// </example>
        /// <returns>Ключ</returns>
        private string GetKey(long chatId, string chetMemberName)
        {
            return $"{chatId}_{chetMemberName}";
        }
    }
}