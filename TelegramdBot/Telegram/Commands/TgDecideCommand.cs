using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Extensions;
using TelegramBot.Helpers;
using TelegramBot.Models;
using TelegramBot.Telegram.Commands.Common;

namespace TelegramBot.Telegram.Commands;

/// <summary>
/// Команда телеграмм бота по выбору из нескольких значений
/// </summary>
public class TgDecideCommand : PublicCommand
{
    public override string Name => "/decide";
    public TgDecideCommand(IServiceProvider serviceProvider, ITelegramBotClient botClient) : base(serviceProvider, botClient) { }
        
    /// <inheritdoc />
    protected override async Task<Result> Execute(Chat chat, User senderUser, int replyMessageId, List<string> tgMessageEntities)
    {
        try
        {
            var texts = tgMessageEntities
                .Where(m => !string.IsNullOrWhiteSpace(m)) 
                .ToList();
                
            if (texts.IsNullOrEmpty())
            {
                await this.BotClient.SendTextMessageAsync(chat.Id, $"Пустой текст. Напиши что-то типа: /decide тут или там", replyToMessageId: replyMessageId);
                return Result.Ok();
            }
                
            var separator = "или";
                
            var separatorIndex = tgMessageEntities.IndexOf(separator);
                
            if (separatorIndex == default)
            {
                await this.BotClient.SendTextMessageAsync(chat.Id, $"Не найден разделитель: **{separator}**", replyToMessageId: replyMessageId, parseMode:ParseMode.MarkdownV2);
                return Result.Ok();
            }

            var variants = new List<string>();
            var message = new StringBuilder();
            for (int i = 0; i < texts.Count; i++)
            {
                if (texts[i].ToLower() != separator.ToLower())
                {
                    message.Append($"{texts[i]} ");
                }
                else
                {
                    variants.Add(message.ToString());
                    message.Clear();
                }
                if(i == texts.Count -1)
                    variants.Add(message.ToString());
            }
                
            var selection = variants[new Random().Next(variants.Count)].TrimEnd();
                
            await this.BotClient.SendTextMessageAsync(chat.Id, $"Определенно \"{selection}\"", replyToMessageId: replyMessageId, parseMode: ParseMode.MarkdownV2);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Команда '/decide' ошибка: {ex}");
        }
        return Result.Ok();
    }
}