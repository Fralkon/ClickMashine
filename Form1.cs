using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Threading;
using System.Xml.Linq;

namespace ClickMashine
{
    public partial class Form1 : Form
    {
        AutoClicker? autoClicker;
        public int Step { private set; get; }
        public string PATH_SETTING = @"C:/ClickMashine/Settings/";
        public Form1(string[] args)
        {
            InitializeComponent();
            CefSettings settings = new CefSettings();
            if (args.Length > 0) { Step = int.Parse(args[0]); }
            else { Step = 0; }
            var XML = XDocument.Load(PATH_SETTING + Step + ".xml");
            XElement? settingXML = XML.Element("Setting");
            Console.WriteLine(settingXML);
            settings.CachePath = PATH_SETTING + "Cache/" + Step.ToString();            
            settings.UserAgent = settingXML.Element("UserAgent").Value.Trim();
            settings.AcceptLanguageList = settingXML.Element("AcceptLanguageList").Value.Trim();
            Cef.Initialize(settings);
        }
        public EventWaitHandle event_eny = new EventWaitHandle(false, EventResetMode.ManualReset);
        private void button1_Click(object sender, EventArgs e)
        {
            event_eny.Set();
        }
        private void button2_Click(object sender, EventArgs e)
        {

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            autoClicker.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            autoClicker = new AutoClicker(this);
            Thread thread = new Thread(autoClicker.ClickSurf);
            thread.Start();
        }
    }
}
