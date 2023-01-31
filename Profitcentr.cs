using CefSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickMashine_10._0
{
	class Profitcentr : Site
	{
		public Profitcentr(Form1 form, TeleBot teleBot) : base(form, teleBot)
		{
			homePage = "https://profitcentr.com/";
			type.enam = EnumTypeSite.Profitcentr;
		}
		public override void Auth(Auth auth)
		{
			Sleep(5);
			LoadPage(0, "https://profitcentr.com/login");
			while (true)
			{
				string ev = SendJSReturn(0, "var js = document.querySelector('.out-capcha').getBoundingClientRect().toJSON();" +
	"JSON.stringify({ X: parseInt(js.x), Y: parseInt(js.y),  Height: parseInt(js.height), Width: parseInt(js.width)});");
				Rectangle rect_img = JsonSerializer.Deserialize<Rectangle>(ev);
				FocusBrowser(browsers[0]);
				Bitmap img = MakeScreenshot(rect_img);
				string answer_telebot = teleBot.SendQuestion(img);

				string auth_js = "document.querySelector('input[name=\"username\"]').value = '" + auth.Login + "';" +
				"document.querySelector('input[name=\"password\"]').value = '" + auth.Password + "';";
				foreach (char ch in answer_telebot)
					auth_js += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
				auth_js += "document.querySelector('.btn_big_green').click();";
				eventLoadPage.Reset();
				SendJS(0, auth_js);
				if (eventLoadPage.WaitOne(5000))
					break;
			}
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
			try
			{
				YouTubeSurf();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			CloseAllBrowser();
		}
		private void ClickSurf()
		{
			LoadPage(0, "https://profitcentr.com/");
			Sleep(2);
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
	if (n + 4 >= surf_cl.length) return 'end_surf';
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
					break;
				else if (ev == "continue")
					continue;
				else if (ev == "click")
				{
					for (int i = 0; i < 10; i++)
					{
						ev = SendJSReturn(0, "surf();");
						if (ev == "wait")
							Sleep(2);
						else if (ev == "surf")
						{
							Sleep(3);
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
				Sleep(2);
				CloseСhildBrowser();
			}
		}
		private void YouTubeSurf()
		{
			LoadPage(0, "https://profitcentr.com/");
			Sleep(2);
			SendJS(0, "document.querySelector('#mnu_tblock1 > a:nth-child(6)').click();");
			Sleep(4);
			//AntiBot();
			IFrame main_frame = browsers[0].MainFrame;
			string js_links = @"var surf_cl = document.querySelectorAll('.work-serf');var n = 2;
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
					if(start_ln.innerText == 'Площадка не найдена') {n++; return 'continue';}
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
							Sleep(3);
							IFrame yotube_frame = browsers[1].MainFrame;
							ev = SendJSReturn(yotube_frame,
@"player.setVolume(0); b = true; player.seekTo(0, true); document.querySelector('#tmr').innerText;");
							if (ev != "error")
							{
								Sleep(ev);

								ev = WaitButtonClick(yotube_frame, "document.querySelector('.butt-nw');");
								if (ev == "errorWait")
								{
									CM("Error end youtube watch");
								}
							}
							break;
						}
						else if (ev == "sec_wait")
						{
							Sleep(1);
						}
						else if (ev == "continue")
						{
							CloseСhildBrowser();
							break;
						}
					}
				}
				else MessageBox.Show("Ошибка блять");
				Sleep(2);
				CloseСhildBrowser();
			}
		}
		private void MailSurf()
		{
			LoadPage(0, "https://profitcentr.com/");
			Sleep(2);
			SendJS(0, "document.querySelector('#mnu_tblock1 > a:nth-child(2)').click();");
			Sleep(4);
			string js =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 1;
function surf()
{
	var start_ln = surf_cl[n].querySelector('.butt-yes-test');
	if (start_ln != null) { start_ln.click(); n++; return 'surf'); }
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
		private void AntiBot()
		{
			string jsAntiBot =
@"var captha_lab = document.querySelectorAll('.out-capcha-lab');
if(captha_lab.length != 0){
    var js = document.querySelector('.out-capcha').getBoundingClientRect().toJSON();
    JSON.stringify({ X: parseInt(js.x), Y: parseInt(js.y),  Height: parseInt(js.height), Width: parseInt(js.width)});
}
else 'ok';";
			string evAntiBot = SendJSReturn(0, jsAntiBot);
			CM(evAntiBot);
			if (evAntiBot == "ok")
			{
				return;
			}
			else if (evAntiBot == "error")
			{
				CM("ERROR");
				throw new Exception("Jib,rf ,kz");
			}
			else
			{
				Rectangle rect_img = JsonSerializer.Deserialize<Rectangle>(evAntiBot);
				FocusBrowser(browsers[0]);
				Bitmap img = MakeScreenshot(rect_img);
				string answer_telebot = teleBot.SendQuestion(img);

				jsAntiBot = "";
				foreach (char ch in answer_telebot)
					jsAntiBot += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
				jsAntiBot += "document.querySelector('.btn').click();";

				eventLoadPage.Reset();
				SendJS(0, jsAntiBot);
				eventLoadPage.WaitOne();
			}
		}
	}
}