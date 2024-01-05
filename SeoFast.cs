using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CefSharp;
using CefSharp.DevTools.Page;
using CefSharp.WinForms;
using Microsoft.VisualBasic;

namespace ClickMashine
{
    enum SeoFastEnumNN
    {
        автомобилями,
        апельсинами,
        бантиками,
        велосипедами,
        клавиатурами,
        котятами,
        лампочками,
        лошадьми,
        мышками,
        носками,
        пандой,
        поездами,
        поросятами,
        самолетами,
        слонами,
        собаками,
        стульями,
        телефонами,
        тиграми,
        цветами,
        чайниками,
        экскаваторами,
        яблоками
    }
    class YouTubeSeoFastSurfing : Surfing
    {
        public YouTubeSeoFastSurfing(Site site, string page, string firstStep) : base()
        {
            Site = site;
            Page = page;
            FirstStep = firstStep;
        }
        public override bool Surf(int Wait = 5)
        {
            try
            {
                var browser = Site.GetBrowser(0);
                if (browser == null)
                    return false;
                Site.LoadPage(browser, Page);
                if (AntiBot != null)
                    if (!AntiBot(browser))
                        return false;
                Site.SendJS(browser, FirstStep);
                List<Task> youtubeWatch = new List<Task>();
                bool f = true;
                do
                {
                    Site.eventBrowserCreated.Reset();
                    switch (Site.InjectJS(browser, "FirstStep();"))
                    {
                        case StatusJS.End:
                            f = false;
                            break;
                        case StatusJS.Continue:
                            break;
                        case StatusJS.OK:
                            switch (Site.FunctionWait(browser, "SecondStep();"))
                            {
                                case StatusJS.OK:
                                    {
                                        var browserSurf1 = Site.WaitCreateBrowser();
                                        if (browserSurf1 == null)
                                        {
                                            Error++;
                                            break;
                                        }
                                        try
                                        {
                                            youtubeWatch.Add(YouTubeWatch(browserSurf1));
                                            Site.Sleep(10);
                                        }
                                        catch (Exception ex)
                                        {
                                            Site.Error("Error watch youtube task." + ex.Message);
                                            browserSurf1.GetHost().CloseBrowser(true);
                                        }
                                        break;
                                    }
                            }
                            break;
                        case StatusJS.OK1:
                            var browserSurf = Site.WaitCreateBrowser();
                            if (browserSurf == null)
                            {
                                Error++;
                                break;
                            }
                            try
                            {
                                youtubeWatch.Add(YouTubeWatch(browserSurf));
                                Site.Sleep(10);
                            }
                            catch (Exception ex)
                            {
                                Site.Error("Error watch youtube task." + ex.Message);
                                browserSurf.GetHost().CloseBrowser(true);
                            }
                            break;
                        case StatusJS.Error:
                            Error++;
                            break;
                        default:
                            throw new Exception($"Error StatusJS");


                    }
                    Site.Sleep(1);
                }
                while (f);

                if (!Task.WaitAll(youtubeWatch.ToArray(), 20000))
                    Error++;
            }
            catch (Exception ex)
            {
                Site.Error(ex.Message);
                return false;
            }
            Site.CloseСhildBrowser();
            return true;
        }
        private async Task YouTubeWatch(IBrowser browserYouTube)
        {
            await Task.Run(() =>
            {
                string js =
@"var timeYouRube = 0;
if(rutube == '0')
{
    b = true;
    var timer_youtube = document.querySelector('#tmr');
    if (timer_youtube != null) { timeYouRube = parseInt(timer_youtube.innerText); "+(int)StatusJS.OK+@"}
    else " + (int)StatusJS.Error + @";
}
else {
    timer();
    if(timer_v != null){
        timeYouRube = timer_v;
    " + (int)StatusJS.OK+@"
    }
    else " + (int)StatusJS.Error + @";
}";
                Site.Sleep(2);
                if (Site.InjectJS(browserYouTube, js) == StatusJS.OK)
                {
                    Site.WaitTime(browserYouTube, "timeYouRube");
                    string jsWaitYouTube =
@"function WaitEnd(){
if(document.querySelector('#capcha-tr-block').innerText.indexOf(""засчитан"") != -1)
    return " + (int)StatusJS.OK+ @";
else
    return " + (int)StatusJS.Wait+@";}";
                    Site.form.FocusTab(browserYouTube);
                    if (Site.FunctionWait(browserYouTube, "WaitEnd();", jsWaitYouTube, 10) == StatusJS.OK)
                        Count++;
                    else
                        Error++;
                }
                else
                    Error++;
                browserYouTube.GetHost().CloseBrowser(false);
            });
        }
    }
    class SeoFast : Site
    {
        SeoFastNN nn;
        Surfing Click;
        Surfing Visit;
        YouTubeSeoFastSurfing RuTube;
        YouTubeSeoFastSurfing ExpensiveTube;
        YouTubeSeoFastSurfing SimpeTube;
        YouTubeSeoFastSurfing BonusTube;
        SurfingMail Mail;
        public SeoFast(Form1 form, Auth auth) : base(form, auth)
        {
            Surfing.AntiBotDelegate AntiBotDelegate = new Surfing.AntiBotDelegate(AntiBotImage);
            homePage = "https://seo-fast.ru/";
            Type = EnumTypeSite.SeoFast;
            string clickFirstStep = 
@"var surf_cl = document.querySelectorAll('a.surf_ckick');var n = 1; 
function FirstStep()
{
    if (n >= surf_cl.length) return " + (int)StatusJS.End + @";
    else if (surf_cl[n].innerText == '')
    {
        n++; return " + (int)StatusJS.Continue + @";
    }
    else
    {
        surf_cl[n].click(); n++; return " + (int)StatusJS.OK + @";
    }
}
function SecondStep()
{
    var start_ln = document.querySelectorAll('.start_link_a');
    if (start_ln.length != 0) { start_ln[0].click(); return " + (int)StatusJS.OK + @"; }
    else { return " + (int)StatusJS.Wait + @"; }
}";
            Click = new Surfing(this,"https://seo-fast.ru/work_surfing",clickFirstStep,new Surfing.MiddleStepDelegate(ClickMiddleStep))
            { AntiBot = AntiBotDelegate };

            string FirstStepJSYouTube =
@"var surf_cl = document.querySelectorAll('a.surf_ckick');var n = 0;
var youtube_premium = null;
function FirstStep()
{
    if (n >= surf_cl.length) return " + (int)StatusJS.End + @";
    else if (surf_cl[n].innerText == '')
    {
        n++; return " + (int)StatusJS.Continue + @";
    }
else{ 
    youtube_premium = surf_cl[n].parentElement.parentElement.parentElement;
    if(youtube_premium == null){n++;return " + (int)StatusJS.Continue + @";}
    if(youtube_premium.id.indexOf('v123') != -1){
        surf_cl[n].click(); 
        n++; 
        return " + (int)StatusJS.OK + @";
    }       
    else{
        surf_cl[n].click(); 
        n++; 
        return " + (int)StatusJS.OK1 + @"; 
        }
    }
}
function SecondStep()
{
    var start_ln = youtube_premium.querySelector('.start_link_youtube');
	if (start_ln != null)
    { 
		if(start_ln.innerText != 'Приступить к просмотру') {n++; return " + (int)StatusJS.Continue + @";}
		else {start_ln.click(); n++; return " + (int)StatusJS.OK + @"; }
	}
	else { return " + (int)StatusJS.Wait + @"; }
}";

            RuTube = new YouTubeSeoFastSurfing(this, "https://seo-fast.ru/work_youtube?rutube_video", FirstStepJSYouTube)
            { AntiBot = AntiBotDelegate };
            SimpeTube = new YouTubeSeoFastSurfing(this, "https://seo-fast.ru/work_youtube?youtube_video_simple", FirstStepJSYouTube)
            { AntiBot = AntiBotDelegate };
            ExpensiveTube = new YouTubeSeoFastSurfing(this, "https://seo-fast.ru/work_youtube?youtube_expensive", FirstStepJSYouTube)
            { AntiBot = AntiBotDelegate };
            BonusTube = new YouTubeSeoFastSurfing(this, "https://seo-fast.ru/work_youtube?youtube_video_bonus", FirstStepJSYouTube)
            { AntiBot = AntiBotDelegate };
            string mailFirstStep =
@"var surf_cl = document.querySelectorAll('a.surf_ckick');var n = 0;
function SecondStep()
{
    var start_ln = document.querySelectorAll('.start_link_a');
    if (start_ln.length != 0) { start_ln[0].click(); return " + (int)StatusJS.OK + @";}
    else { return " + (int)StatusJS.Wait + @"; }
}
function FirstStep()
{
    if (n >= surf_cl.length) return " + (int)StatusJS.End + @";
    else if (surf_cl[n].innerText == '')
    {
        n++; 
        return " + (int)StatusJS.Continue + @";
    }
    else
    {
        surf_cl[n].click(); 
        return " + (int)StatusJS.OK + @";
    }
}";
            Mail = new SurfingMail(this, "https://seo-fast.ru/work_mails", mailFirstStep, new SurfingMail.MailClickDelegate(MailCLick), new Surfing.MiddleStepDelegate(MailMiddleClick))
            { AntiBot = AntiBotDelegate };
            string visitFirstStep =
@"var surf_cl = document.querySelectorAll('a.surf_ckick');var n = 1;
function FirstStep()
{
    if (n >= surf_cl.length) return " + (int)StatusJS.End + @";
    else if (surf_cl[n].innerText == '')
    {
        n++; return " + (int)StatusJS.Continue + @";
    }
    else
    {
        surf_cl[n].click(); n++; return " + (int)StatusJS.OK + @";
    }
}
function SecondStep()
{
    var start_ln = document.querySelectorAll('.start_link_a');
    if (start_ln.length != 0) { start_ln[0].click(); return " + (int)StatusJS.OK + @"; }
    else { return " + (int)StatusJS.Wait + @"; }
}";
            Visit = new Surfing(this, "https://seo-fast.ru/work_transitions", visitFirstStep, new Surfing.MiddleStepDelegate(VisitMiddle))
            { AntiBot = AntiBotDelegate };

            ManagerSurfing.AddSurfing(Visit);
            ManagerSurfing.AddSurfing(Click);
            ManagerSurfing.AddSurfing(Mail);
            ManagerSurfing.AddSurfing(RuTube);
            ManagerSurfing.AddSurfing(ExpensiveTube);
            ManagerSurfing.AddSurfing(SimpeTube);
            ManagerSurfing.AddSurfing(BonusTube);
        }
        protected override void Initialize()
        {
            nn = new SeoFastNN(@"C:/ClickMashine/Settings/Net/SeoFast.h5");
            base.Initialize();
        }
        public override bool Auth(Auth auth)
        {
            var browser = GetBrowser(0);
            if (browser == null)
                return false;
            LoadPage(browser, "https://seo-fast.ru/main");
            Sleep(5);
            eventLoadPage.Reset();

            switch (InjectJS(browser,
@"var l_b = document.querySelector('.loginbutton');
if (l_b != null){
    l_b.click();
    " + (int)StatusJS.Error + @";}
else {" + (int)StatusJS.OK + @";}"))
            {
                case StatusJS.Error:
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Sleep(5);
                            string js = $"document.querySelector('#logusername').value = '{auth.Login}';" +
                                    $"document.querySelector('#logpassword').value = '{auth.Password}';";
                            SendJS(browser, js);
                            //BoundObject boundObject = new BoundObject();
                            //WaitChangeElement(browser, boundObject, "document.querySelector('.result_echo')");
                            OutCaptchaLab1(browser,
                                nn,
                                Enum.GetNames(typeof(SeoFastEnumNN)).ToList(),
                                "document.querySelector('.out-capcha-title').querySelector('span')",
                                "document.querySelector('.out-capcha')",
                                "document.querySelectorAll('.out-capcha-lab')",
                                5,
                                "document.querySelector('.sf_button')");
                            if (WaitElement(browser, "document.querySelector('.main_balance')"))
                            {
                                //InjectJS(browser, @"if(document.querySelector('.popup2').style.display != 'none'){document.querySelector('.popup2-content .sf_button').click();}");
                                Sleep(2);
                                return true;
                            }
                            else
                            {
                                //Console.WriteLine(boundObject.GetValue());
                                eventLoadPage.Reset();
                                browser.Reload();
                                eventLoadPage.WaitOne(5000);
                            }
                        }
                        return false;
                    }
            }
            return true;
        }
        private bool ClickMiddleStep(IBrowser browser)
        {
            string js =
@"function clace(){
if (document.querySelector('#stop_clic').value == '1') { click_site(); }
    setTimeout(clace, 1000);
}
function go(){
    isa = 0;
    if (isc > 0) clace();
    return " + (int)StatusJS.OK + @";
}
go();";
            if (InjectJS(browser.MainFrame, js) == StatusJS.OK)
            {
                WaitTime(browser, "document.querySelector('#time').innerText");
                if (!WaitButtonClick(browser, "document.querySelector('.button_s');"))
                {
                    js =
@"$.ajax({
	type: 'POST', url: domail_s+'/ajax/ajax_surfing2.php',  
	data: { 'sf' : 'load_captcha_sf', 'v_surfing_lc' : v_surfing_lc, 'id_rek' : id_rek, 'type' : 'surf' }, 
    beforeSend: function(){ $('#timer_lo').remove(); $('#timer_lo_error').remove(); $('#code').css({display: 'block'}); },
    success: function(data){ localStorage.setItem('id_rek_l', id_rek); $('#code').html(data); }
});";
                    InjectJS(browser, js);
                    if (WaitButtonClick(browser, "document.querySelector('.button_s');"))
                        return true;
                }
            }
            else
                return true;
            return false;
        }
        private bool MailCLick(IBrowser browser)
        {
            string ev = SendJSReturn(browser, @"surf_cl[n++].querySelector("".desctext"").innerText;");
            ev = Regex.Replace(ev, @"\D", "", RegexOptions.IgnoreCase);
            ev = GetMailAnswer(browser.MainFrame, $"document.querySelector('#window_mail{ev}> tbody > tr:nth-child(2) > td')",
               "document.querySelector('.question_am')",
               "document.querySelectorAll('.button_a_m')");
            if (ev == "errorMail")
            {
                Random rnd = new Random();
                ev = rnd.Next(0, 3).ToString();
            }
            string js =
@"function WaitReturn(){
    var start_surf_mail = document.querySelector('.start_link_a');
    if (start_surf_mail != null)
    {
        start_surf_mail.click(); return "+(int)StatusJS.OK+ @";
    }    
    else { return "+(int)StatusJS.Wait+@"; }
}";
            InjectJS(browser, "document.querySelectorAll('.button_a_m')[" + ev + @"].click();");
            if (FunctionWait(browser.MainFrame, "WaitReturn();", js)==StatusJS.OK)
                return true;
            return false;
        }
        private bool MailMiddleClick(IBrowser browser)
        {
            string js = @"isa = 0;";
            InjectJS(browser, js);
            WaitTime(browser, "document.querySelector('#time').innerText;");
            if (!WaitButtonClick(browser.MainFrame, "document.querySelector('.button_s');"))
            {
                string ev = @"
$.ajax({
	type: 'POST', url: domail_s+'/ajax/ajax_surfing2.php',  
	data: { 'sf' : 'load_captcha_sf', 'v_surfing_lc' : v_surfing_lc, 'id_rek' : id_rek, 'type' : 'surf' }, 
    beforeSend: function(){ $('#timer_lo').remove(); $('#timer_lo_error').remove(); $('#code').css({display: 'block'}); },
    success: function(data){ localStorage.setItem('id_rek_l', id_rek); $('#code').html(data); }
});";
                InjectJS(browser, ev);
                if (WaitButtonClick(browser, "document.querySelector('.button_s');"))
                    return true;
            }
            else
                return true;
            return false;
        }
        private bool AntiBotImage(IBrowser browser)
        {
            //captcha_new
            string js = @"var img_captcha = document.querySelector('.out-capcha');
if(img_captcha != null)
    'antiBot';
else 'notAntiBot';";
            if (SendJSReturn(browser.MainFrame, js) != "antiBot")
                return true;
            //BoundObject boundObject = new BoundObject();
            //WaitChangeElement(browser, boundObject, "document.querySelector('.result_echo')");
            for (int i = 0; i < 10; i++)
            {
                var history = OutCaptchaLab1(browser,
                    nn,
                    Enum.GetNames(typeof(SeoFastEnumNN)).ToList(),
                    "document.querySelector('.out-capcha-title').querySelector('span')",
                    "document.querySelector('.out-capcha')",
                    "document.querySelectorAll('.out-capcha-lab')",
                    5,
                    "document.querySelector('.sf_button')");
                Sleep(5);
                if (SendJSReturn(browser.MainFrame, js) != "antiBot")
                    return true;
                SaveHistoryCaptcha1(history, Enum.GetNames(typeof(SeoFastEnumNN)).ToList());
                SendJS(browser, "document.querySelector('.fa-refresh').click();");
                Sleep(4);
            }
            return false;
        }
        private bool YouTubeMiddle(IBrowser browser)
        {
            //{
            //    IBrowser? browserYouTube = WaitCreateBrowser();
            //    if (browserYouTube == null)
            //        continue;
            //    try
            //    {
            //        youtubeWatch.Add(YouTubeWatch(browserYouTube));
            //    }
            //    catch (Exception ex)
            //    {
            //        Error("Error watch youtube task." + ex.Message);
            //        browserYouTube.GetHost().CloseBrowser(true);
            //    }
            //    count++;
            //    Sleep(5);
            //}
            //else if (ev == "surf_premium")
            //{
            //    for (int i = 0; i < 10; i++)
            //    {
            //        ev = SendJSReturn(0, "surf();");
            //        if (ev == "surf")
            //        {
            //            IBrowser? browserYouTube = WaitCreateBrowser();
            //            if (browserYouTube == null)
            //                continue;
            //            try
            //            {
            //                youtubeWatch.Add(YouTubeWatch(browserYouTube));
            //            }
            //            catch (Exception ex)
            //            {
            //                Error("Error watch youtube task." + ex.Message);
            //                browserYouTube.GetHost().CloseBrowser(true);
            //            }
            //            count++;
            //            Sleep(5);
            //            break;
            //        }
            //        else if (ev == "sec_wait")
            //            Sleep(1);
            //        else if (ev == "continue")
            //            break;
            //        if (i == 9)
            //        {
            //            SendJS(0, "n++;");
            //            break;
            //        }
            //    }
            //}
            //Sleep(1);
            //}
            //Task.WaitAll(youtubeWatch.ToArray(), 20000);
            //CloseСhildBrowser();
            return false;
        }
        private bool VisitMiddle(IBrowser browser)
        {
            string js =
@"function w() {
    if(counter == 0) {
	    return " + (int)StatusJS.OK + @";
    }
    else return " + (int)StatusJS.Wait + @";
};
window.onfocus = function () {
	$.ajax({ 
		type: ""POST"", url: ""https://seo-fast.ru/site_transitions/ajax/ajax_transitions_pay.php"", data: { 'sf' : 'viewing_transitions', 'id' : id, 'timer' : timer, 'pam' : '2' }, 
		success: function(data){
			if(data == '1'){ close(); }else{ $('#start_transitions').html(data); }
		}
	});	
    $.cookie('win'+id, 'false', { expires: -1, path: '/', });
};";
            InjectJS(browser, js);
            if (FunctionWait(browser, "w();") != StatusJS.OK)
                return false;
            WaitTime(browser, "timer");
            var browserVisit2 = GetBrowser(2);
            if (browserVisit2 != null)
            {
                Sleep(2);
                browserVisit2.CloseBrowser(false);
                form.FocusTab(browser);
                Sleep(2);
                return true;
            }
            return false;
        }
        private void TakeMoney(IBrowser browser)
        {
            int money = int.Parse(SendJSReturn(browser, "document.querySelector('#ajax_load > div > div:nth-child(3) > span > span:nth-child(1)').innerText"));
            if (money >= 30)
            {
                LoadPage(browser, "https://seo-fast.ru/payment_user");
                string js =
@"var payeer_box = document.querySelector('#echoall > table > tbody > tr:nth-child(2) > td:nth-child(2) a');
if(payeer_box != null) payeer_box.click(); 'online';
else 'offline';";
                if (SendJSReturn(browser, js) == "online")
                {
                    eventLoadPage.Reset();
                    if (eventLoadPage.WaitOne(10000))
                    {
                        SendJS(browser, "all_money();i_not_robot();payment_money();");
                    }
                }
            }
        }
        private void CheckCaptcha()
        {
            Sleep(3);
            var browserAuth = GetBrowser(0);
            if (browserAuth == null) return;
            string ev = SendJSReturn(0,
@"var check = document.querySelector('.h-captcha');
if (check != null) { 'captcha'; }
else {'end';}");
            if (ev == "captcha")
            {
                List<long> list_id = browserAuth.GetFrameIdentifiers();
                IFrame? frameCheckbox = null, frameChallenge = null;
                for (int i = 0; i < list_id.Count; i++)
                {
                    IFrame frame = browserAuth.GetFrame(list_id[i]);
                    if (frame.Url.IndexOf("challenge") != -1)
                        frameChallenge = frame;
                    else if (frame.Url.IndexOf("checkbox") != -1)
                        frameCheckbox = frame;
                }
                if (frameChallenge == null || frameCheckbox == null)
                    throw new Exception("Ошибка капчи");
                string js = "var check_box = document.querySelector('#checkbox'); if(check_box==null){ 'error'; } else {" +
                        "check_box.click(); 'ok'; }";
                ev = SendJSReturn(frameCheckbox, js);
                if (ev == "ok")
                {
                    WaitElement(frameChallenge, "document.querySelector('.challenge')");
                    while (true)
                    {
                        Sleep(1);
                        Bitmap img = GetImgBrowser(browserAuth.MainFrame, "document.querySelector('[title=\"Main content of the hCaptcha challenge\"]')");
                        string answerTelebot = SendQuestion(img, "");
                        js = "var items = document.querySelectorAll('.task-image');";
                        string jsCaptch =
    @"var buttonEnd = document.querySelector('.button-submit');
if (buttonEnd.innerText == 'Дальше')
{
    buttonEnd.click(); 'next';
}
else if (buttonEnd.innerText == 'Готово')
{
    buttonEnd.click(); 'end';
}
else 'skip';";
                        foreach (var ch in answerTelebot)
                        {
                            js += "items[" + ch + "].click();";
                        }
                        SendJS(frameChallenge, js);
                        ev = SendJSReturn(frameChallenge, jsCaptch);
                        if (ev == "end")
                        {
                            Sleep(2);
                            ev = SendJSReturn(0,
    @"var chal = document.querySelector('.challenge');
if(chal != null) 'error_chal';
else 'ok';");
                            if (ev == "error_chal")
                                continue;
                            else
                            {
                                SendJS(0, "sf_hcaptcha();");
                                Sleep(2);
                                break;
                            }
                        }
                        else if (ev == "skip")
                        {
                            MessageBox.Show("ЕРРРОР БЛЯ");
                        }
                        Sleep(1);
                    }
                }
            }
            ev = SendJSReturn(0,
@"var capt = document.querySelector('.out-capcha');
if(capt != null) 'captcha';
else 'end';");
            if (ev == "captcha")
            {
                while (true)
                {
                    Bitmap image = GetImgBrowser(browserAuth.MainFrame, "document.querySelector('.out-capcha')");
                    string answer_telebot = SendQuestion(image, "");

                    string auth_js = "";
                    foreach (char ch in answer_telebot)
                        auth_js += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
                    auth_js += "document.querySelector('.sf_button').click();";
                    eventLoadPage.Reset();
                    SendJS(0, auth_js);
                    Sleep(2);
                    string js =
@"var echoError = document.querySelector('.echo_error');
if(echoError != null) 'echoError';
else 'ok';";
                    ev = SendJSReturn(0, js);
                    if (ev == "ok")
                        break;
                    else
                    {
                        eventLoadPage.Reset();
                        browserAuth.Reload();
                        eventLoadPage.WaitOne();
                    }
                }
            }
        }
        private int ClickSurf()
        {
            int Count = 0;
            var browserMain = GetBrowser(0);
            if (browserMain == null) return -1;
            LoadPage(browserMain, "https://seo-fast.ru/work_surfing?go");
            Sleep(2);
            if (!AntiBotImage(browserMain))
                return -1;
            string js =
@"var surf_cl = document.querySelectorAll('a.surf_ckick');var n = 1;
function surf()
{
    var start_ln = document.querySelectorAll('.start_link_a');
    if (start_ln.length != 0) { start_ln[0].click(); return 'click'; }
    else { return 'wait'; }
}
function click_s()
{
    if (n >= surf_cl.length) return 'end_surf';
    else if (surf_cl[n].innerText == '')
    {
        n++; return 'continue';
    }
    else
    {
        surf_cl[n].click(); n++; return 'surf';
    }
}";
            SendJS(browserMain, js);
            while (true)
            {
                string ev = SendJSReturn(browserMain, "click_s();");
                if (ev == "end_surf")
                    break;
                else if (ev == "continue")
                    continue;
                else if (ev == "surf")
                {
                    for (int i = 0; i < 5; i++)
                    {
                        ev = SendJSReturn(browserMain, "surf();");
                        if (ev == "wait")
                            Sleep(1);
                        else if (ev == "click")
                        {
                            var browserSurf = WaitCreateBrowser();
                            if (browserSurf == null)
                                continue;
                            js =
@"function clace(){
if (document.querySelector('#stop_clic').value == '1') { click_site(); }
    setTimeout(clace, 1000);
}
function go(){
    isa = 0;
    if (isc > 0) clace();
    return document.querySelector('#time').innerText;
}
go();";
                            ev = SendJSReturn(browserSurf.MainFrame, js);
                            Sleep(ev);
                            if (!WaitButtonClick(browserSurf.MainFrame, "document.querySelector('.button_s');"))
                            {
                                ev =
@"$.ajax({
	type: 'POST', url: domail_s+'/ajax/ajax_surfing2.php',  
	data: { 'sf' : 'load_captcha_sf', 'v_surfing_lc' : v_surfing_lc, 'id_rek' : id_rek, 'type' : 'surf' }, 
    beforeSend: function(){ $('#timer_lo').remove(); $('#timer_lo_error').remove(); $('#code').css({display: 'block'}); },
    success: function(data){ localStorage.setItem('id_rek_l', id_rek); $('#code').html(data); }
});";
                                SendJS(browserSurf.MainFrame, ev);
                                if (WaitButtonClick(browserSurf.MainFrame, "document.querySelector('.button_s');"))
                                {
                                    Count++;
                                }
                            }
                            else
                            {
                                Count++;
                            }
                            Sleep(2);
                            break;
                        }
                    }
                }
                CloseСhildBrowser();
            }
            return Count;           
        }
        private int VisitSurf()
        {
            int Count = 0;
            var browserMain = GetBrowser(0);
            if (browserMain == null) return -1;
            LoadPage(0, "https://seo-fast.ru/work_transitions");
            Sleep(2);
            if (!AntiBotImage(browserMain))
                return -1;
            string js =
           @"var surf_cl = document.querySelectorAll('a.surf_ckick');var n = 1;
function surf()
{
    var start_ln = document.querySelectorAll('.start_link_a');
    if (start_ln.length != 0) { start_ln[0].click(); return 'click'; }
    else { return 'wait'; }
}
function click_s()
{
    if (n >= surf_cl.length) return 'end_surf';
    else if (surf_cl[n].innerText == '')
    {
        n++; return 'continue';
    }
    else
    {
        surf_cl[n].click(); n++; return 'surf';
    }
}";
            SendJS(browserMain, js);
            while (true)
            {
                string ev = SendJSReturn(browserMain, "click_s();");
                if (ev == "end_surf")
                    break;
                else if (ev == "continue")
                    continue;
                else if (ev == "surf")
                {
                    for (int i = 0; i < 5; i++)
                    {
                        ev = SendJSReturn(browserMain, "surf();");
                        if (ev == "wait")
                            Sleep(1);
                        else if (ev == "click")
                        {
                            var browserVisit = WaitCreateBrowser();
                            if (browserVisit != null)
                            {
                                js =
    @"function w() {
if(counter == 0) {
	return timer.toString();
}
else { return 'wait' }};";
                                ev = WaitFunction(browserVisit.MainFrame, "w();", js);
                                if (ev != "errorWait")
                                {
                                    Sleep(ev);
                                    Sleep(1);
                                    var browserVisit2 = WaitCreateBrowser();
                                    if (browserVisit2 != null)
                                    {
                                        Sleep(SendJSReturn(browserVisit, "document.title.toString();"));
                                        Sleep(1);
                                        CloseBrowser(browserVisit2);
                                        Count++;
                                        Sleep(1);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                CloseСhildBrowser();
                Sleep(1);
            }
            return Count;
        }
        private int MailSurf()
        {
            int Count = 0;
            var browserMain = GetBrowser(0);
            if (browserMain == null) return -1;
            LoadPage(0, "https://seo-fast.ru/work_mails");
            Sleep(2);
            if (!AntiBotImage(browserMain))
                return -1;
            string js =
@"var surf_cl = document.querySelectorAll('a.surf_ckick');var n = 0;
function surf()
{
    var start_ln = document.querySelectorAll('.start_link_a');
    if (start_ln.length != 0) { start_ln[0].click(); return surf_cl[n++].querySelector("".desctext"").innerText;}
     else { return 'wait'; }
}
function click_s()
{
    if (n >= surf_cl.length) return 'end_surf';
    else if (surf_cl[n].innerText == '')
    {
        n++; return 'continue';
    }
    else
    {
        surf_cl[n].click(); return 'click';
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
                    ev = WaitFunction(browserMain.MainFrame, "surf();");
                    if (ev != "errorWait")
                    {
                        ev = Regex.Replace(ev, @"\D", "", RegexOptions.IgnoreCase);
                        ev = GetMailAnswer(browserMain.MainFrame, "document.querySelector('#window_mail" + ev + " > tbody > tr:nth-child(2) > td')",
                           "document.querySelector('.question_am')",
                           "document.querySelectorAll('.button_a_m')");
                        if (ev == "errorMail")
                        {
                            Random rnd = new Random();
                            ev = rnd.Next(0, 3).ToString();
                        }
                        js =
@"function waitReturn(){
    var start_surf_mail = document.querySelector('.start_link_a');
    if (start_surf_mail != null)
    {
        start_surf_mail.click(); return 'click';
    }    
    else { return 'wait'; }
}";

                        SendJS(0, "document.querySelectorAll('.button_a_m')[" + ev + @"].click();");
                        ev = WaitFunction(browserMain.MainFrame, "waitReturn();", js);
                        if (ev == "click")
                        {
                           
                        }
                    }
                }
                CloseСhildBrowser();
                Sleep(2);
            }
            return Count;
        }
        private bool Captcha(IBrowser browser)
        {
            for (int i = 0; i < 5; i++)
            {
                string jsAntiBot =
    @"var captha_lab = document.querySelectorAll('.out-capcha-lab');
if(captha_lab.length != 0){
    'captcha';
}
else 'ok';";
                string evAntiBot = SendJSReturn(browser.MainFrame, jsAntiBot);
                CM(evAntiBot);
                if (evAntiBot == "ok")
                {
                    SendJS(browser.MainFrame, "PopUpHide_see();");
                    Sleep(1);
                    return true;
                }
                else if (evAntiBot == "error")
                {
                    CM("ERROR");
                    Error("Ошибка капчи");
                    return false;
                }
                else
                {

                }
            }
            Error("Ошибка ввода капчи");
            return false;
        }
        private void AntiBot(IBrowser browser)
        {
            string oldURL = browser.MainFrame.Url;
            string ev = @"var info = document.querySelector('.info');
if(info != null)
{
    if(document.querySelector('.info').innerText == 'Перейдите в раздел ""Задания"" и откройте любое задание, далее вернитесь на страницу ""YouTube"".') 'antiBot';
    else 'notAntiBot';
}
else 'notAntiBot';";
            ev = SendJSReturn(browser.MainFrame, ev);
            if (ev == "antiBot")
            {
                LoadPage(0, "https://seo-fast.ru/work_tasks");
                Sleep(2);
                eventLoadPage.Reset();
                SendJS(0, "document.querySelectorAll('.list_rek_table a')[0].click();");
                eventLoadPage.WaitOne(3000);
                Sleep(2);
                CloseСhildBrowser();
                LoadPage(0, oldURL);
            }
        }
    }
}