﻿using CefSharp;
using OpenCvSharp;
using Size = OpenCvSharp.Size;

namespace ClickMashine
{
    class WmrFast : Site
	{
		ImageControlWmrClick imageConrolWmrClick;
		public WmrFast(Form1 form, Auth auth) : base(form, auth)
		{
			homePage = "https://wmrfast.com/";
			Type = EnumTypeSite.WmrFast; 
			imageConrolWmrClick = new ImageControlWmrClick(@"C:/ClickMashine/Settings/Net/WmrFast/WmrFastClick.h5");
		}
		public override bool Auth(Auth auth)
		{
			LoadPage("https://wmrfast.com/");
			Sleep(2);
			ImageControlWmrAuth imageConrolWmrAuth = new ImageControlWmrAuth(@"C:/ClickMashine/Settings/Net/WmrFast/WmrFastAuth.h5");
			while (true)
			{
				string ev = SendJSReturn(browsers[0], "var but_log = document.querySelector('#logbtn'); if(but_log != null) {but_log.click(); 'login';} else 'end';");
				if (ev == "login")
				{
					Sleep(2);
                    ev = SendJSReturn(browsers[0],
@"if(document.querySelector(""#h-captcha"")) 'h-captcha';
else if(document.querySelector(""#login_cap"")) 'login_cap';");
					if (ev == "login_cap")
					{
						string js =
@"document.querySelector('#vhusername').value = '" + auth.Login + @"';
document.querySelector('#vhpass').value = '" + auth.Password + @"';
document.querySelector('#cap_text').value = '" + imageConrolWmrAuth.Predict(GetImgBrowser(browsers[0].MainFrame, "document.querySelector('#login_cap')")) + @"';
document.querySelector('#vhod1').click();";
						eventLoadPage.Reset();
						SendJS(browsers[0], js);
						eventLoadPage.WaitOne();
						Sleep(3);
					}
					else
					{
						return false;
					}
                }
				else
					break;
			}
			return true;
		}
		protected override void StartSurf()
		{
			Initialize();
			if (!Auth(auth))
				return;
			CM("123123123");
			try
			{
				while(ClickSurf()>5);
			}
			catch (Exception ex)
			{
				Error("Ошибка Click: " + ex.Message);
			}
			try
			{
				while(VisitSurf()>5);
			}
			catch (Exception ex)
			{
                Error("Ошибка Visit: " + ex.Message);
			}
			try
			{
				while(YouTubeSurf()>5);
			}
			catch (Exception ex)
			{
				Error("Ошибка YouTube: " + ex.Message);
			}
			CloseAllBrowser();
		}
		private int YouTubeSurf()
		{
			int Count = 0;
			LoadPage(0, "https://wmrfast.com/serfing_ytn.php");
            Sleep(2);
            string js =
@"var surf_cl = document.querySelectorAll('.serf_hash');var n = 0;		
function click_s()
{
	if (n >= surf_cl.length) return 'end';
	else
	{
		surf_cl[n].click(); n++; return 'surf';
	}
}";
			SendJS(0, js);
			while (true)
			{
				string ev = SendJSReturn(0, "click_s();");
				if (ev == "surf")
				{
					IBrowser? browser = WaitCreateBrowser();
					if (browser == null)
					{
						CloseСhildBrowser();
						continue;
					}
					Sleep(1);
					if (WaitElement(browser.MainFrame, "document.querySelector(\"#tt\")"))
					{
						ev = SendJSReturn(browser, "vs = true;timer.toString();");
						if (ev != "error")
						{
							Sleep(ev);
							WaitButtonClick(browsers[1].MainFrame, "document.querySelector('a');");
							Count++;
							Sleep(2);
						}
					}
				}
				else if (ev == "end")
					break;
				CloseСhildBrowser();
				Sleep(1);
			}
			return Count;
		}
		private int ClickSurf()
		{
			int Count = 0;
			LoadPage("https://wmrfast.com/serfing.php");
            Sleep(2);
            string js =
@"var surf_cl = document.querySelectorAll('.serf_hash');var n = 0;		
function click_s()
{
	if (n >= surf_cl.length) return 'end';
	else
	{
		surf_cl[n].click(); n++; return 'surf';
	}
}";
			SendJS(0, js);
			while (true)
			{
				string ev = SendJSReturn(0, "click_s();");
				if (ev == "surf")
				{
					IBrowser? browser = GetBrowser(1);
					if (browser!= null)
					{
						ev = SendJSReturn(1, "counter.toString();");
						if (ev != "error")
						{
							Sleep(ev);
							js =
@"function waitCounter(){
	if(counter == -1) return 'ok';
	else return 'wait';
}";
							ev = WaitFunction(browsers[1].MainFrame, "waitCounter();", js);
							if (ev == "ok")
							{
								for (int i = 0; i < 10; i++)
								{
									string evClick = "";
									Sleep(1);
									string value;
									try
									{
										value = imageConrolWmrClick.Predict(GetImgBrowser(browsers[1].MainFrame, "document.querySelector('#captcha-image')"));
									}
									catch (Exception ex)
                                    {
										Console.WriteLine(ex.ToString()); 
										SendJS(1, "document.querySelector('#capcha > tbody > tr > td:nth-child(1) > a').click();");
										Sleep(2);
										continue;
                                    }
									if (value.Length == 3)
									{
										js = @"function endClick() {var butRet = document.querySelectorAll('[method=""POST""]');
for (var i = 0; i < butRet.length; i++)
{
	if (butRet[i].querySelector('.submit').value == " + value + @")
	{ butRet[i].querySelector('.submit').click(); return 'ok'}
}
return 'errorClick';}endClick();";
										evClick = SendJSReturn(1, js);
									}
									if (evClick == "ok")
									{
										Sleep(2);
										Count++;
										break;
									}
									SendJS(1, "document.querySelector('#capcha > tbody > tr > td:nth-child(1) > a').click();");
									Sleep(2);
								}
							}
						}
					}
				}
				else if (ev == "end")
					break;
				CloseСhildBrowser();
				Sleep(2);
			}
			return Count;
		}	
		private int VisitSurf()
        {
			int Count = 0;
			LoadPage("https://wmrfast.com/serfing.php");
			Sleep(2);
			string js =
@"var surf_cl = document.querySelectorAll('.serf_hash');var n = 0;		
function click_s()
{
	if (n < surf_cl.length) {
		surf_cl[n].click(); 
		return surf_cl[n++].getAttribute('timer').toString();
	}
	else
	{
		return 'end';
	}
}";
			SendJS(0, js);
			while (true)
			{
				string ev = SendJSReturn(0, "click_s();");
				if (ev == "end")
					break;
                else
                {
					WaitCreateBrowser(1);
					Sleep(2);					
					Sleep(ev);
					Count++;
				}
				CloseСhildBrowser();
				Sleep(2);
			}
			return Count;
		}
	}
}