using CefSharp;
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
        public Aviso(Form1 form, TeleBot teleBot, Auth auth) : base(form, teleBot, auth)
        {
            homePage = "https://aviso.bz";
            type.enam = EnumTypeSite.Aviso;
        }
        public override bool Auth(Auth auth)
        {
            Sleep(2);
            LoadPage(0, "https://aviso.bz/login");
            string js_auth = "var login_box = document.querySelector('.login-box');" +
            "if (login_box != null) {document.querySelector('[name=\"username\"]').value = '" + auth.Login + "';" +
            "document.querySelector('[name=\"password\"]').value = '" + auth.Password + "';" +
            "document.querySelector('.button__text').click();" +
            "'login'; }" +
            "else { 'is_auth' };";
            string ev_lodin = SendJSReturn(0, js_auth);
            CM(ev_lodin);
            Sleep(7);
            CM("Auth Aviso");
            return true;
        }
        public override void StartSurf()
        {
            try
            {
                MailSurf();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                ClickSurf();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //try
            //{
            //    YouTubeSurf();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            CloseAllBrowser();
        }
        private void YouTubeSurf()
        {
            CM("YouTUbeSurf");
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
	    if (surf_cl[n].attributes.id.value.at(0) == 'p' || surf_cl[n].attributes.id.value.at(0) == 'l')
	    {
		    n++;
			return 'continue';
		}
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
                            continue;
                        }
                        else if (ev == "error_link")
                        {
                            break;
                        }
                        else if (ev == "ads")
                        {
                            CM("See youtube");
                            Sleep(2);
                            IFrame youTubeFrame = browsers[1].MainFrame;
                            ev = SendJSReturn(youTubeFrame, @"player.setVolume(0); b = true; player.seekTo(0, true); timerInitial;");
                            Sleep(ev);
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
                Sleep(2);
                CloseСhildBrowser();
                Sleep(1);
            }
        }
        private void ClickSurf()
        {
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
                            Sleep(1);
                            continue;
                        }
                        else if (ev == "surf")
                        {
                            Sleep(2);
                            List<long> frameIndif = browsers[1].GetFrameIdentifiers();
                            foreach (long id in frameIndif)
                            {
                                IFrame frame = browsers[1].GetFrame(id);
                                if (frame.Url.IndexOf("vlss") == -1)
                                {
                                    continue;
                                }
                                else
                                {
                                    ev = SendJSReturn(frame, @"var timer_inp = document.querySelector('.timer'); if (timer_inp != null) timer_inp.innerText; else 'error_timer';");
                                    if (ev != "error")
                                    {
                                        Sleep(ev);
                                        SendJS(frame, "document.getElementById('time').innerText = 0;");
                                        Sleep(1);
                                        ev = SendJSReturn(frame, @"var button_finish = document.querySelector('.btn_capt'); if (button_finish == null) { 'error_click'; } else { button_finish.click(); 'end'; }");
                                    }
                                    step = 10;
                                    break;
                                }
                            }
                        }
                    }
                }
                Sleep(2);
                CloseСhildBrowser();
            }
        }
        private void MailSurf()
        {
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
                            Sleep(5);
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
                            }
                            Sleep(2);
                            break;
                        }
                    }
                }
                CloseСhildBrowser();
            }
        }
    }
}
