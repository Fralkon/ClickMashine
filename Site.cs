using CefSharp;
using CefSharp.WinForms;
using System.Text.Json;
using ClickMashine.Models;
using ClickMashine.Exceptions;
using ClickMashine.Api;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

namespace ClickMashine
{
    abstract class Site
    {
        protected ManagerSurfing ManagerSurfing = new ManagerSurfing();
        public MainForm form;
        public EventWaitHandle eventLoadPage = new EventWaitHandle(false, EventResetMode.ManualReset);
        public EventWaitHandle eventBrowserCreated = new EventWaitHandle(false, EventResetMode.ManualReset);
        protected List<(IWebBrowser, IBrowser)> BrowserConrols = new List<(IWebBrowser, IBrowser)>();
        protected IBrowser? LastBrowser;
        protected string homePage = String.Empty;
        protected string HostName = String.Empty;
        public EnumTypeSite Type { get; protected set; }
        public ChromiumWebBrowser? main_browser;
        public MyLifeSplanHandler? lifeSplanHandler;
        protected AuthData? auth;
        public TCPMessageManager TCPMessageManager;
        protected ToolStripMenuItem menuItemSite;
        protected ToolStripComboBox siteStripComboBox;
        public CancellationTokenSource cancellationToken { get; set; } = new CancellationTokenSource();

        protected TaskCompletionSource<bool> waitHandle = new TaskCompletionSource<bool>();
        protected ApiClient ApiClient { get; set; }
        public Site(MainForm form, AuthData auth) : base()
        {
            this.form = form;
            this.auth = auth;
            //TCPMessageManager = new TCPMessageManager(form.ID, IPManager.GetEndPoint(mySQL, 1));
            ApiClient = new ApiClient();
        }
        public abstract Task<bool> Auth(AuthData auth);
        public async Task StartSurf()
        {
            Initialize();

            if (!await Auth(auth))
            {
                if (!await waitHandle.Task)
                    return;
            }

            //while (!cancellationToken.IsCancellationRequested) // Используем токен отмены
            //{
            //    await ManagerSurfing.StartSurf();
            //    await Task.Delay(600);
            //}
        }
        public void Stop()
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("[Warning] Задача не запущена или уже остановлена.");
                return;
            }

            cancellationToken.Cancel();
        }
        protected virtual void Initialize()
        {
            lifeSplanHandler = new MyLifeSplanHandler(this);
            main_browser = new ChromiumWebBrowser(homePage);
            main_browser.LifeSpanHandler = lifeSplanHandler;

            menuItemSite = new ToolStripMenuItem();
            menuItemSite.Name = Type.ToString();
            menuItemSite.Size = new Size(180, 22);
            menuItemSite.Text = Type.ToString();

            siteStripComboBox = new ToolStripComboBox()
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = Type.ToString(),
                Size = new Size(121, 23)
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
        protected string LoadJSOnFileBase(string nameFile,params object[] args)
        {
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sites", nameFile + ".js");

            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"Файл {scriptPath} не найден.");

            string script = File.ReadAllText(scriptPath);

            // Подставляем значения enum StatusJS
            foreach (StatusJS status in Enum.GetValues(typeof(StatusJS)))
            {
                script = script.Replace($"STATUS_{status.ToString().ToUpper()}", ((int)status).ToString());
            }
            script = Regex.Replace(script, @"\{(\d+)\}", m =>
            {
                int index = int.Parse(m.Groups[1].Value);
                return index < args.Length ? args[index].ToString() : m.Value;
            });
            return script;
        }
        protected string LoadJSOnFile(string nameFile, params object[] args)
        {
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sites", Type.ToString(), nameFile + ".js");

            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"Файл {scriptPath} не найден.");

            string script = File.ReadAllText(scriptPath, Encoding.UTF8);

  
            // Подставляем значения enum StatusJS
            foreach (StatusJS status in Enum.GetValues(typeof(StatusJS)))
            {
                script = script.Replace($"STATUS_{status.ToString().ToUpper()}", ((int)status).ToString());
            }
            script = Regex.Replace(script, @"\{(\d+)\}", m =>
            {
                int index = int.Parse(m.Groups[1].Value);
                return index < args.Length ? args[index].ToString() : m.Value;
            });

            return script;
        }
        private void SiteStripComboBox_TextChanged(object? sender, EventArgs e)
        {
            ToolStripComboBox? item = sender as ToolStripComboBox;
            if (item != null)
            {
                switch ((StatusSite)item.SelectedIndex)
                {
                    case StatusSite.online:
                        waitHandle.TrySetResult(true); // Разблокировать ожидание
                        break;
                }
            }
            else
            {
                MessageBox.Show("Error");
            }
        }
        public async Task<StatusJS> InjectJSAsync(IFrame frame, string JS)
        {
            string JS_TRY = LoadJSOnFileBase("injectJS" ,JS);

#if DEBUG
            Console.WriteLine("---------------------------\nSend JS:");
            Console.WriteLine(JS_TRY);
            Console.WriteLine("Type: " + Type);
#endif

            try
            {
                var task = await frame.EvaluateScriptAsync(JS_TRY);
                if (task.Result != null && task.Result is int result)
                {
                    StatusJS statusJS = (StatusJS)result;

#if DEBUG
                    Console.WriteLine("Return: " + statusJS);
                    Console.WriteLine("---------------------------");
#endif

                    if (statusJS == StatusJS.TryError)
                        throw new ExceptionJS(Type, JS, task.Message);

                    return statusJS;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JS Execution Error: {ex.Message}");
            }

            return StatusJS.None;
        }
        public async Task<string> ValueElementAsync(IFrame frame, string element)
        {
            string JS = 
            @"try{
                " + element + @"
            }
            catch(e)
            {
                '" + StatusJS.TryError + @"';
            }";
#if DEBUG
            Console.WriteLine("---------------------------\nSend JS:");
            Console.WriteLine(JS);
            Console.WriteLine("Type: " + Type.ToString());
#endif
            var result = await frame.EvaluateScriptAsync(JS);
            if (result.Result != null)
            {
#if DEBUG
                Console.WriteLine("Return: " + result.Result.ToString());
                Console.WriteLine("---------------------------");
#endif
                if (result.Result.ToString() == StatusJS.TryError.ToString())
                {
                    throw new Exception("Type: " + Type.ToString() + "\nError JS");
                }
                string? val = result.Result.ToString();
                if (val != null)
                    return val;
            }
            throw new ExceptionJS(Type, JS, result.Message);
        }
        public async Task<string> ValueElementAsync(IBrowser browser, string element)
        {
            await ActiveAsync(browser);
            return await ValueElementAsync(browser.MainFrame, element);
        }
        public async Task<StatusJS> InjectJSAsync(IBrowser browser, string JS)
        {
            await ActiveAsync(browser);
            return await InjectJSAsync(browser.MainFrame, JS);
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
        public async Task<IBrowser> GetBrowserAsync(int id)
        {
            eventLoadPage.Reset();
            if (BrowserConrols.Count <= id)
            {
                eventBrowserCreated.Reset();
                if (!await Task.Run(() => eventBrowserCreated.WaitOne(15000)))
                {
                    Console.WriteLine("Ошибка: браузер не был создан за 15 секунд!");
                    throw new Exception();
                }
            }

            await Task.Delay(100); // Минимальная задержка для избежания deadlock
            bool loaded = await Task.Run(() => eventLoadPage.WaitOne(5000));

            if (!loaded)
            {
                Console.WriteLine("Ошибка: страница не загрузилась за 5 секунд!");
            }

            return BrowserConrols[id].Item2;
        }
        public async Task<IBrowser?> WaitCreateBrowserAsync()
        {
            eventLoadPage.Reset();

            var browserCreated = await Task.Run(() => eventBrowserCreated.WaitOne(15000));
            if (!browserCreated)
                return null;

            var pageLoaded = await Task.Run(() => eventLoadPage.WaitOne(15000));
            return pageLoaded ? LastBrowser : null;
        }
        public async Task LoadPageAsync(int id_browser, string page)
        {
            if (id_browser < BrowserConrols.Count)
            {
                await LoadPageAsync(BrowserConrols[id_browser].Item2, page);
            }
            else
            {
                throw new Exception("Browser index out of range");
            }
        }
        public async Task ActiveAsync(IBrowser browser, int timeoutMs = 5000, int checkIntervalMs = 500)
        {
            int elapsedTime = 0;

            while (elapsedTime < timeoutMs)
            {
                if (browser.IsValid)
                    return;

                await Task.Delay(checkIntervalMs);
                elapsedTime += checkIntervalMs;
            }

            throw new Exception("No valid browser");
        }
        public async Task ActiveAsync(IFrame frame, int timeoutMs = 5000, int checkIntervalMs = 500)
        {
            int elapsedTime = 0;

            while (elapsedTime < timeoutMs)
            {
                if (frame.IsValid)
                    return;

                await Task.Delay(checkIntervalMs);
                elapsedTime += checkIntervalMs;
            }

            throw new Exception("No valid frame");
        }

        public Task LoadPageAsync(string page)
        {
            return LoadPageAsync(0, page);
        }
        public async Task LoadPageAsync(IBrowser browser, string page)
        {
            var browserItem = BrowserConrols.Find(item => item.Item2.Identifier == browser.Identifier);
            if (browserItem.Item2 == null)
                throw new Exception("Not valid browser");

            await ActiveAsync(browser);

            Console.WriteLine("---------------------------\nOpen: " + page);
            Console.WriteLine("Type: " + Type.ToString());
            Console.WriteLine("---------------------------");

            await ActiveAsync(browser.MainFrame);
            eventLoadPage.Reset();
            browser.MainFrame.LoadUrl(page);
            eventLoadPage.WaitOne();
        }

        public async Task<string?> SendJSReturnAsync(int id_browser, string JS)
        {
            if (BrowserConrols.Count > id_browser)
            {
                IFrame frame = BrowserConrols[id_browser].Item2.MainFrame;
                return await SendJSReturnAsync(frame, JS);
            }
            return null;
        }

        public Task SendJSAsync(int id_browser, string JS)
        {
            if (BrowserConrols.Count <= id_browser)
                throw new ArgumentOutOfRangeException(nameof(id_browser), "Некорректный индекс браузера");

            IFrame frame = BrowserConrols[id_browser].Item2.MainFrame;
            return SendJSAsync(frame, JS);
        }
        public async Task SendJSAsync(IFrame frame, string JS)
        {
            await ActiveAsync(frame);
            Console.WriteLine("---------------------------\nSend JS:");
            Console.WriteLine(JS);
            Console.WriteLine("Type: " + Type.ToString());
            Console.WriteLine("---------------------------");
            frame.ExecuteJavaScriptAsync(JS);
        }
        public async Task SendJSAsync(IBrowser browser, string JS)
        {
            await ActiveAsync(browser);
            await SendJSAsync(browser.MainFrame, JS);
        }
        public async Task<string> SendJSReturnAsync(IFrame frame, string JS)
        {
            await ActiveAsync(frame);
            string JS_TRY = $"try{{\n{JS}\n}}catch(e){{'error';}}";

            Console.WriteLine("---------------------------\nSend JS:");
            Console.WriteLine(JS_TRY);
            Console.WriteLine("Type: " + Type.ToString());

            var task = await frame.EvaluateScriptAsync(JS_TRY);

            if (task.Result != null)
            {
                string? result = task.Result.ToString();

                if (result == "error")
                {
                    throw new Exception($"Type: {Type}\nError JS");
                }

                Console.WriteLine("Return: " + result);
                Console.WriteLine("---------------------------");
                if(result != null)
                    return result;
            }

            Console.WriteLine("Not return!!!!!!!");
            Console.WriteLine("---------------------------");
            throw new ArgumentNullException();
        }

        public async Task<string> SendJSReturnAsync(IBrowser browser, string JS)
        {
            await ActiveAsync(browser);
            return await SendJSReturnAsync(browser.MainFrame, JS);
        }
        public void Error(string text)
        {
            string Message = "---------------------------\nError: " +
            text +
            "\nType: " + Type.ToString() +
            "\n---------------------------";
            Console.WriteLine(Message);
        }
        public void Info(string text)
        {
            string Message = "---------------------------\nInfo: " +
            text +
            "\nType: " + Type.ToString() +
            "\n---------------------------";
            Console.WriteLine(Message);
            TCPMessageManager.SendInfo(Message, Type);
        }
        public void Error(IBrowser browser, string text)
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
        public void CM(string text)
        {
            Console.WriteLine("---------------------------");
            Console.WriteLine(text);
            Console.WriteLine("Type: " + Type.ToString());
            Console.WriteLine("---------------------------");
        }
        public async Task SleepAsync(int sec)
        {
#if DEBUG
            Console.WriteLine("---------------------------");
            Console.WriteLine("Sleep: " + sec);
            Console.WriteLine("Type: " + Type.ToString());
            Console.WriteLine("---------------------------");
#endif
            await Task.Delay(sec * 1000);
        }

        public async Task SleepAsync(string sec)
        {
            if (int.TryParse(sec, out int seconds))
            {
                await SleepAsync(seconds);
            }
            else
            {
                Console.WriteLine("Ошибка: не удалось распознать число.");
            }
        }

        public async Task<bool> WaitTimeAsync(IFrame frame, string element)
        {
            string sec;
            try
            {
                sec = await ValueElementAsync(frame, element);
            }
            catch
            {
#if DEBUG
                Console.WriteLine("---------------------------");
                Console.WriteLine("Error Sleep noot return");
                Console.WriteLine("Type: " + Type.ToString());
                Console.WriteLine("---------------------------");
#endif
                return false;
            }
#if DEBUG
            Console.WriteLine("---------------------------");
            Console.WriteLine("Sleep: " + sec);
            Console.WriteLine("Type: " + Type.ToString());
            Console.WriteLine("---------------------------");
#endif
            Task.Delay(int.Parse(sec) * 1000).Wait();
            return true;
        }
        public async Task<bool> WaitTimeAsync(IBrowser browser, string element)
        {
            await ActiveAsync(browser);
            return await WaitTimeAsync(browser.MainFrame, element);
        }
        public void SendKeysEvent(int id_browser, string text)
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
        public void CloseСhildBrowser()
        {
            for (int i = 1; i < BrowserConrols.Count; i++)
            {
                BrowserConrols[i].Item2.CloseBrowser(false);
            }
            SleepAsync(1).Wait();
        }
        public void CloseAllBrowser()
        {
            for (int i = 0; i < BrowserConrols.Count; i++)
            {
                BrowserConrols[i].Item2.CloseBrowser(false);
            }
            SleepAsync(1).Wait();
        }
        public async Task<bool> WaitButtonClickAsync(IFrame frame, string element, int sec = 10)
        {
            string js_wait = LoadJSOnFileBase("waitButtonClick", element);

            await InjectJSAsync(frame, js_wait); // Асинхронная вставка JS

            for (int i = 0; i < sec; i++)
            {
                StatusJS status = await InjectJSAsync(frame, "wait_element();"); // Асинхронный вызов JS

                if (status == StatusJS.OK)
                    return true;
                else if (status == StatusJS.Wait)
                    await Task.Delay(1000); // Асинхронная задержка
            }

            return false;
        }

        public async Task<bool> WaitButtonClickAsync(IBrowser browser, string element, int sec = 10)
        {
            await ActiveAsync(browser);
            return await WaitButtonClickAsync(browser.MainFrame, element, sec);
        }
        public async Task<string> GetMailAnswerAsync(IFrame frame, string mail, string question, string answer, int sec = 5)
        {
            string js = LoadJSOnFileBase("getMailAnswer", mail, answer, question);
            for (int i = 0; i < sec; i++)
            {
                string ev = await SendJSReturnAsync(frame, js);
                if (ev == ((int)StatusJS.Wait).ToString())
                    await SleepAsync(1);
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
        public async Task<bool> WaitElementAsync(IFrame frame, string element, int sec = 10)
        {
            string JS = LoadJSOnFileBase("waiitElement");
            for (int i = 0; i < sec; i++)
            {
                switch (await InjectJSAsync(frame, JS))
                {
                    case StatusJS.Wait:
                        break;
                    case StatusJS.OK:
                        return true;
                }
                await Task.Delay(1000);
            }
            return false;
        }
        public Task<bool> WaitElementAsync(IBrowser browser, string element, int sec = 10)
        {
            return WaitElementAsync(browser.MainFrame, element, sec);
        }
        public async Task<bool> WaitElementAsync(IBrowser browser, string nameFrame, string element, int sec = 10)
        {
            IFrame frame = browser.GetFrameByName(nameFrame);
            await ActiveAsync(frame);
            return await WaitElementAsync(frame, element, sec);          
        }
        public async Task<string> WaitFunctionAsync(IFrame frame, string function, string? functionStart = null, int time = 5)
        {
            if (functionStart != null)
                await SendJSAsync(frame, functionStart);
            for (int j = 0; j < time; j++)
            {
                string ev = await SendJSReturnAsync(frame, function);
                if (ev == "wait")
                    await SleepAsync(1);
                else return ev;
            }
            return "errorWait";
        }
        public Task<string> WaitFunctionAsync(IBrowser browser, string function, string? functionStart = null, int time = 5)
        {
            return WaitFunctionAsync(browser.MainFrame, function, functionStart, time);
        }
        public async Task<StatusJS> FunctionWaitAsync(IFrame frame, string function, string? functionStart = null, int time = 5)
        {
            if (functionStart != null)
                await InjectJSAsync(frame, functionStart); // Асинхронный вызов JS

            for (int j = 0; j < time; j++)
            {
                StatusJS status = await InjectJSAsync(frame, function); // Ожидание результата

                if (status == StatusJS.Wait)
                    await Task.Delay(1000); // Асинхронная задержка
                else
                    return status;
            }

            return StatusJS.ErrorWait;
        }

        public Task<StatusJS> FunctionWaitAsync(IBrowser browser, string function, string? functionStart = null, int time = 5)
        {
            return FunctionWaitAsync(browser.MainFrame, function, functionStart, time);
        }
        public async Task WaitChangeElementAsync(IBrowser browser, BoundObject boundObject, string element)
        {
            var control = BrowserConrols.Find(item => item.Item2.Identifier == browser.Identifier);

            if (control.Item1 != null)
            {
                control.Item1.JavascriptObjectRepository.Register("boundAsync", boundObject, BindingOptions.DefaultBinder);
                string js = LoadJSOnFileBase("boundAsync", element);

                await InjectJSAsync(control.Item2, js); // Асинхронный вызов InjectJS
            }
            else
            {
                throw new Exception("Not valid browser");
            }
        }
        public async Task<Bitmap> GetImgBrowserAsync(IFrame frame, string element)
        {
            if (await WaitElementAsync(frame, element))
            {
                string js = LoadJSOnFileBase("getImgBrowser", element);
                string rec = await ValueElementAsync(frame, js);
                Rectangle rectElement = JsonSerializer.Deserialize<Rectangle>(rec);
                return form.MakeScreenshot(frame.Browser, rectElement);
            }
            else
                throw new Exception("Ошибка ожидания элемента для скриншота");
        }
        //public void SaveImage(Bitmap bmp)
        //{
        //    string path = @"C:\ClickMashine\Settings\Image\" + Type.ToString() + @"\";
        //    Directory.CreateDirectory(path);
        //    bmp.Save(path + new DirectoryInfo(path).GetFiles().Length.ToString() + ".png");
        //}
        //public void SaveHistoryCaptcha1(List<(Bitmap, PredictNN)> historyCaptcha, List<string> enumString)
        //{
        //    string path = $"{Form1.PATH_SETTING}Image/Errors/{Type}/{DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss")}/";
        //    Directory.CreateDirectory(path);
        //    for (int i = 0; i < historyCaptcha.Count; i++)
        //    {
        //        string file = "";
        //        (Bitmap, PredictNN) captcha = historyCaptcha[i];
        //        captcha.Item1.Save(path + i.ToString() + ".png");
        //        var tensor = captcha.Item2.Tensor.ToArray<float>();
        //        for (int j = 0; j < tensor.Length; j++)
        //            file += enumString[j] + " : " + tensor[j].ToString() + Environment.NewLine;
        //        File.WriteAllText(path + $"debug{i}.txt", file);
        //    }
        //}
        //public void SaveHistoryCaptcha(List<(Bitmap, PredictNN)> historyCaptcha, List<string> enumString, string value)
        //{
        //    string path = $"{Form1.PATH_SETTING}Image/Errors/{Type}/{DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss")}/";
        //    Directory.CreateDirectory(path);
        //    for (int i = 0; i < historyCaptcha.Count; i++)
        //    {
        //        string file = $"{value}{Environment.NewLine}----------------------------{Environment.NewLine}";
        //        (Bitmap, PredictNN) captcha = historyCaptcha[i];
        //        captcha.Item1.Save(path + i.ToString() + ".png");
        //        var tensor = captcha.Item2.Tensor.ToArray<float>();
        //        for (int j = 0; j < tensor.Length; j++)
        //            file += enumString[j] + " : " + tensor[j].ToString() + Environment.NewLine;
        //        File.WriteAllText(path + $"debug{i}.txt", file);
        //    }
        //}
        //public void AccountBlock()
        //{
        //    mySQL.SendSQL("UPDATE auth SET status = 'Block' WHERE id_object = " + form.ID.ToString() + " , step = " + form.Step.ToString() + " , site = " + Type.ToString());
        //    MessageBox.Show("Account block");
        //}
        //public void GetTrainBD(IBrowser browser, string title, string element, string reload, int countElement, int count)
        //{
        //    string path = @"C:\ClickMashine\Settings\Image\" + Type.ToString() + @"\";
        //    for (int i = 0; i < count; i++)
        //    {
        //        string name = SendJSReturn(browser.MainFrame, title + ".innerText");
        //        string[] name_item = name.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //        Directory.CreateDirectory(path + name_item[name_item.Length - 1] + @"\");
        //        SendJS(0, element + ".forEach((element) => element.style.border = '0px');");
        //        Sleep(1);
        //        for (int j = 0; j < countElement; j++)
        //        {
        //            //PredictNN predict = nn.Predict(GetImgBrowser(Browsers[0].MainFrame, "document.querySelectorAll('.out-capcha-lab')[" + j.ToString() + "]"));
        //            //foreach (var v in predict.Tensor.numpy())
        //            //{
        //            //    foreach (var v2 in v)
        //            //        Console.WriteLine(v2.ToString());
        //            //}
        //            //Console.WriteLine((ProfiCentrEnumNN)predict.Num);
        //            //Console.ReadLine();
        //            //GetImgBrowser(browser.MainFrame, element + "[" + j.ToString() + "]")
        //            //    .Save(path + new DirectoryInfo(path).GetFiles().Length.ToString() + ".png");
        //        }
        //        SendJS(0, reload + ".click();");
        //        Sleep(2);
        //    }
        //}
        public byte[] GetScreenAsync()
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
        //        public List<(Bitmap, PredictNN)> OutCaptchaLab1(IBrowser browser, NN nn, List<string> EnumValues, string title, string captcha, string input, int countInput, string button)
        //        {
        //            string js =
        //@"var img_captcha = " + captcha + @";
        //if(img_captcha != null)
        //    'captcha';
        //else 'ok';";
        //            string nameImage = SendJSReturn(browser, $"{title}.innerText;");
        //            string? value = EnumValues.Find(item => nameImage.IndexOf(item) != -1);
        //            SendJS(browser, $"{input}.forEach((element) => element.style.border = '0px');");
        //            Sleep(2);
        //            List<(Bitmap, PredictNN)> imageHistoryPredict = new List<(Bitmap, PredictNN)>();
        //            for (int i = 0; i < countInput; i++)
        //            {
        //                var task = GetImgBrowser(browser.MainFrame, $"{input}[" + i.ToString() + "]");
        //                task.Wait();
        //                Bitmap image = task.Result;
        //                PredictNN predict = nn.Predict(image);
        //                imageHistoryPredict.Add((image, predict));
        //                if (value == EnumValues[predict.Num])
        //                    SendJS(browser, $"{input}[" + i + "].querySelector('input').checked = true;");
        //            }
        //            SendJS(browser, $"{button}.click();");
        //            return imageHistoryPredict;
        //        }
        public async Task<StatusJS> OutCaptchaLabAsync(
          IBrowser browser,
          List<string> EnumValues,
          string title,
          string captcha,
          string input,
          int countInput,
          string button,
          string information,
          string? reload = null)
        {
            string js = @$"var img_captcha = {captcha};
                   if(img_captcha != null) 
                       {(int)StatusJS.Error};
                   else {(int)StatusJS.OK};";

            if (await InjectJSAsync(browser, js) == StatusJS.OK)
                return StatusJS.OK;

            BoundObject boundObject = new BoundObject();
            await WaitChangeElementAsync(browser, boundObject, information);

            for (int iteration = 0; iteration < 10; iteration++)
            {
                string nameImage = await SendJSReturnAsync(browser, $"{title}.innerText;");
                string? value = EnumValues.Find(item => nameImage.IndexOf(item, StringComparison.OrdinalIgnoreCase) != -1);

                if (value == null)
                    return StatusJS.Error;

                await SendJSAsync(browser, $"{input}.forEach((element) => element.style.border = '0px');");
                await Task.Delay(2000);

                List<Bitmap> imageList = new();
                for (int i = 0; i < countInput; i++)
                {
                    var img = await GetImgBrowserAsync(browser.MainFrame, $"{input}[{i}]");
                    imageList.Add(img);
                }

                ImageDecode imageDecode = new()
                {
                    Files = imageList,
                    Site = Type,
                    ImageCategory = 1
                };

                await ApiClient.SendBitmapsAsync(imageDecode);

                if (value == EnumValues[1])
                    await SendJSAsync(browser, $"{input}[1].querySelector('input').checked = true;");

                boundObject.ResetEvent();
                await SendJSAsync(browser, $"{button}.click();");

                string ev = boundObject.GetValue();
                if (ev == "error" || ev == "Нужно подтвердить, что Вы не робот!")
                {
                    // SaveHistoryCaptcha(imageHistoryPredict, EnumValues, value);
                }
                else if (ev == "Ваш аккаунт заблокирован")
                {
                    return StatusJS.Block;
                }
                else
                    return StatusJS.OK;

                if (reload != null)
                    await SendJSAsync(browser, $"{reload}.click();");

                await Task.Delay(3000);
            }

            return StatusJS.Error;
        }
    }
}