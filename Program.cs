namespace ClickMashine
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm(args));
        }
    }
}
//Copyright © 2014 The CefSharp Authors. All rights reserved.

// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.