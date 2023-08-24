﻿using CefSharp;
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
			LoadPage(0, "https://profitcentr.com/login");
			string auth_js = "document.querySelector('input[name=\"username\"]').value = '" + auth.Login + "';" +
							 "document.querySelector('input[name=\"password\"]').value = '" + auth.Password + "';";
            SendJS(0, auth_js);
			if (AntiBot())
			{
				string ev = GetMoney(browsers[0], "document.querySelector('#new-money-ballans')");
				if (ev == "error")
					return false;
                siteStripComboBox.Text = StatusSite.online.ToString();
                return true;
			}
			return false;
		}
		protected override void StartSurf()
		{
			Initialize();
			TrainBD();

            waitHandle.WaitOne();

            if (!Auth(auth))
				waitHandle.WaitOne();

            mSurf.AddFunction(ClickSurf);
            mSurf.AddFunction(MailSurf);
            mSurf.AddFunction(VisitSurf);
            mSurf.AddFunction(YouTubeSurf);
            mSurf.AddFunction(RuTubeSurf);
            while (true)
			{
				mSurf.GoSurf();
				Sleep(600);
			}
		}
		private int ClickSurf()
		{
			int Count = 0;
			IBrowser mainBrowser = browsers[0];
			LoadPage("https://profitcentr.com/");
            LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(1)').href"));

            if (!OutCaptchaLab(browsers[0], "document.querySelector('.out-capcha')", "document.querySelectorAll('.out-capcha-inp')", "document.querySelector('.btn').click();"))
            {
                Error("Error captcha youtube");
                return Count;
            }

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
								if (WaitElement(browserSurf, "frminfo", "document.querySelector('#timer_inp')"))
                                {
                                    IFrame frame = browserSurf.GetFrame("frminfo");
                                    ev = SendJSReturn(frame,
@"b = false;
window.top.start = 0;
var timer_s = document.querySelector('#timer_inp');
if (timer_s != null)
	timer_s.innerText;
else 'error_surf';");
									if (ev != "error_surf")
									{
										Sleep(ev);
										if (WaitElement(browserSurf, "frminfo", @"document.querySelector('[type=""range""]')"))
                                        {
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
											Count++;
											Sleep(2);
										}
										break;
									}
								}
							}
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
			IBrowser mainBrowser = browsers[0];
			LoadPage(mainBrowser, "https://profitcentr.com/");
            LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(2)').href"));
            if (!OutCaptchaLab(mainBrowser, "document.querySelector('.out-capcha')", "document.querySelectorAll('.out-capcha-inp')", "document.querySelector('.btn').click();"))
            {
                Error("Error captcha youtube");
                return Count;
            }

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
			SendJS(mainBrowser, js);
			while (true)
			{
				eventBrowserCreated.Reset();
				string ev = SendJSReturn(mainBrowser, "click_s();");
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
						{
							Count++;
							Sleep(ev.Substring(pointStart, pointEnd - pointStart));
						}
						Sleep(2);
					}
                }
				CloseСhildBrowser();
                Sleep(2);
            }
			return Count;
		}
		private int YouTubeSurf()
		{
			int Count = 0;
			var mainBrowser = browsers[0];
			LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(6)').href"));
            if (!OutCaptchaLab(mainBrowser, "document.querySelector('.out-capcha')", "document.querySelectorAll('.out-capcha-inp')", "document.querySelector('.btn').click();"))
            {
                Error("Error captcha youtube");
                return Count;
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
				Sleep(2);
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
			function surf()
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
				eventBrowserCreated.Reset();
				string ev = SendJSReturn(main_frame, "click_s();");
				if (ev == "end_surf")
				{
					break;
				}
				else if (ev == "click")
				{
					for (int i = 0; i < 10; i++)
					{
						ev = SendJSReturn(main_frame, "surf();");
						if (ev == "surf")
						{
							var browserYouTube = WaitCreateBrowser();
							if (browserYouTube != null)
							{
								IFrame yotube_frame = browserYouTube.MainFrame;
								ev = SendJSReturn(yotube_frame,
	@"c = true;  b = true; document.querySelector('#tmr').innerText;");
								if (ev != "error")
								{
									Sleep(ev);
									if (WaitButtonClick(yotube_frame, "document.querySelector('.butt-nw');"))
										Count++;
									Sleep(2);
								}
							}
							break;
						}
						else if (ev == "sec_wait")
							Sleep(1);
						else if (ev == "continue")
							break;
						if(i == 9)
						{
							SendJS(main_frame, "n++;");
						}
					}
				}
				else
				{
					Sleep(1);
				}
				CloseСhildBrowser();
				Sleep(1);
			}
			return Count;
		}
		private int RuTubeSurf()
		{
			int Count = 0;
			IBrowser mainBrowser = browsers[0];
            LoadPage("https://profitcentr.com/");
            LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(7)').href"));
            if (!OutCaptchaLab(mainBrowser, "document.querySelector('.out-capcha')", "document.querySelectorAll('.out-capcha-inp')", "document.querySelector('.btn').click();"))
            {
                Error("Error captcha youtube");
                return Count;
            }

            IFrame main_frame = mainBrowser.MainFrame;
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
			function surf()
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
                eventBrowserCreated.Reset();
                string ev = SendJSReturn(main_frame, "click_s();");
                if (ev == "end_surf")
                {
                    break;
                }
                else if (ev == "click")
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ev = SendJSReturn(main_frame, "surf();");
                        if (ev == "surf")
                        {
                            var browserYouTube = WaitCreateBrowser();
							if (browserYouTube != null)
							{
								IFrame youtubeFrame = browserYouTube.MainFrame;
								if(!WaitElement(youtubeFrame, "document.querySelector('#tmr')"))
								{
									Error("YouTube error");
									break;
								}
								ev = SendJSReturn(youtubeFrame,
	@"c = true; b = true; document.querySelector('#tmr').innerText;");
								if (ev != "error")
								{
									Sleep(ev);

									if (!WaitButtonClick(youtubeFrame, "document.querySelector('.butt-nw');"))
									{
										Error("Error end youtube watch");
                                        break;
                                    }
									Count++;
									Sleep(2);
								}
								break;
							}
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
			return Count;
		}
		private int MailSurf()
		{
			int Count = 0;
            IBrowser mainBrowser = browsers[0];
            LoadPage("https://profitcentr.com/");
            LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(3)').href"));
			string js =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 1;
function surf()
{
	var start_ln = surf_cl[n].querySelector('.butt-yes-test');
	if (start_ln != null) { start_ln.click(); n++; return 'surf'; }
	else { return 'wait'; }
}
function click_s()
{
	if (n >= surf_cl.length) { return 'end_surf';}
	else
	{
		surf_cl[n].querySelector('a').click(); return 'click';
	}
}";
			SendJS(0, js);
			while (true)
            {
                eventBrowserCreated.Reset();
                string ev = SendJSReturn(0, "click_s();");
				if (ev == "end_surf")
					break;
				else if (ev == "contionue")
					continue;
				else if (ev == "click")
				{
					for (int i = 0; i < 10; i++)
					{
						ev = SendJSReturn(0, "surf();");
						if (ev == "wait")
							Sleep(1);
						else if (ev == "surf")
						{
							if (WaitElement(mainBrowser, "document.querySelector('.taskdescription')"))
							{
								ev = GetMailAnswer(mainBrowser.MainFrame, "document.querySelector('.taskdescription')",
									"document.querySelector('.taskquestion')",
									"document.querySelectorAll('.mails-otvet-new a')");
								if (ev == "errorMail")
								{
									Random rnd = new Random();
									ev = rnd.Next(0, 3).ToString();
								}
								SendJS(0, "document.querySelectorAll('.mails-otvet-new a')[" + ev + "].click();");
								IBrowser? browserMail = WaitCreateBrowser();
								if (browserMail != null)
								{
									ev = SendJSReturn(browserMail,
	@"b = false;
var timer1 = document.querySelector('#tmr');
if (timer1 != null)
	timer1.innerText;
else 'error_surf';");
									if (ev != "error")
									{
										Sleep(ev);
										if (WaitElement(browserMail, @"document.querySelector('[type=""range""]')"))
										{
											string function =
	@"var range = document.querySelector('[type=""range""]');
if (range != null)
{
	range.value = range.max;
	document.querySelector('button').click();
	'end';
}
else
{
	'error';
}";
											ev = SendJSReturn(browserMail, function);
											if (ev != "error")
											{
												Count++;
												Sleep(2);
											}
										}
									}
								}
							}
						}
					}
				}
				CloseСhildBrowser();
			}
			return Count;
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
					for(int j = 0; j< 6; j++)
					{
                        SaveImage(GetImgBrowser(browsers[0].MainFrame, "document.querySelectorAll('.out-capcha-lab')["+j.ToString()+"]"));
                    }

                    Bitmap img = GetImgBrowser(browsers[0].MainFrame, "document.querySelector('.out-capcha')");
                    string answer_telebot = SendQuestion(img, "");
					if(answer_telebot == "")
					jsAntiBot = "";
					foreach (char ch in answer_telebot)
						jsAntiBot += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
					jsAntiBot += "document.querySelector('.btn_big_green').click();";

					SendJS(0, jsAntiBot);
					Sleep(10);
				}
			}
			Error("Ошибка ввода капчи");
			return false;
		}
		private void TrainBD()
        {
			LoadPage("https://profitcentr.com/login");
            string path = @"C:\ClickMashine\Settings\Image\" + Type.ToString() + @"\";
            for (int i = 0; i < 100; i++) {
				string name = SendJSReturn(browsers[0].MainFrame, "document.querySelector('.out-capcha-title').innerText");
				string[] name_item = name.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				Directory.CreateDirectory(path + name_item[name_item.Length-1] + @"\");

                SendJS(0, "for(var i = 0; i< 6;i++)document.querySelectorAll('.out-capcha-lab')[i].style.border = '0px';");
				Sleep(1);
                for (int j = 0; j < 6; j++)
				{
                    GetImgBrowser(browsers[0].MainFrame, "document.querySelectorAll('.out-capcha-lab')[" + j.ToString() + "]")
						.Save(path + new DirectoryInfo(path).GetFiles().Length.ToString() + ".png");
				}
				SendJS(0, "document.querySelector('.out-reload').click();");
				Sleep(2);
			}
        }
	}
}