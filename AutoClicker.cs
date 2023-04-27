﻿using System.Data;
using System.Net;
using System.Xml.Linq;

namespace ClickMashine
{
    class AutoClicker
    {
        //TeleBot teleBot = new TeleBot();
        //Router? router;
        public MySQL mySQL;
        public TCPControl TCPControl { get; private set; }
        List<Site> siteList = new List<Site>();
        public AutoClicker(Form1 form)
        {
            mySQL = new MySQL("clicker"); 
            TCPControl = new TCPControl(IPManager.GetEndPoint(mySQL, form.ID));
            TCPControl.MessageReceived += TCPControl_MessageReceived;
            TCPControl.StartListing();
            try
            {                
                var authXML = XDocument.Load(form.PATH_SETTING + "Auth/" + form.Step.ToString() + ".xml");
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
                //siteList.Add(new WebofSar(form, teleBot, new Auth(authX.Element("webof-sar"))));
                siteList.Add(new SeoFast(form, teleBot, new Auth(authX.Element("seo-fast"))));
                //siteList.Add(new Profitcentr(form, teleBot, new Auth(authX.Element("profitcentr"))));
                siteList.Add(new WmrFast(form, teleBot, new Auth(authX.Element("wmrfast"))));
                using(DataTable authData = mySQL.GetDataTableSQL("SELECT login, password, site FROM auth WHERE id_object = " + form.ID.ToString() + " AND step = " + form.Step.ToString()))
                {
                    if(authData.Rows.Count > 0)
                    {
                        foreach (DataRow row in authData.Rows)
                        {
                            if (Enum.TryParse(row["site"].ToString(), out EnumTypeSite site))
                            {
                                Auth auth = new Auth(row["login"].ToString(), row["password"].ToString());
                                switch (site)
                                {
                                    case EnumTypeSite.SeoFast:
                                        {
                                            siteList.Add(new SeoFast(form, auth));
                                            break;
                                        }
                                    case EnumTypeSite.Profitcentr:
                                        {
                                            siteList.Add(new Profitcentr(form, auth));
                                            break;
                                        }
                                    default:
                                        throw new Exception("Нет такого сайта.");
                                }
                            }
                            else
                            {
                                throw new Exception("Ошибка парсинга сайта.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void TCPControl_MessageReceived(object? sender, EventArgTCPClient e)
        {
            foreach(Site site in siteList)
            {
                if(site.Type == e.Message.Site)
                {
                    site.TCPMessageManager.SetMessage(message: e.Message);
                    return;
                }
            }
        }
        public void ClickSurf()
        {       
            foreach (var site in siteList)
                site.Start();
        }
        public void Close()
        {
            foreach (var site in siteList)
                site.Stop();
        }
    }
}