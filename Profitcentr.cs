using CefSharp;
using System.Text.Json;

namespace ClickMashine
{
	class Profitcentr : Site
	{
		public Profitcentr(Form1 form, Auth auth) : base(form, auth)
		{
			homePage = "https://profitcentr.com/";
			Type = EnumTypeSite.Profitcentr;
		}
		public override bool Auth(Auth auth)
		{
			Sleep(5);
			LoadPage(0, "https://profitcentr.com/login");
			string auth_js = "document.querySelector('input[name=\"username\"]').value = '" + auth.Login + "';" +
							 "document.querySelector('input[name=\"password\"]').value = '" + auth.Password + "';";

            SendJS(0, auth_js);
            AntiBot();
			//while (true)
			//{
			//	Bitmap img = GetImgBrowser(browsers[0].MainFrame, "document.querySelector('.out-capcha')");
			//	string answer_telebot = teleBot.SendQuestion(img);

			//	string auth_js = "document.querySelector('input[name=\"username\"]').value = '" + auth.Login + "';" +
			//	"document.querySelector('input[name=\"password\"]').value = '" + auth.Password + "';";
			//	foreach (char ch in answer_telebot)
			//		auth_js += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
			//	auth_js += "document.querySelector('.btn_big_green').click();";
			//	eventLoadPage.Reset();
			//	SendJS(0, auth_js);
			//	if (eventLoadPage.WaitOne(10000))
			//		break;
			//}
			return true;
		}
		protected override void StartSurf()
		{
			Initialize();
			if (!Auth(auth))
				return;
			while (true)
			{
				//try
				//{
				//	MailSurf();
				//}
				//catch (Exception ex)
				//{
				//	MessageBox.Show(ex.Message);
				//}
				try
				{
					ClickSurf();
				}
				catch (Exception ex)
				{
                    Error(ex.Message);
				}
				try
				{
					VisitSurf();
				}
				catch (Exception ex)
				{
                    Error(ex.Message);
				}				
                try
				{
					YouTubeSurf();
					YouTubeSurf();
					YouTubeSurf();
				}
				catch (Exception ex)
				{
                    Error(ex.Message);
				}
                try
                {
                    RuTubeSurf();
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                }
                Sleep(200);
			}
			CloseAllBrowser();
		}
		private void ClickSurf()
		{
			LoadPage(0, "https://profitcentr.com/");
			SendJS(0, "document.querySelector('#mnu_tblock1 > a:nth-child(1)').click();");
			Sleep(4);
			//AntiBot();
			string js =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
function surf()
{
	var start_ln = surf_cl[n].querySelector('.butt-yes-test');
	if (start_ln != null) { start_ln.click(); n++; return 'surf'; }
	else { return 'wait'; }
}
function click_s()
{
	if (n >= surf_cl.length) return 'end_surf';
	else
	{	
		if(surf_cl[n].querySelector('.workquest')!=null)
			{
				if(surf_cl[n].querySelectorAll('td')[2].querySelector('a')==null || surf_cl[n].getBoundingClientRect().height == 0)
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
							IBrowser? browserSurf = GetBrowser(1);
							if (browserSurf == null)
								continue;
							Sleep(3);
							IFrame frame = browserSurf.GetFrame("frminfo");
							ev = SendJSReturn(frame,
@"b = false;
window.top.start = 0;
var timer1 = document.querySelector('#time');
var timer2 = document.querySelector('#timer_inp');
if (timer1 != null)
	timer1.innerText;
else if (timer2 != null)
	timer2.innerText;
else 'error_surf';");
							if (ev != "error")
							{
								Sleep(ev);
								Sleep(2);
								frame = browserSurf.GetFrame("frminfo");
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
								Sleep(2);
                            }
                            break;
                        }
					}
				}
				CloseСhildBrowser();
                Sleep(2);
            }
		}
		private void VisitSurf()
        {

			LoadPage(0, "https://profitcentr.com/");
			SendJS(0, "document.querySelector('#mnu_tblock1 > a:nth-child(2)').click();");
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
		if(link.querySelectorAll('td')[2].querySelector('a')==null || link.getBoundingClientRect().height == 0)
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
                    int pointStart = ev.IndexOf("Таймер: ") + 8;
					int pointEnd = ev.IndexOf(' ', pointStart);
					int countText = pointEnd- pointStart;
					if(pointStart ==-1 || pointEnd == -1 || countText > 0)
						Sleep(ev.Substring(pointStart, pointEnd - pointStart));
                    Sleep(2);
                }
				CloseСhildBrowser();
                Sleep(2);
            }
		}
		private void YouTubeSurf()
		{
			SendJS(0, "document.querySelector('#mnu_tblock1 > a:nth-child(6)').click();");
			Sleep(4);
            for (int i = 0; i < 5; i++)
            {
                string jsAntiBot =
    @"var captha_lab = document.querySelectorAll('.out-capcha-lab');
if(captha_lab.length != 0){
    'captcha';
}
else 'ok';";
                string evAntiBot = SendJSReturn(0, jsAntiBot);
                CM(evAntiBot);
				if (evAntiBot == "ok")
					break;
                else if (evAntiBot == "error")
                {
                    CM("ERROR");
                    Error("Ошибка капчи");
					return;
                }
                else
                {
                    Bitmap img = GetImgBrowser(browsers[0].MainFrame, "document.querySelector('.out-capcha')");
					
                    string answer_telebot = SendQuestion(img, "");

					jsAntiBot = "";
                    foreach (char ch in answer_telebot)
                        jsAntiBot += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
                    jsAntiBot += "document.querySelector('.btn').click();";

                    SendJS(0, jsAntiBot);
                    Sleep(5);
                }
            }
            IFrame main_frame = browsers[0].MainFrame;
			SendJS(main_frame, @"var loadPage = document.querySelector('#load-pages');
if(loadPage != null) loadPage.click();");
			Sleep(2);

			for (int i = 0; i < 5; i++)
			{
				string goBottom = SendJSReturn(main_frame, @"var goBottom = document.querySelector('#Go_Bottom');
if(goBottom.style.display != ""none"") {goBottom.click(); 'reply';}
else 'end';");
				Sleep(1);
				if (goBottom == "end")
					break;
			}
			string js_links = @"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
			function click_s()
			{
				if (n >= surf_cl.length) return 'end_surf';
				else
				{
					surf_cl[n].querySelector('span').click(); return 'click';
				}
			}
			function cl()
			{
				var start_ln = surf_cl[n].querySelector('.youtube-button');
				if (start_ln != null) { 
					if(start_ln.innerText != 'Приступить к просмотру') {n++; return 'continue';}
					else {start_ln.querySelector('span').click(); n++; return 'surf'; }
				}
				else { return 'sec_wait'; }
			}";
			SendJS(main_frame, js_links);
			while (true)
			{
				string ev = SendJSReturn(main_frame, "click_s();");
				if (ev == "end_surf")
				{
					break;
				}
				else if (ev == "click")
				{
					for (int i = 0; i < 10; i++)
					{
						ev = SendJSReturn(main_frame, "cl();");
						if (ev == "surf")
                        {
                            Sleep(2);
                            var browserYouTube = GetBrowser(1);
							if (browserYouTube == null)
								break ;
							IFrame yotube_frame = browserYouTube.MainFrame;
							ev = SendJSReturn(yotube_frame,
@"c = true;  b = true; document.querySelector('#tmr').innerText;");
							if (ev != "error")
							{
								Sleep(ev);
								ev = WaitButtonClick(yotube_frame, "document.querySelector('.butt-nw');");
								if (ev == "errorWait")
									Error("Error end youtube watch");
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
				else MessageBox.Show("Ошибка блять");
				CloseСhildBrowser();
				Sleep(1);
			}
		}
		private void RuTubeSurf()
		{
            SendJS(0, "document.querySelector('#mnu_tblock1 > a:nth-child(7)').click();");
            Sleep(4);
            for (int i = 0; i < 5; i++)
            {
                string jsAntiBot =
    @"var captha_lab = document.querySelectorAll('.out-capcha-lab');
if(captha_lab.length != 0){
    'captcha';
}
else 'ok';";
                string evAntiBot = SendJSReturn(0, jsAntiBot);
                CM(evAntiBot);
                if (evAntiBot == "ok")
                    break;
                else if (evAntiBot == "error")
                {
                    Error("Ошибка капчи");
                    return;
                }
                else
                {
                    Bitmap img = GetImgBrowser(browsers[0].MainFrame, "document.querySelector('.out-capcha')");

                    string answer_telebot = SendQuestion(img,"");

                    jsAntiBot = "";
                    foreach (char ch in answer_telebot)
                        jsAntiBot += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
                    jsAntiBot += "document.querySelector('.btn').click();";

                    SendJS(0, jsAntiBot);
                    Sleep(5);
                }
            }
            IFrame main_frame = browsers[0].MainFrame;
            SendJS(main_frame, @"var loadPage = document.querySelector('#load-pages');
if(loadPage != null) loadPage.click();");
            Sleep(2);

            for (int i = 0; i < 5; i++)
            {
                string goBottom = SendJSReturn(main_frame, @"var goBottom = document.querySelector('#Go_Bottom');
if(goBottom.style.display != ""none"") {goBottom.click(); 'reply';}
else 'end';");
                Sleep(1);
                if (goBottom == "end")
                    break;
            }
            string js_links = @"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
			function click_s()
			{
				if (n >= surf_cl.length) return 'end_surf';
				else
				{
					surf_cl[n].querySelector('span').click(); return 'click';
				}
			}
			function cl()
			{
				var start_ln = surf_cl[n].querySelector('.youtube-button');
				if (start_ln != null) { 
					if(start_ln.innerText != 'Приступить к просмотру') {n++; return 'continue';}
					else {start_ln.querySelector('span').click(); n++; return 'surf'; }
				}
				else { return 'sec_wait'; }
			}";
            SendJS(main_frame, js_links);
            while (true)
            {
                string ev = SendJSReturn(main_frame, "click_s();");
                if (ev == "end_surf")
                {
                    break;
                }
                else if (ev == "click")
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ev = SendJSReturn(main_frame, "cl();");
                        if (ev == "surf")
                        {
                            var browserYouTube = GetBrowser(1);
                            if (browserYouTube == null)
                                break;
                            Sleep(2);
                            IFrame yotube_frame = browserYouTube.MainFrame;
                            ev = SendJSReturn(yotube_frame,
@"c = true; b = true; document.querySelector('#tmr').innerText;");
                            if (ev != "error")
                            {
                                Sleep(ev);

                                ev = WaitButtonClick(yotube_frame, "document.querySelector('.butt-nw');");
                                if (ev == "errorWait")
                                {
                                    Error("Error end youtube watch");
                                }
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
                else MessageBox.Show("Ошибка блять");
                CloseСhildBrowser();
                Sleep(1);
            }
        }
		private void MailSurf()
		{
			LoadPage(0, "https://profitcentr.com/");
			Sleep(2);
			SendJS(0, "document.querySelector('#mnu_tblock1 > a:nth-child(3)').click();");
			Sleep(4);
			string js =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 1;
function surf()
{
	var start_ln = surf_cl[n].querySelector('.butt-yes-test');
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
					return;
				else if (ev == "contionue")
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
							ev = GetMailAnswer(browsers[0].MainFrame, "document.querySelector('.taskdescription')",
								"document.querySelector('.taskquestion')",
								"document.querySelectorAll('.mails-otvet-new a')");
							if (ev == "errorMail")
							{
								Random rnd = new Random();
								ev = rnd.Next(0, 3).ToString();
							}
							SendJS(0, "document.querySelectorAll('.mails-otvet-new a')[" + ev + "].click();");
							Sleep(5);
							IFrame frame = browsers[1].GetFrame("frminfo");
							ev = SendJSReturn(frame,
@"b = false;
window.top.start = 0;
var timer1 = document.querySelector('#time');
var timer2 = document.querySelector('#timer_inp');
if (timer1 != null)
	return timer1.innerText;
else if (timer2 != null)
	return timer2.innerText;
else 'error_surf';");
							if (ev != "error")
							{
								Sleep(ev);
								Sleep(2);
								frame = browsers[1].GetFrame("frminfo");
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
	'error_surf';
	location.replace(""vlss?view=ok"");
}");
								Sleep(2);
							}
						}
					}
				}
				CloseСhildBrowser();
			}
		}
		private bool AntiBot()
		{
			for (int i = 0; i < 5; i++)
			{
				string jsAntiBot =
	@"var captha_lab = document.querySelectorAll('.out-capcha-lab');
if(captha_lab.length != 0){
    'captcha';
}
else 'ok';";
				string evAntiBot = SendJSReturn(0, jsAntiBot);
				CM(evAntiBot);
				if (evAntiBot == "ok")
					return true;
				else if (evAntiBot == "error")
				{
					CM("ERROR");
					Error("Ошибка капчи");
					return false;
				}
				else
				{
					Bitmap img = GetImgBrowser(browsers[0].MainFrame, "document.querySelector('.out-capcha')");

					string answer_telebot = SendQuestion(img, "");

					jsAntiBot = "";
					foreach (char ch in answer_telebot)
						jsAntiBot += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
					jsAntiBot += "document.querySelector('.btn_big_green').click();";

					SendJS(0, jsAntiBot);
					Sleep(5);
				}
			}
			Error("Ошибка ввода капчи");
			return false;
		}
	}
}