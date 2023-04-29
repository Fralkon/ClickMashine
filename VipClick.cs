using CefSharp;
using OpenCvSharp;
using Size = OpenCvSharp.Size;

namespace ClickMashine
{
	class VipClick : Site
	{
		VipClickImageConrol imageConrol = new VipClickImageConrol( @"C:/ClickMashine/Settings/Net/VipClick.h5");
		public VipClick(Form1 form, Auth auth) : base(form, auth)
		{
			homePage = "https://vip-click.com/";
			Type = EnumTypeSite.VipClick;
		}
		public override bool Auth(Auth auth)
		{
			IBrowser loginBrowser = browsers[0];
			LoadPage(loginBrowser, "https://vip-click.com/login");
			string ev = SendJSReturn(0, "var but_log = document.querySelector('#logbtn'); if(but_log != null) {but_log.click(); 'login';} else 'end';");
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
			Console.WriteLine(imageConrol.Predict(new Bitmap(@"C:/Users/Boyarkin/Desktop/captcha.png")));
			Console.ReadLine();
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
				string ev = SendJSReturn(0, "click_s();");
				if (ev == "surf")
				{
					Sleep(3);
					IBrowser? youTube = GetBrowser(1);
					if(youTube != null)
					{
						ev = SendJSReturn(youTube.MainFrame, "vtime");
						if (ev != "error")
						{
							Sleep(ev);
							if (Captcha(youTube, "document.querySelector('.clocktable img')"))
							{
								Count++;
								Sleep(2);
							}
						}
					}
				}
				else if (ev == "end")
					break;
				CloseСhildBrowser();
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
				string ev = SendJSReturn(0, "click_s();");
				if (ev == "surf")
				{
					IBrowser? browser = GetBrowser(1);
					if (browser != null)
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
										value = imageConrol.Predict(GetImgBrowser(browsers[1].MainFrame, "document.querySelector('#captcha-image')"));
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
		private bool Captcha(IBrowser browser, string img)
		{
			if (WaitElement(browser.MainFrame, img))
			{
				string predict = imageConrol.Predict(GetImgBrowser(browser.MainFrame, img));
				if(predict == "error")
					return false;
				SendJS(browser.MainFrame, @"document.querySelectorAll('[nowrap=""nowrap""] span')["+predict+@"-1].click();");
				return true;
			}
			return false;
        }
	}
}