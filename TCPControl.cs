using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
    enum TypeMessage
    {
        CaptchaImage,
        Captcha,
        Error,
        Info
    }
    class TCPMessageManager
    {
        TCPMessage bufferMessage = new TCPMessage();
        IPEndPoint endPointTelebot;
        public TCPMessageManager()
        {
            endPointTelebot = IPManager.GetEndPoint(new MySQL("clicker"),1);
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
            TCPMessage message = new TCPMessage(text, TypeMessage.CaptchaImage,site);
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
            TCPMessage message = new TCPMessage(text, TypeMessage.Error, site);
            SendTCPMesage(message);
        }
        
        private EventWaitHandle eventTCPHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
    }
    class TCPMessage
    {
        public string Text { get; set; }
        public byte[] Data { get; set; }
        [JsonConstructor]
        public TCPMessage()
        {

        }
        public TCPMessage(string text, TypeMessage type, EnumTypeSite typeSite)
        {
            Type = type;
            Text = text;
            Site = typeSite;
        }
        public TypeMessage Type { get; set; }
        public EnumTypeSite Site { get; set; }
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
        public event EventHandler<EventArgTCPClient> MessageReceived;
        TcpListener Listener;
        public TCPControl(IPEndPoint pEndPoint)
        {
            Listener = new TcpListener(pEndPoint);
            Listener.Start();            
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
        ~TCPControl()
        {
            if (Listener != null)
            {
                Listener.Stop();
            }
        }
        private async void ClientThread(TcpClient client)
        {
            Console.WriteLine("asdasdas");
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
