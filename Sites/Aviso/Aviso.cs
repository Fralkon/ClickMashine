using CefSharp;
using ClickMashine.Models;

namespace ClickMashine.Sites.Aviso
{
    class Aviso : Site
    {
        Surfing YouTube;
        Surfing Click;
        SurfingMail Mail;
        public Aviso(MainForm form, AuthData auth) : base(form, auth)
        {
            homePage = "https://aviso.bz";
            Type = EnumTypeSite.Aviso;

            string YouTubeJS = LoadJSOnFile("youtube");
            YouTube = new Surfing(this, "https://aviso.bz/work-youtube", YouTubeJS, YouTubeMiddle, SurfingType.YouTube);

            string ClickJS = LoadJSOnFile("click");
            Click = new Surfing(this, "https://aviso.bz/work-serf", ClickJS, ClickMiddle, SurfingType.Click);

            Mail = new SurfingMail(this, "https://aviso.bz/mails_new", ClickJS, MailClick, MailMiddle);

            //ManagerSurfing.AddSurfing(YouTube);
            ManagerSurfing.AddSurfing(Click);
            ManagerSurfing.AddSurfing(Mail);
        }
        public override async Task<bool> Auth(AuthData auth)
        {
            IBrowser browser = await GetBrowserAsync(0);
            if (browser == null)
                return false;
            await LoadPageAsync(browser, "https://aviso.bz/login");
            await SleepAsync(3);
            string js_auth = LoadJSOnFile("auth", auth.Login,auth.Password);

            if (StatusJS.OK == await InjectJSAsync(browser, js_auth))
                await SleepAsync(7);
            await InjectJSAsync(browser, "window.open(\"https://www.google.com\", \"_blank\");");
            return true;
        }
        private async Task<bool> YouTubeMiddle(IBrowser browser)
        {
            await WaitElementAsync(browser, "player");
            if (await WaitTimeAsync(browser, @"player.setVolume(0); player.seekTo(0, true); b = true; c = true;  timerInitial;"))
            {
                string jsWaitYouTube =
@"function WaitEnd(){
if(document.querySelector('#capcha-tr-block').innerText.length > 3)
    return " + (int)StatusJS.OK + @";
else
    return " + (int)StatusJS.Wait + @";}";
                form.FocusTab(browser);
                if (StatusJS.OK != await FunctionWaitAsync(browser, "WaitEnd();", jsWaitYouTube))
                {
                    await WaitElementAsync(browser, "player");
                    if (await WaitTimeAsync(browser, @"player.setVolume(0); player.seekTo(0, true); b = true; c = true;  timerInitial;"))
                    {
                        form.FocusTab(browser);
                        if (StatusJS.OK == await FunctionWaitAsync(browser, "WaitEnd();", jsWaitYouTube))
                            return true;
                    }
                }
            }
            return false;
        }
        private async Task<bool> ClickMiddle(IBrowser browser)
        {
            var frameIndif = browser.GetFrameIdentifiers();
            foreach (var id in frameIndif)
            {
                IFrame frame = browser.GetFrameByIdentifier(id);
                if (frame.Url.IndexOf("vlss") == -1)
                {
                    continue;
                }
                else
                {
                    if (await WaitElementAsync(frame, "document.querySelector('.timer')"))
                    {
                        if (await WaitTimeAsync(frame, "document.querySelector('.timer').innerText"))
                        {
                            if (!await WaitButtonClickAsync(frame, "document.querySelector('.btn_capt')", 5))
                            {
                                await SendJSAsync(frame, "document.querySelector('.timer').innerText = 0");
                                if (await WaitButtonClickAsync(frame, "document.querySelector('.btn_capt')", 5))
                                    return true;
                            }
                            else
                                return true;
                        }
                    }
                    break;
                }
            }
            return false;
        }
        private async Task<bool> MailMiddle(IBrowser browser)
        {
            if (await WaitTimeAsync(browser, "document.querySelector('#tmr').innerText"))
            {
                string js =
@"var range = document.querySelector('[type=""range""]');
if (range != null)
{
    range.value = range.max;
    document.querySelector('form').submit(); " + (int)StatusJS.End + @";
}
else { " + (int)StatusJS.Error + @"; }";

                if (await InjectJSAsync(browser, js) == StatusJS.End)
                    return true;
            }
            return false;
        }
        private async Task<bool> MailClick(IBrowser browser)
        {
            string ev = await GetMailAnswerAsync(browser.MainFrame, "document.querySelector('#js-popup > div > div:nth-child(3)')",
                               "document.querySelector('#js-popup > div > div:nth-child(4)')",
                               "document.querySelectorAll('.mails-otvet-new a')");
            if (ev == "errorMail")
            {
                Random rnd = new Random();
                ev = rnd.Next(0, 3).ToString();
            }
            await InjectJSAsync(browser, "document.querySelectorAll('.mails-otvet-new a')[" + ev + "].click();");
            return true;
        }
    }
}
