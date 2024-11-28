## Installation

1. Create telegram bot using bot father
2. Create `.env` file, with the token from the bot

```sh
TELEGRAM_TOKEN=Your-Telegram-Token
```

3. Point telegram webhooks to your machine.

There's a helper [/tests/set-webhooks.http](/template/tests/set-webhook.http) file with the configuration request. You'll need to get an https address for your local machine, first. You can get one from tools like ngrok. After you started an ngrok instance set the received url in the `.env` file.

```sh
MY_URL=https://50d3-2a0b-6204-1fcb-5a00-d424-2ddd-8190-1a0e.ngrok-free.app
```