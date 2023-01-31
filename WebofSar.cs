﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMashine_10._0
{
    internal class WebofSar : Site
    {
        public WebofSar(Form1 form, TeleBot teleBot) : base(form, teleBot)
        {
            homePage = "https://webof-sar.ru/";
            type.enam = EnumTypeSite.WebofSar;
        }
        public override void StartSurf()
        {
            // MailSurf();
            //ClickSurf(); 
            VisitSites();
            //YouTubeSurf();
        }
        public override void Auth(Auth auth)
        {
            LoadPage(0, "https://webof-sar.ru/");
            eventLoadPage.Reset();
            string ev = SendJSReturn(0, @"var l_b = document.querySelector('.sub-log-user');
            if (l_b != null){
                l_b.click();
               'login';}
            else {'go';}");
            if (ev == "login")
            {
                eventLoadPage.WaitOne(5000);
                SendJS(0, "captcha_choice('2');onclick=\"save_enter();\"");
                Sleep(1);
                WaitElement(browsers[0].MainFrame, "document.querySelector('#auth_user_log');");

                string js = "document.querySelector('#auth_user_log').value = '" + auth.Login + "';" +
                "document.querySelector('#auth_user_pas').value = '" + auth.Password + "';" +
                "document.querySelector('[class=\"btn btn-primary\"]').click();";
                eventLoadPage.Reset();
                SendJS(0, js);
                eventLoadPage.WaitOne();
                Sleep(2);
            }
        }
        private void ClickSurf()
        {
            LoadPage("https://webof-sar.ru/work-surfings");
            string js =
@"var surf_cl = document.querySelectorAll('.wfsts');var n = 0;
function surf()
{
    var start_ln = surf_cl[n].querySelector('[type=""button""]');
    if (start_ln != null) { start_ln.click(); n++; return 'click'; }
    else { return 'wait'; }
}
function click_s()
{
    if (n >= surf_cl.length) return 'end_surf';
    else
    {
        surf_cl[n].querySelector('[class=""td-serfm work-surf-start""]').click(); return 'surf';
    }
}";
            SendJS(0, js);
            while (true)
            {
                string ev = SendJSReturn(0, "click_s();");
                if (ev == "end_surf")
                    break;
                else if (ev == "continue")
                    continue;
                else if (ev == "surf")
                {
                    ev = WaitFunction(browsers[0].MainFrame, "surf();");
                    if (ev == "errorWait")
                    {
                        SendJS(0, "n++;");
                        Sleep(1);
                    }
                    else if (ev == "click")
                    {
                        WaitCreateBrowser(1);
                        Sleep(2);
                        SendJS(browsers[1].MainFrame, @"loadFrame();
$(window).on(""focus"", function () {   
    wFocus = true;
    dFocus = true;
});
$(window).on(""blur"", function() {                       
    wFocus = true;
    dFocus = true;
});
$(window).focus();");
                        browsers[1].GetHost().SetFocus(true);
                        Sleep(1);
                        ev = WaitElement(browsers[1].MainFrame, "document.querySelector('#Timer')");
                        if (ev == "end")
                        {
                            ev = SendJSReturn(browsers[1].MainFrame, @"document.querySelector('#Timer').innerText;");
                            Sleep(ev);
                            if (WaitButtonClick(browsers[1].MainFrame, "document.querySelector('[class=\"block-success work-check\"]')", 5) == "errorWait")
                            {
                                SendJS(browsers[1].MainFrame, 
@"clearInterval(idInterval[""Timer""]);
$(""#BlockWait"").remove();
$(""#BlockTimer"").fadeIn(""fast"");
var aDefOpts = {
    elemTimer: selectorTimer, 
    interval: intervalTimer, 
}
var aOpts = $.extend(aDefOpts);
var param = $(aOpts.elemTimer);

statusTimer = 1;
clearTimeout(idTimeout[""Timer""]);
clearInterval(idInterval[""Timer""]);
fnWork(param, param.data(""id""), param.data(""op""), param.data(""token""));");
                                WaitButtonClick(browsers[1].MainFrame, "document.querySelector('[class=\"block-success work-check\"]')");
                            }
                            Sleep(3);
                        }
                    }
                }
                CloseСhildBrowser();
            }
        }
        private void VisitSites()
        {
            LoadPage("https://webof-sar.ru/work-pay-visits");
            string js =
@"var surf_cl = document.querySelectorAll('.td-work');var n = 0;
function surf()
{
    var start_ln = surf_cl[n].querySelector('span');
    if (start_ln != null) { surf_cl[n].querySelector('span').click(); n++; return 'click'; }
    else { return 'wait'; }
}
function click_s()
{
    if (n >= surf_cl.length) return 'end_surf';
    else
    {
        surf_cl[n].click(); return 'surf';
    }
}";
            SendJS(0, js);
            while (true)
            {
                string ev = SendJSReturn(0, "click_s();");
                if (ev == "end_surf")
                    break;
                else if (ev == "continue")
                    continue;
                else if (ev == "surf")
                {
                    ev = WaitFunction(browsers[0].MainFrame, "surf();");
                    if (ev == "errorWait")
                    {
                        SendJS(0, "n++;");
                        Sleep(1);
                    }
                    else if (ev == "click")
                    {
                        WaitCreateBrowser(1);
                        Sleep(4);
                        WaitCreateBrowser(2);
                        Sleep(3);
                        CloseBrowser(browsers[2]);
                        Sleep(2);
                    }
                }
                CloseСhildBrowser();
            }
        }
        private void MailSurf()
        {

        }
    }
}
