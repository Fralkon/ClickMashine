﻿using CefSharp;

namespace ClickMashine
{
    class Surfing
    {
        public Site Site { get; set; }
        public int Count { get; protected set; } = 0;
        public int Error { get; protected set; } = 0;
        public string FirstStep { get; set; }
        public string? SecondStep { get; set; }
        public string Page { get; set; }
        public delegate bool AntiBotDelegate(IBrowser browser);
        public AntiBotDelegate? AntiBot { get; set; }
        public delegate bool MiddleStepDelegate(IBrowser browser);
        public MiddleStepDelegate MiddleStep { get; set; }

        public delegate void OpenPageDelegate(IBrowser browser, string Page);
        public OpenPageDelegate OpenPage { get; set; }
        public Surfing(Site site, string page, string firstStep, MiddleStepDelegate middleStep)
        {
            Site = site;
            Page = page;
            OpenPage = new OpenPageDelegate(OpenNormalPath);
            FirstStep = firstStep;
            MiddleStep = middleStep;
        }
        public Surfing(Site site, string page, string firstStep, string secondStep, MiddleStepDelegate middleStep)
        {
            Site = site;
            Page = page;
            OpenPage = new OpenPageDelegate(OpenNormalPath);
            FirstStep = firstStep;
            SecondStep = secondStep;
            MiddleStep = middleStep;
        }
        public Surfing(Site site, OpenPageDelegate openPage, string page, string firstStep, MiddleStepDelegate middleStep)
        {
            Site = site;
            Page = page;
            OpenPage = openPage;
            FirstStep = firstStep;
            MiddleStep = middleStep;
        }
        public Surfing(Site site, OpenPageDelegate openPage, string page, string firstStep, string secondStep, MiddleStepDelegate middleStep)
        {
            Site = site;
            Page = page;
            OpenPage = openPage;
            FirstStep = firstStep;
            SecondStep = secondStep;
            MiddleStep = middleStep;
        }
        public void OpenNormalPath(IBrowser browser, string Page)
        {
            Site.LoadPage(browser, Page);
        }
        protected Surfing() { }
        public virtual bool Surf(int Wait = 5)
        {
            var browser = Site.GetBrowser(0);
            if (browser == null)
                return false;
            OpenPage(browser, Page);
            if (AntiBot != null)
                if (!AntiBot(browser))
                    return false;
            Site.InjectJS(browser, FirstStep);
            if(SecondStep != null)
                Site.InjectJS(browser, SecondStep);
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
                            if (SecondStep != null)
                                switch (Site.FunctionWait(browser, "SecondStep();"))
                                {
                                    case StatusJS.OK:
                                        {
                                            Middle();
                                            break;
                                        }
                                }
                            else
                                Middle();
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
        protected void Middle()
        {
            var browserSurf = Site.GetBrowser(1);
            if (browserSurf == null)
            {
                Error++;
                return;
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
            Site.Sleep(2);
            return;
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
        public SurfingMail(Site site, string page, string firstStep, string secondStep, MailClickDelegate mailClick, MiddleStepDelegate middleStep) : base(site, page, firstStep, secondStep, middleStep)
        {
            MailClick = mailClick;
        }
        public SurfingMail(Site site, OpenPageDelegate openPage, string page, string firstStep, MailClickDelegate mailClick, MiddleStepDelegate middleStep) : base(site, openPage, page, firstStep, middleStep)
        {
            MailClick = mailClick;
        }
        public SurfingMail(Site site, OpenPageDelegate openPage, string page, string firstStep, string secondStep, MailClickDelegate mailClick, MiddleStepDelegate middleStep) : base(site, openPage, page, firstStep, secondStep, middleStep)
        {
            MailClick = mailClick;
        }
        public override bool Surf(int Wait = 5)
        {
            var browser = Site.GetBrowser(0);
            if (browser == null)
                return false;
            OpenPage(browser, Page);
            if (AntiBot != null)
                if (!AntiBot(browser))
                    return false;
            Site.InjectJS(browser, FirstStep);
            if (SecondStep != null)
                Site.InjectJS(browser, SecondStep);
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
                            if (SecondStep != null)
                            {
                                switch (Site.FunctionWait(browser, "SecondStep();"))
                                {
                                    case StatusJS.OK:
                                        {
                                            if (MailClick(browser))
                                            {
                                                Middle();
                                            }
                                            else
                                                Error++;
                                            Site.Sleep(2);
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                if (MailClick(browser))
                                {
                                    Middle();
                                }
                                else
                                    Error++;
                                Site.Sleep(2);
                                break;
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
