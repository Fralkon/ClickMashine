using System.Drawing.Imaging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ClickMashine
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
        const long IdAdminTG = 597484739;
        object obj = new object();
        TelegramBotClient botClient;
        CancellationTokenSource cts;
        UpdateHandler handler;
        ReceiverOptions receiverOptions;
        List<long> IdChats;
        string buffer= ""; 
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
            IdChats.Add(IdAdminTG);
        }
        public async void Start()
        {
            botClient.StartReceiving(handler, receiverOptions, cancellationToken: cts.Token);
            Console.WriteLine("Bot started");
            await Task.Delay(-1, cancellationToken: cts.Token); // Такой вариант советуют MS: https://github.com/dotnet/runtime/issues/28510#issuecomment-458139641
            Console.WriteLine("Bot stopped");
        }
        public void Stop()
        {
            cts.Cancel();
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
        public async void SendError(Site site, string Message)
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