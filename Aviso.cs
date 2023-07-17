﻿using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickMashine
{
    class Aviso : Site
    {
        public Aviso(Form1 form, Auth auth) : base(form, auth)
        {
            homePage = "https://aviso.bz";
            Type = EnumTypeSite.Aviso;
        }
        public override bool Auth(Auth auth)
        {
            LoadPage(0, "https://aviso.bz/login");
            Sleep(2);
            string js_auth = "var login_box = document.querySelector('.login-box');" +
            "if (login_box != null) {document.querySelector('[name=\"username\"]').value = '" + auth.Login + "';" +
            "document.querySelector('[name=\"password\"]').value = '" + auth.Password + "';" +
            "document.querySelector('.button__text').click();" +
            "'login'; }" +
            "else { 'is_auth' };";
            string ev_lodin = SendJSReturn(0, js_auth);
            CM(ev_lodin);
            if (ev_lodin == "login")
                Sleep(7);
            CM("Auth Aviso");
            return true;
        }
        protected override void StartSurf()
        {
            Initialize();
            if (!Auth(auth))
                return;
            mSurf.AddFunction(YouTubeSurf);
            mSurf.AddFunction(MailSurf);
            mSurf.AddFunction(ClickSurf);
            while (true)
            {
                mSurf.GoSurf();
                Sleep(600);
            }
            CloseAllBrowser();
        }
        private int YouTubeSurf()
        {
            int LinkCount = 0;
            CM("YouTUbeSurf");
            int error = 0;
            LoadPage(0, "https://aviso.bz/work-youtube");
            IFrame mainFrame = browsers[0].MainFrame;
            string start_surf_js =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
function link()
{
	var link_youtube = surf_cl[n].querySelector('.go-link-youtube');
	var error_youtube = surf_cl[n].querySelector('.youtube-error');
	if (link_youtube != null)
	{
		if (surf_cl[n].attributes.id.value.at(0) == 'p') { link_youtube.click(); n++; return 'podp'; }
		else if (surf_cl[n].attributes.id.value.at(0) == 'a') { link_youtube.click(); n++; return 'ads'; }
		else if (surf_cl[n].attributes.id.value.at(0) == 'l') { link_youtube.click();  n++; return 'likes'; }
		else { n++; return 'error_link'; }
	}
	else if (error_youtube != null)
    {
		if (error_youtube.innerText == 'Площадка не найдена') {  n++; return 'continue'; }
		else { n++; return error_youtube.innerText; }
	}
	else { return 'wait'; }
}
function click_s()
{
	if (n >= surf_cl.length) return 'end_surf';
	else{
		surf_cl[n].querySelector('span').click();    
        return 'click';
	}
}";
            SendJS(mainFrame, start_surf_js);
            while (true)
            {
                string ev = SendJSReturn(mainFrame, "click_s();");
                if (ev == "end_surf")
                {
                    break;
                }
                else if (ev == "click")
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ev = SendJSReturn(mainFrame, "link();");
                        if (ev == "wait")
                        {
                            Sleep(1);
                            if(i == 9)
                            {
                                SendJS(mainFrame, "n++");
                                break;
                            }
                            continue;
                        }
                        else if (ev == "error_link")
                        {
                            break;
                        }
                        else if (ev == "ads")
                        {
                            try
                            {
                                CM("See youtube");
                                var browserYouTube = WaitCreateBrowser();
                                if (browserYouTube != null)
                                {
                                    WaitElement(browserYouTube.MainFrame, "player");
                                    ev = SendJSReturn(browserYouTube.MainFrame, @"player.setVolume(0); player.seekTo(0, true); b = true; c = true;  timerInitial;");
                                    Sleep(ev);
                                    string jsWaitYouTube =
@"function WaitEnd(){
if(document.querySelector('#capcha-tr-block').innerText.length > 3)
    return 'ok';
else
    return 'wait';}";
                                    form.FocusTab(browserYouTube);
                                    if("ok" != WaitFunction(browserYouTube.MainFrame, "WaitEnd();", jsWaitYouTube))
                                    {
                                        WaitElement(browserYouTube.MainFrame, "player");
                                        ev = SendJSReturn(browserYouTube.MainFrame, @"player.setVolume(0); player.seekTo(0, true); b = true; c = true;  timerInitial;");
                                        Sleep(ev);
                                        form.FocusTab(browserYouTube);
                                        WaitFunction(browserYouTube.MainFrame, "WaitEnd();", jsWaitYouTube);
                                    }
                                    LinkCount++;
                                }
                            }
                            catch (Exception e) { Error(e.Message); error++; }
                            
                            break;                        
                        }
                    }
                }
                //CM("Podp youtube");
                ////					WaitCreateBrowser(1);
                //Sleep(4);
                //ev = SendJSReturn(1, @"player.setVolume(0); b = true; player.seekTo(0, true); window.ev(timerInitial);");
                //if (ev != "error")
                //{
                //    CM(ev);
                //    Sleep(ev);
                //}
                //Sleep(2);
                //ev = SendJSReturn(1, @"document.querySelector('.go-link-youtube').click(); document.querySelector('#link_youtube').innerText;");

                //if (ev != "error")
                //{
                //    CM(ev);
                //    int point = ev.find("be/");
                //    if (point != -1)
                //    {
                //        point += 3;
                //        WaitCreateBrowser(2);
                //        string link_youtube_podp = "https://www.youtube.com/watch?v=" + ev.substr(point, ev.size() - point);
                //        LoadPage(2, link_youtube_podp.c_str());
                //        string ev_youtube_podp = R"(try{document.querySelector('#subscribe-button > ytd-subscribe-button-renderer > tp-yt-paper-button').click();window.ev(document.querySelector('#channel-name').innerText);}
                //catch (e) { window.ev("error"); })";

                //                        SendJS(2, ev_youtube_podp);
                //        WaitTime(2);
                //        ev_youtube_podp = GetStringMessage();
                //        if (ev_youtube_podp != "error")
                //        {
                //            string ev_end_youtube_podp = R"(try{surf_cl[n-1].querySelector('input').value = )" + ev_youtube_podp + R"(;surf_cl[n-1].querySelector('.status-link-youtube').click();window.ev('OK');}
                //catch (e) { window.ev("error"); })";

                //                            SendJS(0, ev_end_youtube_podp.c_str());
                //            CM(GetStringMessage().c_str());
                //        }
                //        WaitTime(2);
                //    }
                //}
                //		else if (ev == "ads")
                //			{
                //				
                //		else if (ev == "liked") {
                //			WaitTime(2);
                //			if (ev.find("likes") != -1) {
                //				CloseСhildBrowser();
                //				continue;
                //				WaitTime(5);
                //				if (YouTubeLike(browsers[1]))
                //					WaitTime(15);
                //		SendJS(0, R"(var start_ln = surf_cl[n-1].querySelector('.youtube-button');
                //					if(start_ln ! = null)
                //		{
                //			start_ln.querySelectorAll('span')[1].click();
                //		})");
                //			}
                //}
                //		else if (ev == "continue")
                //{
                //	CloseСhildBrowser();
                //	continue;
                //}
                //else
                //	CM(ev.c_str());
                Sleep(1);
                CloseСhildBrowser();
                if (error > 8)
                    return LinkCount;
                Sleep(1);
            }
            return LinkCount;
        }
        private int ClickSurf()
        {
            int LinkCount = 0;
            LoadPage(0, "https://aviso.bz/work-serf");
            IFrame mainFrame = browsers[0].MainFrame;
            string jsSurf =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
function surf()
{
    var start_ln = surf_cl[n].querySelector('.start-yes-serf');
    if (start_ln != null) { n++; start_ln.click(); return 'surf'; }
    else { return 'sec_wait'; }
}
function click_s()
{
    if (n >= surf_cl.length) return 'end_surf';
    else if (surf_cl[n].innerText.length > 200) { n++; return 'continue'; }
    else
    {
        surf_cl[n].querySelector('a').click(); return 'click';
    }
}";
            SendJS(mainFrame, jsSurf);
            while (true)
            {
                eventBrowserCreated.Reset();
                string ev = SendJSReturn(mainFrame, "click_s();");
                if (ev == "end_surf")
                    break;
                else if (ev == "continue")
                    continue;
                else if (ev == "click")
                {
                    for (int step = 0; step < 10; step++)
                    {
                        ev = SendJSReturn(mainFrame, "surf();");
                        if (ev == "sec_wait")
                        {
                            if(step == 9)
                            {
                                SendJS(mainFrame, "n++");
                                CloseСhildBrowser();
                                break;
                            }
                            Sleep(1);
                            continue;
                        }
                        else if (ev == "surf")
                        {
                            var browserClick = WaitCreateBrowser();
                            if (browserClick != null)
                            {
                                List<long> frameIndif = browserClick.GetFrameIdentifiers();
                                foreach (long id in frameIndif)
                                {
                                    IFrame frame = browserClick.GetFrame(id);
                                    if (frame.Url.IndexOf("vlss") == -1)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        if (WaitElement(frame, "document.querySelector('.timer')", 5))
                                        {
                                            ev = SendJSReturn(frame, "document.querySelector('.timer').innerText");
                                            Sleep(ev);
                                            if(!WaitButtonClick(frame, "document.querySelector('.btn_capt')", 5))
                                            {
                                                SendJS(frame, "document.querySelector('.timer').innerText = 0");
                                                WaitButtonClick(frame, "document.querySelector('.btn_capt')", 5);
                                            }
                                            LinkCount++;
                                        }
                                        step = 10;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                Sleep(2);
                CloseСhildBrowser();
            }
            return LinkCount;
        }
        private int MailSurf()
        {
            int LinkCount = 0;
            LoadPage(0, "https://aviso.bz/mails_new");
            Sleep(2);
            IFrame mainFrame = browsers[0].MainFrame;
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
    else if (surf_cl[n].innerText.length > 200) { n++; return 'continue'; }
    else
    {
        surf_cl[n].querySelector('a').click(); return 'click';
    }
 }";
            SendJS(mainFrame, js);
            while (true)
            {
                string ev = SendJSReturn(mainFrame, "click_s();");

                if (ev == "end_surf")
                    break;
                else if (ev == "continue")
                {
                    continue;
                }
                else if (ev == "click")
                {
                    for (int step = 0; step < 10; step++)
                    {
                        ev = SendJSReturn(mainFrame, "surf();");
                        if (ev == "wait")
                        {
                            Sleep(1);
                            continue;
                        }
                        else if (ev == "surf")
                        {
                            ev = GetMailAnswer(browsers[0].MainFrame, "document.querySelector('#js-popup > div > div:nth-child(3)')",
                                "document.querySelector('#js-popup > div > div:nth-child(4)')",
                                "document.querySelectorAll('.mails-otvet-new a')");
                            CM(ev);
                            if (ev == "errorMail")
                            {
                                Random rnd = new Random();
                                ev = rnd.Next(0, 3).ToString();
                            }
                            SendJS(0, "document.querySelectorAll('.mails-otvet-new a')[" + ev + "].click();"); 
                            var browserClick = WaitCreateBrowser();
                            if (browserClick != null)
                            {
                                js =
@"var timer_page = document.querySelector('#tmr');
var timer_end_page;
if (timer_page != null)
{
    timer_page.innerText;
}
else 'error_mail';";
                                ev = SendJSReturn(1, js);

                                if (ev != "error_mail")
                                {
                                    Sleep(ev);
                                    js =
    @"var range = document.querySelector('[type=""range""]');
if (range != null)
{
    range.value = range.max;
    document.querySelector('form').submit(); 'end_surf';
}
else { 'error_mail'; }";

                                    SendJSReturn(1, js);
                                    LinkCount++;
                                }
                            }
                            Sleep(2);
                            break;
                        }
                    }
                }
                CloseСhildBrowser();
            }
            return LinkCount;
        }
    }
}
