namespace ClickMashine
{
    internal class WebofSar : Site
    {
        public WebofSar(Form1 form, Auth auth) : base(form, auth)
        {
            homePage = "https://webof-sar.ru/";
            Type = EnumTypeSite.WebofSar;
        }
        protected override void StartSurf()
        {
            Initialize();
            if (!Auth(auth))
                return;
            ClickSurf(); 
            VisitSites(); 
            AutoClick();
            CloseAllBrowser();
        }
        public override bool Auth(Auth auth)
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
            return true;
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
                        if (WaitElement(browsers[1].MainFrame, "document.querySelector('#Timer')"))
                        {
                            ev = SendJSReturn(browsers[1].MainFrame, @"document.querySelector('#Timer').innerText;");
                            Sleep(ev);
                            if (WaitButtonClick(browsers[1].MainFrame, "document.querySelector('[class=\"block-success work-check\"]')", 5))
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
            Sleep(2);
            string js =
@"var surf_cl = document.querySelectorAll('.td-work');var n = 0;
function surf()
{
    var start_ln = surf_cl[n].querySelector('.sub_work');
    if (start_ln != null) { start_ln.click(); n++; return 'click'; }
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
                        Sleep(7);
                    }
                }
                CloseСhildBrowser();
            }
        }
        private void MailSurf()
        {
            LoadPage("https://webof-sar.ru/read-mails");
            string js =
@"var surf_cl = document.querySelector('.table-serf').querySelectorAll('td a');var n = 0;
function surf()
{
    var start_ln = surf_cl[n].querySelector('[type=""button""]');
    if (start_ln != null) { start_ln.click(); n+=2; return 'click'; }
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
                        if (WaitElement(browsers[1].MainFrame, "document.querySelector('#Timer')"))
                        {
                            ev = SendJSReturn(browsers[1].MainFrame, @"document.querySelector('#Timer').innerText;");
                            Sleep(ev);
                            if (WaitButtonClick(browsers[1].MainFrame, "document.querySelector('[class=\"block-success work-check\"]')", 5))
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
        private void AutoClick()
        {
            LoadPage(main_browser.GetBrowser(), "https://webof-sar.ru/work-auto-surfings");
            string ev = SendJSReturn(browsers[0], "document.querySelector('.text-bold').innerText");
            if(ev != "0")
            {
                SendJS(browsers[0], "document.querySelector('.btn-block').click()");
                while (true)
                {
                    Sleep(5);
                    ev = SendJSReturn(browsers[1],
@"var timer_r = document.querySelector('.timer');
if(timer_r != null) timer.innerText;
else 'none';");

                    if (ev == "none")
                    {
                        break;
                    }
                    else { Sleep(ev); }
                }
            }
        }
    }
}
