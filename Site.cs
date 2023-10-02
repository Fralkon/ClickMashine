using CefSharp;
using CefSharp.WinForms;
using System.Xml.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using ClickMashine_11;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using CefSharp.DevTools;

namespace ClickMashine
{
    enum StatusSite { 
        online,
        offline,
        wait,
        error,
        login
    }
    public enum EnumTypeSite
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
        VipClick,
        Adaso
    }
    enum EnumAnswerBot { 
        replace,
        exit
    }
    enum StatusCaptcha { 
        OK,
        BLOCK,
        ERROR
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
    class Surf {
        public Surf(FunctionSurf function)
        {
            Count = 10;
            Function = function;
        }
        public int Count { get; set; }
        public FunctionSurf Function { get; set; }
        public delegate int FunctionSurf();
    }
    class ManagerSurf {
        public ManagerSurf() { }
        private List<Surf> ListSurf = new List<Surf>();
        public void AddFunction(Surf.FunctionSurf functionSurf)
        {
            ListSurf.Add(new Surf(functionSurf));
        }
        public void GoSurf()
        {
            bool b;
            do
            {
                b = false;
                for (int i = 0; i < ListSurf.Count; i++)
                {
                    if (ListSurf[i].Count > 5)
                    {
                        try{
                            ListSurf[i].Count = ListSurf[i].Function();
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                        b = true;
                    }
                }
            }
            while (b);
            for (int i = 0; i < ListSurf.Count; i++)
                ListSurf[i].Count = 10;
        }
    }
    public class BoundObject
    {
        private EventWaitHandle EventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        string Text { get; set; }
        public void ResetEvent() =>
            EventWaitHandle.Reset();
        public void Event(string text)
        {
            Text = text;
            EventWaitHandle.Set();
        }
        public string GetValue(int time = 20)
        {
            if (EventWaitHandle.WaitOne(time * 1000))
                return Text;
            else
                return "error";
        }
    }
    abstract class Site : MyTask
    {
        protected ManagerSurf mSurf = new ManagerSurf();
        public Form1 form;
        protected EventWaitHandle eventLoadPage = new EventWaitHandle(false, EventResetMode.ManualReset);
        protected EventWaitHandle eventBrowserCreated = new EventWaitHandle(false, EventResetMode.ManualReset);
        protected List<(IWebBrowser, IBrowser)> BrowserConrols = new List<(IWebBrowser, IBrowser)>();
        protected IBrowser? LastBrowser;
        protected string homePage = String.Empty;
        protected string HostName = String.Empty;
        public EnumTypeSite Type { get; protected set; }
        public ChromiumWebBrowser? main_browser;
        public MyLifeSplanHandler? lifeSplanHandler;
        protected Auth? auth;
        public TCPMessageManager TCPMessageManager;
        protected MySQL mySQL;
        protected ToolStripMenuItem menuItemSite;
        protected ToolStripComboBox siteStripComboBox;
        protected EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        public Site(Form1 form, Auth auth)
        {
            this.form = form;
            this.mySQL = form.mySQL;
            this.auth = auth;
            TCPMessageManager = new TCPMessageManager(form.ID, IPManager.GetEndPoint(mySQL, 1));
        }
        public Site(Form1 form)
        {
            this.form = form;
            this.mySQL = form.mySQL;
            TCPMessageManager = new TCPMessageManager(form.ID, IPManager.GetEndPoint(mySQL, 1));
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

            menuItemSite = new ToolStripMenuItem();
            menuItemSite.Name = Type.ToString();
            menuItemSite.Size = new System.Drawing.Size(180, 22);
            menuItemSite.Text = Type.ToString();

            siteStripComboBox = new ToolStripComboBox()
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = Type.ToString(),
                Size = new System.Drawing.Size(121, 23)
            };
            foreach (string flavourName in Enum.GetNames(typeof(StatusSite)))
                siteStripComboBox.Items.Add(flavourName);

            siteStripComboBox.SelectedIndexChanged += SiteStripComboBox_TextChanged;

            menuItemSite.DropDownItems.Add(siteStripComboBox);

            siteStripComboBox.Text = StatusSite.login.ToString();

            form.Invoke(new Action(() =>
            {
                form.waitToolStripMenuItem.DropDownItems.Add(menuItemSite);
                TabPage newTabPage = new TabPage();
                newTabPage.Text = Type.ToString();
                newTabPage.BorderStyle = BorderStyle.None;
                newTabPage.Dock = DockStyle.Fill;
                newTabPage.Controls.Add(main_browser);
                main_browser.Dock = DockStyle.Fill;
                form.tabControl1.TabPages.Add(newTabPage);
                form.tabControl1.SelectedTab = newTabPage;
            }));

            eventLoadPage.WaitOne(15000);
        }
        private void SiteStripComboBox_TextChanged(object? sender, EventArgs e)
        {
            ToolStripComboBox? item = sender as ToolStripComboBox;
            if (item != null)
            {
                switch ((StatusSite)item.SelectedIndex)
                {
                    case StatusSite.online:
                        waitHandle.Set();
                        break;
                }
            }
            else
            {
                MessageBox.Show("Error");
            }
        }
        protected void SetBDInfo(int val)
        {
            mySQL.SendSQL("UPDATE auth SET last_day = last_day + " + val.ToString() + " WHERE id_object = " + form.ID.ToString() + " , step = " + form.Step.ToString() + " , site = " + Type.ToString());
        }
        protected void SetBDInfoStart()
        {
            mySQL.SendSQL("UPDATE auth SET status = 'Surf' WHERE id_object = " + form.ID.ToString() + " , step = " + form.Step.ToString() + " , site = " + Type.ToString());
        }
        protected void SetBDInfoStop()
        {
            mySQL.SendSQL("UPDATE auth SET status = 'Activate' WHERE id_object = " + form.ID.ToString() + " , step = " + form.Step.ToString() + " , site = " + Type.ToString());
        }
        protected void WaitConnect()
        {

        }
        public void AfterCreated(IWebBrowser browserControl, IBrowser browser)
        {
            LastBrowser = browser;
            browserControl.LoadingStateChanged += Browser_LoadingStateChanged;
            BrowserConrols.Add((browserControl, browser));
            eventBrowserCreated.Set();
        }
        public void CloseBrowser(IBrowser browser)
        {
            for (int i = 0; i < BrowserConrols.Count; i++)
            {
                if (BrowserConrols[i].Item2.Identifier == browser.Identifier)
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
                    BrowserConrols.RemoveAt(i);
                    if (i != 0)
                    {
                        form.FocusTab(BrowserConrols[i - 1].Item2);
                    }
                    return;
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
            eventLoadPage.Reset();
            if (BrowserConrols.Count <= id)
            {
                eventBrowserCreated.Reset();
                if (!eventBrowserCreated.WaitOne(5000))
                    return null;
            }
            eventLoadPage.WaitOne(5000);
            Sleep(1);
            return BrowserConrols[id].Item2;
        }
        protected IBrowser? WaitCreateBrowser()
        {
            eventLoadPage.Reset();
            if (!eventBrowserCreated.WaitOne(15000))
                return null;
            eventLoadPage.WaitOne(15000);
            return LastBrowser;
        }
        protected void LoadPage(int id_browser, string page)
        {
            if (BrowserConrols.Count > id_browser)
            {
                LoadPage(BrowserConrols[id_browser].Item2, page);
            }
        }
        private void Active(IBrowser browser)
        {
            for (int i = 0; i < 10; i++)
                if (browser.IsValid)
                    return;
                else Thread.Sleep(500);
            throw new Exception("No valid browser");
        }
        private void Active(IFrame frame)
        {
            for (int i = 0; i < 10; i++)
                if (frame.IsValid)
                    return;
                else Thread.Sleep(500);
            throw new Exception("No valid frame");
        }
        protected void LoadPage(string page)
        {
            LoadPage(0, page);
        }
        protected void LoadPage(IBrowser browser, string page)
        {
            if (BrowserConrols.Find(item => item.Item2.Identifier == browser.Identifier).Item2 != null)
            {
                Active(browser);
                Console.WriteLine("---------------------------\nOpen: " + page);
                Console.WriteLine("Type: " + Type.ToString());
                Console.WriteLine("---------------------------");
                eventLoadPage.Reset();
                Active(browser.MainFrame);
                browser.MainFrame.LoadUrl(page);
                eventLoadPage.WaitOne();
            }
            else
                throw new Exception("Not valid browser");
        }
        protected string SendJSReturn(int id_browser, string JS)
        {
            if (BrowserConrols.Count > id_browser)
            {
                IFrame frame = BrowserConrols[id_browser].Item2.MainFrame;
                return SendJSReturn(frame, JS);
            }
            else return null;
        }
        protected void SendJS(int id_browser, string JS)
        {
            if (BrowserConrols.Count > id_browser)
            {
                IFrame frame = BrowserConrols[id_browser].Item2.MainFrame;
                SendJS(frame, JS);
            }
        }
        protected void SendJS(IFrame frame, string JS)
        {
            Active(frame);
            Console.WriteLine("---------------------------\nSend JS:");
            Console.WriteLine(JS);
            Console.WriteLine("Type: " + Type.ToString());
            Console.WriteLine("---------------------------");
            frame.ExecuteJavaScriptAsync(JS);
        }
        protected void SendJS(IBrowser browser, string JS)
        {
            Active(browser);
            SendJS(browser.MainFrame, JS);
        }
        protected string SendJSReturn(IFrame frame, string JS)
        {
            Active(frame);
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
            Active(browser);
            return SendJSReturn(browser.MainFrame, JS);
        }
        protected void Error(string text)
        {
            string Message = "---------------------------\nError: " +
            text +
            "\nType: " + Type.ToString() +
            "\n---------------------------";
            Console.WriteLine(Message);
            TCPMessageManager.SendError(Message, Type);
        }
        protected void Info(string text)
        {
            string Message = "---------------------------\nInfo: " +
            text +
            "\nType: " + Type.ToString() +
            "\n---------------------------";
            Console.WriteLine(Message);
            TCPMessageManager.SendInfo(Message, Type);
        }
        protected void Error(IBrowser browser, string text)
        {
            browser.GetSourceAsync().ContinueWith(v =>
            {
                string path = @"C:\ClickMashine\Settings\Errors\Pages\";
                File.WriteAllText(path + "html_page_error" + new DirectoryInfo(path).GetFiles().Length.ToString() + ".txt", v.Result);
            }).Wait();
            string Message = "---------------------------\nError: " +
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
            Task.Delay(sec * 1000).Wait();
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
                BrowserConrols[id_browser].Item2.GetHost().SendKeyEvent(eventKey);
            }
        }
        protected void CloseСhildBrowser()
        {
            for (int i = 1; i < BrowserConrols.Count; i++)
            {
                BrowserConrols[i].Item2.CloseBrowser(false);
            }
            Sleep(1);
        }
        protected void CloseAllBrowser()
        {
            for (int i = 0; i < BrowserConrols.Count; i++)
            {
                BrowserConrols[i].Item2.CloseBrowser(false);
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
            string JS =
@"function WaitElement()
{
    var element = " + element + @";
    if (element != null) { return 'end'; }
    else return 'wait';
}
WaitElement();";
            for (int i = 0; i < sec; i++)
            {
                string? ev_js_wait = SendJSReturn(frame, JS);
                if (ev_js_wait == null)
                    continue;
                if (ev_js_wait == "end")
                    return true;
                Thread.Sleep(1000);
            }
            return false;
        }
        protected bool WaitElement(IBrowser browser, string element, int sec = 10)
        {
            string JS =
@"function WaitElement()
{
    var element = " + element + @";
    if (element != null) { return 'end'; }
    else return 'wait';
}
WaitElement();";
            for (int i = 0; i < sec; i++)
            {
                string? ev_js_wait = SendJSReturn(browser, JS);
                if (ev_js_wait == null)
                    continue;
                if (ev_js_wait == "end")
                    return true;
                Thread.Sleep(1000);
            }
            return false;
        }
        protected bool WaitElement(IBrowser browser, string nameFrame, string element, int sec = 10)
        {
            string JS =
@"function WaitElement()
{
    var element = " + element + @";
    if (element != null) { return 'end'; }
    else return 'wait';
}
WaitElement();";
            for (int i = 0; i < sec; i++)
            {
                string ev_js_wait = SendJSReturn(browser.GetFrame(nameFrame), JS);
                if (ev_js_wait == "end")
                    return true;
                Thread.Sleep(1000);
            }
            return false;
        }
        protected string WaitFunction(IFrame frame, string function, string? functionStart = null, int time = 5)
        {
            if (functionStart != null)
                SendJS(frame, functionStart);
            for (int j = 0; j < time; j++)
            {
                string ev = SendJSReturn(frame, function);
                if (ev == "wait")
                    Sleep(1);
                else return ev;
            }
            return "errorWait";
        }
        protected string WaitFunction(IBrowser browser, string nameFrame, string function, int sec = 5)
        {
            for (int j = 0; j < sec; j++)
            {
                string ev = SendJSReturn(browser.GetFrame(nameFrame), function);
                if (ev == "wait")
                    Sleep(1);
                else return ev;
            }
            return "errorWait";
        }
        protected void WaitChangeElement(IBrowser browser, BoundObject boundObject, string element)
        {
            var control = BrowserConrols.Find(item => item.Item2.Identifier == browser.Identifier);
            if (control.Item1 != null)
            {
                control.Item1.JavascriptObjectRepository.Register("boundAsync", boundObject, BindingOptions.DefaultBinder);
                string js =
@"var target = " + element + @";
const config = {
  childList: true,
        attributes: true,
        subtree: true,
        characterData: true,
        attributeOldValue: true,
        characterDataOldValue: true
};
const callback = function (mutationsList, observer) {
(async function()
{
	await CefSharp.BindObjectAsync(""boundAsync"");
	boundAsync.event(target.innerText);
})();
};
const observer = new MutationObserver(callback);
observer.observe(target, config);
";
                SendJS(control.Item2, js);
            }
            else
                throw new Exception("Not valid browser");
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
        protected string GetMoney(IBrowser browser, string selector)
        {
            string js =
@"var ballans = " + selector + @";
if(ballans != null)
    ballans.innerText;
else 'error';";
            string ev = SendJSReturn(browser.MainFrame, js);
            if (ev != "error")
                Info("Money: " + ev);
            return ev;
        }
        protected void SaveImage(Bitmap bmp)
        {
            string path = @"C:\ClickMashine\Settings\Image\" + Type.ToString() + @"\";
            Directory.CreateDirectory(path);
            bmp.Save(path + new DirectoryInfo(path).GetFiles().Length.ToString() + ".png");
        }
        protected void SaveHistoryCaptcha1(List<(Bitmap, PredictNN)> historyCaptcha, List<string> enumString)
        {
            string path = $"{Form1.PATH_SETTING}Image/Errors/{Type.ToString()}/{DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss")}/";
            Directory.CreateDirectory(path);
            for (int i = 0; i < historyCaptcha.Count; i++)
            {
                string file = "";
                (Bitmap, PredictNN) captcha = historyCaptcha[i];
                captcha.Item1.Save(path + i.ToString() + ".png");
                var tensor = captcha.Item2.Tensor.ToArray<float>();
                for (int j = 0; j < tensor.Length; j++)
                    file += enumString[j] + " : " + tensor[j].ToString() + Environment.NewLine;
                File.WriteAllText(path + $"debug{i}.txt", file);
            }
        }
        protected void SaveHistoryCaptcha(List<(Bitmap, PredictNN)> historyCaptcha, List<string> enumString, string value)
        {
            string path = $"{Form1.PATH_SETTING}Image/Errors/{Type.ToString()}/{DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss")}/";
            Directory.CreateDirectory(path);
            for (int i = 0; i < historyCaptcha.Count; i++)
            {
                string file = $"{value}{Environment.NewLine}----------------------------{Environment.NewLine}";
                (Bitmap, PredictNN) captcha = historyCaptcha[i];
                captcha.Item1.Save(path + i.ToString() + ".png");
                var tensor = captcha.Item2.Tensor.ToArray<float>();
                for (int j = 0; j < tensor.Length; j++)
                    file += enumString[j] + " : " + tensor[j].ToString() + Environment.NewLine;
                File.WriteAllText(path + $"debug{i}.txt", file);
            }
        }
        protected void AccountBlock()
        {
            mySQL.SendSQL("UPDATE auth SET status = 'Block' WHERE id_object = " + form.ID.ToString() + " , step = " + form.Step.ToString() + " , site = " + Type.ToString());
            MessageBox.Show("Account block");
        }
        protected void GetTrainBD(IBrowser browser, string title, string element, string reload, int countElement, int count)
        {
            string path = @"C:\ClickMashine\Settings\Image\" + Type.ToString() + @"\";
            for (int i = 0; i < count; i++)
            {
                string name = SendJSReturn(browser.MainFrame, title + ".innerText");
                string[] name_item = name.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Directory.CreateDirectory(path + name_item[name_item.Length - 1] + @"\");
                SendJS(0, element + ".forEach((element) => element.style.border = '0px');");
                Sleep(1);
                for (int j = 0; j < countElement; j++)
                {
                    //PredictNN predict = nn.Predict(GetImgBrowser(Browsers[0].MainFrame, "document.querySelectorAll('.out-capcha-lab')[" + j.ToString() + "]"));
                    //foreach (var v in predict.Tensor.numpy())
                    //{
                    //    foreach (var v2 in v)
                    //        Console.WriteLine(v2.ToString());
                    //}
                    //Console.WriteLine((ProfiCentrEnumNN)predict.Num);
                    //Console.ReadLine();
                    GetImgBrowser(browser.MainFrame, element + "[" + j.ToString() + "]")
                        .Save(path + new DirectoryInfo(path).GetFiles().Length.ToString() + ".png");
                }
                SendJS(0, reload + ".click();");
                Sleep(2);
            }
        }
        protected byte[] GetScreenAsync()
        {
            Console.WriteLine(23123);
            var devToolsClient = BrowserConrols[0].Item2.GetDevToolsClient();
            
                Console.WriteLine(23123);
                var result = devToolsClient.Page.CaptureScreenshotAsync().Result;
                Console.WriteLine(23123);
                Console.WriteLine(result.ToString());
                Console.WriteLine(23123);
                Console.WriteLine(result.Data);
                Console.WriteLine(23123);
                return result.Data;

        }
        protected List<(Bitmap, PredictNN)> OutCaptchaLab1(IBrowser browser, NN nn, List<string> EnumValues, string title, string captcha, string input, int countInput, string button)
        {
            string js =
@"var img_captcha = " + captcha + @";
if(img_captcha != null)
    'captcha';
else 'ok';";
            string nameImage = SendJSReturn(browser, $"{title}.innerText;");
            string? value = EnumValues.Find(item => nameImage.IndexOf(item) != -1);
            SendJS(browser, $"{input}.forEach((element) => element.style.border = '0px');");
            Sleep(2);
            List<(Bitmap, PredictNN)> imageHistoryPredict = new List<(Bitmap, PredictNN)>();
            for (int i = 0; i < countInput; i++)
            {
                Bitmap image = GetImgBrowser(browser.MainFrame, $"{input}[" + i.ToString() + "]");
                PredictNN predict = nn.Predict(image);
                imageHistoryPredict.Add((image, predict));
                if (value == EnumValues[predict.Num])
                    SendJS(browser, $"{input}[" + i + "].querySelector('input').checked = true;");
            }
            SendJS(browser, $"{button}.click();");
            return imageHistoryPredict;
        }
        protected StatusCaptcha OutCaptchaLab(IBrowser browser, NN nn, List<string> EnumValues, string title, string captcha, string input, int countInput, string button, string information, string? reload = null)
        {
            string js =
@"var img_captcha = " + captcha + @";
if(img_captcha != null)
    'captcha';
else 'ok';";
            if (SendJSReturn(browser.MainFrame, js) != "captcha")
                return StatusCaptcha.OK;           
            BoundObject boundObject = new BoundObject();
            WaitChangeElement(browser, boundObject, information);
            for (int iteration = 0; iteration < 10; iteration++)
            {
                string nameImage = SendJSReturn(browser, $"{title}.innerText;");
                string? value = EnumValues.Find(item => nameImage.IndexOf(item) != -1);
                if (value == null)
                    return StatusCaptcha.ERROR;
                SendJS(browser, $"{input}.forEach((element) => element.style.border = '0px');");
                Sleep(2);
                List<(Bitmap, PredictNN)> imageHistoryPredict = new List<(Bitmap, PredictNN)>();
                for(int i =0; i < countInput; i++)
                {
                    Bitmap image = GetImgBrowser(browser.MainFrame, $"{input}[" + i.ToString() + "]");
                    PredictNN predict = nn.Predict(image);
                    imageHistoryPredict.Add((image, predict));
                    if (value == EnumValues[predict.Num])
                        SendJS(browser, $"{input}[" + i + "].querySelector('input').checked = true;");
                }
                boundObject.ResetEvent();
                SendJS(browser, $"{button}.click();");
                string ev = boundObject.GetValue();
                if (ev == "error")
                {
                    SaveHistoryCaptcha(imageHistoryPredict, EnumValues, value);
                }
                else if (ev == "Нужно подтвердить, что Вы не робот!")
                {
                    SaveHistoryCaptcha(imageHistoryPredict, EnumValues, value);
                }
                else if (ev == "Ваш аккаунт заблокирован")
                {
                    AccountBlock();
                    return StatusCaptcha.BLOCK;
                }
                else
                    return StatusCaptcha.OK;
                if(reload != null)
                    SendJS(browser, $"{reload}.click();");
                Sleep(3);
            }
            return StatusCaptcha.ERROR;
        }
    }
}