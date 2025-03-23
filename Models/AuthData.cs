using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClickMashine.Models
{
    public enum EnumTypeSite
    {
        None,
        Router,
        SeoFast,
        Aviso,
        Profitcentr,
        WmrFast,
        WebofSar,
        Losena,
        SeoClub,
        VipClick,
        Adaso
    }
    public class AuthData
    {
        public AuthData() { }
        public AuthData(string login, string password, EnumTypeSite site) 
        {
            Login = login;
            Password = password;
            Site = site;
        }
        public string Login { get; set; }
        public string Password { get; set; }
        public EnumTypeSite Site { get; set; }
    }
    class Auth
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public Auth(string login, string password)
        {
            Login = login;
            Password = password;
        }
        public Auth(XElement? xml)
        {
            if (xml == null)
                throw new Exception("Not valid auth site");
            Login = xml.Element("login").Value.Trim();
            Password = xml.Element("password").Value.Trim();
        }
    }
}
