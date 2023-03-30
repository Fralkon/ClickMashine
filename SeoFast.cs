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
        public SeoFast(Form1 form, TeleBot teleBot, Auth auth) : base(form, teleBot, auth)
        {
            homePage = "https://seo-fast.ru/";
            type = EnumTypeSite.SeoFast;
        }
        protected override void StartSurf()
        {
            Initialize();
            if (!Auth(auth))
                return;
            while (true)
            {
                try
                {
                    MailSurf();
                }
                catch (Exception ex)
                {
                    Error("Error mail\n" + ex.Message);
                }
                try
                {
                    ClickSurf();
                }
                catch (Exception ex)
                {
                    Error("Error Click\n" + ex.Message);
                }
                try
                {
                    VisitSurf();
                }
                catch (Exception ex)
                {
                    Error("Error visit\n" + ex.Message);
                }
                try
                {
                    YouTubeSurf("https://seo-fast.ru/work_youtube?youtube_expensive");
                }
                catch (Exception ex)
                {
                    Error("Error youtube1\n" + ex.Message);
                }
                try
                {
                    YouTubeSurf("https://seo-fast.ru/work_youtube?youtube_simple");
                }
                catch (Exception ex)
                {
                    Error("Error youtube2\n" + ex.Message);
                }
                try
                {
                    YouTubeSurf("https://seo-fast.ru/work_youtube?youtube_video_bonus");
                }
                catch (Exception ex)
                {
                    Error("Error youtube3\n" + ex.Message);
                }
            }
            CloseAllBrowser();
        }
        public override bool Auth(Auth auth)
        {
            LoadPage(0, "https://seo-fast.ru/");
            eventLoadPage.Reset();
            string ev = SendJSReturn(0, "var l_b = document.querySelector('.loginbutton');" +
            "if (l_b != null){" +
            "   l_b.click();" +
            "   'login';}" +
            "else {'go';}");
            if (ev == "login")
            {
                eventLoadPage.WaitOne(5000);
                string js = "document.querySelector('#logusername').value = '" + auth.Login + "';" +
                "document.querySelector('#logpassword').value = '" + auth.Password + "';";
                SendJS(0, js);
                if (!Captcha(browsers[0]))
                    return false;
                LoadPage(0, "https://seo-fast.ru/work_surfing?go");
                AntiBot(browsers[0]);
                //                SendJS(0, "captcha_choice('2');onclick=\"save_enter();\"");
                //                Sleep(1);
                //                while (true)
                //                {
                //                    ev = SendJSReturn(0, "var js = document.querySelector('#captcha_new').getBoundingClientRect().toJSON();" +
                //        "JSON.stringify({ X: parseInt(js.x), Y: parseInt(js.y),  Height: parseInt(js.height), Width: parseInt(js.width)});");
                //                    Rectangle rect_img = JsonSerializer.Deserialize<Rectangle>(ev);
                //                    FocusBrowser(browsers[0]);
                //                    Bitmap img = MakeScreenshot(rect_img);
                //                    string answer_telebot = teleBot.SendQuestion(img);

                //                    string js = "document.querySelector('#logusername').value = '" + auth.Login + "';" +
                //                    "document.querySelector('#logpassword').value = '" + auth.Password + "';" +
                //                    "document.querySelector('#code').value = '" + answer_telebot + "';" +
                //                    "document.querySelector('.sf_button').click();";
                //                    eventLoadPage.Reset();
                //                    SendJS(0, js);
                //                    eventLoadPage.WaitOne();
                //                    Sleep(5);
                //                    js =
                //@"var echoError = document.querySelector('.echo_error');
                //if(echoError != null) 'echoError';
                //else 'ok';";
                //                    ev = SendJSReturn(0, js);
                //                    if (ev == "ok")
                //                        break;
                //                }
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
                        string answerTelebot = teleBot.SendQuestion(img);
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
                    string answer_telebot = teleBot.SendQuestion(image);

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
        private void YouTubeSurf(string url)
        {
            LoadPage(0, url);
            //CheckCaptcha();
            AntiBot(browsers[0]);
            string js =
@"var surf_cl = document.querySelectorAll('a.surf_ckick');var n = 0;
function click_s()
{
    if (n >= surf_cl.length) return 'end_surf';
    else if (surf_cl[n].innerText == '')
    {
        n++; return 'continue';
    }
    else { surf_cl[n].click(); n++; return 'surf'; }
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
                    Sleep(2);
                    var browserYouTube = GetBrowser(1);
                    if (browserYouTube == null)
                    {
                        CloseСhildBrowser();
                        continue;
                    }
                    js =
@"b = true;
var timer_youtube = document.querySelector('#tmr');
if (timer_youtube != null) timer_youtube.innerText;
else 'error_youtube';";
                    ev = SendJSReturn(browserYouTube.MainFrame, js);
                    if (ev != "error_youtube")
                    {
                        Sleep(ev);
                        WaitButtonClick(browserYouTube.MainFrame, "document.querySelector('.sf_button');");
                    }
                }
                Sleep(2);
                CloseСhildBrowser();
            }
        }
        private void ClickSurf()
        {
            LoadPage(0, "https://seo-fast.ru/work_surfing?go");
            //CheckCaptcha();
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
                            var browserSurf = GetBrowser(1);
                            if (browserSurf == null)
                                continue;
                            Sleep(2);
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
                            if (WaitButtonClick(browserSurf.MainFrame, "document.querySelector('.button_s');") != "click")
                            {
                                ev = @"
$.ajax({
	type: 'POST', url: domail_s+'/ajax/ajax_surfing2.php',  
	data: { 'sf' : 'load_captcha_sf', 'v_surfing_lc' : v_surfing_lc, 'id_rek' : id_rek, 'type' : 'surf' }, 
    beforeSend: function(){ $('#timer_lo').remove(); $('#timer_lo_error').remove(); $('#code').css({display: 'block'}); },
    success: function(data){ localStorage.setItem('id_rek_l', id_rek); $('#code').html(data); }
});";
                                WaitButtonClick(browserSurf.MainFrame, "document.querySelector('.button_s');");
                            }
                            Sleep(2);
                            break;
                        }
                    }
                }
                CloseСhildBrowser();
            }
        }
        private void VisitSurf()
        {
            LoadPage(0, "https://seo-fast.ru/work_transitions");
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
                            var browserVisit = GetBrowser(1);
                            if (browserVisit == null)
                            {
                                CloseСhildBrowser();
                                continue;
                            }
                            Sleep(2);
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
                                var browserVisit2 = GetBrowser(2);
                                if (browserVisit2 == null)
                                {
                                    CloseСhildBrowser();
                                    continue;
                                }
                                CloseBrowser(browserVisit2);
                                Sleep(1);
                                break;
                            }
                        }
                    }
                }
                CloseСhildBrowser();
                Sleep(1);
            }
        }
        private void MailSurf()
        {
            LoadPage(0, "https://seo-fast.ru/work_mails");
            //CheckCaptcha();
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
                            Sleep(2);
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
                                if (WaitButtonClick(browserSurf.MainFrame, "document.querySelector('.button_s');") != "click")
                                {
                                    ev = @"
$.ajax({
	type: 'POST', url: domail_s+'/ajax/ajax_surfing2.php',  
	data: { 'sf' : 'load_captcha_sf', 'v_surfing_lc' : v_surfing_lc, 'id_rek' : id_rek, 'type' : 'surf' }, 
    beforeSend: function(){ $('#timer_lo').remove(); $('#timer_lo_error').remove(); $('#code').css({display: 'block'}); },
    success: function(data){ localStorage.setItem('id_rek_l', id_rek); $('#code').html(data); }
});";
                                    WaitButtonClick(browserSurf.MainFrame, "document.querySelector('.button_s');");
                                }
                                Sleep(2);
                            }
                        }
                    }
                }
                CloseСhildBrowser();
                Sleep(2);
            }
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
                    return true;
                else if (evAntiBot == "error")
                {
                    CM("ERROR");
                    Error("Ошибка капчи");
                    return false;
                }
                else
                {
                    Bitmap img = GetImgBrowser(browser.MainFrame, "document.querySelector('.out-capcha')");

                    string answer_telebot = teleBot.SendQuestion(img);

                    jsAntiBot = "";
                    foreach (char ch in answer_telebot)
                        jsAntiBot += "document.querySelectorAll('.out-capcha-inp')[" + ch + "].checked = true;";
                    jsAntiBot += "login('1');";

                    SendJS(0, jsAntiBot);
                    Sleep(7);
                }
            }
            Error("Ошибка ввода капчи");
            return false;
        }       
        private void AntiBot(IBrowser browser)
        {
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
                LoadPage(0, "https://seo-fast.ru/work_youtube");
            }
        }
    }
}