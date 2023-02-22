using System.Xml.Linq;

namespace ClickMashine
{
    class AutoClicker
    {
        TeleBot teleBot = new TeleBot();
        //Router? router;

        Thread? teleBotThread;

        List<Site> siteList = new List<Site>();
        public AutoClicker(Form1 form)
        {
            try
            {
                var authXML = XDocument.Load(form.PATH_SETTING + "Auth/" + form.Step + ".xml");
                XElement? authX = authXML.Element("Auth");
                if (authX == null)
                    throw new Exception("Нет файла логинов и паролей");
                teleBotThread = new Thread(teleBot.Start);
                teleBotThread.Start();

                //List<Auth> authRouter = new List<Auth>() { new Auth(authX.Element("google")), new Auth(authX.Element("vk")) };
                //router = new Router(form, teleBot);
                //router.Initialize();
                //router.Auth(authRouter);

                //siteList.Add(new Losena(form, teleBot, new Auth(authX.Element("losena"))));
                siteList.Add(new WebofSar(form, teleBot, new Auth(authX.Element("webof-sar"))));
                siteList.Add(new SeoFast(form, teleBot, new Auth(authX.Element("seo-fast"))));
                siteList.Add(new Profitcentr(form, teleBot, new Auth(authX.Element("profitcentr"))));
                //siteList.Add(new WmrFast(form, teleBot, new Auth(authX.Element("wmrfast"))));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } 
        public void ClickSurf()
        {       
            foreach (var site in siteList)
                site.Start();
            foreach (var site in siteList)
                site.Join();
        }
        public void Close()
        {
            teleBot.Stop();
            foreach (var site in siteList)
                site.Stop();
        }
    }
}