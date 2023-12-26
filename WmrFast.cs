using CefSharp;

namespace ClickMashine
{
    class WmrFast : Site
	{
		ImageControlWmrClick imageConrolWmrClick;
        public WmrFast(Form1 form, Auth auth) : base(form, auth)
		{
			homePage = "https://wmrfast.com/";
			HostName = "wmrfast.com";
            Type = EnumTypeSite.WmrFast;

			string FirstStep =
@"var surf_cl = document.querySelectorAll('.serf_hash');var n = 0;		
function FirstStep()
{
	if (n >= surf_cl.length) return "+(int)StatusJS.End+ @";
	else
	{
		surf_cl[n].click();
		n++;
		return "+(int)StatusJS.OK+@";
	}
}";
			ManagerSurfing.AddSurfing(new Surfing(this, "https://wmrfast.com/serfing_ytn.php", FirstStep, new Surfing.MiddleStepDelegate(YouTubeMiddle)));
			ManagerSurfing.AddSurfing(new Surfing(this, "https://wmrfast.com/serfing.php", FirstStep, new Surfing.MiddleStepDelegate(ClickMiddle)));
            ManagerSurfing.AddSurfing(new Surfing(this, "https://wmrfast.com/serfing.php", FirstStep, new Surfing.MiddleStepDelegate(VisitMiddle)));
        }
		public override bool Auth(Auth auth)
        {
            var browserAuth = GetBrowser(0);
            if (browserAuth == null) return false;
            LoadPage("https://wmrfast.com/");
			ImageControlWmrAuth imageConrolWmrAuth = new ImageControlWmrAuth(@"C:/ClickMashine/Settings/Net/WmrFast/WmrFastAuth.h5");
			while (true)
			{
				if (InjectJS(browserAuth, "var but_log = document.querySelector('#logbtn'); if(but_log != null) {but_log.click(); " + (int)StatusJS.Error + ";} else " + (int)StatusJS.OK + ";") == StatusJS.Error)
				{
					Sleep(2);
					InjectJS(browserAuth,
@"document.querySelector('#vhusername').value = '" + auth.Login + @"';
document.querySelector('#vhpass').value = '" + auth.Password + @"';");
					switch (InjectJS(browserAuth,
@"if(document.querySelector(""#anchor"")) " + (int)StatusJS.OK1 + @";
else if(document.querySelector(""#login_cap"")) " + (int)StatusJS.OK + @";
else 'wait_login';"))
					{
						case StatusJS.OK:
							string js =
	@"document.querySelector('#cap_text').value = '" + imageConrolWmrAuth.Predict(GetImgBrowser(browserAuth.MainFrame, "document.querySelector('#login_cap')")) + @"';
document.querySelector('#vhod1').click();";
							eventLoadPage.Reset();
							SendJS(browserAuth, js);
							eventLoadPage.WaitOne();
							Sleep(3);
							break;
						default:
							return false;
					}
				}
				else
					break;
			}
            TakeMoney(browserAuth);
            return true;
		}
		protected override void Initialize()
        {
            imageConrolWmrClick = new ImageControlWmrClick(@"C:/ClickMashine/Settings/Net/WmrFast/WmrFastClick.h5");
            base.Initialize();
        }
        private bool YouTubeMiddle(IBrowser browser)
		{
            if (WaitElement(browser.MainFrame, "document.querySelector(\"#tt\")"))
            {
                if(WaitTime(browser, "vs = true;timer;"))
                {
                    WaitButtonClick(browser, "document.querySelector('a');");
					return true;
                }
            }
            return false;
		}
		private bool ClickMiddle(IBrowser browser)
		{
			if (WaitTime(browser, "counter"))
			{
				if (ValueElement(browser, "counter") != "-1") {
					InjectJS(browser, "counter = 0;flag = 1;");
					Sleep(2);
				}
				for (int i = 0; i < 10; i++)
				{
					string value;
					try
					{
						value = imageConrolWmrClick.Predict(GetImgBrowser(browser.MainFrame, "document.querySelector('#captcha-image')"));
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
						InjectJS(browser, "document.querySelector('#capcha > tbody > tr > td:nth-child(1) > a').click();");
						Sleep(2);
						continue;
					}
					if (value.Length == 3)
					{
						string js =
@"function endClick() {var butRet = document.querySelectorAll('[method=""POST""]');
for (var i = 0; i < butRet.length; i++)
{
	if (butRet[i].querySelector('.submit').value == " + value + @")
	{ butRet[i].querySelector('.submit').click(); return " + (int)StatusJS.OK + @";}
}
return " + (int)StatusJS.Error + @";}endClick();";
						if (InjectJS(browser, js) == StatusJS.OK)
							return true;
					}
					InjectJS(browser, "document.querySelector('#capcha > tbody > tr > td:nth-child(1) > a').click();");
					Sleep(2);
				}
			}
			return false;
		}
		private bool VisitMiddle(IBrowser browser)
		{
			var browserStart = GetBrowser(0);
			if (WaitTime(browserStart, "surf_cl[n-1].getAttribute('timer')"))
				return true;
			return false;
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
        private void TakeMoney(IBrowser browser)
        {
			string ret = SendJSReturn(browser, "document.querySelector('#osn_money').innerText;");
			CM(ret);
            double.TryParse(ret,out double money);
            if (money >= 30)
            {
                LoadPage(browser, "https://wmrfast.com/convert_wm.php");
                string js =
@"var payeer_box = document.querySelector('#pay_payeer');
if(payeer_box != null) payeer_box.click(); 'online';
else 'offline';";
                if (SendJSReturn(browser, js) == "online")
                {
                    eventLoadPage.Reset();
                    if (eventLoadPage.WaitOne(10000))
                    {
                        SendJS(browser,"document.querySelector('#pay_payeer').click();");
						Sleep(10);
                    }
                }
            }
        }
    }
}