using System.Data;
using System.Net;
using System.Xml.Linq;

namespace ClickMashine
{
    class AutoClicker
    {
        public TCPControl TCPControl { get; private set; }
        List<Site> siteList = new List<Site>();
        public AutoClicker(Form1 form, MySQL mySQL)
        {
            TCPControl = new TCPControl(mySQL, form.ID);
            TCPControl.MessageReceived += TCPControl_MessageReceived;
            TCPControl.StartListing();
            try
            {                
                using(DataTable authData = mySQL.GetDataTableSQL("SELECT login, password, site FROM auth WHERE id_object = " + form.ID.ToString() + " AND step = " + form.Step.ToString() + " AND status = 'Activate'"))
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
                                    case EnumTypeSite.Adaso:
                                        {
                                            siteList.Add(new Adaso(form, auth));
                                            break;
                                        }
                                    case EnumTypeSite.Aviso:
                                        {
                                            siteList.Add(new Aviso(form, auth));
                                            break;
                                        }
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
                                    case EnumTypeSite.WmrFast:
                                        {
                                            siteList.Add(new WmrFast(form, auth));
                                            break;
                                        }
                                    case EnumTypeSite.WebofSar:
                                        {
                                            siteList.Add(new WebofSar(form, auth));
                                            break;
                                        }
                                    case EnumTypeSite.VipClick:
                                        {
                                            siteList.Add(new VipClick(form, auth));
                                            break;
                                        }
                                    case EnumTypeSite.SeoClub:
                                        {
                                            siteList.Add(new SeoClub(form, auth));
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