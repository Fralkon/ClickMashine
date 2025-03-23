//using CefSharp;
//using ClickMashine.Models;

//namespace ClickMashine
//{
//    class Losena : Site
//    {
//        public Losena(MainForm form, Auth auth) : base(form, auth)
//        {
//            homePage = "https://losena.net/";
//            Type = EnumTypeSite.Losena;
//        }
//        public override bool Auth(Auth auth)
//        {
//            Sleep(2);
//            LoadPage(0, "https://losena.net/login");
//            string js_auth = @"var login_box = document.querySelector('#login-form');
//            if (login_box != null) {document.querySelector('[name=""username""]').value = '" + auth.Login + "';" +
//            "document.querySelector('[name=\"password\"]').value = '" + auth.Password + "';" +
//            "document.querySelector('button').click();" +
//            "'login'; }" +
//            "else { 'is_auth' };";
//            string ev_lodin = SendJSReturn(0, js_auth);
//            CM(ev_lodin);
//            Sleep(7);
//            CM("Auth Aviso");
//            return true;
//        }
//        protected override void StartSurf()
//        {
//            Initialize();
//            while (true)
//            {
//                if (!Auth(auth))
//                    return;
//                try
//                {
//                    YouTubeSurf();
//                }
//                catch (Exception ex)
//                {
//                    Error(ex.Message);
//                }
//                try
//                {
//                    MailSurf();
//                }
//                catch (Exception ex)
//                {
//                    Error(ex.Message);
//                }
//                try
//                {
//                    ClickSurf();
//                }
//                catch (Exception ex)
//                {
//                    Error(ex.Message);
//                }
//                Sleep(60 * 60);
//            }
//        }
//        private void YouTubeSurf()
//        {
//            var mainBrowser = GetBrowser(0);
//            if (mainBrowser == null) return;
//            SendJS(0, "document.querySelector('#mnu_tblock1 > a:nth-child(5)').click();");
//            Sleep(2);
//            IFrame mainFrame = mainBrowser.MainFrame;
//            string start_surf_js =
//@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
//function link()
//{
//	var link_youtube = surf_cl[n].querySelector('.go-link-youtube');
//	var error_youtube = surf_cl[n].querySelector('.youtube-error');
//	if (link_youtube != null)
//	{
//		if (surf_cl[n].attributes.id.value.at(0) == 'p') { link_youtube.click(); n++; return 'podp'; }
//		else if (surf_cl[n].attributes.id.value.at(0) == 'a') { link_youtube.click(); n++; return 'ads'; }
//		else if (surf_cl[n].attributes.id.value.at(0) == 'l') { link_youtube.click();  n++; return 'likes'; }
//		else { n++; return 'error_link'; }
//	}
//	else if (error_youtube != null)
//    {
//		if (error_youtube.innerText == 'Площадка не найдена') {  n++; return 'continue'; }
//		else { n++; return error_youtube.innerText; }
//	}
//	else { return 'wait'; }
//}
//function click_s()
//{
//	if (n >= surf_cl.length) return 'end_surf';
//	else{
//	    if (surf_cl[n].attributes.id.value.at(0) == 'p' || surf_cl[n].attributes.id.value.at(0) == 'l')
//	    {
//		    n++;
//			return 'continue';
//		}
//		surf_cl[n].querySelector('span').click();    
//        return 'click';
//	}
//}";
//            SendJS(mainFrame, start_surf_js);
//            while (true)
//            {
//                string ev = SendJSReturn(mainFrame, "click_s();");
//                if (ev == "end_surf")
//                {
//                    break;
//                }
//                else if (ev == "click")
//                {
//                    for (int i = 0; i < 10; i++)
//                    {
//                        ev = SendJSReturn(mainFrame, "link();");
//                        if (ev == "wait")
//                        {
//                            Sleep(1);
//                            continue;
//                        }
//                        else if (ev == "error_link")
//                        {
//                            break;
//                        }
//                        else if (ev == "ads")
//                        {
//                            CM("See youtube");
//                            IBrowser? youtube_Browser = GetBrowser(1);
//                            if (youtube_Browser == null)
//                            {
//                                CloseСhildBrowser();
//                                continue;
//                            }
//                            IFrame youTubeFrame = youtube_Browser.MainFrame;
//                            ev = SendJSReturn(youTubeFrame, @"player.setVolume(0); b = true; c = true; player.seekTo(0, true); document.querySelector('#tmr').innerText;");
//                            Sleep(ev);
//                            if (WaitElement(youTubeFrame, @"document.querySelector('[type=""range""]')"))
//                            {
//                                string js =
//    @"var range = document.querySelector('[type=""range""]');
//if (range != null)
//{
//    range.value = range.max;
//    document.querySelector('button').click(); 'end_surf';
//}
//else { 'error_range'; }";
//                                SendJSReturn(youTubeFrame, js);
//                            }
//                            break;
//                        }
//                    }
//                }
//                Sleep(2);
//                CloseСhildBrowser();
//                Sleep(1);
//            }
//        }
//        private void ClickSurf()
//        {
//            var mainBrowser = GetBrowser(0);
//            if (mainBrowser == null) return;
//            LoadPage(mainBrowser, "https://losena.net/");
//            SendJS(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(1)').click();");
//            Sleep(2);
//            IFrame mainFrame = mainBrowser.MainFrame;
//            string jsSurf =
//@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
//function surf()
//{
//    var start_ln = surf_cl[n].querySelector('.start-yes-serf');
//    if (start_ln != null) { n++; start_ln.click(); return 'surf'; }
//    else { return 'sec_wait'; }
//}
//function click_s()
//{
//    if (n >= surf_cl.length) return 'end_surf';
//    else if (surf_cl[n].innerText.length > 200) { n++; return 'continue'; }
//    else
//    {
//        if(!surf_cl[n].querySelector('[title = ""Стоимость просмотра""]')){ n++; return 'continue'; }
//        surf_cl[n].querySelector('a').click();
//        return 'click';
//    }
//}";
//            SendJS(mainFrame, jsSurf);
//            while (true)
//            {
//                string ev = SendJSReturn(mainFrame, "click_s();");
//                if (ev == "end_surf")
//                    break;
//                else if (ev == "continue")
//                    continue;
//                else if (ev == "click")
//                {
//                    for (int step = 0; step < 10; step++)
//                    {
//                        ev = SendJSReturn(mainFrame, "surf();");
//                        if (ev == "sec_wait")
//                        {
//                            Sleep(1);
//                            continue;
//                        }
//                        else if (ev == "surf")
//                        {
//                            IBrowser? click_browser = GetBrowser(1);
//                            if (click_browser == null)
//                            {
//                                CloseСhildBrowser();
//                                continue;
//                            }
//                            var frameIndif = click_browser.GetFrameIdentifiers();
//                            foreach (var id in frameIndif)
//                            {
//                                IFrame frame = click_browser.GetFrameByIdentifier(id);
//                                if (frame.Url.IndexOf("vlss") == -1)
//                                {
//                                    continue;
//                                }
//                                else
//                                {
//                                    ev = SendJSReturn(frame, @"var timer_inp = document.querySelector('.timer'); if (timer_inp != null) timer_inp.innerText; else 'error_timer';");
//                                    if (ev != "error")
//                                    {
//                                        Sleep(ev);
//                                        SendJS(frame, "document.getElementById('time').innerText = 0;");
//                                        Sleep(1);
//                                        SendJSReturn(frame,
//@"var range = document.querySelector('[type=""range""]');
//if (range != null)
//{
//	range.value = range.max;
//	document.querySelector('button').click();
//	'end';
//}
//else
//{
//	location.replace(""vlss?view=ok"");
//	'error_surf';
//}");
//                                    }
//                                    step = 10;
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                }
//                Sleep(2);
//                CloseСhildBrowser();
//            }
//        }
//        private void MailSurf()
//        {
//            var mainBrowser = GetBrowser(0);
//            if (mainBrowser == null) return;
//            SendJS(0, "document.querySelector('#mnu_tblock1 > a:nth-child(2)').click();");
//            Sleep(2);
//            IFrame mainFrame = mainBrowser.MainFrame;
//            string js =
//@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
//function surf()
//{
//	var start_ln = surf_cl[n].querySelector('.start-yes-serf');
//	if (start_ln != null) { start_ln.click(); n++; return 'surf'; }
//    else { return 'wait'; }
//}
//function click_s()
//{
//    if (n >= surf_cl.length) return 'end_surf';
//    else if (surf_cl[n].innerText.length > 200) { n++; return 'continue'; }
//    else
//    {
//        surf_cl[n].querySelector('a').click(); return 'click';
//    }
// }";
//            SendJS(mainFrame, js);
//            while (true)
//            {
//                string ev = SendJSReturn(mainFrame, "click_s();");

//                if (ev == "end_surf")
//                    break;
//                else if (ev == "continue")
//                {
//                    continue;
//                }
//                else if (ev == "click")
//                {
//                    for (int step = 0; step < 10; step++)
//                    {
//                        ev = SendJSReturn(mainFrame, "surf();");
//                        if (ev == "wait")
//                        {
//                            Sleep(1);
//                            continue;
//                        }
//                        else if (ev == "surf")
//                        {
//                            ev = GetMailAnswer(mainBrowser.MainFrame, "document.querySelector('#js-popup > div > div:nth-child(3)')",
//                                "document.querySelector('#js-popup > div > div:nth-child(4)')",
//                                "document.querySelectorAll('.mails-otvet-new a')");
//                            CM(ev);
//                            if (ev == "errorMail")
//                            {
//                                Random rnd = new Random();
//                                ev = rnd.Next(0, 3).ToString();
//                            }
//                            SendJS(0, "document.querySelectorAll('.mails-otvet-new a')[" + ev + "].click();");
//                            Sleep(5);
//                            js =
//@"var timer_page = document.querySelector('#tmr');
//var timer_end_page;
//if (timer_page != null)
//{
//    timer_page.innerText;
//}
//else 'error_mail';";
//                            ev = SendJSReturn(1, js);

//                            if (ev != "error_mail")
//                            {
//                                Sleep(ev);
//                                js =
//@"var range = document.querySelector('[type=""range""]');
//if (range != null)
//{
//    range.value = range.max;
//    document.querySelector('form').submit(); 'end_surf';
//}
//else { 'error_mail'; }";

//                                SendJSReturn(1, js);
//                            }
//                            Sleep(1);
//                            break;
//                        }
//                    }
//                }
//                CloseСhildBrowser();
//            }
//        }
//    }
//}
