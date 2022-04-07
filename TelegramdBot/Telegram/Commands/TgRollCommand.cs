using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Helpers;
using TelegramBot.Models;
using TelegramBot.Telegram.Commands.Common;

namespace TelegramBot.Telegram.Commands;

/// <summary>
/// Команда телеграмм бота "Бросить кости"
/// </summary>
public class TgRollCommand : PublicCommand
{
    public override string Name => "/roll";
    public TgRollCommand(IServiceProvider serviceProvider, ITelegramBotClient botClient) : base(serviceProvider, botClient) { }
        
    /// <inheritdoc />
    protected override async Task<Result> Execute(Chat chat, User senderUser, int replyMessageId, List<string> tgMessageEntities)
    {
        try
        {
            var maxRandomValue = 20;

            var maxValue = tgMessageEntities.FirstOrDefault();
            if (maxValue != null && !string.IsNullOrWhiteSpace(maxValue))
            {
                if (!int.TryParse(maxValue, out maxRandomValue))
                {
                    await this.BotClient.SendTextMessageAsync(chat.Id, $"Вторым аргументом команды должно быть целое число", replyToMessageId: replyMessageId);
                    return Result.Ok();
                }
            }
            await this.BotClient.SendTextMessageAsync(chat.Id, $"🎲 Шанс {new Random().Next(0, maxRandomValue)} из {maxRandomValue}", replyToMessageId: replyMessageId);
                
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Команда '/roll' ошибка: {ex}");
        }
    }
}