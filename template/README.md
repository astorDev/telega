## Installation

1. Create telegram bot using bot father
2. Create `.env` file, with the token from the bot

```sh
BOT_TOKEN=Your-Telegram-Token
```

MediatR 15 supports a license key through the `MEDIATR_LICENSE_KEY` environment variable. See [MediatR's licensing information](https://mediatr.io/) for the applicable terms.

3. (Optional) Point Telegram webhooks to your machine.

There's a helper [tests/set-webhook.http](./tests/set-webhook.http) file with the configuration request. To receive Telegram updates locally, you'll need an HTTPS address for your machine. You can get one from tools like ngrok. After starting an ngrok instance, set the received URL in the `.env` file.

```sh
MY_URL=https://50d3-2a0b-6204-1fcb-5a00-d424-2ddd-8190-1a0e.ngrok-free.app
```