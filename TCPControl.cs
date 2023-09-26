using System.Data;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClickMashine
{
    public enum EnumIPManager
    {
        MainControl,
        BotTelegram,
        ClickMashine
    }
    public static class IPManager
    {
        public static IPEndPoint GetEndPoint(MySQL mySQL, int IDObject)
        {
            using (DataTable dt = mySQL.GetDataTableSQL("SELECT ip, port FROM object WHERE id = " + IDObject.ToString()))
            {
                if (dt.Rows.Count > 0)
                {
                    return new IPEndPoint(IPAddress.Parse(dt.Rows[0]["ip"].ToString()),
                        int.Parse(dt.Rows[0]["port"].ToString()));
                }
                else return null;
            }
        }
    }
    public enum TypeMessage
    {
        CaptchaImage,
        Captcha,
        Error,
        Info
    }
    public class TCPMessageManager
    {
        TCPMessage bufferMessage = new TCPMessage();
        IPEndPoint endPointTelebot;
        int IDMashine;
        public TCPMessageManager(int IDMashine, IPEndPoint teleBot)
        {
            this.IDMashine = IDMashine;
            endPointTelebot = teleBot;
        }
        private void SendTCPMesage(TCPMessage message)
        {
            TcpClient ControlServer = new TcpClient();
            ControlServer.Connect(endPointTelebot);
            ControlServer.GetStream().Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize<TCPMessage>(message) + "\0"));
            ControlServer.Close();
        }
        public string SendQuestion(Bitmap image, string text, EnumTypeSite site)
        {
            TCPMessage message = new TCPMessage(text, IDMashine, TypeMessage.CaptchaImage,site);
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format: ImageFormat.Png);
                message.Data = ms.ToArray();
            }
            SendTCPMesage(message);
            eventTCPHandle.WaitOne();
            return bufferMessage.Text;
        }
        public void SetMessage(TCPMessage message)
        {
            Console.WriteLine(JsonSerializer.Serialize<TCPMessage>(message));
            bufferMessage = message;
            eventTCPHandle.Set();
        }
        public void SendError(string text, EnumTypeSite site)
        {
            TCPMessage message = new TCPMessage(text, IDMashine, TypeMessage.Error, site);
            SendTCPMesage(message);
        }
        public void SendInfo(string text, EnumTypeSite site)
        {
            TCPMessage message = new TCPMessage(text, IDMashine, TypeMessage.Info, site);
            SendTCPMesage(message);
        }
        private EventWaitHandle eventTCPHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
    }
    public class TCPMessage
    {
        public string Text { get; set; }
        public byte[] Data { get; set; }
        [JsonConstructor]
        public TCPMessage()
        {

        }
        public TCPMessage(string text, int IDMashine, TypeMessage type, EnumTypeSite typeSite)
        {
            this.IDMashine = IDMashine;
            Type = type;
            Text = text;
            Site = typeSite;
        }
        public TypeMessage Type { get; set; }
        public EnumTypeSite Site { get; set; }
        public int IDMashine { get; set; }
    }
    class EventArgTCPClient: EventArgs
    {
        public TCPMessage Message { get; set; }
        public EventArgTCPClient(TCPMessage message)
        {
            Message = message;
        }
    }
    class TCPControl
    {
        public const int Port = 7000;
        public event EventHandler<EventArgTCPClient> ?MessageReceived;
        TcpListener Listener;
        public TCPControl(MySQL mySQL, int IDMashine)
        {            
            //IPAddress iP = Dns.GetHostAddresses(Dns.GetHostName()).First<IPAddress>(f => f.AddressFamily == AddressFamily.InterNetwork && f.MapToIPv4().ToString().IndexOf("192") != -1);
            IPAddress iP = Dns.GetHostAddresses(Dns.GetHostName()).First<IPAddress>(f => f.AddressFamily == AddressFamily.InterNetwork && f.MapToIPv4().ToString().IndexOf("172") != -1);
            if (iP == null)
                throw new Exception("Error IP server");

            mySQL.SendSQL("UPDATE object SET status = 'online' , ip = '" + iP.ToString() + "' , port = " + Port.ToString() + " WHERE id = " + IDMashine.ToString());

            Listener = new TcpListener(iP,Port);
            Listener.Start();
        }
        ~TCPControl()
        {
            Listener?.Stop();
        }
        public async void StartListing()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    ClientThread(Listener.AcceptTcpClient());
                }
            });
        }
        private async void ClientThread(TcpClient client)
        {
            await Task.Run(() =>
            {
                var stream = client.GetStream();
                string message = string.Empty;
                do
                {
                    byte buffer = (byte)stream.ReadByte();
                    if (buffer == 0)
                        break;
                    message += (char)buffer;
                }
                while (true);
                Console.WriteLine(message);
                TCPMessage? m = JsonSerializer.Deserialize<TCPMessage>(message);
                if (m == null)
                    Console.WriteLine("Error serelize");
                else
                    MessageReceived?.Invoke(this,new EventArgTCPClient(m));
                client.Close();
            });
        }
    }
}
