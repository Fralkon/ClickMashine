using CefSharp;
using ClickMashine.Exceptions;
using ClickMashine.Models;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace ClickMashine
{
    enum SurfingType
    {
        Surfing,
        YouTube,
        RuTube,
        Mail,
        Click,
        Visit
    }
    class ExceptionSurfing : Exception
    {
        public SurfingType SurfingType {  get; set; }
        public EnumTypeSite SiteType { get; set; }
        string mess;
        public ExceptionSurfing(SurfingType surfingType, EnumTypeSite siteType, string message) : base()
        {
            SurfingType = surfingType;
            SiteType = siteType;
            mess = message;
        }
        public new string Message { get { return $"Site : {SiteType}\nSirfing : {SurfingType}\nMessage : {mess}"; } }
    }
    class Surfing
    {
        protected SurfingType Type { get; set; }
        public Site Site { get; set; }
        public int Count { get; protected set; } = 0;
        public int Error { get; protected set; } = 0;
        public string FirstStep { get; set; }
        public string Page { get; set; }
        public delegate Task<bool> AntiBotDelegate(IBrowser browser);
        public AntiBotDelegate? AntiBot { get; set; }
        public delegate Task<bool> MiddleStepDelegate(IBrowser browser);
        public MiddleStepDelegate MiddleStep { get; set; }

        public delegate Task OpenPageDelegate(IBrowser browser, string Page);
        public OpenPageDelegate OpenPage { get; set; }
        public Surfing(Site site, string page, string firstStep, MiddleStepDelegate middleStep, SurfingType type = SurfingType.Surfing)
        {
            Site = site;
            Page = page;
            OpenPage = new OpenPageDelegate(OpenNormalPath);
            FirstStep = firstStep;
            MiddleStep = middleStep;
            Type = type; 
        }
        public Surfing(Site site, OpenPageDelegate openPage, string page, string firstStep, MiddleStepDelegate middleStep, SurfingType type = SurfingType.Surfing)
        {
            Site = site;
            Page = page;
            OpenPage = openPage;
            FirstStep = firstStep;
            MiddleStep = middleStep;
            Type = type;
        }
        public async Task OpenNormalPath(IBrowser browser, string Page)
        {
            await Site.LoadPageAsync(browser, Page);
        }
        protected Surfing() { }
        public virtual async Task<bool> SurfAsync(int waitTime = 5)
        {
            var browser = await Site.GetBrowserAsync(0);
            if (browser == null)
                return false;

            await OpenPage(browser, Page);

            if (AntiBot != null && await AntiBot(browser))
                return false;

            await Site.InjectJSAsync(browser, FirstStep);
            bool isRunning = true;

            try
            {
                while (isRunning)
                {
                    Site.eventBrowserCreated.Reset();
                    StatusJS status = await Site.InjectJSAsync(browser, "FirstStep();");

                    switch (status)
                    {
                        case StatusJS.End:
                            isRunning = false;
                            break;
                        case StatusJS.Continue:
                            continue;
                        case StatusJS.OK:
                            if (await Site.FunctionWaitAsync(browser, "SecondStep();", time: 10) == StatusJS.OK)
                            {
                                await Middle();
                            }
                            else
                            {
                                await HandleErrorAsync(browser);
                            }
                            break;
                        case StatusJS.OK1:
                            await Middle();
                            break;
                        case StatusJS.Error:
                        case StatusJS.ErrorWait:
                            await HandleErrorAsync(browser);
                            break;
                        default:
                            throw new ExceptionSurfing(Type, Site.Type, "Unexpected StatusJS value");
                    }

                    await Site.SleepAsync(2000);
                    Site.CloseСhildBrowser();
                    Site.form.FocusTab(browser);
                }
            }
            catch (Exception ex) when (ex is ExceptionSurfing || ex is ExceptionJS)
            {
                Site.Error(ex.Message);
                return false;
            }

            return true;
        }


        private async Task HandleErrorAsync(IBrowser browser)
        {
            Error++;
            await Site.InjectJSAsync(browser, "n++");
        }

        protected async Task Middle()
        {
            var browserSurf = await Site.GetBrowserAsync(1);
            if (browserSurf == null)
            {
                Site.Error("Ошибка: BrowserSurf не найден!");
                Error++;
                return;
            }

            try
            {
                if (await MiddleStep(browserSurf))
                    Count++;
                else
                    Error++;
            }
            catch (Exception ex)
            {
                Site.Error(ex.Message);
                Error++;
                return;
            }

            await Site.SleepAsync(2);
        }

    }
    class SurfingMail : Surfing
    {
        public delegate Task<bool> MailClickDelegate(IBrowser browser);
        public MailClickDelegate MailClick { get; set; }

        public SurfingMail(Site site, string page, string firstStep, MailClickDelegate mailClick, MiddleStepDelegate middleStep)
            : base(site, page, firstStep, middleStep, SurfingType.Mail)
        {
            MailClick = mailClick;
        }

        public SurfingMail(Site site, OpenPageDelegate openPage, string page, string firstStep, MailClickDelegate mailClick, MiddleStepDelegate middleStep)
            : base(site, openPage, page, firstStep, middleStep, SurfingType.Mail)
        {
            MailClick = mailClick;
        }

        public override async Task<bool> SurfAsync(int waitTime = 5)
        {
            var browser = await Site.GetBrowserAsync(0);
            if (browser == null)
                return false;

            await OpenPage(browser, Page);
            if (AntiBot != null && await AntiBot(browser))
                return false;

            await Site.InjectJSAsync(browser, FirstStep);
            bool isRunning = true;

            try
            {
                while (isRunning)
                {
                    Site.eventBrowserCreated.Reset();
                    StatusJS status = await Site.InjectJSAsync(browser, "FirstStep();");

                    switch (status)
                    {
                        case StatusJS.End:
                            isRunning = false;
                            break;
                        case StatusJS.Continue:
                            continue;
                        case StatusJS.OK:
                            if (await Site.FunctionWaitAsync(browser, "SecondStep();") == StatusJS.OK)
                            {
                                if (await MailClick(browser))
                                    await Middle();
                                else
                                    await HandleErrorAsync(browser);
                            }
                            else
                            {
                                await HandleErrorAsync(browser);
                            }
                            break;
                        case StatusJS.OK1:
                            if (await MailClick(browser))
                                await Middle();
                            else
                                Error++;
                            break;
                        case StatusJS.Error:
                        case StatusJS.ErrorWait:
                            await HandleErrorAsync(browser);
                            break;
                        default:
                            throw new ExceptionSurfing(Type, Site.Type, "Unexpected StatusJS value");
                    }

                    await Site.SleepAsync(2000);                    
                    Site.CloseСhildBrowser();
                }
            }
            catch (Exception ex) when (ex is ExceptionSurfing || ex is ExceptionJS)
            {
                Site.Error(ex.Message);
                return false;
            }

            return true;
        }


        private async Task HandleErrorAsync(IBrowser browser)
        {
            Error++;
            await Site.InjectJSAsync(browser, "n++");
        }

    }

    class ManagerSurfing
    {
        List<Surfing> Surfings = new List<Surfing>();
        public ManagerSurfing() { }
        public void AddSurfing(Surfing s) => Surfings.Add(s);
        public async Task StartSurf()
        {
            foreach (Surfing s in Surfings)
            {
                await s.SurfAsync();
            }
        }
    }
}