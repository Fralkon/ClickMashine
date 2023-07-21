using CefSharp;
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
@"if(document.querySelector(""#anchor"")) 'anchor';
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
					else if(ev == "anchor")
					{

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
			{
				waitHandle.WaitOne();
			}
			mSurf.AddFunction(YouTubeSurf);
			//mSurf.AddFunction(ClickSurf);
			while (true)
			{
				mSurf.GoSurf();
				Sleep(300);
			}
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
                eventBrowserCreated.Reset();
				try
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
				}
				catch (Exception ex) { CM(ex.Message); }
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
				eventBrowserCreated.Reset();
				string ev = SendJSReturn(0, "click_s();");
				if (ev == "surf")
				{
					IBrowser? browser = WaitCreateBrowser();
					if (browser!= null)
					{
						ev = SendJSReturn(browser, "counter.toString();");
						if (ev != "error")
						{
							Sleep(ev);
							ev = SendJSReturn(browser, "counter.toString();");
							if(ev != "-1")
							{
								SendJS(browser, "counter = 0;flag = 1;");
								Sleep(2);
                            }
                            for (int i = 0; i < 10; i++)
                            {
                                Sleep(1);
                                string value;
                                try
                                {
                                    value = imageConrolWmrClick.Predict(GetImgBrowser(browser.MainFrame, "document.querySelector('#captcha-image')"));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                    SendJS(browser, "document.querySelector('#capcha > tbody > tr > td:nth-child(1) > a').click();");
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
									if (SendJSReturn(browser, js) == "ok")
									{
                                        Sleep(2);
                                        Count++;
                                        break;
                                    }
                                }
                                SendJS(browser, "document.querySelector('#capcha > tbody > tr > td:nth-child(1) > a').click();");
                                Sleep(2);
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
				eventBrowserCreated.Reset();
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

        protected bool Anchor(IBrowser browser, string captcha, string picture, string input, string button)
        {
            string js =
@"var img_captcha = " + captcha + @";
if(img_captcha != null)
    'antiBot';
else 'notAntiBot';";
            int iteration = 0;
            SendJS(browser, captcha + ".click();");
            if (!WaitElement(browser, picture))
                return false;
            while (SendJSReturn(browser.MainFrame, js) == "antiBot")
            {

                if (iteration == 10)
                    return false;
                string jsAntiBot = String.Empty;
                foreach (char ch in SendQuestion(GetImgBrowser(browser.MainFrame, picture), ""))
                    jsAntiBot += input + "[" + ch + "].click();";
                jsAntiBot += button + ";";
                SendJS(browser.MainFrame, jsAntiBot);
                Sleep(4);
                iteration++;

                eventLoadPage.Reset();
                browser.Reload();
                eventLoadPage.WaitOne(5000);
            }
            return true;
        }
    }
}