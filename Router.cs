using ClickMashine.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickMashine
{
    class Router : Site
    {
        public override async Task<bool> Auth(AuthData auth)
        {
            return true;
        }
        public Router(MainForm form, AuthData auth) : base(form, auth)
        {
            homePage = "https://www.google.com/";
            Type = EnumTypeSite.Router;
        }
        //public void Auth(List<Auth> auth)
        //{
        //    Sleep(2);
        //    AuthGoogle(auth[0]);
        //    AuthVK(auth[1]);
        //}
        
        //private bool AuthGoogle(Auth auth)
        //{
        //    LoadPage(0, "https://www.google.com/");

        //    string returnJS = SendJSReturn(0, "document.body.innerText;");
        //    if (returnJS.Contains("Войти"))
        //    {
        //        eventLoadPage.Reset();
        //        SendJS(0, "document.querySelectorAll('a')[3].click();");
        //        eventLoadPage.WaitOne();
        //        string JS = "document.querySelector('input').value = '" +
        //            auth.Login + "';" +
        //            "document.querySelector('#identifierNext > div > button > div.VfPpkd-RLmnJb').click();";
        //        SendJS(0, JS);
        //        Thread.Sleep(4000);
        //        JS = "document.querySelector('#password > div.aCsJod.oJeWuf > div > div.Xb9hP > input').value = '" +
        //            auth.Password + "';" +
        //            "document.querySelector('#passwordNext > div > button > div.VfPpkd-RLmnJb').click();";
        //        eventLoadPage.Reset();
        //        SendJS(0, JS);
        //        eventLoadPage.WaitOne();
        //        Thread.Sleep(7000);
        //    }
        //    return true;
        //}
        //private bool AuthVK(Auth auth)
        //{
        //    LoadPage(0, "https://vk.com");

        //    string inner_body = SendJSReturn(0, "return document.body.innerText;");
        //    CM(inner_body);
        //    if (inner_body.Contains("Войти"))
        //    {
        //        CM("VK NOT AUTH");
        //        eventLoadPage.Reset();
        //        SendJS(0, "document.querySelector('#index_login > div > form > button.FlatButton.FlatButton--primary.FlatButton--size-l.FlatButton--wide.VkIdForm__button.VkIdForm__signInButton').click();");
        //        eventLoadPage.WaitOne();
        //        SendKeysEvent(0, auth.Login);
        //        SendJS(0, "document.querySelector('.vkc__Button__title').click();");
        //        Sleep(2);
        //        SendKeysEvent(0, auth.Password);
        //        eventLoadPage.Reset();
        //        SendJS(0, "document.querySelector('.vkc__Button__title').click()");
        //        eventLoadPage.WaitOne();
        //    }
        //    else
        //        CM("VK AUTH");
        //    return true;
        //}

        //protected override void StartSurf()
        //{
        //}
    }
}
