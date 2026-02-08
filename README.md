# Turn-Based Tomacco

A turn-based fantasy game with Telegram bot integration.

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- Telegram Bot Token (from @BotFather)

### Setup

1. Clone the repository
```bash
git clone <your-repo>
cd Tomacco
```

2. Configure bot token
```bash
dotnet user-secrets set "Telegram:BotToken" "YOUR_BOT_TOKEN_HERE" --project src/Tomacco.TelegramBot
```

3. Run the application
```bash
dotnet run --project src/Tomacco.TelegramBot
```

## 📁 Project Structure

- **Domain**: Core entities and business rules
- **Application**: Business logic and services
- **Infrastructure**: Database and external services
- **TelegramBot**: Telegram bot presentation layer

## 🎮 Available Commands

- `/start` - Register
- `/profile` - View profile
- `/creategame` - Create new game
- `/games` - List available games
- `/startgame <code>` - Start game
- `/language` - Change language

## 🔒 Security

Never commit sensitive data! Use:
- User Secrets (recommended for development)
- Environment variables (recommended for production)
- appsettings.Development.json (gitignored)

## 📝 License

MIT