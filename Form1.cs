using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Threading;
using System.Xml.Linq;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Data;
using System.Net;
using System.Net.Sockets;

namespace ClickMashine
{
    public partial class Form1 : Form
    {
        object lockTabFocus = new object();
        AutoClicker? autoClicker;
        public int Step { private set; get; }
        public int ID { private set; get; }
        public string PATH_SETTING = @"C:/ClickMashine/Settings/";
        public Form1(string[] args)
        {

            InitializeComponent();
            CefSettings settings = new CefSettings();
            if (args.Length > 0) { Step = int.Parse(args[0]); }
            else { Step = 0; }
            using (StreamReader reader = new StreamReader(PATH_SETTING + "IDMashine.txt"))
            {
                string text = reader.ReadToEnd();
                ID = int.Parse(text);
            }
            using (DataTable settingData = new MySQL("clicker").GetDataTableSQL("SELECT user_agent, language FROM step WHERE step = " + Step.ToString() + " AND id_object = " + ID.ToString()))
            {
                if (settingData.Rows.Count > 0)
                {
                    settings.UserAgent = settingData.Rows[0]["user_agent"].ToString();
                    settings.AcceptLanguageList = settingData.Rows[0]["language"].ToString();
                }
                else
                {
                    throw new Exception("Error load setting CefSharp");
                }
                settings.CachePath = PATH_SETTING + "Cache/" + Step.ToString();
                settings.CefCommandLineArgs.Add("disable-gpu", "");
            }
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
            new MySQL("clicker").SendSQL("UPDATE object SET status = 'offline' WHERE id = " + ID.ToString());
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            autoClicker = new AutoClicker(this);
            Thread thread = new Thread(autoClicker.ClickSurf);
            thread.Start();
        }
        public void FocusTab(IBrowser browser)
        {
            lock (lockTabFocus)
            {
                if (this.WindowState == FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Maximized;
                browser.GetHost().SetFocus(true);
                var controlBrowser = Control.FromChildHandle(browser.GetHost().GetWindowHandle());
                if (controlBrowser != null)
                    controlBrowser.Invoke(new Action(() =>
                    {
                        TabPage? parentControl = controlBrowser.Parent as TabPage;
                        if (parentControl != null)
                        {
                            tabControl1.SelectedTab = parentControl;
                        }
                        else
                        {
                            tabControl1.SelectedTab = (TabPage)controlBrowser;
                        }
                    }));
            }
        }
        public Bitmap MakeScreenshot(IBrowser browser, Rectangle rect)
        {
            lock (lockTabFocus)
            {
                if (this.WindowState == FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Maximized;
                browser.GetHost().SetFocus(true);
                var controlBrowser = Control.FromChildHandle(browser.GetHost().GetWindowHandle());
                return (Bitmap)controlBrowser.Invoke(new DelegateMakeScreen(GetBitmap), controlBrowser, rect);
            }
        }
        delegate Bitmap DelegateMakeScreen(Control control, Rectangle rect);
        Bitmap GetBitmap(Control control, Rectangle rect)
        {
            this.Activate();
            TabPage? parentControl = control.Parent as TabPage;
            if (parentControl != null)
            {
                tabControl1.SelectedTab = parentControl;
                Thread.Sleep(1000);
                return ControlSnapshot.Snapshot(parentControl, rect);
            }
            else
            {
                tabControl1.SelectedTab = (TabPage)control;
                Thread.Sleep(1000);
                return ControlSnapshot.Snapshot(control, rect);
            }
        }
    }
    public static class ControlSnapshot
    {
        public static Bitmap Snapshot(Control c)
        {
            int width = 0, height = 0;
            IntPtr hwnd = IntPtr.Zero;
            IntPtr dc = IntPtr.Zero;
            c.Invoke(new MethodInvoker(() =>
            {
                width = c.ClientSize.Width;
                height = c.ClientSize.Height;
                hwnd = c.Handle;
                dc = GetDC(hwnd);
            }));
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            if (dc != IntPtr.Zero)
            {
                try
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        IntPtr bdc = g.GetHdc();
                        try
                        {
                            BitBlt(bdc, 0, 0, width, height, dc, 0, 0, TernaryRasterOperations.SRCCOPY);
                        }
                        finally
                        {
                            g.ReleaseHdc(bdc);
                        }
                    }
                }
                finally
                {
                    ReleaseDC(hwnd, dc);
                }
            }
            return bmp;
        }
        public static Bitmap Snapshot(Control c, Rectangle rectangle)
        {
            int width = 0, height = 0;
            IntPtr hwnd = IntPtr.Zero;
            IntPtr dc = IntPtr.Zero;
            c.Invoke(new MethodInvoker(() =>
            {
                width = c.ClientSize.Width;
                height = c.ClientSize.Height;
                hwnd = c.Handle;
                dc = GetDC(hwnd);
            }));
            Bitmap bmp = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppRgb);
            if (dc != IntPtr.Zero)
            {
                try
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        IntPtr bdc = g.GetHdc();
                        try
                        {
                            BitBlt(bdc, 0, 0, rectangle.Width, rectangle.Height, dc, rectangle.X, rectangle.Y, TernaryRasterOperations.SRCCOPY);
                        }
                        finally
                        {
                            g.ReleaseHdc(bdc);
                        }
                    }
                }
                finally
                {
                    ReleaseDC(hwnd, dc);
                }
            }
            return bmp;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);
        public enum TernaryRasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0x00CC0020,
            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0x00EE0086,
            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x008800C6,
            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x00660046,
            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x00440328,
            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x00330008,
            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x001100A6,
            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0x00C000CA,
            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0x00BB0226,
            /// <summary>dest = pattern</summary>
            PATCOPY = 0x00F00021,
            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0x00FB0A09,
            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x005A0049,
            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x00550009,
            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x00000042,
            /// <summary>dest = WHITE</summary>
            WHITENESS = 0x00FF0062,
            /// <summary>
            /// Capture window as seen on screen.  This includes layered windows
            /// such as WPF windows with AllowsTransparency="true"
            /// </summary>
            CAPTUREBLT = 0x40000000
        }
    }
}
