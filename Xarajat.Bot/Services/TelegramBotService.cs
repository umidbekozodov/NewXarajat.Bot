using Microsoft.Extensions.Options;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;

namespace Xarajat.Bot.Services
{
    public class TelegramBotService
    {
        private readonly TelegramBotClient _bot;

        public TelegramBotService(IConfiguration configuration)
        {
            _bot = new TelegramBotClient(configuration["BotToken"]);
        }

        public async Task SendMessage(long chatId, string message, IReplyMarkup? reply = null)
        {
            await _bot.SendTextMessageAsync(chatId, message, replyMarkup: reply);
        }

        public async Task SendMessage(long chatId, string message, Stream image, IReplyMarkup? reply = null)
        {
            await _bot.SendPhotoAsync(chatId, new InputOnlineFile(image), message, replyMarkup: reply);
        }

        public async Task EditMessageButtons(long chatId, int messageId, InlineKeyboardMarkup reply)
        {
            await _bot.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: reply);
        }

        public ReplyKeyboardMarkup GetKeyboard(List<string> buttonsText)
        {
            var buttons = new KeyboardButton[buttonsText.Count][];

            for (var i = 0; i < buttonsText.Count; i++) 
                buttons[i] = new KeyboardButton[] { new(buttonsText[i]) };

            return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
        }

        public InlineKeyboardMarkup GetInlineKeyboard(List<string> buttonsText)
        {
            var buttons = new InlineKeyboardButton[buttonsText.Count][];

            for (var i = 0; i < buttonsText.Count; i++)
            {
                buttons[i] = new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(
                text: buttonsText[i],
                callbackData: buttonsText[i]) };
            }

            return new InlineKeyboardMarkup(buttons);
        }
    }
}
