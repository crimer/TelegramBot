using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Extentions;
using TelegramBot.Models;
using TelegramBot.Telegram.Commands.Common;

namespace TelegramBot.Telegram.Commands
{
    /// <summary>
    /// Команда телеграмм бота по выбору из нескольких значений
    /// </summary>
    public class TgDecideCommand : PublicCommand
    {
        public override string Name => "/decide";
        public TgDecideCommand(TelegramBotClient botClient, TgMessageHelper tgMessageHelper) : base(botClient, tgMessageHelper) { }
        
        /// <inheritdoc />
        protected override async Task<Result> Execute(Chat chat, User senderUser, ChatMember targetUser, int replyMessageId, List<TgMessageEntity> tgMessageEntities)
        {
            try
            {
                var texts = tgMessageEntities
                    .Where(m => m.Type == MessageEntityType.Unknown && !string.IsNullOrWhiteSpace(m.Text)) 
                    .ToList();
                
                if (texts.IsNullOrEmpty())
                {
                    await TgMessageHelper.SendMessageAsync(chat.Id, $"Пустой текст. Напиши что-то типа: /decide тут или там", replyMessageId);
                    return Result.Ok();
                }
                
                var separator = "или";
                
                var separatorIndex = tgMessageEntities.IndexOf(new TgMessageEntity() {Text = separator, Type = MessageEntityType.Unknown});
                
                if (separatorIndex == default)
                {
                    await TgMessageHelper.SendMessageAsync(chat.Id, $"Не найден разделитель: **{separator}**", replyMessageId, ParseMode.MarkdownV2);
                    return Result.Ok();
                }

                var variants = new List<string>();
                var message = new StringBuilder();
                for (int i = 0; i < texts.Count; i++)
                {
                    if (texts[i].Text.ToLower() != separator.ToLower())
                    {
                        message.Append($"{texts[i].Text} ");
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
                
                await TgMessageHelper.SendMessageAsync(chat.Id, $"Определенно \"{selection}\"", replyMessageId, ParseMode.MarkdownV2);
            }
            catch (Exception ex)
            {
                return Result.Fail($"Команда '/decide' ошибка: {ex}");
            }
            return Result.Ok();
        }
    }
}