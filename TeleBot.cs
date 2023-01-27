using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace ClickMashine_10._0
{
    public class EventQuestionArg : EventArgs
    {
        public EventQuestionArg(string text)
        {
            this.text = text;
        }
        public string text { get; }
    }
    class TeleBot
    {
        object obj = new object();
        TelegramBotClient botClient;
        CancellationTokenSource cts;
        UpdateHandler handler;
        ReceiverOptions receiverOptions;
        List<long> IdChats;
        string buffer;
        EventWaitHandle eventMessage = new EventWaitHandle(false, EventResetMode.AutoReset);
        public TeleBot()
        {
            botClient = new TelegramBotClient("1607643022:AAGen7PfJb_EhTByGZk4cqooDCGCukcNG4w");
            cts = new CancellationTokenSource();
            handler = new UpdateHandler();
            handler.Question = false;
            handler.eventQuestion += GetMessageQuestion;
            receiverOptions = new ReceiverOptions();
            IdChats = new List<long>();
            IdChats.Add(597484739);
        }
        public async void Start()
        {
            botClient.StartReceiving(handler, receiverOptions, cancellationToken: cts.Token);
            Console.WriteLine("Bot started");
            await Task.Delay(-1, cancellationToken: cts.Token); // Такой вариант советуют MS: https://github.com/dotnet/runtime/issues/28510#issuecomment-458139641
            Console.WriteLine("Bot stopped");
        }
        private async void SendPhoto(Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                ms.Position = 0;
                await botClient.SendPhotoAsync(IdChats[0], new InputMedia(ms, "Screen"));
            };
        }
        public string SendQuestion(Bitmap image)
        {
            lock (obj)
            {
                SendPhoto(image);
                handler.Question = true;
                eventMessage.WaitOne();
            }
            return buffer;
        }
        public void SendError(Site site, string Message)
        {            
            await botClient.SendPhotoAsync(IdChats[0], Message);
        }
        public void GetMessageQuestion(object e, EventQuestionArg arg)
        {
            buffer = arg.text;
            eventMessage.Set();
        }
        class UpdateHandler : IUpdateHandler
        {
            public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
                if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    if (Question)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "OK");
                        eventQuestion(this, new EventQuestionArg(message.Text));
                        Question = false;
                    }
                    else
                        await botClient.SendTextMessageAsync(message.Chat, "Опоздал малеха");
                }
            }
            public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                Console.Error.WriteLine(exception);
                return Task.CompletedTask;
            }

            public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public bool Question { get; set; }
            public EventHandler<EventQuestionArg> eventQuestion { get; set; }
        }
    }
}