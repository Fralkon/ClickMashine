using CefSharp;
using CefSharp.WinForms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ClickMashine.Models;

namespace ClickMashine
{
    public partial class MainForm : Form
    {
        AutoClicker autoClicker;
        public MainForm(string[] args)
        {
            InitializeComponent();
            CefSettings settings = new CefSettings();
            var settingBrowser = new SettingBrowser()
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36",
                AcceptLanguageList = "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7",
                Auths = [new AuthData("iliya9401@gmail.com", "Ussd1801", EnumTypeSite.Aviso)]
            };
            autoClicker = new AutoClicker(this, settingBrowser.Auths);
            settings.UserAgent = settingBrowser.UserAgent;
            settings.AcceptLanguageList = settingBrowser.AcceptLanguageList;
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
            Cef.Initialize(settings);
        }
        public EventWaitHandle event_eny = new EventWaitHandle(false, EventResetMode.ManualReset);
        private void button1_Click(object sender, EventArgs e)
        {
            event_eny.Set();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            autoClicker?.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Run(autoClicker.ClickSurfAsync);
        }
        public void FocusTab(IBrowser browser)
        {
            lock (tabControl1)
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
            FocusTab(browser);
            lock (tabControl1)
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
                return ControlSnapshot.Snapshot(parentControl, rect);
            }
            else
            {
                tabControl1.SelectedTab = (TabPage)control;
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
