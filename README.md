# Telegram Bot
## Запуск

1) Создать своего бота в телеграмм при помощи BotFather
2) Создать команды для бота:
   - roll - Бросить кубик
   - 8ball - Магический шар
   - decide - Выбери
3) В корне проекта создать файл `appsettings.json`, структуры вида:

```json
   "Config": {
     "BotToken": "<токен вашего бота>",
     "RedisDatabaseNumber": <номер бд Redis (int)>,
     "RedisConnection": "<адрес Redis>:<порт Redis>,name=<имя пользователя>,password=<пароль пользователя>,abortConnect=false"
   } 
```

