using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using VvsuParser.Models;

namespace TelegramBot.Repository;

/// <summary>
/// Репозиторий для работы с Redis
/// </summary>
public class RedisPersistenceRepository
{
    private readonly RedisClient _redisClient;
    private readonly ILogger<RedisPersistenceRepository> _logger;
        
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

    public async Task<Result> SaveAllVvsuGroupsAsync(List<string> allGroups)
    {
        try
        {
            var redis = _redisClient.GetDatabase();

            var data = allGroups.Select(s => new RedisValue(s)).ToArray();
            var containUser = await redis.ListRightPushAsync("telegram:bot:allVvsuGroups", data);
            
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError($"При сохранении списка групп произошла ошибка: {e}");
            return Result.Fail($"При сохранении списка групп произошла ошибка");
        }
    }
    
    public async Task<Result<List<string>>> GetAllVvsuGroupsAsync()
    {
        try
        {
            var redis = _redisClient.GetDatabase();
            var redisData = await redis.ListRangeAsync("telegram:bot:allVvsuGroups");
                
            if (redisData.Length == 0)
                return Result.Fail<List<string>>($"Не удалось найти группы");

            var allGroups = redisData
                .Select(el => el.ToString())
                .ToList();

            return Result.Ok(allGroups);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Не удалось найти группы: {ex}");
            return Result.Fail($"Не удалось найти группы");
        }
    }
    
    public async Task<Result> SaveCurrentGroupAsync(long chatId, string groupName)
    {
        try
        {
            var redis = _redisClient.GetDatabase();
            var successSave = await redis.HashSetAsync("telegram:bot:chatCurrentGroup", chatId, groupName);
                
            if (!successSave)
                return Result.Fail($"Не удалось сохранить текущую группу для чата: {chatId}");

            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError($"При сохранении текущей группы для чата произошла ошибка: {e}");
            return Result.Fail($"При сохранении текущей группы для чата произошла ошибка");
        }
    }
    
    public async Task<Result<string>> GetCurrentGroupAsync(long chatId)
    {
        try
        {
            var redis = _redisClient.GetDatabase();
            var redisData = redis.HashGet("telegram:bot:chatCurrentGroup", chatId);
                
            if (!redisData.HasValue)
                return Result.Fail<string>($"Не удалось найти запись текущей группы для чата: {chatId}");

            var chatMember = (string)redisData;

            return Result.Ok(chatMember);
        }
        catch (Exception e)
        {
            _logger.LogError($"При получении текущей группы чата произошла ошибка: {e}");
            return Result.Fail($"При получении текущей группы чата произошла ошибка");
        }
    }
    
    public async Task<Result<List<VvsuStudyScheduleWeek>>> GetGroupScheduleAsync(string group)
    {
        try
        {
            var redis = _redisClient.GetDatabase();
            var redisValue = await redis.HashGetAsync("telegram:bot:groupSchedule", group);
            if (!redisValue.HasValue)
                return Result.Fail($"Не удалось получить расписание группы: {group}");

            var json = (string)redisValue;
            var scheduleWeeks = JsonConvert.DeserializeObject<List<VvsuStudyScheduleWeek>>(json);
            
            return Result.Ok(scheduleWeeks);
        }
        catch (Exception e)
        {
            _logger.LogError($"При получении расписания групы произошла ошибка: {e}");
            return Result.Fail($"При получении расписания групы произошла ошибка");
        }
    }
    
    public async Task<Result> SaveGroupScheduleAsync(string group, List<VvsuStudyScheduleWeek> scheduleWeeks)
    {
        try
        {
            var redis = _redisClient.GetDatabase();
            var scheduleWeeksJson = JsonConvert.SerializeObject(scheduleWeeks);
            
            var successSet = await redis.HashSetAsync("telegram:bot:groupSchedule", group, scheduleWeeksJson);
            if (!successSet)
                return Result.Fail($"Не удалось сохранить расписание группы: {group}");
            
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError($"При сохранении расписания групы произошла ошибка: {e}");
            return Result.Fail($"При сохранении расписания групы произошла ошибка");
        }
    }
}