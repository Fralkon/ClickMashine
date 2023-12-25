using CefSharp;
using System.Xml.Linq;

namespace ClickMashine
{
    enum SeoClubEnumNN
    {
        апельсинами,
        грибом,
        коровой,
        лошадьми,
        поросятами,
        собакой,
        стулом,
        тиграми,
        цветами,
        яблоками
    }
    class SeoClub : Site
    {
        SeoClubNN nn;
        Surfing YouTube;
        Surfing Click;
        SurfingMail Mail;
        Surfing Visit;
        public SeoClub(Form1 form, Auth auth) : base(form, auth)
        {
            homePage = "https://seoclub.su/";
            Type = EnumTypeSite.SeoClub;
            Surfing.OpenPageDelegate openPage = new Surfing.OpenPageDelegate(OpenPage);
            YouTube = new Surfing(this, openPage, "document.querySelector('#mnu_tblock1 > a:nth-child(2)').href",
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
function FirstStep()
{
	if (n >= surf_cl.length) return " + (int)StatusJS.End + @";
	else
	{
		surf_cl[n].querySelector('span').click(); return " + (int)StatusJS.OK + @";
	}
}
function SecondStep()
{
	var start_ln = surf_cl[n].querySelector('.youtube-button');
	if (start_ln != null) { 
		if(start_ln.innerText != 'Приступить к просмотру') {n++; return " + (int)StatusJS.Continue + @";}
		else {start_ln.querySelector('span').click(); n++; return " + (int)StatusJS.OK + @"; }
	}
	else { return " + (int)StatusJS.Wait + @"; }
}", new Surfing.MiddleStepDelegate(YouTubeMiddle));

            Click = new Surfing(this, openPage, "document.querySelector('#mnu_tblock1 > a:nth-child(4)').href",
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
function SecondStep()
{
	var start_ln = surf_cl[n].querySelector('.start-yes-serf');
	if (start_ln != null) { start_ln.click(); n++; return " + (int)StatusJS.OK + @"; }
	else { return " + (int)StatusJS.Wait + @"; }
}
function FirstStep()
{
	if (n >= surf_cl.length) return " + (int)StatusJS.End + @";
	else
	{	
		if(surf_cl[n].querySelector('[id]')!=null)
			{
				if(surf_cl[n].querySelector('a')==null || surf_cl[n].getBoundingClientRect().height == 0)
					{n++; return " + (int)StatusJS.Continue + @";}
				else {surf_cl[n].querySelector('a').click(); return " + (int)StatusJS.OK + @";}
			}
		else
			{n++;return " + (int)StatusJS.Continue + @";}
	}
}", new Surfing.MiddleStepDelegate(ClickMiddle));

            Visit = new Surfing(this, openPage, "document.querySelector('#mnu_tblock1 > a:nth-child(5)').href",
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
function FirstStep()
{
	if (n >= surf_cl.length) return " + (int)StatusJS.End + @";
	else
	{
		var link = surf_cl[n];
		if(link.querySelectorAll('td')[2]==null || link.getBoundingClientRect().height == 0)
					{n++; return " + (int)StatusJS.Continue + @";}
		else {link.querySelector('a').click(); n++; return " + (int)StatusJS.OK + @";}
	}
}
function SecondStep()
{
return "+(int)StatusJS.OK+";}", new Surfing.MiddleStepDelegate(VisitMiddle));

            Mail = new SurfingMail(this, openPage, "document.querySelector('#mnu_tblock1 > a:nth-child(6)').href",
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 1;
function SecondStep()
{
	var start_ln = surf_cl[n].querySelector('.start-yes-serf');
	if (start_ln != null) { start_ln.click(); n++; return "+(int)StatusJS.OK+ @"; }
	else { return "+(int)StatusJS.Wait+ @"; }
}
function FirstStep()
{
	if (n >= surf_cl.length) return "+(int)StatusJS.End+ @";
	else
	{
		surf_cl[n].querySelector('a').click(); return "+(int)StatusJS.OK+@";
	}
}", new SurfingMail.MailClickDelegate(MailClick), new Surfing.MiddleStepDelegate(ClickMiddle));

            ManagerSurfing.AddSurfing(YouTube);
            ManagerSurfing.AddSurfing(Click);
            ManagerSurfing.AddSurfing(Mail);
            ManagerSurfing.AddSurfing(Visit);
        }
        public override bool Auth(Auth auth)
        {
            var browserAuth = GetBrowser(0);
            if (browserAuth == null) { return false; }
            LoadPage(browserAuth, "https://seoclub.su/login");
            string auth_js = "document.querySelector('input[name=\"username\"]').value = '" + auth.Login + "';" +
                             "document.querySelector('input[name=\"password\"]').value = '" + auth.Password + "';";
            InjectJS(browserAuth, auth_js);
            StatusCaptcha status = OutCaptchaLab(browserAuth,
               nn,
               Enum.GetNames(typeof(SeoClubEnumNN)).ToList(),
               "document.querySelector('.out-capcha-title')",
               "document.querySelector('.out-capcha')",
               "document.querySelectorAll('.out-capcha-lab')",
               5,
               "document.querySelector('.btn')",
               "document.querySelector('.login-error')");
            if (status == StatusCaptcha.OK)
            {
                //string ev = GetMoney(browserAuth, "document.querySelector('#new-money-ballans')");
                //if (ev == "error")
                //    return false;
                siteStripComboBox.Text = StatusSite.online.ToString();
                return true;
            }
            return false;         
        }
        protected override void Initialize()
        {
            nn = new SeoClubNN(@"C:/ClickMashine/Settings/Net/SeoClub.h5");
            base.Initialize();
        }
        private bool YouTubeMiddle(IBrowser browser)
        {
            if(WaitTime(browser, "c = true;  b = true; document.querySelector('#tmr').innerText;"))
            {
                if (!WaitButtonClick(browser, "document.querySelector('.butt-nw');"))
                    Error("Error end youtube watch");
                return true;
            }
            return false;
        }
        private bool ClickMiddle(IBrowser browser)
        {
            IFrame frame = browser.GetFrame("frminfo");
            if (WaitTime(frame, @"b = false; window.top.start = 0; timerWait.innerText;"))
            {
                if (!WaitElement(frame, "document.querySelector('[type=\"range\"]')"))
                {
                    SendJS(frame, "location.replace(\"vlss?view=ok\");");
                    if (!WaitElement(frame, "document.querySelector('[type=\"range\"]')"))
                        return false;
                }
                if (InjectJS(frame,
@"var range = document.querySelector('[type=""range""]');
if (range != null)
{
	range.value = range.max;
	document.querySelector('button').click();
	" + (int)StatusJS.OK + @";
}
else
	" + (int)StatusJS.Error + @";") == StatusJS.OK)
                    return true;
            }
            return false;
        }
        private bool VisitMiddle(IBrowser browser)
        {
            var browserMain = GetBrowser(0);
            if (browserMain != null)
            {
                string element = ValueElement(browserMain, "surf_cl[n-1].querySelectorAll('td')[1].querySelectorAll('div')[1].innerText");
                int pointStart = element.IndexOf("Таймер: ") + 8;
                int pointEnd = element.IndexOf(' ', pointStart);
                int countText = pointEnd - pointStart;
                if (pointStart == -1 || pointEnd == -1 || countText > 0)
                {
                    Sleep(element.Substring(pointStart, pointEnd - pointStart));
                    return true;
                }
            }
            return false;
        }
        private bool MailClick(IBrowser browser)
        {
            string ev = GetMailAnswer(browser.MainFrame, "document.querySelector('#js-popup > div:nth-child(3)')",
                               "document.querySelector('#js-popup > div:nth-child(4)')",
                               "document.querySelectorAll('.mails-otvet-new a')");
            if (ev == "errorMail")
            {
                Random rnd = new Random();
                ev = rnd.Next(0, 2).ToString();
            }
            InjectJS(browser, "document.querySelectorAll('.mails-otvet-new a')[" + ev + "].click();");
            return true;
        }
        private void OpenPage(IBrowser browser, string page)
        {
            LoadPage(ValueElement(browser, page));
        }

        private int ClickSurf()
        {
            int Count = 0;
            var mainBrowser = GetBrowser(0);
            if (mainBrowser == null) return -1;
            LoadPage("https://seoclub.su/");
            LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(2)').href"));
            //AntiBot();
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
	else
	{	
		if(surf_cl[n].querySelector('[id]')!=null)
			{
				if(surf_cl[n].querySelector('a')==null || surf_cl[n].getBoundingClientRect().height == 0)
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
                                IFrame frame = browserSurf.GetFrame("frminfo");
                                ev = SendJSReturn(frame,
@"b = false;
window.top.start = 0;
var timerWait = document.querySelector('.timer');
if (timerWait != null)
	timerWait.innerText;
else 'error_surf';");
                                if (ev != "error")
                                {
                                    Sleep(ev);
                                    if (!WaitElement(frame, "document.querySelector('[type=\"range\"]')"))
                                    {
                                        SendJS(frame, "location.replace(\"vlss?view=ok\");");
                                        if (!WaitElement(frame, "document.querySelector('[type=\"range\"]')"))
                                            break;
                                    }
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
                            }
                            else Error("Error wait browser");
                            break;
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
            IBrowser mainBrowser = GetBrowser(0);
            if (mainBrowser == null) return -1;
            LoadPage("https://seoclub.su/");
            LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(3)').href"));
            //AntiBot();
            string js =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
function click_s()
{
	if (n >= surf_cl.length) return 'end_surf';
	else
	{
		var link = surf_cl[n];
		if(link.querySelectorAll('td')[2]==null || link.getBoundingClientRect().height == 0)
					{n++; return 'continue';}
		else {link.querySelector('a').click(); n++; return link.querySelectorAll('td')[1].querySelectorAll('div')[1].innerText;}
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
                else
                {
                    if (WaitCreateBrowser() != null)
                    {
                        int pointStart = ev.IndexOf("Таймер: ") + 8;
                        int pointEnd = ev.IndexOf(' ', pointStart);
                        int countText = pointEnd - pointStart;
                        if (pointStart == -1 || pointEnd == -1 || countText > 0)
                            Sleep(ev.Substring(pointStart, pointEnd - pointStart));
                        Sleep(2);
                        Count++;
                    }
                }
                Sleep(2);
                CloseСhildBrowser();
                Sleep(2);
            }
            return Count;
        }
        private int YouTubeSurf()
        {
            int Count = 0;
            var mainBrowser = GetBrowser(0);
            if (mainBrowser == null) return -1;
            LoadPage("https://seoclub.su/");
            LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(7)').href"));
            //if (!OutCaptchaLab(mainBrowser, "document.querySelector('.out-capcha')", "document.querySelectorAll('.out-capcha-inp')", "document.querySelector('.btn_big_green').click()"))
            //{
            //    return 0;
            //}            

            string js_links = 
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 0;
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
	else { return 'wait'; }
}";
            SendJS(mainBrowser, js_links);
            while (true)
            {
                eventBrowserCreated.Reset();
                string ev = SendJSReturn(mainBrowser, "click_s();");
                if (ev == "end_surf")
                {
                    break;
                }
                else if (ev == "click")
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ev = SendJSReturn(mainBrowser, "surf();");
                        if (ev == "wait")
                            Sleep(1);
                        else if (ev == "surf")
                        {
                            var browserYouTube = WaitCreateBrowser();
                            if (browserYouTube != null){
                                
                            }
                            else if (ev == "sec_wait")
                                Sleep(1);
                            else if (ev == "continue")
                                break;
                        }
                    }
                }
                else Error("Ошибка блять");
                CloseСhildBrowser();
                Sleep(1);
            }
            return Count;
        }
        private int MailSurf()
        {
            int Count = 0;
            var mainBrowser = GetBrowser(0);
            if (mainBrowser == null) return -1;
            LoadPage("https://seoclub.su/");
            LoadPage(SendJSReturn(mainBrowser, "document.querySelector('#mnu_tblock1 > a:nth-child(4)').href"));
            string js =
@"var surf_cl = document.querySelectorAll('.work-serf');var n = 1;
function surf()
{
	var start_ln = surf_cl[n].querySelector('.start-yes-serf');
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
            SendJS(mainBrowser, js);
            while (true)
            {
                eventBrowserCreated.Reset();
                string ev = SendJSReturn(mainBrowser, "click_s();");
                if (ev == "end_surf")
                    return Count;
                else if (ev == "continue")
                    continue;
                else if (ev == "click")
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ev = SendJSReturn(mainBrowser, "surf();");
                        if (ev == "wait")
                            Sleep(1);
                        else if (ev == "surf")
                        {
                            ev = GetMailAnswer(mainBrowser.MainFrame, "document.querySelector('#js-popup > div:nth-child(3)')",
                                "document.querySelector('#js-popup > div:nth-child(4)')",
                                "document.querySelectorAll('.mails-otvet-new a')");
                            if (ev == "errorMail")
                            {
                                Random rnd = new Random();
                                ev = rnd.Next(0, 2).ToString();
                            }
                            SendJS(0, "document.querySelectorAll('.mails-otvet-new a')[" + ev + "].click();");
                            var browser = WaitCreateBrowser();
                            if (browser != null) {
                                ev = SendJSReturn(browser,
@"b = false;
window.top.start = 0;
var timer1 = document.querySelector('.timer');
if (timer1 != null)
	return timer1.innerText;
else 'error_surf';");
                                if (ev != "error")
                                {
                                    Sleep(ev);
                                    if (!WaitElement(browser, "document.querySelector('[type=\"range\"]')"))
                                    {
                                        SendJS(browser, "location.replace(\"vlss?view=ok\");");
                                        if (!WaitElement(browser, "document.querySelector('[type=\"range\"]')"))
                                            break;
                                    }
                                    SendJSReturn(browser,
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
                            }
                        }
                    }
                }
                Sleep(2);
                CloseСhildBrowser();
            }
        }
        private void AntiBot(IBrowser browser)
        {
            string jsAntiBot =
@"var captha_lab = document.querySelectorAll('.out-capcha-lab');
if(captha_lab.length != 0){
    'captcha';
}
else 'ok';";
            string evAntiBot = SendJSReturn(browser, jsAntiBot);
            if (evAntiBot == "ok")
                return;
            else
            {
                string answer_telebot = SendQuestion(GetImgBrowser(browser.MainFrame, "document.querySelector('.out-capcha')"), "");

                jsAntiBot = "";
                foreach (char ch in answer_telebot)
                    jsAntiBot += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
                SendJS(browser, jsAntiBot);
            }
        }
        private bool AuthorizationAntiBot(IBrowser browser)
        {
            SeoClubNN nn = new SeoClubNN(@"C:/Users/Boyarkin/Desktop/SeoClub.h5");
            BoundObject boundObject = new BoundObject();
            for (int i = 0; i < 5; i++)
            {
                string jsAntiBot =
    @"var captha_lab = document.querySelectorAll('.out-capcha-lab');
if(captha_lab.length != 0){
    'captcha';
}
else 'ok';";
                string evAntiBot = SendJSReturn(browser, jsAntiBot);
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
                    SendJS(browser, "document.querySelectorAll('.out-capcha-lab').forEach((element) => element.style.border = '0px');");
                    Sleep(1);
                    SeoClubEnumNN? enumNN = null;
                    string nameImage = SendJSReturn(browser, "document.querySelector('.out-capcha-title').innerText;");
                    foreach (string item in Enum.GetNames(typeof(SeoClubEnumNN)))
                    {
                        if (nameImage.IndexOf(item) != -1)
                        {
                            enumNN = Enum.Parse<SeoClubEnumNN>(item);
                            break;
                        }
                    }
                    if (enumNN != null)
                    {
                        List<(Bitmap, PredictNN)> imageHistoryPredict = new List<(Bitmap, PredictNN)>();
                        for (int j = 0; j < 5; j++)
                        {
                            Bitmap image = GetImgBrowser(browser.MainFrame, "document.querySelectorAll('.out-capcha-lab')[" + j.ToString() + "]");
                            PredictNN predict = nn.Predict(image);
                            imageHistoryPredict.Add((image, predict));
                            if (enumNN == (SeoClubEnumNN)predict.Num)
                                SendJS(browser, "document.querySelectorAll('.out-capcha-inp')[" + j + "].checked = true;");
                        }
                        WaitChangeElement(browser, boundObject, "document.querySelector('.login-error')");
                        boundObject.ResetEvent();
                        SendJS(browser, "document.querySelector('.btn').click();");
                        string ev = boundObject.GetValue();
                        if (ev == "error")
                        {
                            //SaveHistoryCaptcha(imageHistoryPredict, enumNN);
                        }
                        else if (ev == "Нужно подтвердить, что Вы не робот!")
                        {
                            //SaveHistoryCaptcha(imageHistoryPredict, enumNN);
                        }
                        else if (ev == "Ваш аккаунт заблокирован")
                        {
                            AccountBlock();
                            return false;
                        }
                        else
                            return true;
                        //Bitmap img = GetImgBrowser(browsers[0].MainFrame, "document.querySelector('.out-capcha')");
                        //string answer_telebot = SendQuestion(img, "");
                        //if (answer_telebot == "")
                        //	jsAntiBot = "";
                        //foreach (char ch in answer_telebot)
                        //	jsAntiBot += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
                        //jsAntiBot += "document.querySelector('.btn_big_green').click();";
                    }
                    SendJS(browser, jsAntiBot);
                    Sleep(10);
                }
            }
            Error("Ошибка ввода капчи");
            return false;
        }
        private void TrainBD()
        {
            var browser = GetBrowser(0);
            LoadPage("https://seoclub.su/login");
            string path = @"C:\ClickMashine\Settings\Image\" + Type.ToString() + @"\";
            for (int i = 0; i < 100; i++)
            {
                string name = SendJSReturn(browser.MainFrame, "document.querySelector('.out-capcha-title').innerText");
                string[] name_item = name.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Directory.CreateDirectory(path + name_item[name_item.Length - 1] + @"\");

                SendJS(0, "document.querySelectorAll('.out-capcha-lab').forEach((element) => element.style.border = '0px');");
                Sleep(1);
                for (int j = 0; j < 5; j++)
                {

                    //PredictNN predict = nn.Predict(GetImgBrowser(Browsers[0].MainFrame, "document.querySelectorAll('.out-capcha-lab')[" + j.ToString() + "]"));
                    //foreach (var v in predict.Tensor.numpy())
                    //{
                    //    foreach (var v2 in v)
                    //        Console.WriteLine(v2.ToString());
                    //}
                    //Console.WriteLine((ProfiCentrEnumNN)predict.Num);
                    //Console.ReadLine();

                    GetImgBrowser(browser.MainFrame, "document.querySelectorAll('.out-capcha-lab')[" + j.ToString() + "]")
                        .Save(path + new DirectoryInfo(path).GetFiles().Length.ToString() + ".png");
                }
                SendJS(0, "document.querySelector('.out-reload').click();");
                Sleep(2);
            }
        }
    }
}