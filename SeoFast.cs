using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CefSharp;

namespace ClickMashine
{
    class SeoFast : Site
    {
        public SeoFast(Form1 form, Auth auth) : base(form, auth)
        {
            homePage = "https://seo-fast.ru/";
            Type = EnumTypeSite.SeoFast;
        }
        protected override void StartSurf()
        {
            Initialize();
            if (!Auth(auth))
            {
                if (!waitHandle.WaitOne())
                    return;
            }
            TakeMoney(browsers[0]);
            mSurf.AddFunction(MailSurf);
            mSurf.AddFunction(ClickSurf);
            mSurf.AddFunction(VisitSurf);
            while (true)
            {
                int youTube = 10;
                while (youTube > 9)
                {
                    //mSurf.GoSurf();
                    youTube = 0;
                    try
                    {
                        youTube += YouTubeSurf("https://seo-fast.ru/work_youtube?rutube_video");
                    }
                    catch (Exception ex)
                    {
                        CloseСhildBrowser();
                        Error("Error youtube1\n" + ex.Message);
                    }
                    try
                    {
                        youTube += YouTubeSurf("https://seo-fast.ru/work_youtube?youtube_expensive");
                    }
                    catch (Exception ex)
                    {
                        CloseСhildBrowser();
                        Error("Error youtube1\n" + ex.Message);
                    }
                    try
                    {
                        youTube += YouTubeSurf("https://seo-fast.ru/work_youtube?youtube_video_simple");
                    }
                    catch (Exception ex)
                    {
                        CloseСhildBrowser();
                        Error("Error youtube2\n" + ex.Message);
                    }
                    try
                    {
                        youTube += YouTubeSurf("https://seo-fast.ru/work_youtube?youtube_video_bonus");
                    }
                    catch (Exception ex)
                    {
                        CloseСhildBrowser();
                        Error("Error youtube3\n" + ex.Message);
                    }
                }
                //SetBDInfo(youTube);
                //mSurf.GoSurf();
                Sleep(600);
            }
        }
        public override bool Auth(Auth auth)
        {
            LoadPage(0, "https://seo-fast.ru/main");
            Sleep(5);
            eventLoadPage.Reset();
            string ev = SendJSReturn(0, "var l_b = document.querySelector('.loginbutton');" +
            "if (l_b != null){" +
            "   l_b.click();" +
            "   'login';}" +
            "else {'go';}");
            if (ev == "login")
            {
                IBrowser browser = browsers[0];
                for (int i = 0; i < 10; i++)
                {
                    if (i == 9)
                        return false;
                    eventLoadPage.WaitOne(5000);
                    if (WaitElement(browser.MainFrame, "document.querySelector('.out-capcha')"))
                    {
                        string js = "document.querySelector('#logusername').value = '" + auth.Login + "';" +
                        "document.querySelector('#logpassword').value = '" + auth.Password + "';";
                        SendJS(0, js);
                        Bitmap img = GetImgBrowser(browser.MainFrame, "document.querySelector('.out-capcha')");

                        string answer_telebot = SendQuestion(img, "");

                        string jsAntiBot = "";
                        foreach (char ch in answer_telebot)
                            jsAntiBot += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
                        jsAntiBot += "login('1');";

                        SendJS(0, jsAntiBot);
                        Sleep(7);
                        if (WaitElement(browser.MainFrame, "document.querySelector('.main_balance')"))
                        {
                            SendJS(browser.MainFrame, @"if(document.querySelector('.popup2').style.display != 'none'){document.querySelector('.popup2-content .sf_button').click();}");
                            Sleep(2);
                            return true;
                        }
                        eventLoadPage.Reset();
                        browser.Reload();
                    }
                    else
                        return false;
                }
            }
            return true;
        }
        private void CheckCaptcha()
        {
            Sleep(3);
            string ev = SendJSReturn(0,
@"var check = document.querySelector('.h-captcha');
if (check != null) { 'captcha'; }
else {'end';}");
            if (ev == "captcha")
            {
                List<long> list_id = browsers[0].GetFrameIdentifiers();
                IFrame? frameCheckbox = null, frameChallenge = null;
                for (int i = 0; i < list_id.Count; i++)
                {
                    IFrame frame = browsers[0].GetFrame(list_id[i]);
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
                        Bitmap img = GetImgBrowser(browsers[0].MainFrame, "document.querySelector('[title=\"Main content of the hCaptcha challenge\"]')");
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
                    Bitmap image = GetImgBrowser(browsers[0].MainFrame, "document.querySelector('.out-capcha')");
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
                        browsers[0].Reload();
                        eventLoadPage.WaitOne();
                    }
                }
            }
        }
        private int YouTubeSurf(string url)
        {
            int count = 0;
            LoadPage(0, url);
            Sleep(5);
            //CheckCaptcha();
            AntiBotImage(browsers[0]);
            string js =
@"var surf_cl = document.querySelectorAll('a.surf_ckick');var n = 0;
var youtube_premium = null;
function click_s()
{
    if (n >= surf_cl.length) return 'end_surf';
    else if (surf_cl[n].innerText == '')
    {
        n++; return 'continue';
    }
    else{ 
        youtube_premium = surf_cl[n].parentElement.parentElement.parentElement;
        if(youtube_premium == null){n++;return 'continue';}
        if(youtube_premium.id.indexOf('v123') != -1){
            surf_cl[n].click(); 
            n++; 
            return 'surf_premium';
        }       
        else{
            surf_cl[n].click(); 
            n++; 
            return 'surf'; 
        }
    }
}
function surf(){
    var start_ln = youtube_premium.querySelector('.start_link_youtube');
	if (start_ln != null) { 
		if(start_ln.innerText != 'Приступить к просмотру') {n++; return 'continue';}
		else {start_ln.click(); n++; return 'surf'; }
	}
	else { return 'sec_wait'; }
}";

            SendJS(0, js);
            Thread.Sleep(200);
            while (true)
            {
                eventBrowserCreated.Reset();
                string ev = SendJSReturn(0, "click_s();");
                if (ev == "end_surf")
                    break;
                else if (ev == "continue")
                    continue;
                else if (ev == "surf")
                {
                    IBrowser? browserYouTube = WaitCreateBrowser();
                    if (browserYouTube == null)
                        continue;
                    try
                    {
                        YouTubeWatch(browserYouTube);
                    }
                    catch (Exception ex)
                    {
                        Error("Error watch youtube task.");
                        browserYouTube.GetHost().CloseBrowser(true);
                    }
                    count++;
                    Sleep(5);
                }
                else if(ev == "surf_premium")
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ev = SendJSReturn(0, "surf();");
                        if (ev == "surf")
                        {
                            IBrowser? browserYouTube = WaitCreateBrowser();
                            if (browserYouTube == null)
                                continue;
                            try
                            {
                                YouTubeWatch(browserYouTube);
                            }
                            catch (Exception ex)
                            {
                                Error("Error watch youtube task.");
                                browserYouTube.GetHost().CloseBrowser(true);
                            }
                            count++;
                            Sleep(5);
                            break;
                        }
                        else if (ev == "sec_wait")
                            Sleep(1);
                        else if (ev == "continue")
                            break;
                        if (i == 9)
                        {
                            SendJS(0, "n++;");
                            break;
                        }
                    }
                }
                Sleep(1);
            }
            CloseСhildBrowser();
            return count;
        }
        private async void YouTubeWatch(IBrowser browserYouTube)
        {
            await Task.Run(() =>
            {
                string js =
@"if(rutube == '0')
{
    b = true;
    var timer_youtube = document.querySelector('#tmr');
    if (timer_youtube != null) timer_youtube.innerText;
    else 'error_youtube';
}
else {
    timer();
    timer_v;
}";
                string ev = SendJSReturn(browserYouTube.MainFrame, js);
                if (ev != "error_youtube")
                {
                    Sleep(ev);
                }
                string jsWaitYouTube =
@"function WaitEnd(){
if(document.querySelector('#capcha-tr-block').innerText.indexOf(""засчитан"") != -1)
    return 'ok';
else
    return 'wait';}";
                form.FocusTab(browserYouTube);
                WaitFunction(browserYouTube.MainFrame, "WaitEnd();", jsWaitYouTube, 10);
                browserYouTube.GetHost().CloseBrowser(true);
            });
        }
        private int ClickSurf()
        {
            int Count = 0;
            LoadPage(0, "https://seo-fast.ru/work_surfing?go");
            Sleep(2);
            AntiBotImage(browsers[0]);
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
            SendJS(0, js);
            while (true)
            {
                string ev = SendJSReturn(0, "click_s();");
                if (ev == "end_surf")
                    break;
                else if (ev == "continue")
                    continue;
                else if (ev == "surf")
                {
                    for (int i = 0; i < 5; i++)
                    {
                        ev = SendJSReturn(0, "surf();");
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
            LoadPage(0, "https://seo-fast.ru/work_transitions");
            Sleep(2);
            AntiBotImage(browsers[0]);
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
            SendJS(0, js);
            while (true)
            {
                string ev = SendJSReturn(0, "click_s();");
                if (ev == "end_surf")
                    break;
                else if (ev == "continue")
                    continue;
                else if (ev == "surf")
                {
                    for (int i = 0; i < 5; i++)
                    {
                        ev = SendJSReturn(0, "surf();");
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
            LoadPage(0, "https://seo-fast.ru/work_mails");
            Sleep(2);
            AntiBotImage(browsers[0]);
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
                    ev = WaitFunction(browsers[0].MainFrame, "surf();");
                    if (ev != "errorWait")
                    {
                        ev = Regex.Replace(ev, @"\D", "", RegexOptions.IgnoreCase);
                        ev = GetMailAnswer(browsers[0].MainFrame, "document.querySelector('#window_mail" + ev + " > tbody > tr:nth-child(2) > td')",
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
                        ev = WaitFunction(browsers[0].MainFrame, "waitReturn();", js);
                        if (ev == "click")
                        {
                            var browserSurf = GetBrowser(1);
                            if (browserSurf != null)
                            {
                                js =
@"function go(){
    isa = 0;
    return document.querySelector('#time').innerText;
}
go();";
                                ev = SendJSReturn(browserSurf.MainFrame, js);
                                Sleep(ev);
                                if (!WaitButtonClick(browserSurf.MainFrame, "document.querySelector('.button_s');"))
                                {
                                    ev = @"
$.ajax({
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
                                    Count++;
                                Sleep(2);
                            }
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
        private bool AntiBotImage(IBrowser browser)
        {
            //captcha_new
            string js = @"var img_captcha = document.querySelector('.out-capcha');
if(img_captcha != null)
    'antiBot';
else 'notAntiBot';";
            int iteration = 0;
            while (SendJSReturn(browser.MainFrame, js) == "antiBot")
            {
                if (iteration == 10)
                    return false;
                string jsAntiBot = String.Empty;
                string answerBot = SendQuestion(GetImgBrowser(browser.MainFrame, "document.querySelector('.out-capcha')"), "");
                if (answerBot != "replace")
                {
                    foreach (char ch in answerBot)
                        jsAntiBot += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
                    jsAntiBot += "document.querySelector('.sf_button').click();";
                    SendJS(browser.MainFrame, jsAntiBot);
                    Sleep(4);
                }
                iteration++;
                eventLoadPage.Reset();
                browser.Reload();
                eventLoadPage.WaitOne(5000);
            }
            return true;
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
                        SendJS(browser,"all_money();i_not_robot();payment_money();");
                    }
                }
            }
        }
    }
}