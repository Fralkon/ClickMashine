using System.Xml.Linq;

namespace ClickMashine
{
    class AutoClicker
    {
        TeleBot teleBot = new TeleBot();
        Router? router;

        Thread? teleBotThread;

        List<SiteClicker> siteClickers = new List<SiteClicker>();
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

                router = new Router(form, teleBot);
                router.Initialize();
                //List<Auth> authRouter = new List<Auth>() { new Auth(authX.Element("google")), new Auth(authX.Element("vk")) };
                //router.Auth(authRouter);

                //siteClickers.Add(new SiteClicker(new WebofSar(form, teleBot), authX.Element("webof-sar")));
                //siteClickers.Add(new SiteClicker(new SeoFast(form, teleBot), authX.Element("seo-fast")));
                //siteClickers.Add(new SiteClicker(new Profitcentr(form, teleBot), authX.Element("profitcentr")));
                siteClickers.Add(new SiteClicker(new WmrFast(form, teleBot), authX.Element("wmrfast")));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } 
        public void ClickSurf()
        {       
            for(int i = 0; i < siteClickers.Count; i++)
                siteClickers[i].Start();
        }
        public void Close()
        {
            for (int i = 0; i < siteClickers.Count; i++)
                siteClickers[i].Stop();
        }
    }
    class SiteClicker {
        Site Site;
        Auth auth;
        public SiteClicker(Site site, XElement? element)
        {
            auth = new Auth(element);
            this.Site = site;
        }
        public void Start()
        {
            Site.Initialize();
            Site.Auth(auth);
            Site.Start();
        }
        public void Stop()
        {
            Site.Stop();
        }
    }
}