using CefSharp;
using OpenCvSharp;
using Size = OpenCvSharp.Size;

namespace ClickMashine
{
	class VipClick : Site
	{
		VipClickImageConrol imageConrol;
		public VipClick(Form1 form, Auth auth) : base(form, auth)
		{
			homePage = "https://vip-click.com/";
			Type = EnumTypeSite.VipClick;
			imageConrol = new VipClickImageConrol(@"C:/ClickMashine/Settings/Net/VipClick.h5");
        }
		public override bool Auth(Auth auth)
		{
			IBrowser loginBrowser = browsers[0];
			LoadPage(loginBrowser, "https://vip-click.com/login");
			string ev = SendJSReturn(0, "var but_log = document.querySelector('#login-form'); if(but_log != null) 'login'; else 'end';");
			if (ev == "login")
			{
				Sleep(2);
				string js =
@"document.querySelector('#login').value = '" + auth.Login + @"';
document.querySelector('#pwd').value = '" + auth.Password + @"';
document.querySelector('#login-form > center > button').click();";
				eventLoadPage.Reset();
				SendJS(0, js);
				eventLoadPage.WaitOne(5000);
				Sleep(3);
				return true;
			}
			else
				return true;
		}
		protected override void StartSurf()
		{
			Initialize();
			if (!Auth(auth))
				return;
			try
			{
				while (ClickSurf() > 5) ;
			}
			catch (Exception ex)
			{
				Error("Ошибка Click: " + ex.Message);
			}
			//try
			//{
			//	while (VisitSurf() > 5) ;
			//}
			//catch (Exception ex)
			//{
			//	Error("Ошибка Visit: " + ex.Message);
			//}
			try
			{
				while (YouTubeSurf() > 5) ;
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
			LoadPage(0, "https://vip-click.com/youtube");
			string js =
@"var surf_cl = document.querySelectorAll('.detn_whole_block');var n = 0;		
function click_s()
{
	if (n >= surf_cl.length) return 'end';
	else
	{
		surf_cl[n].querySelector('button').click(); n++; return 'surf';
	}
}";
			SendJS(0, js);
			while (true)
			{
				eventBrowserCreated.Reset();
				string ev = SendJSReturn(0, "click_s();");
				if (ev == "surf")
				{
					IBrowser? youTube = GetBrowser(1);
					if(youTube != null)
					{
						ev = SendJSReturn(youTube.MainFrame, "timers_w");
						if (ev != "error")
						{
							Sleep(ev);
							if (Captcha(youTube, "document.querySelector('.clocktable img')"))
							{
								Count++;
							}
							else
							{
								SendJS(youTube, "getCaptcha();");
                                if (Captcha(youTube, "document.querySelector('.clocktable img')"))
                                {
                                    Count++;
                                }
                            }
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
			LoadPage("https://vip-click.com/serf");
			string js =
@"var surf_cl = document.querySelectorAll('.detn_whole_block');var n = 0;		
function click_s()
{
	if (n >= surf_cl.length) return 'end';
	else
	{
		surf_cl[n].querySelector('button').click(); n++; return 'surf';
	}
}";
			SendJS(0, js);
			while (true)
			{
				eventBrowserCreated.Reset();
				string ev = SendJSReturn(0, "click_s();");
				if (ev == "surf")
				{
					IBrowser? browser = GetBrowser(1);
					if (browser != null)
					{
						ev = SendJSReturn(1, "time.toString();");
						if (ev != "error")
						{
							Sleep(ev);
							if (Captcha(browser, "document.querySelector('#blockverify > table > tbody > tr > td:nth-child(1) > img')"))
							{
								Count++;
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
			string js =
@"var surf_cl = document.querySelectorAll('.serf_hash');var n = 0;		
function click_s()
{
	if (n >= surf_cl.length) return surf_cl[0].getAttribute('timer').toString();
	else
	{
		surf_cl[n].click(); n++; return 'surf';
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
					WaitCreateBrowser();
					Sleep(2);
					Sleep(ev);
					Count++;
				}
				CloseСhildBrowser();
				Sleep(2);
			}
			return Count;
		}
		private bool Captcha(IBrowser browser, string img)
		{
			if (WaitElement(browser.MainFrame, img,5))
			{
				string predict = imageConrol.Predict(GetImgBrowser(browser.MainFrame, img));
				if(predict == "error")
					return false;
				SendJS(browser.MainFrame, @"document.querySelectorAll('[nowrap=""nowrap""] span')["+predict+@"-1].click();");
				Sleep(2);
				return true;
			}
			return false;
        }
	}
}