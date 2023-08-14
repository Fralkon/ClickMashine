using CefSharp;
using CefSharp.DevTools.Page;
using System.Text.Json;

namespace ClickMashine
{
    class SeoClub : Site
    {
        public SeoClub(Form1 form, Auth auth) : base(form, auth)
        {
            homePage = "https://seoclub.su/";
            Type = EnumTypeSite.SeoClub;
        }
        public override bool Auth(Auth auth)
        {
            IBrowser loginBrowser = browsers[0];
            LoadPage(loginBrowser, "https://seoclub.su/login");
            string auth_js = "document.querySelector('input[name=\"username\"]').value = '" + auth.Login + "';" +
                             "document.querySelector('input[name=\"password\"]').value = '" + auth.Password + "';";

            SendJS(0, auth_js);
            for (int i = 0; i < 5; i++) {
                if (WaitElement(loginBrowser.MainFrame, "document.querySelector('.out-capcha')",2))
                {
                    AntiBot(loginBrowser);
                    SendJS(loginBrowser, "document.querySelector('.btn').click();");
                    Sleep(7);
                }
                else
                {
                    if (!WaitElement(loginBrowser.MainFrame, "document.querySelector('#new-money-ballans')"))
                    {
                        Error("NOT AUTH");
                    }
                    try
                    {
                        CM("Money : " + SendJSReturn(loginBrowser, "document.querySelector('#new-money-ballans').innerText"));
                    }
                    catch (Exception ex)
                    {
                        Error(ex.ToString());
                        return false;
                    }
                    return true;
                }
            }
            return false;            
        }
        protected override void StartSurf()
        {
            Initialize();
            if (!Auth(auth))
                return;
            mSurf.AddFunction(YouTubeSurf);
            mSurf.AddFunction(VisitSurf);
            mSurf.AddFunction(MailSurf);
            mSurf.AddFunction(ClickSurf);
            while (true)
            {
                mSurf.GoSurf();
                Sleep(600);
            }

            CloseAllBrowser();
        }
        private int ClickSurf()
        {
            int Count = 0;
            LoadPage("https://seoclub.su/");
            SendJS(0, "document.querySelector('#mnu_tblock1 > a:nth-child(2)').click();");
            Sleep(4);
            //AntiBot();
            string js =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
function surf()
{
	var start_ln = surf_cl[n].querySelector('.start-yes-serf');
	if (start_ln != null) { start_ln.click(); n++; return 'surf'; }
	else { return 'wait'; }
}
function click_s()
{
	if (n >= surf_cl.length) return 'end_surf';
	else
	{	
		if(surf_cl[n].querySelector('[id]')!=null)
			{
				if(surf_cl[n].querySelector('a')==null || surf_cl[n].getBoundingClientRect().height == 0)
					{n++; return 'continue';}
				else {surf_cl[n].querySelector('a').click(); return 'click';}
			}
		else
			{n++;return 'continue';}
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
                else if (ev == "click")
                {
                    for (int i = 0; i < 5; i++)
                    {
                        ev = SendJSReturn(0, "surf();");
                        if (ev == "wait")
                            Sleep(1);
                        else if (ev == "surf")
                        {
                            IBrowser? browserSurf = WaitCreateBrowser();
                            if (browserSurf != null)
                            {
                                IFrame frame = browserSurf.GetFrame("frminfo");
                                ev = SendJSReturn(frame,
@"b = false;
window.top.start = 0;
var timerWait = document.querySelector('.timer');
if (timerWait != null)
	timerWait.innerText;
else 'error_surf';");
                                if (ev != "error")
                                {
                                    Sleep(ev);
                                    if(!WaitElement(frame, "document.querySelector('[type=\"range\"]')"))
                                    {
                                        SendJS(frame, "location.replace(\"vlss?view=ok\");");
                                        if (!WaitElement(frame, "document.querySelector('[type=\"range\"]')"))
                                            break;
                                    }
                                    SendJSReturn(frame,
@"var range = document.querySelector('[type=""range""]');
if (range != null)
{
	range.value = range.max;
	document.querySelector('button').click();
	'end';
}
else
{
	location.replace(""vlss?view=ok"");
	'error_surf';
}");
                                    Count++;
                                    Sleep(2);
                                }
                            }
                            else Error("Error wait browser");
                            break;
                        }
                    }
                }
                CloseСhildBrowser();
                Sleep(2);
            }
            return Count;
        }
        private int VisitSurf()
        {
            int Count = 0;
            LoadPage(0, "https://profitcentr.com/");
            SendJS(0, "document.querySelector('#mnu_tblock1 > a:nth-child(3)').click();");
            Sleep(4);
            //AntiBot();
            string js =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
function click_s()
{
	if (n >= surf_cl.length) return 'end_surf';
	else
	{
		var link = surf_cl[n];
		if(link.querySelectorAll('td')[2]==null || link.getBoundingClientRect().height == 0)
					{n++; return 'continue';}
		else {link.querySelector('a').click(); n++; return link.querySelectorAll('td')[1].querySelectorAll('div')[1].innerText;}
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
                else
                {
                    if (WaitCreateBrowser() != null)
                    {
                        int pointStart = ev.IndexOf("Таймер: ") + 8;
                        int pointEnd = ev.IndexOf(' ', pointStart);
                        int countText = pointEnd - pointStart;
                        if (pointStart == -1 || pointEnd == -1 || countText > 0)
                            Sleep(ev.Substring(pointStart, pointEnd - pointStart));
                        Sleep(2);
                        Count++;
                    }
                }
                Sleep(2);
                CloseСhildBrowser();
                Sleep(2);
            }
            return Count;
        }
        private int YouTubeSurf()
        {
            int Count = 0;
            SendJS(0,"document.querySelector('#mnu_tblock1 > a:nth-child(7)').click();");
            Sleep(4);
            IBrowser mainBrowser = browsers[0];
            if(!OutCaptchaLab(mainBrowser, "document.querySelectorAll('.out-capcha-lab')", "document.querySelectorAll('.out-capcha-inp')", "document.querySelector('.btn_big_green').click()"))
            {
                return 0;
            }            

            string js_links = 
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
function click_s()
{
	if (n >= surf_cl.length) return 'end_surf';
	else
	{
		surf_cl[n].querySelector('span').click(); return 'click';
	}
}
function surf()
{
	var start_ln = surf_cl[n].querySelector('.youtube-button');
	if (start_ln != null) { 
		if(start_ln.innerText != 'Приступить к просмотру') {n++; return 'continue';}
		else {start_ln.querySelector('span').click(); n++; return 'surf'; }
	}
	else { return 'sec_wait'; }
}";
            SendJS(mainBrowser, js_links);
            while (true)
            {
                string ev = SendJSReturn(mainBrowser, "click_s();");
                if (ev == "end_surf")
                {
                    break;
                }
                else if (ev == "click")
                {
                    eventBrowserCreated.Reset();
                    for (int i = 0; i < 10; i++)
                    {
                        ev = SendJSReturn(mainBrowser, "surf();");
                        if (ev == "surf")
                        {
                            var browserYouTube = WaitCreateBrowser();
                            if (browserYouTube != null){
                                IFrame yotube_frame = browserYouTube.MainFrame;
                                ev = SendJSReturn(yotube_frame,
    @"c = true;  b = true; document.querySelector('#tmr').innerText;");
                                if (ev != "error")
                                {
                                    Sleep(ev);
                                    if (!WaitButtonClick(yotube_frame, "document.querySelector('.butt-nw');"))
                                        Error("Error end youtube watch");
                                    Count++;
                                    Sleep(2);
                                }
                                break;
                            }
                            else if (ev == "sec_wait")
                                Sleep(1);
                            else if (ev == "continue")
                                break;
                        }
                    }
                }
                else Error("Ошибка блять");
                CloseСhildBrowser();
                Sleep(1);
            }
            return Count;
        }
        private int MailSurf()
        {
            int Count = 0;
            LoadPage(0, "https://profitcentr.com/");
            Sleep(2);
            SendJS(0, "document.querySelector('#mnu_tblock1 > a:nth-child(4)').click();");
            Sleep(4);
            string js =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 1;
function surf()
{
	var start_ln = surf_cl[n].querySelector('.start-yes-serf');
	if (start_ln != null) { start_ln.click(); n++; return 'surf'; }
	else { return 'wait' }
}
function click_s()
{
	if (n >= surf_cl.length) return 'end_surf';
	else
	{
		surf_cl[n].querySelector('a').click(); return 'click';
	}
}";
            SendJS(0, js);
            while (true)
            {
                string ev = SendJSReturn(0, "click_s();");
                if (ev == "end_surf")
                    return Count;
                else if (ev == "continue")
                    continue;
                else if (ev == "click")
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ev = SendJSReturn(0, "surf();");
                        if (ev == "wait")
                            Sleep(1);
                        else if (ev == "click")
                        {
                            ev = GetMailAnswer(browsers[0].MainFrame, "document.querySelector('#js-popup > div:nth-child(3)')",
                                "document.querySelector('#js-popup > div:nth-child(4)')",
                                "document.querySelectorAll('.mails-otvet-new a')");
                            if (ev == "errorMail")
                            {
                                Random rnd = new Random();
                                ev = rnd.Next(0, 2).ToString();
                            }
                            SendJS(0, "document.querySelectorAll('.mails-otvet-new a')[" + ev + "].click();");
                            var browser = WaitCreateBrowser();
                            if (browser != null) {
                                ev = SendJSReturn(browser,
@"b = false;
window.top.start = 0;
var timer1 = document.querySelector('.timer');
if (timer1 != null)
	return timer1.innerText;
else 'error_surf';");
                                if (ev != "error")
                                {
                                    Sleep(ev);
                                    if (!WaitElement(browser, "document.querySelector('[type=\"range\"]')"))
                                    {
                                        SendJS(browser, "location.replace(\"vlss?view=ok\");");
                                        if (!WaitElement(browser, "document.querySelector('[type=\"range\"]')"))
                                            break;
                                    }
                                    SendJSReturn(browser,
@"var range = document.querySelector('[type=""range""]');
if (range != null)
{
	range.value = range.max;
	document.querySelector('button').click();
	'end';
}
else
{
	location.replace(""vlss?view=ok"");
	'error_surf';
}");
                                    Count++;
                                    Sleep(2);
                                }
                            }
                        }
                    }
                }
                Sleep(2);
                CloseСhildBrowser();
            }
        }
        private void AntiBot(IBrowser browser)
        {
            string jsAntiBot =
@"var captha_lab = document.querySelectorAll('.out-capcha-lab');
if(captha_lab.length != 0){
    'captcha';
}
else 'ok';";
            string evAntiBot = SendJSReturn(browser, jsAntiBot);
            if (evAntiBot == "ok")
                return;
            else
            {
                string answer_telebot = SendQuestion(GetImgBrowser(browser.MainFrame, "document.querySelector('.out-capcha')"), "");

                jsAntiBot = "";
                foreach (char ch in answer_telebot)
                    jsAntiBot += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
                SendJS(browser, jsAntiBot);
            }
        }
    }
}