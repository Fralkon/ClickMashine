using ClickMashine.Sites.Aviso;
using System.Data;
using System.Net;
using System.Xml.Linq;

namespace ClickMashine
{
    class AutoClicker
    {
        public TCPControl TCPControl { get; private set; }
        List<Site> siteList = new List<Site>();
        public AutoClicker(Form1 form)
        {
            Auth auth = new Auth("iliya9401@gmail.com", "Ussd1801");
            siteList.Add(new Aviso(form, auth));
        }
        private void TCPControl_MessageReceived(object? sender, EventArgTCPClient e)
        {
            foreach (Site site in siteList)
                if (site.Type == e.Message.Site)
                {
                    site.TCPMessageManager.SetMessage(message: e.Message);
                    return;
                }
        }
        public void ClickSurf()
        {
            foreach (var site in siteList) 
            {                 
                site.Start(); 
                Thread.Sleep(2000); 
            }
        }
        public void Close()
        {
            foreach (var site in siteList)
                site.Stop();
        }
    }
}