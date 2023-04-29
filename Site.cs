using CefSharp;
using CefSharp.WinForms;
using System.Xml.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ClickMashine
{
    enum EnumTypeSite
    {
        None,
        Router,
        SeoFast,
        Aviso,
        Profitcentr,
        WmrFast,
        WebofSar,
        Losena,
        SeoClub,
        VipClick
    }
    class Auth
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public Auth(string login, string password)
        {
            Login = login;
            Password = password;
        }
        public Auth(XElement? xml)
        {
            if (xml == null)
                throw new Exception("Not valid auth site");
            Login = xml.Element("login").Value.Trim();
            Password = xml.Element("password").Value.Trim();
        }
    }
    class MailSurf
    {
        public string Mail { get; set; }
        public string Question { get; set; }
        public List<string> Answer { get; set; }
        public string GetAnswer()
        {

            Mail = Mail.ToLower();
            Mail = Regex.Replace(Mail, @"\s", "", RegexOptions.IgnoreCase);
            List<bool> tryValye = new List<bool>();
            int try_int_val = 0;
            for (int i = 0; i < Answer.Count; i++)
            {
                Answer[i] = Answer[i].ToLower();
                Answer[i] = Regex.Replace(Answer[i], @"\s", "", RegexOptions.IgnoreCase);
                tryValye.Add(false);
                if (Mail.IndexOf(Answer[i]) != -1)
                {
                    tryValye[i] = true;
                    try_int_val++;
                }
            }
            Console.WriteLine(Mail);
            Console.WriteLine(Answer[0]);
            Console.WriteLine(Answer[1]);
            Console.WriteLine(Answer[2]);
            if (try_int_val == 1)
            {
                for (int i = 0; i < Answer.Count; i++)
                {
                    if (tryValye[i])
                        return i.ToString();
                }
            }
            return "errorMail";
        }
    }
    class Report {
        int Click { get; set; }
        int Visit { get; set; }
        int YouTube { get; set; }
        public Report()
        {

        }
    }
    abstract class Site : MyThread
    {
        public Form1 form;
        protected EventWaitHandle eventLoadPage = new EventWaitHandle(false, EventResetMode.ManualReset);
        protected EventWaitHandle eventBrowserCreated = new EventWaitHandle(false, EventResetMode.ManualReset);
        protected List<IBrowser> browsers = new List<IBrowser>();
        protected IBrowser LastBrowser;
        protected string homePage;
        public EnumTypeSite Type { get; protected set; }
        public ChromiumWebBrowser main_browser;
        public MyLifeSplanHandler lifeSplanHandler;
        protected Auth auth;
        public TCPMessageManager TCPMessageManager = new TCPMessageManager();
        public Site(Form1 form, Auth auth)
        {
            this.auth = auth;
            this.form = form;
        }
        public Site(Form1 form)
        {
            this.form = form;
        }
        public abstract bool Auth(Auth auth);
        public void SomedoIt()
        {
        }
        public void Initialize()
        {
            lifeSplanHandler = new MyLifeSplanHandler(this);
            main_browser = new ChromiumWebBrowser(homePage);
            main_browser.LifeSpanHandler = lifeSplanHandler;
            form.Invoke(new Action(() =>
            {
                TabPage newTabPage = new TabPage();
                newTabPage.Text = Type.ToString();
                newTabPage.BorderStyle = BorderStyle.None;
                newTabPage.Dock = DockStyle.Fill;
                newTabPage.Controls.Add(main_browser);
                main_browser.Dock = DockStyle.Fill;
                form.tabControl1.TabPages.Add(newTabPage);
                form.tabControl1.SelectedTab = newTabPage;
            }));
        }
        public void AfterCreated(IWebBrowser browserControl, IBrowser browser)
        {
            LastBrowser = browser;
            browsers.Add(browser);
            browserControl.LoadingStateChanged += Browser_LoadingStateChanged;
            eventBrowserCreated.Set();
        }
        public void CloseBrowser(IBrowser browser)
        {
            for (int i = 0; i < browsers.Count; i++)
            {
                if (browsers[i].Identifier == browser.Identifier)
                {
                    var controlBrowser = Control.FromChildHandle(browser.GetHost().GetWindowHandle());
                    if (controlBrowser != null)
                    {
                        controlBrowser.Invoke(new Action(() =>
                        {
                            TabPage? parentControl = controlBrowser.Parent as TabPage;
                            if (parentControl != null)
                            {
                                form.tabControl1.TabPages.Remove(parentControl);
                            }
                            else
                            {
                                var tabControl = controlBrowser as TabPage;
                                if (tabControl != null)
                                {
                                    form.tabControl1.TabPages.Remove(tabControl);
                                }
                                else
                                    throw new Exception("Ошибка удаления TabControl");
                            }
                        }));
                        controlBrowser.Invoke(() => { controlBrowser.Dispose(); });
                    }
                    browsers.RemoveAt(i);
                    if (i != 0)
                    {
                        form.FocusTab(browsers[i - 1]);
                    }
                }
            }
        }
        public void Browser_LoadingStateChanged(object? sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
                eventLoadPage.Set();
        }
        protected IBrowser? GetBrowser(int id)
        {
            eventBrowserCreated.Reset();
            eventLoadPage.Reset();
            if (browsers.Count <= id)
            {
                eventBrowserCreated.Reset();
                if (!eventBrowserCreated.WaitOne(5000))
                    return null;
            }
            eventLoadPage.WaitOne(5000);
            Sleep(1);
            return browsers[id];
        }
        protected bool WaitCreateBrowser(int id)
        {
            eventBrowserCreated.Reset();
            eventLoadPage.Reset();
            if (browsers.Count <= id)
                if (!eventBrowserCreated.WaitOne(3000))
                    if (!eventLoadPage.WaitOne(5000))
                    {
                    }
            CM("End create wait");
            return true;
        }
        protected IBrowser? WaitCreateBrowser()
        {
            eventBrowserCreated.Reset();
            eventLoadPage.Reset();
            if (!eventBrowserCreated.WaitOne(3000))
                return null;
            eventLoadPage.WaitOne(5000);
            return LastBrowser;
        }
        protected void LoadPage(int id_browser, string page)
        {
            if (browsers.Count > id_browser)
            {
                Console.WriteLine("---------------------------\nOpen: " + page);
                Console.WriteLine("Type: " + Type.ToString());
                Console.WriteLine("---------------------------");
                eventLoadPage.Reset();
                browsers[id_browser].MainFrame.LoadUrl(page);
                eventLoadPage.WaitOne();
            }
        }
        protected void LoadPage(string page)
        {
            LoadPage(0, page);
        }
        protected void LoadPage(IBrowser browser, string page)
        {
            if (browsers.Find(item => item.Identifier == browser.Identifier) != null)
            {
                Console.WriteLine("---------------------------\nOpen: " + page);
                Console.WriteLine("Type: " + Type.ToString());
                Console.WriteLine("---------------------------");
                eventLoadPage.Reset();
                browser.MainFrame.LoadUrl(page);
                eventLoadPage.WaitOne(5000);
            }
        }
        protected string SendJSReturn(int id_browser, string JS)
        {
            if (browsers.Count > id_browser)
            {
                IFrame frame = browsers[id_browser].MainFrame;
                return SendJSReturn(frame, JS);
            }
            else return null;
        }
        protected void SendJS(int id_browser, string JS)
        {
            if (browsers.Count > id_browser)
            {
                IFrame frame = browsers[id_browser].MainFrame;
                SendJS(frame, JS);
            }
        }
        protected void SendJS(IFrame frame, string JS)
        {
            Console.WriteLine("---------------------------\nSend JS: \n");
            Console.WriteLine(JS);
            Console.WriteLine("Type: " + Type.ToString());
            Console.WriteLine("---------------------------");
            frame.ExecuteJavaScriptAsync(JS);
        }
        protected void SendJS(IBrowser browser, string JS)
        {
            SendJS(browser.MainFrame, JS);
        }
        protected string SendJSReturn(IFrame frame, string JS)
        {
            string JS_TRY = "try{\n" + JS + "\n}catch(e){'error';}";
            Console.WriteLine("---------------------------\nSend JS:");
            Console.WriteLine(JS_TRY);
            Console.WriteLine("Type: " + Type.ToString());
            var task = frame.EvaluateScriptAsync(JS_TRY);
            task.Wait();
            if (task.Result.Result != null)
            {
                if (task.Result.Result.ToString() == "error")
                {
                    throw new Exception("Type: " + Type.ToString() + "\nError JS");
                }
                Console.WriteLine("Return: " + task.Result.Result.ToString());
                Console.WriteLine("---------------------------");
                return task.Result.Result.ToString();
            }

            Console.WriteLine("Not return!!!!!!!");
            Console.WriteLine("---------------------------");
            return null;
        }
        protected string SendJSReturn(IBrowser browser, string JS)
        {
            return SendJSReturn(browser.MainFrame, JS);
        }
        protected void Error(string text)
        {
            string Message = "---------------------------\n" +
            text +
            "\nType: " + Type.ToString() +
            "\n---------------------------\n";
            Console.WriteLine(Message);
            TCPMessageManager.SendError(Message, Type);
        }
        protected void Error(IBrowser browser, string text)
        {
            browser.GetSourceAsync().ContinueWith(v =>
            {
                string path = @"C:\ClickMashine\Settings\Errors\Pages\";
                File.WriteAllText(path + "html_page_error" + new DirectoryInfo(path).GetFiles().Length.ToString() + ".txt", v.Result);
            }).Wait();
            string Message = "---------------------------\n" +
            text +
            "\nType: " + Type.ToString() +
            "\n---------------------------\n";
            Console.WriteLine(Message);
            TCPMessageManager.SendError(Message, Type);
        }
        protected void CM(string text)
        {
            Console.WriteLine("---------------------------");
            Console.WriteLine(text);
            Console.WriteLine("Type: " + Type.ToString());
            Console.WriteLine("---------------------------");
        }
        protected void Sleep(int sec)
        {
            Console.WriteLine("---------------------------");
            Console.WriteLine("Sleep: " + sec.ToString());
            Console.WriteLine("Type: " + Type.ToString());
            Console.WriteLine("---------------------------");
            Thread.Sleep(sec * 1000);
        }
        protected void Sleep(string sec)
        {
            try
            {
                Sleep(int.Parse(sec));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        protected void SendKeysEvent(int id_browser, string text)
        {
            KeyEvent eventKey = new KeyEvent();
            eventKey.IsSystemKey = false;
            eventKey.Modifiers = 0;
            KeysConverter converter = new KeysConverter();
            foreach (var ch in text)
            {
                eventKey.WindowsKeyCode = ch;
                eventKey.Type = KeyEventType.Char;
                browsers[id_browser].GetHost().SendKeyEvent(eventKey);
            }
        }
        protected void CloseСhildBrowser()
        {
            for (int i = 1; i < browsers.Count; i++)
            {
                browsers[i].CloseBrowser(false);
            }
        }
        protected void CloseAllBrowser()
        {
            for (int i = 0; i < browsers.Count; i++)
            {
                browsers[i].CloseBrowser(false);
            }
        }
        protected bool WaitButtonClick(IFrame frame, string element, int sec = 10)
        {
            string js_wait =
@"function wait_element()
{
    var element = " + element + @"
    if (element != null) { element.click(); return 'click'; }
    else return 'wait';
}";
            SendJS(frame, js_wait);
            for (int i = 0; i < sec; i++)
            {
                string ev_js_wait = SendJSReturn(frame, "wait_element();");
                if (ev_js_wait == "click")
                    return true;
                Thread.Sleep(1000);
            }
            return false;
        }
        protected string GetMailAnswer(IFrame frame, string mail, string question, string answer, int sec = 5)
        {
            string js =
@"function get_mail() {
    var mail_text = " + mail + @";
    if (mail_text != null) {
        var answer = " + answer + @";
        var quest = " + question + @";
        return JSON.stringify(
        {Mail : mail_text.innerText,
        Question : quest.innerText,
        Answer : [answer[0].innerText,
        answer[1].innerText,
        answer[2].innerText]
        });
    }
    else { return 'wait'; }
};
get_mail();";
            for (int i = 0; i < sec; i++)
            {
                string ev = SendJSReturn(frame, js);
                if (ev == "wait")
                    Sleep(1);
                else
                {
                    MailSurf? mailSurf = JsonSerializer.Deserialize<MailSurf>(ev);
                    if (mailSurf == null)
                    {
                        return "errorMail";
                    }
                    return mailSurf.GetAnswer();
                }
            }
            return "errorMail";
        }
        protected bool WaitElement(IFrame frame, string element, int sec = 10)
        {
            string js_wait =
@"function waitElement()
{
    var element = " + element + @";
    if (element != null) { return 'end'; }
    else return 'wait';
}";
            SendJS(frame, js_wait);
            for (int i = 0; i < sec; i++)
            {
                string? ev_js_wait = SendJSReturn(frame, "waitElement();");
                if (ev_js_wait == null)
                    continue;
                if (ev_js_wait == "end")
                    return true;
                Thread.Sleep(1000);
            }
            return false;
        }
        protected string WaitFunction(IFrame frame, string functionName, string? function = null, int sec = 5)
        {
            if (function != null)
                SendJS(frame, function);
            for (int j = 0; j < sec; j++)
            {
                string ev = SendJSReturn(frame, functionName);
                if (ev == "wait")
                    Sleep(1);
                else return ev;
            }
            return "errorWait";
        }
        protected Bitmap GetImgBrowser(IFrame frame, string element)
        {
            if (WaitElement(frame, element))
            {
                string js = @"var elementImg = " + element + @";
if(elementImg != null){
    var js = elementImg.getBoundingClientRect().toJSON();
    JSON.stringify({ X: parseInt(js.x), Y: parseInt(js.y),  Height: parseInt(js.height), Width: parseInt(js.width)});
}
else 'errorImg';";
                string ev = SendJSReturn(frame, js);
                if (ev != "errorImg")
                {
                    Rectangle rectElement = JsonSerializer.Deserialize<Rectangle>(ev);
                    return form.MakeScreenshot(frame.Browser, rectElement);
                }
                else
                    throw new Exception("Ошибка скриншота");
            }
            else
                throw new Exception("Ошибка ожидания элемента для скриншота");
        }
        protected string SendQuestion(Bitmap image, string text)
        {
            return TCPMessageManager.SendQuestion(image, text, Type);
        }
    }
}