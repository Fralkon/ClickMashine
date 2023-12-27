using CefSharp;

namespace ClickMashine
{
    internal class WebofSar : Site
    {
        public WebofSar(Form1 form, Auth auth) : base(form, auth)
        {
            homePage = "https://webof-sar.ru/";
            Type = EnumTypeSite.WebofSar;
            string clickJSFunction =
@"var surf_cl = document.querySelectorAll('.wfsts');var n = 0;
function FirstStep()
{
    if (n >= surf_cl.length) return "+(int)StatusJS.End+ @";
    else
    {
        surf_cl[n].querySelector('.work-surf-start').click(); return "+(int)StatusJS.OK+ @";
    }
}
function SecondStep()
{
    var start_ln = surf_cl[n].querySelector('button');
    if (start_ln != null) { start_ln.click(); n++; return "+(int)StatusJS.OK+ @"; }
    else { return "+(int)StatusJS.Wait+@"; }
}";
            ManagerSurfing.AddSurfing(new Surfing(this, "https://webof-sar.ru/work-surfings", clickJSFunction, ClickMiddle));

            string visitJSFunction =
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
            ManagerSurfing.AddSurfing(new Surfing(this, "https://webof-sar.ru/work-pay-visits", visitJSFunction, VisitMiddle));

            string autoClickJSFunction =
@"function FirstStep(){
    if(document.querySelector('.text-bold')!=null)
        if(document.querySelector('.text-bold').innerText!='0')
            return "+(int)StatusJS.OK1+@";
    return "+StatusJS.End+@";
}";
            ManagerSurfing.AddSurfing(new Surfing(this, "https://webof-sar.ru/work-auto-surfings", autoClickJSFunction, AutoVisitMiddle));
        }
        public override bool Auth(Auth auth)
        {
            var authBrowser = GetBrowser(0);
            if(authBrowser == null) { return false; }
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
                WaitElement(authBrowser.MainFrame, "document.querySelector('#auth_user_log');");

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
        private bool ClickMiddle(IBrowser browser)
        {
            InjectJS(browser,
    @"loadFrame();
$(window).on(""focus"", function () {   
    wFocus = true;
    dFocus = true;
});
$(window).on(""blur"", function() {                       
    wFocus = true;
    dFocus = true;
});
$(window).focus();");
            browser.GetHost().SetFocus(true);
            Sleep(1);
            if (WaitElement(browser, "document.querySelector('#Timer')"))
            {
                if (WaitTime(browser, @"document.querySelector('#Timer').innerText;"))
                {
                    if (!WaitButtonClick(browser, "document.querySelector('[class=\"block-success work-check\"]')", 5))
                    {
                        InjectJS(browser,
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
                        if (WaitButtonClick(browser, "document.querySelector('[class=\"block-success work-check\"]')"))
                            return true;
                    }
                    else
                        return true;
                }
            }
            return false;
        }
        private bool VisitMiddle(IBrowser browser)
        {
            Sleep(7);
            return true;
        }
        private bool AutoVisitMiddle(IBrowser browser)
        {
            while (true)
            {
                if (WaitElement(browser.MainFrame, "document.querySelector('.timer')"))
                {
                    StatusJS status = InjectJS(browser,
@"var timer_r = document.querySelector('.timer');
if(timer_r != null) "+(int)StatusJS.OK+@";
else "+(int)StatusJS.End+@";");
                    if (status == StatusJS.End)
                        break;
                    else { Sleep(TimeSpan.Parse(ValueElement(browser, "timer_r.innerText")).TotalMinutes.ToString()); Sleep(2); }
                }
                else
                    break;
            }
            return true;
        }
        private int ClickSurf()
        {
            int Count = 0;
            var mainBrowser = GetBrowser(0);
            if (mainBrowser == null) { return -1; }
            LoadPage("https://webof-sar.ru/work-surfings");
            string js =
@"var surf_cl = document.querySelectorAll('.wfsts');var n = 0;
function surf()
{
    var start_ln = surf_cl[n].querySelector('button');
    if (start_ln != null) { start_ln.click(); n++; return 'click'; }
    else { return 'wait'; }
}
function click_s()
{
    if (n >= surf_cl.length) return 'end_surf';
    else
    {
        surf_cl[n].querySelector('.work-surf-start').click(); return 'surf';
    }
}";
            SendJS(0, js);
            while (true)
            {
                eventBrowserCreated.Reset();
                string ev = SendJSReturn(0, "click_s();");
                if (ev == "end_surf")
                    break;
                else if (ev == "continue")
                    continue;
                else if (ev == "surf")
                {
                    ev = WaitFunction(mainBrowser.MainFrame, "surf();");
                    if (ev == "errorWait")
                    {
                        SendJS(0, "n++;");
                        Sleep(1);
                    }
                    else if (ev == "click")
                    {
                        var browserClick = WaitCreateBrowser();
                        if (browserClick != null)
                        {
                            SendJS(browserClick,
    @"loadFrame();
$(window).on(""focus"", function () {   
    wFocus = true;
    dFocus = true;
});
$(window).on(""blur"", function() {                       
    wFocus = true;
    dFocus = true;
});
$(window).focus();");
                            browserClick.GetHost().SetFocus(true);
                            Sleep(1);
                            if (WaitElement(browserClick.MainFrame, "document.querySelector('#Timer')"))
                            {
                                ev = SendJSReturn(browserClick.MainFrame, @"document.querySelector('#Timer').innerText;");
                                Sleep(ev);
                                if (!WaitButtonClick(browserClick.MainFrame, "document.querySelector('[class=\"block-success work-check\"]')", 5))
                                {
                                    SendJS(browserClick.MainFrame,
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
                                    WaitButtonClick(browserClick.MainFrame, "document.querySelector('[class=\"block-success work-check\"]')");
                                }
                                Count++;
                            }
                        }
                    }
                }
                Sleep(2);
                CloseСhildBrowser();
            }
            return Count;
        }
        private int VisitSites()
        {
            int Count = 0;
            var mainBrowser = GetBrowser(0);
            if (mainBrowser == null) { return -1; }
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
                eventBrowserCreated.Reset();
                string ev = SendJSReturn(0, "click_s();");
                if (ev == "end_surf")
                    break;
                else if (ev == "continue")
                    continue;
                else if (ev == "surf")
                {
                    ev = WaitFunction(mainBrowser.MainFrame, "surf();");
                    if (ev == "errorWait")
                    {
                        SendJS(0, "n++;");
                        Sleep(1);
                    }
                    else if (ev == "click")
                    {
                        WaitCreateBrowser();
                        Sleep(7);
                        Count++;
                    }
                }
                CloseСhildBrowser();
            }
            return Count;
        }
        private int MailSurf()
        {
            int Count = 0;
            var mainBrowser = GetBrowser(0);
            if (mainBrowser == null) { return -1; }
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
                    ev = WaitFunction(mainBrowser.MainFrame, "surf();");
                    if (ev == "errorWait")
                    {
                        SendJS(0, "n++;");
                        Sleep(1);
                    }
                    else if (ev == "click")
                    {
                        var browser = WaitCreateBrowser();
                        if (browser != null)
                        {
                            Sleep(2);
                            SendJS(browser.MainFrame, @"loadFrame();
$(window).on(""focus"", function () {   
    wFocus = true;
    dFocus = true;
});
$(window).on(""blur"", function() {                       
    wFocus = true;
    dFocus = true;
});
$(window).focus();");
                            browser.GetHost().SetFocus(true);
                            Sleep(1);
                            if (WaitElement(browser.MainFrame, "document.querySelector('#Timer')"))
                            {
                                ev = SendJSReturn(browser.MainFrame, @"document.querySelector('#Timer').innerText;");
                                Sleep(ev);
                                if (WaitButtonClick(browser.MainFrame, "document.querySelector('[class=\"block-success work-check\"]')", 5))
                                {
                                    SendJS(browser.MainFrame,
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
                                    WaitButtonClick(browser.MainFrame, "document.querySelector('[class=\"block-success work-check\"]')");
                                }
                                Count++;
                            }
                        }
                    }
                }
                Sleep(3);
                CloseСhildBrowser();
            }
            return Count;
        }
        private int AutoClickMiddle()
        {
            int Count = 0;
            var mainBrowser = GetBrowser(0);
            if (mainBrowser == null) { return -1; }
            LoadPage(mainBrowser, "https://webof-sar.ru/work-auto-surfings");
            if (WaitElement(mainBrowser, "document.querySelector('.text-bold')", 2))
            {
                string ev = SendJSReturn(mainBrowser, "document.querySelector('.text-bold').innerText");
                if (ev != "0")
                {
                    SendJS(mainBrowser, "document.querySelector('.btn-block').click()");
                    var browser = WaitCreateBrowser();
                    if (browser != null)
                    {
                        while (true)
                        {
                            if (WaitElement(browser.MainFrame, "document.querySelector('.timer')"))
                            {
                                ev = SendJSReturn(browser,
    @"var timer_r = document.querySelector('.timer');
if(timer_r != null) timer_r.innerText;
else 'none';");
                                Count++;
                                if (ev == "none" || ev == "")
                                {
                                    break;
                                }
                                else { Sleep(TimeSpan.Parse(ev).TotalMinutes.ToString()); Sleep(2); }
                            }
                            else
                                break;
                        }
                    }
                }
            }
            CloseСhildBrowser();
            return Count;
        }
    }
}
