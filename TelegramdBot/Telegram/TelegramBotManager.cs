using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Configuration;
using TelegramBot.Extentions;
using TelegramBot.Models;
using TelegramBot.Repository;
using TelegramBot.Telegram.Commands.Common;

namespace TelegramBot.Telegram
{
    /// <summary>
    /// Клиент Телеграм Бота
    /// </summary>
    public class TelegramBotManager
    {
        private readonly RedisPersistenceRepository _redisPersistenceRepository;
        private readonly ILogger<TelegramBotManager> _logger;
        private readonly TelegramBotClient _telegramBot;
        private readonly TgMessageHelper _tgMessageHelper;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="redisPersistenceRepository">Редис репозиторий</param>
        /// <param name="logger">Логгер</param>
        /// <param name="options">Конфиг</param>
        public TelegramBotManager(RedisPersistenceRepository redisPersistenceRepository, ILogger<TelegramBotManager> logger, IOptions<AppConfig> options)
        {
            _logger = logger;
            _telegramBot = new TelegramBotClient(options.Value.BotToken);
            _telegramBot.OnMessage += OnMessageReceived;
            _telegramBot.OnMessageEdited += OnMessageReceived;
            _redisPersistenceRepository = redisPersistenceRepository;
            _tgMessageHelper = new TgMessageHelper(_telegramBot);
        }

        /// <summary>
        /// Обработчик входящего сообщения
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Событие</param>
        private async void OnMessageReceived(object? sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (message == null)
                return;

            try
            {
                switch (message.Type)
                {
                    case MessageType.Text:
                        await HandleTextMessageAsync(message);
                        break;
                    case MessageType.ChatMembersAdded:
                        await HandleChatMembersAddedAsync(message);
                        break;
                    case MessageType.ChatMemberLeft:
                        await HandleChatMembersLeftAsync(message);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"При обработке сообщения {message.Type} из API произошла ошибка: {ex}");
            }
        }

        /// <summary>
        /// Обработчик выхода пользователя в чат
        /// </summary>
        /// <param name="message">Сообщение</param>
        private async Task HandleChatMembersLeftAsync(Message message)
        {
            try
            {
                if(message.LeftChatMember == null)
                    return;

                var userName = MakeUserName(
                    message.LeftChatMember.Username, 
                    message.LeftChatMember.FirstName, 
                    message.LeftChatMember.LanguageCode);
                
                await _redisPersistenceRepository.DeleteChatMemberAsync(message.Chat.Id, userName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Приозошла ошибка во время обработки поступления нового пользователя: {ex}");
            }
        }

        /// <summary>
        /// Обработчик входа пользователя в чат
        /// </summary>
        /// <param name="message">Сообщение</param>
        private async Task HandleChatMembersAddedAsync(Message message)
        {
            try
            {
                if(message.NewChatMembers.IsNullOrEmpty())
                    return;
            
                foreach (var chatMember in message.NewChatMembers)
                {
                    await _redisPersistenceRepository
                        .SaveChatMemberAsync(message.Chat.Id, chatMember.Id, MakeUserName(chatMember.Username, chatMember.FirstName, chatMember.LastName));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Приозошла ошибка во время обработки поступления нового пользователя: {ex}");
            }
        }

        /// <summary>
        /// Парсинг сообщения
        /// </summary>
        /// <param name="telegramMessage">Сообщение</param>
        /// <returns>Спосок элементов сообщения</returns>
        private Result<List<TgMessageEntity>> ParseMessage(Message telegramMessage)
        {
            try
            {
                var messages = new List<TgMessageEntity>();
            
                if (telegramMessage.Entities.IsNullOrEmpty()
                    || telegramMessage.EntityValues.IsNullOrEmpty()
                    || telegramMessage.Entities.Length != telegramMessage.EntityValues.Count())
                    
                    return Result.Ok(messages);

                var text = telegramMessage.EntityValues.ToArray();
                var rawMessage = telegramMessage.Text;
                
                for (int i = 0; i < telegramMessage.Entities.Length; i++)
                {
                    messages.Add(new TgMessageEntity()
                    {
                        Text = text[i],
                        Type = telegramMessage.Entities[i].Type,
                    });
                    rawMessage = rawMessage.Replace(text[i], "").TrimStart();
                }
                
                var messageArgs = rawMessage
                    .Split(" ")
                    .Select(m => new TgMessageEntity()
                {
                    Text = m,
                    Type = MessageEntityType.Unknown
                });

                messages.AddRange(messageArgs);
                
                return Result.Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Приозошла ошибка во время обработки поступления нового пользователя: {ex}");
                return Result.Fail("Приозошла ошибка во время обработки поступления нового пользователя");
            }
        }

        /// <summary>
        /// Метод создания имени пользователя
        /// </summary>
        /// <param name="userName">Никнейм (@nikita)</param>
        /// <param name="firstName">Имя</param>
        /// <param name="lastName">Фамилия</param>
        /// <returns>Имя пользователя</returns>
        private string MakeUserName(string userName, string firstName, string lastName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
                return $"@{userName}";
            
            return $"{lastName} {firstName}";
        }
        
        /// <summary>
        /// Метод обработчик текстового сообщения
        /// </summary>
        /// <param name="telegramMessage">Сообщение</param>
        private async Task HandleTextMessageAsync(Message telegramMessage)
        {
            try
            {
                var chat = telegramMessage.Chat;
                var userFrom = telegramMessage.From;
                
                _logger.LogInformation($"[{chat.Title} - {MakeUserName(userFrom.Username, userFrom.FirstName, userFrom.LastName)}] {telegramMessage.Text}");
                
                if (telegramMessage.From.Id == chat.Id)
                {
                    await _telegramBot.SendTextMessageAsync(chat.Id, "Пока личная переписка с ботм не предусмотрена");
                    return;
                }

                // Игнор простых сообщений, пропускаем только команды бота
                if(telegramMessage.Entities.IsNullOrEmpty() || telegramMessage.Entities.First().Type != MessageEntityType.BotCommand)
                    return;

                var tgMessagesResult = ParseMessage(telegramMessage);
                if (tgMessagesResult.IsFailed || tgMessagesResult.Value == null)
                    return;
                
                var mentionUserEntity = tgMessagesResult.Value.FirstOrDefault(m => m.Type == MessageEntityType.Mention);
                
                var commandText = tgMessagesResult.Value
                    .FirstOrDefault(m => m.Type == MessageEntityType.BotCommand)?
                    .Text.Split("@").FirstOrDefault();
                
                var command = TgCommandFactory.CreateCommand(commandText, _telegramBot, _tgMessageHelper);
                if (command == null)
                {
                    _logger.LogError($"Команда \"{commandText}\" не найдена");
                    return;
                }

                if (mentionUserEntity != null)
                {
                    var mentionUser = await _redisPersistenceRepository.GetChatMemberAsync(chat.Id, mentionUserEntity.Text);
                    if (mentionUser.IsFailed || mentionUser.Value == null)
                    {
                        _logger.LogError($"Не найден пользователь в базе {chat.Id} - {mentionUserEntity.Text}");
                        return;
                    }
                }

                // var chatMember = await _telegramBot.GetChatMemberAsync(mentionUser.Value.ChatId, (int)mentionUser.Value.UserId);
                
                var commandResult = await command.ExecuteAsync(chat, userFrom, null, telegramMessage.MessageId, tgMessagesResult.Value);
                if (commandResult.IsFailed)
                {
                    _logger.LogError(commandResult.ToString());
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"При обработке текстового сообщения произошла ошибка: {ex}");
            }
        }

        /// <summary>
        /// Получить экземпляр клиента бота
        /// </summary>
        /// <returns></returns>
        public TelegramBotClient GetClient() => _telegramBot;

        /// <summary>
        /// Проверка значения флага, подписался ли бот на получение сообщений
        /// </summary>
        public bool IsReceiving()
        {
            return _telegramBot.IsReceiving;
        }

        /// <summary>
        /// Запуск бота
        /// </summary>
        public void Start()
        {
            try
            {
                _telegramBot.StartReceiving();
                _logger.LogInformation("Бот готов ^_^");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Во время старта бота произошла ошибка: {ex}");
            }
        }
        
        /// <summary>
        /// Запуск бота
        /// </summary>
        public void Stop()
        {
            try
            {
                _telegramBot.StopReceiving();
            }
            catch (Exception ex)
            {
                _logger.LogError($"При выключении бота произошла ошибка: {ex}");
            }
        }
    }
}