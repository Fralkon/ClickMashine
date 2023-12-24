using CefSharp;

namespace ClickMashine
{
    class Surfing
    {
        public Site Site { get; set; }
        public int Count { get; protected set; } = 0;
        public int Error { get; protected set; } = 0;
        public string FirstStep { get; set; }
        public string Page { get; set; }
        public delegate bool AntiBotDelegate(IBrowser browser);
        public AntiBotDelegate? AntiBot { get; set; }
        public delegate bool MiddleStepDelegate(IBrowser browser);
        public MiddleStepDelegate MiddleStep { get; set; }
        public Surfing(Site site, string page, string firstStep, MiddleStepDelegate middleStep)
        {
            Site = site;
            Page = page;
            FirstStep = firstStep;
            MiddleStep = middleStep;
        }
        protected Surfing() { }
        public virtual bool Surf(int Wait = 5)
        {
            var browser = Site.GetBrowser(0);
            if (browser == null)
                return false;
            Site.LoadPage(browser, Page);
            if (AntiBot != null)
                if (!AntiBot(browser))
                    return false;
            Site.InjectJS(browser, FirstStep);
            bool f = true;
            try
            {
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
                                        var browserSurf = Site.GetBrowser(1);
                                        if (browserSurf == null)
                                        {
                                            Error++;
                                            break;
                                        }
                                        try
                                        {
                                            if (MiddleStep(browserSurf))
                                                Count++;
                                            else
                                                Error++;
                                        }
                                        catch(Exception ex)
                                        {
                                            Site.Error(ex.Message);
                                            Error++;
                                        }
                                        Site.Sleep(2);
                                        break;
                                    }
                            }
                            break;
                        case StatusJS.Error:
                            Error++;
                            break;
                        default:
                            throw new Exception($"Error StatusJS");
                    }
                    Site.CloseСhildBrowser();
                }
                while (f);
            }
            catch (Exception e)
            {
                Site.Error(e.Message);
                return false;
            }
            return true;
        }
    }
    class SurfingMail : Surfing
    {
        public delegate bool MailClickDelegate(IBrowser browser);
        public MailClickDelegate MailClick { get; set; }
        public SurfingMail(Site site, string page, string firstStep, MailClickDelegate mailClick, MiddleStepDelegate middleStep) : base(site, page, firstStep, middleStep)
        {
            MailClick = mailClick;
        }
        public override bool Surf(int Wait = 5)
        {
            var browser = Site.GetBrowser(0);
            if (browser == null)
                return false;
            Site.LoadPage(browser, Page);
            if (AntiBot != null)
                if (!AntiBot(browser))
                    return false;
            Site.InjectJS(browser, FirstStep);
            bool f = true;
            try
            {
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
                                        if (MailClick(browser))
                                        {
                                            var browserSurf = Site.WaitCreateBrowser();
                                            if (browserSurf == null)
                                            {
                                                Error++;
                                                break;
                                            }
                                            try
                                            {
                                                if (MiddleStep(browserSurf))
                                                    Count++;
                                                else
                                                    Error++;
                                            }
                                            catch (Exception ex)
                                            {
                                                Site.Error(ex.Message);
                                                Error++;
                                            }
                                        }
                                        else
                                            Error++;
                                        Site.Sleep(2);
                                        break;
                                    }
                            }
                            break;
                        case StatusJS.Error:
                            Error++;
                            break;
                        default:
                            throw new Exception($"Error StatusJS");
                    }
                    Site.CloseСhildBrowser();
                }
                while (f);
            }
            catch (Exception e)
            {
                Site.Error(e.Message);
                return false;
            }
            return true;
        }
    }
    class ManagerSurfing
    {
        List<Surfing> Surfings = new List<Surfing>();
        public ManagerSurfing() { }
        public void AddSurfing(Surfing s) => Surfings.Add(s);
        public void StartSurf()
        {
            foreach (Surfing s in Surfings)
            {
                s.Surf();
            }
        }
    }
}
